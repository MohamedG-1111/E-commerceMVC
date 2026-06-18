using Ecommerce.Utility.ResultPattern;

namespace Ecommerce.Utility.Result
{

    // =========================
    // Result (Non Generic)
    // =========================
    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;

        public string? ErrorMessage { get; }
        public string? ErrorKey { get; }
        public ErrorType? ErrorType { get; }

        protected Result(
            bool isSuccess,
            string? errorMessage = null,
            string? key = null,
            ErrorType? errorType = null)
        {
            ResultGuard.ResultValidate(isSuccess, errorMessage);

            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
            ErrorKey = key;
            ErrorType = errorType;
        }

        public static Result Success()
            => new Result(true);

        public static Result Failure(
            string errorMessage,
            string? key = null,
            ErrorType? errorType = null)
            => new Result(false, errorMessage, key, errorType);
    }

    // =========================
    // Result<T> (Generic)
    // =========================
    public class Result<T> : Result
    {
        public T Value { get; }

        protected Result(
            bool isSuccess,
            T value,
            string? errorMessage = null,
            string? key = null,
            ErrorType? errorType = null)
            : base(isSuccess, errorMessage, key, errorType)
        {
            Value = value;
        }

        public static Result<T> Success(T value)
            => new Result<T>(true, value);

        public static Result<T?> Failure(
            string errorMessage,
            string? key = null,
            ErrorType? errorType = null)
            => new Result<T>(false, default, errorMessage, key, errorType);
    }

    // =========================
    // Guard
    // =========================
    public static class ResultGuard
    {
        public static void ResultValidate(bool isSuccess, string? errorMessage)
        {
            if (isSuccess && !string.IsNullOrEmpty(errorMessage))
            {
                throw new ArgumentException(
                    "A successful result cannot have an error message.");
            }

            if (!isSuccess && string.IsNullOrEmpty(errorMessage))
            {
                throw new ArgumentException(
                    "A failed result must have an error message.");
            }
        }
    }
}