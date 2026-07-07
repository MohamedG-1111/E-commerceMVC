using E_commerce.DAL.Entities;

namespace E_commerce.BLL.Services.Interfaces
{
    public interface IPricingService
    {
        Task<(decimal discountAmount, decimal total)> CalculatePricingAsync(decimal subtotal);
        Task<int> PriceForProductAsync(int quantity, Product product);
    }
}
