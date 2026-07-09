namespace E_commerce.BLL.Services.Interfaces
{
    public interface IDailyJobService
    {
        public Task CleanUpUnpaidOrdersOlderThanOneMonthAsync();
    }
}
