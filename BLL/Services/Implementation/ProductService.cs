using BLL.Services.Interfaces;
using DataAccessLayer.Repositories.Interfaces;
using E_commerce.BLL.Services.Interfaces;
using E_commerce.BLL.ViewModels;
using E_commerce.DAL.Entities;
using Microsoft.EntityFrameworkCore;


namespace E_commerce.BLL.Services.Implementation
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAttachmentService attachmentService;

        public ProductService(IUnitOfWork unitOfWork, IAttachmentService AttachmentService)
        {
            _unitOfWork = unitOfWork;
            attachmentService = AttachmentService;
        }


        public async Task<IEnumerable<ProductListVm>?> AllProductsAsync()
        {
            return await _unitOfWork.Repository<Product>().GetAsQuery()
                .Select(x => new ProductListVm()
                {
                    Id = x.Id,
                    Title = x.Title,
                    ISBN = x.ISBN,
                    ListPrice = x.ListPrice,
                    CategoryName = x.Category.Name,
                    ImageUrl = x.ImageUrl,
                }).ToListAsync();
        }


        public async Task<bool> CreateProductAsync(CreateProductViewModel obj)
        {
            if (obj == null || obj.Product == null)
                return false;

            string imageUrl = string.Empty;

            try
            {
                imageUrl = await attachmentService.UploadAttachmentAsync(obj.Cover);

                var product = obj.Product;
                product.ImageUrl = imageUrl;

                await _unitOfWork.Repository<Product>().AddAsync(product);

                var result = await _unitOfWork.SaveChangesAsync();
                if (result <= 0)
                {
                    await attachmentService.DeleteAttachmentAsync(imageUrl);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    await attachmentService.DeleteAttachmentAsync(imageUrl);
                }
                return false;
            }
        }

        public Task<bool> DeleteProductAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Product?> ProductDetailsAsync(int Id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateProductAsync(int id, Product Obj)
        {
            throw new NotImplementedException();
        }
    }
}
