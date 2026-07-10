using E_commerce.BLL.Services.Interfaces;
using Ecommerce.Utility.Settings;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace E_commerce.BLL.Services.Implementation
{
    public class SMSService : ISMSService
    {
        private readonly TwilioSettings twilioSettings;

        public SMSService(IOptions<TwilioSettings> twilioSettings)
        {
            this.twilioSettings = twilioSettings.Value;
        }
        public async Task<MessageResource> SendSMSAsync(string phoneNumber, string message)
        {
            TwilioClient.Init(twilioSettings.AccountSID, twilioSettings.AuthToken);

            var result = await MessageResource.CreateAsync(
    body: message,
    from: new Twilio.Types.PhoneNumber(twilioSettings.TwilioPhoneNumber),
    to: new Twilio.Types.PhoneNumber(phoneNumber));

            Console.WriteLine(result.Sid);
            Console.WriteLine(result.Status);

            return result;
        }
    }
}
