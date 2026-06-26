namespace E_commerce.BLL.ViewModels
{
    public class OrderDetailsVM
    {
        public int Id { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal OrderTotal { get; set; }

        public string OrderStatus { get; set; } = string.Empty;

        public string PaymentStatus { get; set; } = string.Empty;

        public List<OrderDetailsItemVM> Items { get; set; } = new();
    }
}
