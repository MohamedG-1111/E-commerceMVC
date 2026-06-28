namespace Ecommerce.Utility.Pagination
{
    public interface IPaginatedResult
    {
        int PageNumber { get; }

        int PageSize { get; }

        int TotalCount { get; }

        int TotalPages { get; }

        bool HasPrevious { get; }

        bool HasNext { get; }
    }
}
