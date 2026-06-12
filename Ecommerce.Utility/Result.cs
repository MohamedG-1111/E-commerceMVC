namespace Ecommerce.Utility
{
    public class Result
    {
        public bool IsSuccess { get; set; }

        public bool IsFailure => !IsSuccess;

        public string? ErrorMessage { get; set; }

        public string? ErrorKey { get; set; }
        protected Result(bool isSuccess, string errorMessage = null, string? Key = null)
        {
            ResultGuard.ResultValidate(isSuccess, errorMessage);
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
            ErrorKey = Key;
        }

        public static Result Success() => new Result(true);
        public static Result Failure(string errorMessage, string? Key = null)
            => new Result(false, errorMessage, Key);

    }
    public class Result<T> : Result
    {
        public T Value { get; set; }
        protected Result(bool isSuccess, T? value, string? errorMessage = null, string? Key = null)
            : base(isSuccess, errorMessage, Key)
        {
            Value = value;
        }
        public static Result<T> Success(T value) => new Result<T>(true, value);
        public static Result<T> Failure(string errorMessage, string? Key = null)
            => new Result<T>(false, default(T), errorMessage, Key);
    }


    public static class ResultGuard
    {
        public static void ResultValidate(bool isSuccess, string errorMessage)
        {
            if (isSuccess && !string.IsNullOrEmpty(errorMessage))
            {
                throw new ArgumentException("A successful result cannot have an error message.");
            }
            if (!isSuccess && string.IsNullOrEmpty(errorMessage))
            {
                throw new ArgumentException("A failed result must have an error message.");
            }
        }
    }
}