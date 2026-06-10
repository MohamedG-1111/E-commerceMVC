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


        public async Task<IEnumerable<ProductListVm>?> AllProductsAsync(string? searchTerm)
        {
            var productsQuery =
                _unitOfWork.ProductRepository.GetAsQuery();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                productsQuery = _unitOfWork.ProductRepository
                    .Search(searchTerm);
            }

            return await productsQuery
                .Select(x => new ProductListVm
                {
                    Id = x.Id,
                    Title = x.Title,
                    ISBN = x.ISBN,
                    Author = x.Author,
                    ListPrice = x.ListPrice,
                    CategoryName = x.Category.Name,
                    ImageUrl = x.ImageUrl
                })
                .ToListAsync();
        }
        public async Task<bool> CreateProductAsync(CreateOrUpdateProductViewModel obj)
        {
            if (obj == null || obj.Product == null)
                return false;

            string imageUrl = string.Empty;

            try
            {
                imageUrl = await attachmentService.UploadAttachmentAsync(obj.Cover);

                var product = obj.Product;
                product.ImageUrl = imageUrl;

                await _unitOfWork.ProductRepository.AddAsync(product);

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

        public async Task<bool> DeleteProductAsync(int id)
        {
            var productFromDb = await _unitOfWork.ProductRepository.FindAsync(id);
            if (productFromDb == null) return false;
            var ImageUrl = productFromDb.ImageUrl;

            _unitOfWork.ProductRepository.Delete(productFromDb);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result >= 1)
            {
                if (!string.IsNullOrEmpty(productFromDb.ImageUrl))
                    await attachmentService.DeleteAttachmentAsync(ImageUrl);
                return true;

            }
            return false;
        }

        public async Task<Product?> ProductDetailsAsync(int Id)
        {
            return await _unitOfWork.ProductRepository.GetAsQuery()
                .Include(x => x.Category)
                .FirstOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<bool> UpdateProductAsync(CreateOrUpdateProductViewModel obj)
        {
            if (obj == null || obj.Product == null || obj.Product.Id == 0)
                return false;

            var productFromDb = await _unitOfWork.ProductRepository.FindAsync(obj.Product.Id);
            if (productFromDb == null) return false;

            string oldImage = productFromDb.ImageUrl;
            string newImage = string.Empty;

            try
            {
                if (obj.Cover != null)
                    newImage = await attachmentService.UploadAttachmentAsync(obj.Cover);

                productFromDb.Title = obj.Product.Title;
                productFromDb.ISBN = obj.Product.ISBN;
                productFromDb.Description = obj.Product.Description;
                productFromDb.CategoryId = obj.Product.CategoryId;
                productFromDb.ListPrice = obj.Product.ListPrice;
                productFromDb.PriceFor1To50 = obj.Product.PriceFor1To50;
                productFromDb.PriceFor50Plus = obj.Product.PriceFor50Plus;
                productFromDb.PriceFor100Plus = obj.Product.PriceFor100Plus;
                productFromDb.ImageUrl = !string.IsNullOrEmpty(newImage) ? newImage : oldImage;

                _unitOfWork.ProductRepository.Update(productFromDb);
                var result = await _unitOfWork.SaveChangesAsync();

                if (result <= 0)
                {
                    if (!string.IsNullOrEmpty(newImage))
                        await attachmentService.DeleteAttachmentAsync(newImage);
                    return false;
                }

                if (!string.IsNullOrEmpty(newImage) && !string.IsNullOrEmpty(oldImage))
                    await attachmentService.DeleteAttachmentAsync(oldImage);

                return true;
            }
            catch
            {
                if (!string.IsNullOrEmpty(newImage))
                    await attachmentService.DeleteAttachmentAsync(newImage);
                return false;
            }
        }
    }
}
