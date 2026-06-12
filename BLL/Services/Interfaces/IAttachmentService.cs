using Microsoft.AspNetCore.Http;

namespace E_commerce.BLL.Services.Interfaces
{
    public interface IAttachmentService
    {
        public Task<string> UploadAttachmentAsync(IFormFile file, string folderPath);
        public Task DeleteAttachmentAsync(string fileName, string folderPath);
    }
}
