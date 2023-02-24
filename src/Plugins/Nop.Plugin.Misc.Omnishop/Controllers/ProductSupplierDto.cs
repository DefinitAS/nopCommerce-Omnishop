using Nop.Core.Domain.Catalog;

namespace Nop.Plugin.Misc.Omnishop.Controllers
{
    internal class ProductSupplierDto
    {
        public ProductSupplierDto()
        {
        }

        public Product Product { get; set; }
        public object Supplier { get; set; }
    }
}