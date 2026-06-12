using DataAccessLayer.Repositories.Interfaces;
using E_commerce.BLL.Services.Interfaces;
using E_commerce.BLL.ViewModels;
using E_commerce.DAL.Entities.Users;
using E_commerce.Utility.Settings;
using Ecommerce.Utility;
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


        public async Task<bool> LoginAsync(LoginViewModel model)
        {
            if (model == null)
                return false;
            var result = await signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                model.RememberMe,
                false
            );
            if (!result.Succeeded)
                return false;

            return true;
        }



        public async Task<bool> RegisterAsync(RegisterationViewModel model)
        {
            if (model == null) return false;
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
                var result = await userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    await attachmentService.DeleteAttachmentAsync(user.ProfilePicture, FileSettings.ImagesPathProfiles);
                    return false;
                }
                await userManager.AddToRoleAsync(user, Roles.Customer);
                await signInManager.SignInAsync(user, isPersistent: false);


                return true;
            }
            catch (Exception ex)
            {
                await userManager.DeleteAsync(user);
                await attachmentService.DeleteAttachmentAsync(user.ProfilePicture, FileSettings.ImagesPathProfiles);
                return false;
            }
        }

        Task<bool> IAuthService.LogOutAsync()
        {
            throw new NotImplementedException();
        }
    }
}
