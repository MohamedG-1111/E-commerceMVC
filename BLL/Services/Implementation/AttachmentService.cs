using E_commerce.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace E_commerce.BLL.Services.Implementation
{
    public class AttachmentService : IAttachmentService
    {

        private readonly IWebHostEnvironment _environment;

        public AttachmentService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task DeleteAttachmentAsync(string fileName, string folderPath)
        {
            var path = Path.Combine(_environment.WebRootPath, folderPath, fileName);

            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public async Task<string> UploadAttachmentAsync(IFormFile file, string folderPath)
        {
            if (file == null || file.Length == 0)
            {
                return null;
            }
            var CoverName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var FullPath = Path.Combine(_environment.WebRootPath, folderPath, CoverName);
            using var stream = new FileStream(FullPath, FileMode.Create);
            await file.CopyToAsync(stream);
            return CoverName;
        }
    }
}
