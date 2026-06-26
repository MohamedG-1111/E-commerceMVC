using E_commerce.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_commerce.DAL.Data.Configuration
{
    public class OrderDetailsConfiguration : IEntityTypeConfiguration<OrderDetails>
    {
        public void Configure(EntityTypeBuilder<OrderDetails> builder)
        {
            builder.HasKey(x => new { x.OrderId, x.ProductId });

            builder.Property(x => x.Count)
                .IsRequired();

            builder.Property(x => x.Price)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.HasOne(x => x.Order)
                .WithMany(a => a.OrderDetails)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);



            builder.HasOne(x => x.Product)
           .WithMany()
           .HasForeignKey(x => x.ProductId)
           .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
