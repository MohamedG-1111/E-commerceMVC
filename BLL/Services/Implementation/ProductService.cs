using BLL.Services.Interfaces;
using DataAccessLayer.Repositories.Interfaces;
using E_commerce.BLL.Services.Interfaces;
using E_commerce.BLL.ViewModels;
using E_commerce.Utility.Settings;
using Ecommerce.Utility.Pagination;
using Ecommerce.Utility.Result;
using Ecommerce.Utility.ResultPattern;
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



        public async Task<Result<PaginatedResult<ProductListVm>?>> AllProductsAsync(PaginationParameters parameters,
            string? searchTerm = null, string? category = null)
        {
            var query = (string.IsNullOrWhiteSpace(searchTerm) && string.IsNullOrWhiteSpace(category))
                ? _unitOfWork.ProductRepository.GetAsQuery()
                : _unitOfWork.ProductRepository.Search(searchTerm, category);

            var products = await query.Select(x => new ProductListVm
            {
                Id = x.Id,
                Title = x.Title,
                ISBN = x.ISBN,
                Author = x.Author,
                ListPrice = x.ListPrice,
                CategoryName = x.Category.Name,
                ImageUrl = x.ImageUrl
            }).ToPagedResultAsync(parameters);

            return Result<PaginatedResult<ProductListVm>>.Success(products);
        }



        public async Task<Result> CreateProductAsync(CreateOrUpdateProductViewModel obj)
        {
            if (obj == null || obj.Product == null)
                return Result.Failure("Invalid product data.", errorType: ErrorType.VALIDATION);

            if (obj.Cover is null)
            {
                return Result.Failure(
                    "Product image is required.",
                    errorType: ErrorType.VALIDATION);
            }

            string imageUrl = string.Empty;


            try
            {

                imageUrl = await attachmentService.UploadAttachmentAsync(
                       obj.Cover,
                       FileSettings.ImagesPathProducts);

                obj.Product.ImageUrl = imageUrl;

                await _unitOfWork.ProductRepository.AddAsync(obj.Product);

                var result = await _unitOfWork.SaveChangesAsync();
                if (result <= 0)
                {
                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        await attachmentService.DeleteAttachmentAsync(
                            imageUrl,
                            FileSettings.ImagesPathProducts);
                    }
                    return Result.Failure("Failed to create product.", errorType: ErrorType.INTERNAL_ERROR);
                }

                return Result.Success();
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    await attachmentService.DeleteAttachmentAsync(imageUrl, FileSettings.ImagesPathProducts);
                }
                return Result.Failure("Failed to create product.", errorType: ErrorType.INTERNAL_ERROR);
            }
        }

        public async Task<Result> DeleteProductAsync(int id)
        {
            if (id <= 0)
                return Result.Failure("Invalid product ID.", errorType: ErrorType.NOT_FOUND);
            var productFromDb = await _unitOfWork.ProductRepository.FindAsync(id);
            if (productFromDb == null)
                return Result.Failure("Product not found.", errorType: ErrorType.NOT_FOUND);
            var ImageUrl = productFromDb.ImageUrl;

            _unitOfWork.ProductRepository.Delete(productFromDb);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result >= 1)
            {
                if (!string.IsNullOrEmpty(productFromDb.ImageUrl))
                    await attachmentService.DeleteAttachmentAsync(ImageUrl, FileSettings.ImagesPathProducts);
                return Result.Success();

            }
            return Result.Failure("Failed to delete product.", errorType: ErrorType.INTERNAL_ERROR);
        }

        public async Task<Result<ProductDetailsVM?>> ProductDetailsAsync(int Id)
        {
            if (Id <= 0)
                return Result<ProductDetailsVM?>.Failure("Invalid product ID.", errorType: ErrorType.NOT_FOUND);

            var product = await _unitOfWork.ProductRepository.GetAsQuery()
                .Include(x => x.Category)
                .FirstOrDefaultAsync(x => x.Id == Id);
            if (product == null)
                return Result<ProductDetailsVM?>.Failure("Product not found.", errorType: ErrorType.NOT_FOUND);

            var productDetailsVM = new ProductDetailsVM
            {
                Product = product,
                AddToCart = new AddToCartViewModel
                {
                    ProductId = product.Id,
                    Count = 1
                }
            };

            return Result<ProductDetailsVM?>.Success(productDetailsVM);
        }

        public async Task<Result> UpdateProductAsync(CreateOrUpdateProductViewModel obj)
        {
            if (obj == null || obj.Product == null)
                return Result.Failure("Invalid product data.", errorType: ErrorType.VALIDATION);
            if (obj.Product.Id <= 0)
                return Result.Failure("Invalid product ID.", errorType: ErrorType.NOT_FOUND);


            var productFromDb = await _unitOfWork.ProductRepository.FindAsync(obj.Product.Id);
            if (productFromDb == null)
                return Result.Failure("Product not found.", errorType: ErrorType.NOT_FOUND);

            string oldImage = productFromDb.ImageUrl;
            string? newImage = null;
            try
            {
                if (obj.Cover != null)
                    newImage = await attachmentService.UploadAttachmentAsync(obj.Cover, FileSettings.ImagesPathProducts);

                productFromDb.Title = obj.Product.Title;
                productFromDb.ISBN = obj.Product.ISBN;
                productFromDb.Description = obj.Product.Description;
                productFromDb.CategoryId = obj.Product.CategoryId;
                productFromDb.ListPrice = obj.Product.ListPrice;
                productFromDb.PriceFor1To50 = obj.Product.PriceFor1To50;
                productFromDb.PriceFor50Plus = obj.Product.PriceFor50Plus;
                productFromDb.PriceFor100Plus = obj.Product.PriceFor100Plus;
                productFromDb.Author = obj.Product.Author;
                productFromDb.Stock = obj.Product.Stock;
                productFromDb.ImageUrl = !string.IsNullOrEmpty(newImage) ? newImage : oldImage;

                _unitOfWork.ProductRepository.Update(productFromDb);
                var result = await _unitOfWork.SaveChangesAsync();

                if (result <= 0)
                {
                    if (!string.IsNullOrEmpty(newImage))
                        await attachmentService.DeleteAttachmentAsync(newImage, FileSettings.ImagesPathProducts);
                    return Result.Failure("Failed to update product.", errorType: ErrorType.INTERNAL_ERROR);
                }

                if (!string.IsNullOrEmpty(newImage) && !string.IsNullOrEmpty(oldImage))
                    await attachmentService.DeleteAttachmentAsync(oldImage, FileSettings.ImagesPathProducts);

                return Result.Success();
            }
            catch
            {
                if (!string.IsNullOrEmpty(newImage))
                    await attachmentService.DeleteAttachmentAsync(newImage, FileSettings.ImagesPathProducts);
                return Result.Failure("Failed to update product.", errorType: ErrorType.INTERNAL_ERROR);
            }
        }
    }
}
