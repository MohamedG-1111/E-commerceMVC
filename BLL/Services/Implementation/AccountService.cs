using DataAccessLayer.Repositories.Interfaces;
using E_commerce.BLL.Common.Dto;
using E_commerce.BLL.Services.Interfaces;
using E_commerce.BLL.ViewModels;
using E_commerce.DAL.Entities;
using E_commerce.DAL.Entities.Users;
using E_commerce.Utility.Settings;
using Ecommerce.Utility;
using Ecommerce.Utility.Pagination;
using Ecommerce.Utility.Result;
using Ecommerce.Utility.ResultPattern;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace E_commerce.BLL.Services.Implementation
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService emailService;
        private readonly IAttachmentService attachmentService;
        private readonly IUnitOfWork unitOfWork;
        private readonly ICurrentUserService currentUserService;
        private readonly IHttpContextAccessor _httpContextAccessor;




        public AccountService(UserManager<ApplicationUser> _userManager,
            IHttpContextAccessor httpContextAccessor,
            IEmailService emailService,
            IAttachmentService attachmentService,
            IUnitOfWork unitOfWork,
            ICurrentUserService CurrentUserService)
        {
            this._userManager = _userManager;
            this.emailService = emailService;
            this.attachmentService = attachmentService;
            this.unitOfWork = unitOfWork;
            currentUserService = CurrentUserService;
            _httpContextAccessor = httpContextAccessor;

        }
        public async Task<Result> ConfirmEmailAsync(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                return Result.Failure("Invalid confirmation link", errorType: ErrorType.VALIDATION);

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Result.Failure("User not found", errorType: ErrorType.NOT_FOUND);

            if (user.EmailConfirmed)
                return Result.Failure("Email already confirmed", errorType: ErrorType.VALIDATION);
            var decodedToken = Uri.UnescapeDataString(token);


            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    Console.WriteLine(error.Description);
                }
                return Result.Failure("Invalid or expired confirmation link", errorType: ErrorType.VALIDATION);

            }

            return Result.Success();
        }

        public async Task<Result<string>> GetUserEmail(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return Result<string>.Failure("UserId is required", errorType: ErrorType.NOT_FOUND);

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return Result<string>.Failure("User not found", errorType: ErrorType.NOT_FOUND);

            if (string.IsNullOrWhiteSpace(user.Email))
                return Result<string>.Failure("Email not found", errorType: ErrorType.NOT_FOUND);

            return Result<string>.Success(user.Email);
        }

        public async Task<Result> ReSendEmailConfirmationAsync(string emailTo)
        {
            if (string.IsNullOrWhiteSpace(emailTo))
                return Result.Failure("Email is not found", errorType: ErrorType.NOT_FOUND);

            var user = await _userManager.FindByEmailAsync(emailTo);

            if (user == null)
                return Result.Failure("User is not found", errorType: ErrorType.NOT_FOUND);

            if (user.EmailConfirmed)
                return Result.Failure("Email already confirmed", errorType: ErrorType.VALIDATION);


            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = Uri.EscapeDataString(token);

            var request = _httpContextAccessor.HttpContext!.Request;

            var confirmationLink =
            $"{request.Scheme}://{request.Host}/Account/ConfirmEmail?userId={user.Id}&token={encodedToken}";

            // 👇 reuse نفس الميثود
            return await SendEmailConfirmationAsync(user.Email!, confirmationLink);
        }

        public async Task<Result> ResetPasswoAsync(ResetPasswordViewModel model)
        {
            if (model == null || model.Email == null || model.Password == null)
                return Result.Failure("InValid Data", errorType: ErrorType.VALIDATION);
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return Result.Failure("User not found", errorType: ErrorType.NOT_FOUND);

            var result = await _userManager.ResetPasswordAsync(
                user,
                model.Token,
                model.Password);
            await _userManager.UpdateSecurityStampAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return Result.Failure(errors, errorType: ErrorType.VALIDATION);
            }

            return Result.Success();


        }

        public async Task<Result> SendEmailConfirmationAsync(string emailTo, string confirmationLink)
        {
            if (string.IsNullOrWhiteSpace(emailTo))
                return Result.Failure(
                     "Email is not Found",
                    errorType: ErrorType.NOT_FOUND);

            if (string.IsNullOrWhiteSpace(confirmationLink))
                return Result.Failure(
                    "Confirmation link is not Found",
                    errorType: ErrorType.NOT_FOUND);

            var body = EmailTemplateBuilder.BuildActionEmail(
     "Welcome to E-Commerce",
     "Thank you for registering with us. Please confirm your email address to activate your account.",
     "Confirm Email",
     confirmationLink
 );

            var emailRequest = new EmailRequestDto(
                emailTo,
                "Verify your E-Commerce account email",
                body);


            var result = await emailService.SendEmailAsync(emailRequest);
            return result.IsSuccess
                 ? Result.Success()
                 : Result.Failure("Failed to send email", errorType: ErrorType.INTERNAL_ERROR);
        }

        public async Task<Result> SendResetPasswordEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return Result.Failure("Email Is Required", errorType: ErrorType.VALIDATION);

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Result.Failure("Invaild Email Adddress", errorType: ErrorType.VALIDATION);


            if (!user.EmailConfirmed)
                return Result.Failure("Must Confirm Email First", errorType: ErrorType.VALIDATION);

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = Uri.EscapeDataString(token);

            var request = _httpContextAccessor.HttpContext!.Request;

            var resetLink =
                $"{request.Scheme}://{request.Host}/Account/ResetPassword?email={user.Email}&token={encodedToken}";

            var body = EmailTemplateBuilder.BuildActionEmail(
            "Reset Password",
               "Click below to reset your password",
            "Reset Password",
           resetLink
);

            var emailRequest = new EmailRequestDto(
                user.Email!,
                "Reset Password",
                body);

            var result = await emailService.SendEmailAsync(emailRequest);
            return result.IsSuccess
                 ? Result.Success()
                 : Result.Failure("Failed to send email", errorType: ErrorType.INTERNAL_ERROR);
        }


        public async Task<Result> CreateAccountAsync(AccountVM model)
        {
            if (model is null)
                return Result.Failure("Invalid Account Data");

            var existingUser = await _userManager.FindByEmailAsync(model.Email);

            if (existingUser != null)
                return Result.Failure(
                    "User with this email already exists",
                    errorType: ErrorType.VALIDATION);

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                PhoneNumber = model.Phone,
                FirstName = model.FirstName,
                LastName = model.LastName,
                StreetAddress = model.StreetAddress,
                City = model.City,
                PostalCode = model.PostalCode,
                Role = model.Role,
                CompanyId = model.CompanyId,
                EmailConfirmed = true
            };

            try
            {
                if (model.ProfilePicture != null)
                {
                    user.ProfilePicture =
                        await attachmentService.UploadAttachmentAsync(
                            model.ProfilePicture,
                            FileSettings.ImagesPathProfiles);
                }

                var createResult =
                    await _userManager.CreateAsync(user, model.Password);

                if (!createResult.Succeeded)
                {
                    if (user.ProfilePicture != null)
                    {
                        await attachmentService.DeleteAttachmentAsync(
                            user.ProfilePicture,
                            FileSettings.ImagesPathProfiles);
                    }

                    return Result.Failure(
                        string.Join(", ",
                            createResult.Errors.Select(e => e.Description)),
                        errorType: ErrorType.VALIDATION);
                }

                var roleResult =
                    await _userManager.AddToRoleAsync(user, model.Role);

                if (!roleResult.Succeeded)
                {
                    if (user.Id != null)
                        await _userManager.DeleteAsync(user);

                    if (user.ProfilePicture != null)
                    {
                        await attachmentService.DeleteAttachmentAsync(
                            user.ProfilePicture,
                            FileSettings.ImagesPathProfiles);
                    }

                    return Result.Failure(
                        "Failed to assign role",
                        errorType: ErrorType.INTERNAL_ERROR);
                }

                return Result.Success();
            }
            catch
            {
                if (user.ProfilePicture != null)
                {
                    await attachmentService.DeleteAttachmentAsync(
                        user.ProfilePicture,
                        FileSettings.ImagesPathProfiles);
                }

                return Result.Failure(
                    "Registration failed",
                    errorType: ErrorType.INTERNAL_ERROR);
            }
        }


        public async Task<Result<PaginatedResult<AllAccountsViewModel>>> GetAccountsAsync(
       PaginationParameters parameter,
       string? search = null)
        {
            var query = unitOfWork.Repository<ApplicationUser>().GetAsQuery();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x =>
                    x.Email!.Contains(search) ||
                    x.Role!.Contains(search) ||
                    x.FirstName.Contains(search) ||
                    x.LastName.Contains(search));
            }

            var accounts = await query.Select(x => new AllAccountsViewModel
            {
                UserId = x.Id,
                FullName = $"{x.FirstName} {x.LastName}",
                Email = x.Email!,
                Role = x.Role,
                IsLocked = x.LockoutEnd != null &&
                           x.LockoutEnd > DateTimeOffset.UtcNow
            }).ToPagedResultAsync(parameter);

            return Result<PaginatedResult<AllAccountsViewModel>>
                .Success(accounts);
        }

        public async Task<Result<AccountVM>?> GetAccountByUserId(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return Result<AccountVM>.Failure("Account Not Found", errorType: ErrorType.NOT_FOUND);

            if (!currentUserService.IsInRole(Roles.Admin) &&
        currentUserService.UserId != userId)
            {
                return Result<AccountVM>.Failure(
                    "Unauthorized",
                    errorType: ErrorType.UNAUTHORIZED);
            }

            var account = await unitOfWork.Repository<ApplicationUser>().GetAsQuery().FirstOrDefaultAsync(x => x.Id == userId);
            if (account == null)
                return Result<AccountVM>.Failure("Account Not Found", errorType: ErrorType.NOT_FOUND);

            var company = await unitOfWork.Repository<Company>().GetAsQuery()
                .FirstOrDefaultAsync(x => x.Id == account.CompanyId);



            var result = new AccountVM
            {
                FirstName = account.FirstName,
                LastName = account.LastName,
                Email = account.Email!,
                Phone = account.PhoneNumber!,
                StreetAddress = account.StreetAddress,
                City = account.City,
                PostalCode = account.PostalCode,
                CompanyId = account.CompanyId,
                Role = account.Role,
                Image = account.ProfilePicture,
                CompanyName = company?.Name ?? null
            };

            return Result<AccountVM>.Success(result);
        }

        public async Task<Result> LockAccountAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return Result.Failure("Account Not Found", errorType: ErrorType.NOT_FOUND);

            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return Result.Failure("Account Not Found", errorType: ErrorType.NOT_FOUND);

            await _userManager.SetLockoutEndDateAsync(
                user,
                DateTimeOffset.MaxValue
            );
            await _userManager.ResetAccessFailedCountAsync(user);


            return Result.Success();
        }
        public async Task<Result> UnLockAccountAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return Result.Failure("Account Not Found", errorType: ErrorType.NOT_FOUND);

            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return Result.Failure("Account Not Found", errorType: ErrorType.NOT_FOUND);

            await _userManager.SetLockoutEndDateAsync(
                user,
                DateTimeOffset.UtcNow
            );
            await _userManager.ResetAccessFailedCountAsync(user);

            return Result.Success();
        }

        public async Task<Result> DeleteAccountAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return Result.Failure("Account Not Found", errorType: ErrorType.NOT_FOUND);

            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return Result.Failure("Account Not Found", errorType: ErrorType.NOT_FOUND);
            var hasOrders = await unitOfWork.Repository<Order>()
              .AnyAsync(o => o.ApplicationUserId == userId);

            if (hasOrders)
            {
                return Result.Failure("Cannot delete user with existing orders");
            }

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
                return Result.Failure("Can not delete account");


            if (!string.IsNullOrWhiteSpace(user.ProfilePicture))
            {
                await attachmentService.DeleteAttachmentAsync(user.ProfilePicture!, FileSettings.ImagesPathProfiles);
            }

            return Result.Success();

        }

        public async Task<Result> UpdateAccountAsync(string userId, EditAccountVM model)
        {
            if (model is null)
                return Result.Failure("Invalid Account Data");

            if (!currentUserService.IsInRole(Roles.Admin) &&
            currentUserService.UserId != userId)
            {
                return Result.Failure(
                    "Unauthorized",
                    errorType: ErrorType.UNAUTHORIZED);
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return Result.Failure("Account Not Found", errorType: ErrorType.NOT_FOUND);

            var emailExists = await _userManager.FindByEmailAsync(model.Email);

            if (emailExists != null && emailExists.Id != user.Id)
            {
                return Result.Failure(
                    "User with this email already exists",
                    errorType: ErrorType.VALIDATION);
            }

            var oldImage = user.ProfilePicture;

            // update fields
            user.UserName = model.Email;
            user.Email = model.Email;
            user.PhoneNumber = model.Phone;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.StreetAddress = model.StreetAddress;
            user.City = model.City;
            user.PostalCode = model.PostalCode;
            user.CompanyId = model.CompanyId;

            string? newImage = null;

            try
            {
                // 1️⃣ upload image FIRST but keep temp
                if (model.ProfilePicture != null)
                {
                    newImage = await attachmentService.UploadAttachmentAsync(
                        model.ProfilePicture,
                        FileSettings.ImagesPathProfiles);

                    user.ProfilePicture = newImage;
                }

                // 2️⃣ update DB
                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    // rollback uploaded image
                    if (!string.IsNullOrWhiteSpace(newImage))
                    {
                        await attachmentService.DeleteAttachmentAsync(
                            newImage,
                            FileSettings.ImagesPathProfiles);
                    }

                    return Result.Failure(
                        string.Join(", ", result.Errors.Select(e => e.Description)),
                       errorType: ErrorType.VALIDATION);
                }

                // 3️⃣ delete old image AFTER success
                if (!string.IsNullOrWhiteSpace(oldImage) && newImage != null)
                {
                    await attachmentService.DeleteAttachmentAsync(
                        oldImage,
                        FileSettings.ImagesPathProfiles);
                }

                return Result.Success();
            }
            catch
            {
                // rollback new image if something crashes
                if (!string.IsNullOrWhiteSpace(newImage))
                {
                    await attachmentService.DeleteAttachmentAsync(
                        newImage,
                        FileSettings.ImagesPathProfiles);
                }

                return Result.Failure(
                    "Update failed",
                    errorType: ErrorType.INTERNAL_ERROR);
            }
        }
        public async Task<Result<EditAccountVM>> GetAccountToEditAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return Result<EditAccountVM>.Failure("Invalid User Id", errorType: ErrorType.NOT_FOUND);

            if (!currentUserService.IsInRole(Roles.Admin) &&
            currentUserService.UserId != userId)
            {
                return Result<EditAccountVM>.Failure(
                    "Unauthorized",
                   errorType: ErrorType.UNAUTHORIZED);
            }

            var user = await _userManager.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (user is null)
                return Result<EditAccountVM>.Failure("Account Not Found", errorType: ErrorType.NOT_FOUND);

            var model = new EditAccountVM
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email!,
                Phone = user.PhoneNumber!,
                StreetAddress = user.StreetAddress,
                City = user.City,
                PostalCode = user.PostalCode,
                CompanyId = user.CompanyId,
                ExistingImage = user.ProfilePicture,
                Role = user?.Role,

            };

            return Result<EditAccountVM>.Success(model);
        }

        public async Task<Result> UpdateCheckoutInfo(UpdateCheckoutInfoVM model)
        {
            var userId = currentUserService.UserId;
            if (string.IsNullOrWhiteSpace(userId))
                return Result.Failure("Must be Login First", errorType: ErrorType.UNAUTHORIZED);


            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return Result.Failure("User Not Found", errorType: ErrorType.NOT_FOUND);
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.City = model.City;
            user.PostalCode = model.PostalCode;
            user.StreetAddress = model.StreetAddress;
            user.PhoneNumber = model.PhoneNumber;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return Result.Failure("Can not Update CheckoutInfo ", errorType: ErrorType.INTERNAL_ERROR);

            }

            return Result.Success();

        }




    }
}





