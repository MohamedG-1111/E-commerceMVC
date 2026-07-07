using DataAccessLayer.Repositories.Interfaces;
using E_commerce.BLL.Services.Interfaces;
using E_commerce.DAL.Entities;
using Ecommerce.Utility;

namespace E_commerce.BLL.Services.Implementation
{
    public class PricingService : IPricingService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ICurrentUserService currentUser;

        public PricingService(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        {
            this.unitOfWork = unitOfWork;
            this.currentUser = currentUser;
        }

        public async Task<int> PriceForProductAsync(int quantity, Product product)
        {
            return quantity switch
            {
                <= 50 => product.PriceFor1To50,
                <= 100 => product.PriceFor50Plus,
                _ => product.PriceFor100Plus
            };
        }
        public async Task<(decimal discountAmount, decimal total)> CalculatePricingAsync(decimal subtotal)
        {
            var user = await currentUser.GetCurrentUser();

            if (user == null || user.Role != Roles.Company)
                return (0, subtotal);

            var company = unitOfWork.Repository<Company>()
                .GetAsQuery()
                .FirstOrDefault(x => x.Id == user.CompanyId);

            if (company == null)
                return (0, subtotal);

            var discountPercent = company?.DiscountPercentage ?? 0;

            if (discountPercent <= 0)
                return (0, subtotal);

            var discountAmount = Math.Min(subtotal, subtotal * (discountPercent / 100m));
            var total = subtotal - discountAmount;

            return (discountAmount, total);
        }


    }
}
