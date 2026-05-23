using System.ComponentModel.DataAnnotations;

namespace BLL.ViewModels
{
    public class CategoryVM
    {
        [Required]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
        public string Name { get; set; } = string.Empty;
        [Range(0, 100, ErrorMessage = "Display order must be between 0 and 100.")]

        public int DisplayOrder { get; set; } = 0;
    }
}
