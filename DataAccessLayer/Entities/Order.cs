using E_commerce.DAL.Entities.enums;
using E_commerce.DAL.Entities.Users;

namespace E_commerce.DAL.Entities
{
    public class Order
    {
        public int Id { get; set; }

        public string ApplicationUserId { get; set; } = null!;
        public ApplicationUser ApplicationUser { get; set; } = null!;

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public DateTime ShippingDate { get; set; }

        public decimal OrderTotal { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public PaymentStatus PaymentStatus { get; set; }

        public string? TrackingNumber { get; set; }

        public string? Carrier { get; set; }

        public DateTime PaymentDate { get; set; }

        public DateOnly PaymentDueDate { get; set; }

        public string? PaymentIntentId { get; set; }


        public List<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();

    }
}
