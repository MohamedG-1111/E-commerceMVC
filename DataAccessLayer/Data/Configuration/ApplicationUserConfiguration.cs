using E_commerce.DAL.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_commerce.DAL.Data.Configuration
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(10);


            builder.Property(u => u.StreetAddress)
                .HasMaxLength(100);

            builder.Property(u => u.City).HasMaxLength(50);

            builder.Property(u => u.PostalCode).HasMaxLength(20);

            builder.HasIndex(u => u.Email).IsUnique();

            builder.ToTable("Users", t =>
            {
                t.HasCheckConstraint(
                    "CHK_Users_Email",
                    "[Email] IS NOT NULL AND [Email] LIKE '%@%'"
                );

                t.HasCheckConstraint(
                    "CHK_Users_PostalCode",
                    "[PostalCode] IS NULL OR [PostalCode] LIKE '[0-9]%'"
                );
            });




        }
    }
}
