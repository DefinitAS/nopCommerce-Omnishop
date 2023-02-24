using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.Omnishop.Models
{
    public partial record PublishProductsSearchModel : BaseSearchModel
    {
        #region Ctor

        public PublishProductsSearchModel()
        {
            AvailableCategories = new List<SelectListItem>();
            AvailableManufacturers = new List<SelectListItem>();
            AvailableStores = new List<SelectListItem>();
            AvailableWarehouses = new List<SelectListItem>();
            AvailableVendors = new List<SelectListItem>();
            AvailableProductTypes = new List<SelectListItem>();
            AvailablePublishedOptions = new List<SelectListItem>();
            AvailableHasImagesOptions = new List<SelectListItem>();
            AvailableHasShortDescriptionOptions = new List<SelectListItem>();
            AvailableHasFullDescriptionOptions = new List<SelectListItem>();
            AvailableHasPriceOptions = new List<SelectListItem>();
            AvailableHasInventoryOptions = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Admin.Catalog.Products.List.SearchProductName")]
        public string SearchProductName { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.List.SearchCategory")]
        public int SearchCategoryId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.List.SearchIncludeSubCategories")]
        public bool SearchIncludeSubCategories { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.List.SearchManufacturer")]
        public int SearchManufacturerId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.List.SearchStore")]
        public int SearchStoreId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.List.SearchVendor")]
        public int SearchVendorId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.List.SearchWarehouse")]
        public int SearchWarehouseId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.List.SearchProductType")]
        public int SearchProductTypeId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.List.SearchPublished")]
        public int SearchPublishedId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.List.GoDirectlyToSku")]
        public string GoDirectlyToSku { get; set; }


        [NopResourceDisplayName("Plugins.Misc.Omnishop.PublishProducts.List.SearchHasImages")]
        public int SearchHasImagesId { get; set; }

        [NopResourceDisplayName("Plugins.Misc.Omnishop.PublishProducts.List.SearchHasShortDescription")]
        public int SearchHasShortDescriptionId { get; set; }

        [NopResourceDisplayName("Plugins.Misc.Omnishop.PublishProducts.List.SearchHasLongDescription")]
        public int SearchHasLongDescriptionId { get; set; }

        [NopResourceDisplayName("Plugins.Misc.Omnishop.PublishProducts.List.SearchHasPrice")]
        public int SearchHasPriceId { get; set; }

        [NopResourceDisplayName("Plugins.Misc.Omnishop.PublishProducts.List.SearchHasInventory")]
        public int SearchHasInventoryId { get; set; }

        public bool IsLoggedInAsVendor { get; set; }

        public bool AllowVendorsToImportProducts { get; set; }

        public bool HideStoresList { get; set; }

        public IList<SelectListItem> AvailableCategories { get; set; }
        public IList<SelectListItem> AvailableManufacturers { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
        public IList<SelectListItem> AvailableWarehouses { get; set; }
        public IList<SelectListItem> AvailableVendors { get; set; }
        public IList<SelectListItem> AvailableProductTypes { get; set; }
        public IList<SelectListItem> AvailablePublishedOptions { get; set; }       
        public IList<SelectListItem> AvailableHasImagesOptions { get; set; }
        public IList<SelectListItem> AvailableHasShortDescriptionOptions { get; set; }
        public IList<SelectListItem> AvailableHasFullDescriptionOptions { get; set; }
        public IList<SelectListItem> AvailableHasPriceOptions { get; set; }
        public IList<SelectListItem> AvailableHasInventoryOptions { get; set; }
        #endregion
    }
}
