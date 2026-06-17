using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace E_commerce.DAL.Entities.Users
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? StreetAddress { get; set; }

        public string? City { get; set; }
        public string? PostalCode
        {
            get; set;
        }

        public string? ProfilePicture { get; set; }


        [ValidateNever]
        public Company Company { get; set; } = null!;

        public int? CompanyId { get; set; }
    }
}
