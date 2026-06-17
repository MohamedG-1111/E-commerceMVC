using System.ComponentModel.DataAnnotations.Schema;

namespace E_commerce.DAL.Entities.Users
{
    public class Company
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; } = null!;

        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; } = null!;
    }
}
