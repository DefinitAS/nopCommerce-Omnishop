using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Misc.Omnishop.Controllers
{
    //[AutoValidateAntiforgeryToken]
    //[AuthorizeAdmin]
    //[Area(AreaNames.Admin)]
    public partial class ImportAPIProductsController : BasePluginController
    {
        private readonly IPermissionService _permissionService;
        private readonly IPictureService _pictureService;
        private readonly IProductService _productService;

        public ImportAPIProductsController(IPermissionService permissionService, IProductService productService, IPictureService pictureService)
        {
            _permissionService = permissionService;
            _productService = productService;
            _pictureService = pictureService;
        }

        [HttpGet, HttpPost]
        public virtual  IActionResult Ping(string requestSubPath)
        {
            if(!Request.Headers.ContainsKey("X-Nemlig-Hemlig"))
            {
                return Unauthorized();
            }

            return Ok(requestSubPath);
        }

        [HttpPost]
        public virtual async Task<IActionResult> UpdateProduct(string sku, [FromBody] Dictionary<string, object> properties)
        {
            if (!Request.Headers.ContainsKey("X-Nemlig-Hemlig"))
            {
                return Unauthorized();
            }

            var product = await _productService.GetProductBySkuAsync(sku);
            if(product== null)
            {
                return NotFound();
            }

            var productPropertyInfos = product.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            foreach(var propWithValue in properties)
            {
                var pi = productPropertyInfos.FirstOrDefault(x => x.Name == propWithValue.Key);
                if(pi!=null)
                {
                    pi.SetValue(product, propWithValue.Value);
                }
            }
            await _productService.UpdateProductAsync(product);

            return Ok();
        }

        [HttpPost]
        public virtual async Task<IActionResult> AddPictureToProduct(string sku, [FromQuery] string fileName)
        {
            if (!Request.Headers.ContainsKey("X-Nemlig-Hemlig"))
            {
                return Unauthorized();
            }

            var product = await _productService.GetProductBySkuAsync(sku);
            if (product == null)
            {
                return NotFound();
            }

            var newPictureBinary = await ReadAllBytes(Request.Body);
            var seName = await _pictureService.GetPictureSeNameAsync(fileName);
            var mimeType = GetMimeTypeFromFilePath(fileName);

            var newPicture = await _pictureService.InsertPictureAsync(newPictureBinary, mimeType, seName);

            await _productService.InsertProductPictureAsync(new ProductPicture
            {
                //EF has some weird issue if we set "Picture = newPicture" instead of "PictureId = newPicture.Id"
                //pictures are duplicated
                //maybe because entity size is too large
                PictureId = newPicture.Id,
                DisplayOrder = 1,
                ProductId = product.Id
            });

            await _productService.UpdateProductAsync(product);

            return Ok();
        }

        static async Task<byte[]> ReadAllBytes(Stream instream)
        {
            if (instream is MemoryStream)
                return ((MemoryStream)instream).ToArray();

            using (var memoryStream = new MemoryStream())
            {
                await instream.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }

        static string GetMimeTypeFromFilePath(string filePath)
        {
            new FileExtensionContentTypeProvider().TryGetContentType(filePath, out var mimeType);

            //set to jpeg in case mime type cannot be found
            return mimeType ?? MimeTypes.ImageJpeg;
        }

    }
}
