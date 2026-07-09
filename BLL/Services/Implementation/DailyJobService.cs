using DataAccessLayer.Repositories.Interfaces;
using E_commerce.BLL.Services.Interfaces;
using E_commerce.DAL.Entities;
using E_commerce.DAL.Entities.enums;
using Microsoft.EntityFrameworkCore;

namespace E_commerce.BLL.Services.Implementation
{
    public class DailyJobService : IDailyJobService
    {
        private readonly IUnitOfWork unitOfWork;

        public DailyJobService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task CleanUpUnpaidOrdersOlderThanOneMonthAsync()
        {
            var oneMonthAgo = DateTime.UtcNow.AddMonths(-1);

            await unitOfWork.Repository<Order>()
                .GetAsQuery()
                .Where(x =>
                    (x.OrderStatus == OrderStatus.Pending ||
                     x.OrderStatus == OrderStatus.Cancelled) &&

                    (x.PaymentStatus == PaymentStatus.Pending ||
                     x.PaymentStatus == PaymentStatus.Rejected) &&

                    x.OrderDate < oneMonthAgo)
                .ExecuteDeleteAsync();
        }
    }
}