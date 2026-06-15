using E_commerce.BLL.Dto;
using E_commerce.BLL.Services.Interfaces;
using Ecommerce.Utility.Result;
using Ecommerce.Utility.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace E_commerce.BLL.Services.Implementation
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public async Task<Result> SendEmailAsync(EmailRequestDto EmailRequestDto)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(
                _settings.DisplayName ?? "E-Commerce",
                _settings.Email));

            message.To.Add(MailboxAddress.Parse(EmailRequestDto.To));

            message.Subject = EmailRequestDto.Subject;

            var builder = new BodyBuilder
            {
                HtmlBody = EmailRequestDto.Body,
                TextBody = "Please view this email in HTML format"
            };

            message.Body = builder.ToMessageBody();

            try
            {
                using var smtp = new SmtpClient();

                smtp.Timeout = 20000;


                await smtp.ConnectAsync(
                    _settings.Host,
                    _settings.Port,
                    SecureSocketOptions.StartTls);

                await smtp.AuthenticateAsync(
                    _settings.Email,
                    _settings.Password);

                await smtp.SendAsync(message);

                await smtp.DisconnectAsync(true);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }
    }
}