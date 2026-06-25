namespace E_commerce.BLL.ViewModels
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = null!;

        public string Image { get; set; } = null!;

        public int Count { get; set; }

        public decimal Price { get; set; }


    }
}
