namespace Ecommerce.Utility.Pagination
{
    public class PaginationParameters
    {
        private const int MaxPageSize = 20;

        private int _pageNumber = 1;
        private int _pageSize = 8;

        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = value < 1 ? 1 : value;
        }

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value switch
            {
                < 1 => 8,
                > MaxPageSize => MaxPageSize,
                _ => value
            };
        }
    }
}