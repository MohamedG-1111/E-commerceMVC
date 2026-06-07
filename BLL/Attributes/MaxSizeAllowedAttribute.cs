using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace E_commerce.BLL.Attributes
{
    public class MaxSizeAllowedAttribute : ValidationAttribute
    {
        private readonly int _maxSize;
        public MaxSizeAllowedAttribute(int maxSize)
        {
            _maxSize = maxSize;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            if (file is not null)
            {
                if (file.Length > _maxSize)
                {
                    return new ValidationResult($"This file size is not allowed!");
                }
            }
            return ValidationResult.Success!;
        }
    }
}
