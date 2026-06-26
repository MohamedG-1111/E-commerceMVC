namespace E_commerce.BLL.ViewModels
{
    public class OrderVM
    {
        public int Id { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal OrderTotal { get; set; }

        public string OrderStatus { get; set; }

        public string PaymentStatus { get; set; }

        public int ItemsCount { get; set; }
    }
}
