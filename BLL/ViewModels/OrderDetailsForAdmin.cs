using E_commerce.DAL.Entities.enums;

public class OrderDetailsForAdminVM
{
    public int OrderId { get; set; }

    // Customer
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;

    // Order
    public decimal OrderTotal { get; set; }
    public DateTime OrderDate { get; set; }
    public OrderStatus OrderStatus { get; set; }

    // Payment
    public PaymentStatus PaymentStatus { get; set; }
    public DateTime? PaymentDate { get; set; }
    public string? PaymentIntentId { get; set; }

    // Shipping
    public string? Carrier { get; set; }
    public string? TrackingNumber { get; set; }
    public DateTime? ShippingDate { get; set; }

    public List<OrderItemVM> OrderDetails { get; set; } = [];
}

public class OrderItemVM
{
    public string ProductName { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Price { get; set; }
}