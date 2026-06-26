namespace E_commerce.DAL.Entities
{
    public class OrderDetails
    {
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;


        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public decimal Price { get; set; }

        public int Count { get; set; }
    }
}
