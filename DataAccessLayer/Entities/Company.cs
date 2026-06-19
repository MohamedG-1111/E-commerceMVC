namespace E_commerce.DAL.Entities
{
    public class Company
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string? Address { get; set; }

        public string? City { get; set; }

        public string? PostalCode { get; set; }

        public decimal DiscountPercentage { get; set; } = 0;
    }
}
