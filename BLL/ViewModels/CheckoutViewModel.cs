namespace E_commerce.BLL.ViewModels
{
    public class CheckoutViewModel
    {
        public UpdateCheckoutInfoVM UpdateCheckoutInfo { get; set; } = new();
        public List<CartItem> Items { get; set; } = new();

        public decimal SubTotal { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
    }
}
