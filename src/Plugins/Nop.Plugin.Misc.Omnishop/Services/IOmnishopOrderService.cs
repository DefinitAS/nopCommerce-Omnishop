using System.Threading.Tasks;
using Nop.Core.Domain.Orders;

namespace Nop.Plugin.Misc.Omnishop.Services
{
    public interface IOmnishopOrderService
    {
        Task SubmitOrderToOmnishop(Order order);
    }
}