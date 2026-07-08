using E_commerce.DAL.Entities.enums;

namespace E_commerce.BLL.ViewModels
{
    public class UpdateOrderStatus
    {
        public int Id { get; set; }

        public OrderStatus? OrderStatus { get; set; }

        public PaymentStatus? PaymentStatus { get; set; }

        public string? TrackingNumber { get; set; }

        public string? Carrier { get; set; }


    }
}
