namespace E_commerce.BLL.ViewModels
{
    public class OrderPaymentVM
    {
        public int OrderId { get; set; }

        public decimal OrderTotal { get; set; }
        public decimal Discount { get; set; }




        public List<OrderPaymentItemVM> Items { get; set; } = new();
    }

    public class OrderPaymentItemVM
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = null!;

        public string Image { get; set; } = null!;

        public decimal Price { get; set; }

        public int Count { get; set; }

        public decimal Total => Price * Count;
    }
}
