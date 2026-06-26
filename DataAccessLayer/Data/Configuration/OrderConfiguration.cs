using E_commerce.DAL.Entities;
using E_commerce.DAL.Entities.enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_commerce.DAL.Data.Configuration
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.Property(x => x.OrderStatus)
                 .HasConversion<string>()
                 .HasDefaultValue(OrderStatus.Pending);

            builder.Property(x => x.PaymentStatus)
                .HasConversion<string>()
                .HasDefaultValue(PaymentStatus.Pending);

            builder.HasOne(x => x.ApplicationUser)
                .WithMany(a => a.Orders)
                .HasForeignKey(x => x.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.Property(x => x.TrackingNumber)
                .HasMaxLength(100);

            builder.Property(x => x.Carrier)
                   .HasMaxLength(100);

            builder.Property(x => x.PaymentIntentId)
                   .HasMaxLength(200);

            builder.Property(x => x.OrderTotal)
              .HasPrecision(18, 2);

        }
    }
}
