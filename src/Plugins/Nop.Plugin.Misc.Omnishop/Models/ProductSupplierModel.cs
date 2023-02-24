using Nop.Web.Framework.Models;

namespace FourSoundCore.Models.ImportModels
{
    /// <summary>
    /// Represents model for product supplier
    /// </summary>
    public record ProductSupplierModel : BaseNopModel
    {
        public ProductSupplierModel(string omnishopId, string supplierId, 
            string supplierProductId, string supplierName)
        {
            OmnishopId = omnishopId;

            SupplierProductId = supplierProductId;
            SupplierId = supplierId;
            SupplierName = supplierName;
        }

        /// <summary>
        /// Gets or sets a related product omnishop id (sku)
        /// </summary>
        public string OmnishopId { get; set; }

        /// <summary>
        /// Gets or sets a supplier id for product
        /// </summary>
        public string SupplierProductId { get; set; }

        /// <summary>
        /// Gets or sets a supplier id
        /// </summary>
        public string SupplierId { get; set; }

        /// <summary>
        /// Gets or sets a supplier name
        /// </summary>
        public string SupplierName { get; set; }
    }
}