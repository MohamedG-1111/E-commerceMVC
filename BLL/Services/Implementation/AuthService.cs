using DataAccessLayer.Repositories.Interfaces;
using E_commerce.BLL.Dto;
using E_commerce.BLL.Services.Interfaces;
using E_commerce.BLL.ViewModels;
using E_commerce.DAL.Entities.Users;
using E_commerce.Utility.Settings;
using Ecommerce.Utility;
using Ecommerce.Utility.Result;
using Ecommerce.Utility.ResultPattern;
using Microsoft.AspNetCore.Identity;

namespace E_commerce.BLL.Services.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IUnitOfWork unitOfWork;
        private readonly IAttachmentService attachmentService;

        public AuthService(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager
            , IUnitOfWork unitOfWork, IAttachmentService attachmentService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.unitOfWork = unitOfWork;
            this.attachmentService = attachmentService;
        }


        public async Task<Result> LoginAsync(LoginViewModel model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return Result.Failure("Invalid login credentials", errorType: ErrorType.VALIDATION);

            if (!user.EmailConfirmed)
                return Result.Failure("Please confirm your email first", errorType: ErrorType.VALIDATION);

            if (model == null)
                return Result.Failure("Invalid login credentials", errorType: ErrorType.VALIDATION);
            var result = await signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                model.RememberMe,
                false
            );
            if (!result.Succeeded)
                return Result.Failure("Invalid login credentials", errorType: ErrorType.VALIDATION);

            return Result.Success();
        }



        public async Task<Result<RegisterResultDto>> RegisterAsync(RegisterationViewModel model)
        {
            if (model == null)
                return Result<RegisterResultDto>.Failure("Invalid registration data", errorType: ErrorType.VALIDATION);
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
            };
            if (model.ProfilePicture != null)
            {
                var profilePictureUrl = await attachmentService.UploadAttachmentAsync(model.ProfilePicture, FileSettings.ImagesPathProfiles);
                user.ProfilePicture = profilePictureUrl;
            }
            try
            {
                var existingUser = await userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    return Result<RegisterResultDto>.Failure("User with this email already exists", errorType: ErrorType.VALIDATION);
                }
                var result = await userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    await attachmentService.DeleteAttachmentAsync(user.ProfilePicture, FileSettings.ImagesPathProfiles);
                    return Result<RegisterResultDto>.Failure("Registration failed", errorType: ErrorType.VALIDATION);
                }


                var roleResult = await userManager.AddToRoleAsync(user, Roles.Customer);

                if (!roleResult.Succeeded)
                {
                    await userManager.DeleteAsync(user);

                    return Result<RegisterResultDto>.Failure(
                        "Failed to assign role",
                        errorType: ErrorType.INTERNAL_ERROR);
                }

                var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                var encodedToken = Uri.EscapeDataString(token);


                return Result<RegisterResultDto>.Success(new RegisterResultDto(user.Id, user.Email, encodedToken));
            }
            catch (Exception ex)
            {
                await userManager.DeleteAsync(user);
                await attachmentService.DeleteAttachmentAsync(user.ProfilePicture, FileSettings.ImagesPathProfiles);
                return Result<RegisterResultDto>.Failure("Registration failed", errorType: ErrorType.INTERNAL_ERROR);
            }
        }

        public async Task<Result> LogOutAsync()
        {
            try
            {
                await signInManager.SignOutAsync();

                return Result.Success();
            }
            catch
            {
                return Result.Failure(
                    "Failed to log out",
                    errorType: ErrorType.INTERNAL_ERROR);
            }
        }
    }
}
