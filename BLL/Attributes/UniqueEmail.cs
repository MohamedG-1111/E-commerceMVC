using System.ComponentModel.DataAnnotations;
using DataAcessLayer.Data;

namespace E_commerce.BLL.Attributes
{
    public class UniqueEmailAttribute : ValidationAttribute
    {
        override protected ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var dbContext = validationContext.GetService(typeof(AppDbContext)) as AppDbContext;

            if (dbContext == null)
            {
                return new ValidationResult("Database service is not available.");
            }
            var email = value?.ToString()?.Trim();
            if (email != null)
            {
                var ISexistingUser = dbContext.Users.Any(u => u.Email.ToUpper() == email.ToUpper());
                if (ISexistingUser)
                {
                    return new ValidationResult("Email is already in use.");
                }
            }
            return ValidationResult.Success;
        }
    }
}
