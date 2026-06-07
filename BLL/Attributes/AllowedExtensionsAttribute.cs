using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace E_commerce.BLL.Attributes
{
    internal class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string _extensions;
        public AllowedExtensionsAttribute(string extensions)
        {
            _extensions = extensions;
        }
        override protected ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            if (file is not null)
            {
                var extension = Path.GetExtension(file.FileName);
                if (!_extensions.Split(',').Contains(extension, StringComparer.OrdinalIgnoreCase))
                {
                    return new ValidationResult($"This file extension is not allowed!");
                }
            }
            return ValidationResult.Success!;
        }
    }
}