namespace E_commerce.BLL.ViewModels
{
    public class OrderDetailsItemVM
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public string? Image { get; set; }

        public decimal Price { get; set; }

        public int Count { get; set; }

        public decimal Total => Price * Count;
    }
}
