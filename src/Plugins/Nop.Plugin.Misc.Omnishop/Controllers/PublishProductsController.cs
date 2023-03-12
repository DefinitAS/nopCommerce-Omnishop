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
    public class PublishProductsController : BasePluginController
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

        public PublishProductsController(IPermissionService permissionService,
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

        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public async Task<IActionResult> List()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var searchModel = await ConfigureSearch();

            return View("~/Plugins/Misc.Omnishop/Views/PublishProductsList.cshtml", searchModel);
        }

        [HttpPost]
        public async Task<ActionResult<CustomProductListModel>> ProductList(PublishProductsSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return await AccessDeniedDataTablesJson();

            var categoriesIds = new List<int>()
            {
                searchModel.SearchCategoryId
            };

            if (searchModel.SearchIncludeSubCategories)
            {
                categoriesIds.AddRange(await _categoryService.GetChildCategoryIdsAsync(parentCategoryId: searchModel.SearchCategoryId, showHidden: true));
            }

            var productsQuery = FilterProducts(searchModel, categoriesIds);
            var totalProducts = productsQuery.Count();
            var products = productsQuery.Skip((searchModel.Page * searchModel.PageSize) - searchModel.PageSize).Take(searchModel.PageSize).ToList();
            var products_suppliers = from product in products
                select new ProductSupplierDto()
                {
                    Product = product,
                    Supplier = null
                };

            var productPaging = new PagedList<Product>(products, searchModel.Page, searchModel.PageSize, totalProducts);

            var model = await new CustomProductListModel().PrepareToGridAsync(searchModel, productPaging,  () =>
            {
                return products_suppliers.ToList().SelectAwait(async x=>await MapCustomProductModel(x));
            });

            return Json(model);
        }

        [HttpPost]
        public async Task<IActionResult> ImportFromOmnishop(ICollection<int> selectedIds)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
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

        [HttpPost]
        public async Task<IActionResult> ImportProductImageFromUrl([FromQuery] int productId, [FromQuery] string url)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            try
            {
                using (var clt = new HttpClient())
                {
                    var imageBytes = await clt.GetByteArrayAsync(url);
                    var fileName = System.IO.Path.GetFileName(new Uri(url).LocalPath);
                    var seName = await _pictureService.GetPictureSeNameAsync(fileName);
                    var mimeType = GetMimeTypeFromFilePath(fileName);

                    var newPicture = await _pictureService.InsertPictureAsync(imageBytes, mimeType, seName);
                    await _productService.InsertProductPictureAsync(new ProductPicture
                    {
                        //EF has some weird issue if we set "Picture = newPicture" instead of "PictureId = newPicture.Id"
                        //pictures are duplicated
                        //maybe because entity size is too large
                        PictureId = newPicture.Id,
                        DisplayOrder = 1,
                        ProductId = productId
                    });
                }
                return Json(new { Result = true, Message = "Import of productimage from " + url + " completed successfully." });
            }
            catch (Exception e)
            {
                return Json(new { Result = false, Message = e.Message });
            }
        }


        [HttpPost]
        public async Task<IActionResult> PublishSelected(ICollection<int> selectedIds)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            if (selectedIds == null || !selectedIds.Any())
            {
                return Json(new { Result = false, Message = "No products selected" });
            }

            try
            {
                var vendorId = (await _workContext.GetCurrentVendorAsync())?.Id;
                var products = _productRepository.Table.Where(p => (vendorId == null || p.VendorId == vendorId) && selectedIds.Contains(p.Id)).ToList();
                foreach (var item in products)
                {
                    item.Published = true;
                }
                await _productRepository.UpdateAsync(products);
                return Json(new { Result = true, Message= "Publish of products completed successfully." });
            }
            catch (Exception e)
            {
                return Json(new { Result = false, Message = e.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UnpublishSelected(ICollection<int> selectedIds)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            try
            {
                if (selectedIds == null || !selectedIds.Any())
                {
                    return Json(new { Result = false, Message = "No products selected" });
                }

                var currentVendorId = (await _workContext.GetCurrentVendorAsync())?.Id;
                var products = _productRepository.Table.Where(p => (currentVendorId == null || p.VendorId == currentVendorId) && selectedIds.Contains(p.Id)).ToList();
                foreach (var item in products)
                {
                    item.Published = false;
                }
                await _productRepository.UpdateAsync(products);
                return Json(new { Result = true, Message = "Unpublish of products completed successfully." });
            }
            catch (Exception e)
            {
                return Json(new { Result = false, Message = e.Message });
            }
        }
        #region Utility
        static string GetMimeTypeFromFilePath(string filePath)
        {
            new FileExtensionContentTypeProvider().TryGetContentType(filePath, out var mimeType);

            //set to jpeg in case mime type cannot be found
            return mimeType ?? MimeTypes.ImageJpeg;
        }

        private async Task<CustomProductModel> MapCustomProductModel(ProductSupplierDto product)
        {
            var productModel = new CustomProductModel();

            //if (product.Supplier != null)
            //{
            //    productModel.SupplierProductId = product.Supplier.SupplierProductId;
            //    productModel.SupplierName = product.Supplier.SupplierName;
            //}
            //else
            //{
                productModel.SupplierProductId = string.Empty;
                productModel.SupplierName = string.Empty;
            //}

            //little performance optimization: ensure that "FullDescription" is not returned
            productModel.FullDescription = !string.IsNullOrEmpty(product.Product.FullDescription) ? $"Has text ({product.Product.FullDescription.Length} symbols)" : string.Empty;
            productModel.ShortDescription = !string.IsNullOrEmpty(product.Product.ShortDescription) ? $"Has text ({product.Product.ShortDescription.Length} symbols)" : string.Empty;

            productModel.Id = product.Product.Id;
            productModel.Name = product.Product.Name;
            productModel.Price = product.Product.Price;
            productModel.Sku = product.Product.Sku;
            productModel.Published = product.Product.Published;

            var defaultProductPicture = (await _pictureService.GetPicturesByProductIdAsync(product.Product.Id, 1)).FirstOrDefault();
            (productModel.PictureThumbnailUrl, _) = await _pictureService.GetPictureUrlAsync(defaultProductPicture, 75);
            productModel.ProductTypeName = await _localizationService.GetLocalizedEnumAsync(product.Product.ProductType);
            productModel.StockQuantityStr = product.Product.StockQuantity.ToString();
            productModel.UpdatedDate = product.Product.UpdatedOnUtc.ToLocalTime();
            return productModel;
        }

        private async  Task<PublishProductsSearchModel> ConfigureSearch()
        {
            var searchModel = new PublishProductsSearchModel();
            var allOption = new SelectListItem(await _localizationService.GetResourceAsync("Admin.Catalog.Products.List.SearchPublished.All"), "0");

            //Categories
            var categories = await _categoryService.GetAllCategoriesAsync();
            var categoriesListItems = new List<SelectListItem>(categories.Count +1);
            categoriesListItems.Add(allOption);
            foreach (var cat in categories)
            {
                categoriesListItems.Add(
                    new SelectListItem(await _categoryService.GetFormattedBreadCrumbAsync(cat), cat.Id.ToString())
                );
            }
            searchModel.AvailableCategories = categoriesListItems;


            //Manufacturers
            searchModel.AvailableManufacturers =
                (await _manufacturerService.GetAllManufacturersAsync())
                .OrderBy(x=>x.Name)
                .Select(e => new SelectListItem(e.Name, e.Id.ToString()))
                .ToList();
            searchModel.AvailableManufacturers.Insert(0, allOption);

            //ProductTypes
            searchModel.AvailableProductTypes = (await ProductType.SimpleProduct.ToSelectListAsync()).Select(e => new SelectListItem()
            {
                Text = e.Text,
                Value = e.Value
            }).ToList();
            searchModel.AvailableProductTypes.Insert(0, allOption);

            //Warehouses
            searchModel.AvailableWarehouses = (await _shippingService.GetAllWarehousesAsync()).Select(e => new SelectListItem()
            {
                Text = e.Name,
                Value = e.Id.ToString()
            }).ToList();
            searchModel.AvailableWarehouses.Insert(0, allOption);

            //Published status
            searchModel.AvailablePublishedOptions.Add(allOption);
            searchModel.AvailablePublishedOptions.Add(new SelectListItem
            {
                Value = "1",
                Text = await _localizationService.GetResourceAsync("Admin.Catalog.Products.List.SearchPublished.PublishedOnly")
            });
            searchModel.AvailablePublishedOptions.Add(new SelectListItem
            {
                Value = "2",
                Text = await _localizationService.GetResourceAsync("Admin.Catalog.Products.List.SearchPublished.UnpublishedOnly")
            });

            searchModel.AvailableHasFullDescriptionOptions.Add(allOption);
            searchModel.AvailableHasFullDescriptionOptions.Add(new SelectListItem(await _localizationService.GetResourceAsync("Plugins.Misc.Omnishop.PublishProducts.List.SearchHasLongDescription.Has"), "1"));
            searchModel.AvailableHasFullDescriptionOptions.Add(new SelectListItem(await _localizationService.GetResourceAsync("Plugins.Misc.Omnishop.PublishProducts.List.SearchHasLongDescription.HasNot"), "2"));

            searchModel.AvailableHasShortDescriptionOptions.Add(allOption);
            searchModel.AvailableHasShortDescriptionOptions.Add(new SelectListItem(await _localizationService.GetResourceAsync("Plugins.Misc.Omnishop.PublishProducts.List.SearchHasShortDescription.Has"), "1"));
            searchModel.AvailableHasShortDescriptionOptions.Add(new SelectListItem(await _localizationService.GetResourceAsync("Plugins.Misc.Omnishop.PublishProducts.List.SearchHasShortDescription.HasNot"), "2"));

            searchModel.AvailableHasImagesOptions.Add(allOption);
            searchModel.AvailableHasImagesOptions.Add(new SelectListItem(await _localizationService.GetResourceAsync("Plugins.Misc.Omnishop.PublishProducts.List.SearchHasImages.Has"), "1"));
            searchModel.AvailableHasImagesOptions.Add(new SelectListItem(await _localizationService.GetResourceAsync("Plugins.Misc.Omnishop.PublishProducts.List.SearchHasImages.HasNot"), "2"));

            searchModel.AvailableHasPriceOptions.Add(allOption);
            searchModel.AvailableHasPriceOptions.Add(new SelectListItem(await _localizationService.GetResourceAsync("Plugins.Misc.Omnishop.PublishProducts.List.SearchHasPrice.Has"), "1"));
            searchModel.AvailableHasPriceOptions.Add(new SelectListItem(await _localizationService.GetResourceAsync("Plugins.Misc.Omnishop.PublishProducts.List.SearchHasPrice.HasNot"), "2"));

            searchModel.AvailableHasInventoryOptions.Add(allOption);
            searchModel.AvailableHasInventoryOptions.Add(new SelectListItem(await _localizationService.GetResourceAsync("Plugins.Misc.Omnishop.PublishProducts.List.SearchHasInventory.Has"), "1"));
            searchModel.AvailableHasInventoryOptions.Add(new SelectListItem(await _localizationService.GetResourceAsync("Plugins.Misc.Omnishop.PublishProducts.List.SearchHasInventory.HasNot"), "2"));


            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null)
            {
                searchModel.SearchVendorId = currentVendor.Id;
            }

            searchModel.SetGridPageSize();
            return searchModel;
        }

        private IQueryable<Product> FilterProducts(PublishProductsSearchModel searchModel, IList<int> categoriesIds)
        {
            var products = _productRepository.Table;

            if (searchModel.SearchHasImagesId == 1)
            {
                products = from product in products
                           join picture in _productPictureRepository.Table on product.Id equals picture.ProductId
                           select product;
            }
            else if (searchModel.SearchHasImagesId == 2)
            {
                products = from product in products
                           join picture in _productPictureRepository.Table on product.Id equals picture.ProductId into subset
                           from sc in subset.DefaultIfEmpty()
                           where sc == null
                           select product;
            }

            if (searchModel.SearchHasShortDescriptionId == 1)
            {
                products = products.Where(e => !string.IsNullOrEmpty(e.ShortDescription));
            }
            else if(searchModel.SearchHasShortDescriptionId == 2)
            {
                products = products.Where(e => string.IsNullOrEmpty(e.ShortDescription));
            }

            if (searchModel.SearchHasLongDescriptionId == 1)
            {
                products = products.Where(e => !string.IsNullOrEmpty(e.FullDescription));
            }
            else if (searchModel.SearchHasLongDescriptionId == 2)
            {
                products = products.Where(e => string.IsNullOrEmpty(e.FullDescription));
            }

            if (searchModel.SearchHasPriceId == 1)
            {
                products = products.Where(e => e.Price > 0);
            }
            else if (searchModel.SearchHasPriceId == 2)
            {
                products = products.Where(e => e.Price <= 0);
            }

            if (searchModel.SearchHasInventoryId == 1)
            {
                products = products.Where(e => e.StockQuantity > 0);
            }
            else if (searchModel.SearchHasInventoryId == 2)
            {
                products = products.Where(e => e.StockQuantity <= 0);
            }

            if (searchModel.SearchPublishedId == 1)
            {
                products = products.Where(e => e.Published == true);
            }
            else if (searchModel.SearchPublishedId == 2)
            {
                products = products.Where(e => e.Published == false);
            }

            if (searchModel.SearchCategoryId != 0)
            {
                var getSubCats = _productCategoryRepository.Table.Where(e => categoriesIds.Any(i => i == e.CategoryId));

                products = from product in products
                           join category in getSubCats on product.Id equals category.ProductId
                           select product;
            }

            if (searchModel.SearchManufacturerId != 0)
            {
                products = from product in products
                           join manufacturer in _productManufacturerRepository.Table.Where(e => e.ManufacturerId == searchModel.SearchManufacturerId) on product.Id equals manufacturer.ProductId
                           select product;
            }
            
            if (!string.IsNullOrEmpty(searchModel.SearchProductName))
            {
                products = products.Where(e => e.Name.Contains(searchModel.SearchProductName, StringComparison.OrdinalIgnoreCase));
            }

            if (searchModel.SearchProductTypeId != 0)
            {
                products = products.Where(e => e.ProductTypeId == searchModel.SearchProductTypeId);
            }

            if (searchModel.SearchWarehouseId != 0)
            {
                products = from product in products
                           join warehouse in _productWarehouseRepository.Table.Where(e => e.WarehouseId == searchModel.SearchWarehouseId) on product.Id equals warehouse.ProductId
                           select product;
            }

            products = products.Distinct().OrderBy(e => e.Id);
            return products;
        }



        #endregion

    }
}
