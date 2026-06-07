using Microsoft.AspNetCore.Http;

namespace E_commerce.BLL.Services.Interfaces
{
    public interface IAttachmentService
    {
        public Task<string> UploadAttachmentAsync(IFormFile file);
        public Task DeleteAttachmentAsync(string fileName);
    }
}
