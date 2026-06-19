namespace E_commerce.BLL.ViewModels
{
    public class AllAccountsViewModel
    {
        public string UserId { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Role { get; set; } = null!;

        public bool IsLocked { get; set; }
    }
}
