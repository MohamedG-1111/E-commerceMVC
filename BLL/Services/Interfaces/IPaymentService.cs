using E_commerce.BLL.ViewModels;
using Ecommerce.Utility.Result;

namespace E_commerce.BLL.Services.Interfaces
{
    public interface IPaymentService
    {
        public Task<Result<CustomerCart>> CreateOrUpdatePaymentIntent();
    }
}
