namespace E_commerce.BLL.ViewModels
{
    public class CustomerCart
    {
        public string UserId { get; set; } = null!;
        public List<CartItem> Items { get; set; } = new();


        public decimal SubTotal { get; set; }

        public decimal Discount { get; set; }

        public decimal Total { get; set; }
    }
}
