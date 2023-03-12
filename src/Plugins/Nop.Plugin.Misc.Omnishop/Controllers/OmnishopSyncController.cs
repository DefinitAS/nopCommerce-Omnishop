using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Models.Extensions;
using System.Collections.Generic;
using System.Linq;
using Nop.Services.Messages;
using System;
using Nop.Services.ExportImport;
using Newtonsoft.Json;
using System.Text;
using Nop.Data;
using System.Threading.Tasks;
using Nop.Plugin.Misc.Omnishop.Models;
using System.Net.Http;
using Microsoft.AspNetCore.StaticFiles;

namespace Nop.Plugin.Misc.Omnishop.Controllers
{
    [Area(AreaNames.Admin)]
    public class OmnishopSyncController : BasePluginController
    {      
        private const string SEARCH_MODEL_KEY = "SearchModel";

        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;
        private readonly IRepository<Product> _productRepository;
        private readonly IPictureService _pictureService;
        private readonly ILocalizationService _localizationService;
        private readonly IRepository<ProductPicture> _productPictureRepository;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IShippingService _shippingService;
        private readonly IRepository<ProductCategory> _productCategoryRepository;
        private readonly IRepository<ProductManufacturer> _productManufacturerRepository;
        private readonly IRepository<ProductWarehouseInventory> _productWarehouseRepository;
        private readonly IProductAttributeService _productAttributeService;
        private readonly INotificationService _notificationService;
        private readonly INopDataProvider _nopDataProvider;
        private readonly IExportManager _exportManager;
        private readonly HttpContext _httpContextAccessor;

        public OmnishopSyncController(IPermissionService permissionService,
            IWorkContext workContext,
            IRepository<Product> productRepository,
            IPictureService pictureService,
            ILocalizationService localizationService,
            IRepository<ProductPicture> productPictureRepository,
            IProductService productService,
            ICategoryService categoryService,
            IManufacturerService manufacturerService, 
            IShippingService shippingService,
            IRepository<ProductCategory> productCategoryRepository,
            IRepository<ProductManufacturer> productManufacturerRepository,
            IRepository<ProductWarehouseInventory> productWarehouseRepository,
            IProductAttributeService productAttributeService,
            INotificationService notificationService,
            INopDataProvider nopDataProvider,
            IExportManager exportManager)
        {
            _permissionService = permissionService;
            _workContext = workContext;
            _productRepository = productRepository;
            _pictureService = pictureService;
            _localizationService = localizationService;
            _productPictureRepository = productPictureRepository;
            _productService = productService;
            _categoryService = categoryService;
            _manufacturerService = manufacturerService;
            _shippingService = shippingService;
            _productCategoryRepository = productCategoryRepository;
            _productManufacturerRepository = productManufacturerRepository;
            _productWarehouseRepository = productWarehouseRepository;
            _productAttributeService = productAttributeService;
            _notificationService = notificationService;
            _nopDataProvider = nopDataProvider;
            _exportManager = exportManager;
        }

       

        [HttpPost]
        public async Task<IActionResult> SubmitOrdersToOmnishop()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();
            try
            {
                await _nopDataProvider.ExecuteNonQueryAsync("EXEC dbo.spOmniImport");
                return Json(new { Result = true, Message = "Import from Omnishop completed successfully." });
            }
            catch (Exception e)
            {
                return Json(new { Result = false, Message = e.Message });
            }
        }

    }
}
