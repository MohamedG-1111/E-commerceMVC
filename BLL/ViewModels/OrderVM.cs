using E_commerce.DAL.Entities.enums;

namespace E_commerce.BLL.ViewModels
{
    public class OrderVM
    {
        public int Id { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal OrderTotal { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public PaymentStatus PaymentStatus { get; set; }

        public int ItemsCount { get; set; }
    }
}
