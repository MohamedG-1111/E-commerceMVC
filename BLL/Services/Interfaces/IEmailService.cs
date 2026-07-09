using E_commerce.BLL.Common.Dto;
using Ecommerce.Utility.Result;

namespace E_commerce.BLL.Services.Interfaces
{
    public interface IEmailService
    {
        public Task<Result> SendEmailAsync(EmailRequestDto EmailRequestDto);
    }
}
