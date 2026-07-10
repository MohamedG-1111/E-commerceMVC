using Twilio.Rest.Api.V2010.Account;

namespace E_commerce.BLL.Services.Interfaces
{
    public interface ISMSService
    {
        Task<MessageResource> SendSMSAsync(string phoneNumber, string message);
    }
}
