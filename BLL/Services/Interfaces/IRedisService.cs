namespace E_commerce.BLL.Services.Interfaces
{
    public interface IRedisService
    {
        public Task<T?> SetAsync<T>(string key, T value, TimeSpan? TTL = null);

        public Task<T?> GetAsync<T>(string key);


        public Task<bool> DeleteAsync(string key);


    }
}
