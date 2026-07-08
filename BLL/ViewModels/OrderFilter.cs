using E_commerce.DAL.Entities.enums;

namespace E_commerce.BLL.ViewModels
{
    public class OrderFilter
    {
        public string? Search { get; set; }

        public OrderStatus? Status { get; set; }

        public PaymentStatus? PaymentStatus { get; set; }

        public DateTime? From { get; set; }

        public DateTime? To { get; set; }

        public string? Email { get; set; }
    }
}
