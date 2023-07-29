namespace WillBoard.Core.Results
{
    public readonly struct Result<TValue>
    {
        public readonly bool Success;

        public readonly TValue Value;

        private Result(TValue result)
        {
            Success = true;
            Value = result;
        }

        private Result(bool result)
        {
            Success = false;
            Value = default(TValue);
        }

        public static Result<TValue> ValueResult(TValue value)
        {
            return new Result<TValue>(value);
        }

        public static Result<TValue> ErrorResult()
        {
            return new Result<TValue>(false);
        }
    }

    public readonly struct Result<TValue, TError>
    {
        public readonly bool Success;

        public readonly TValue Value;
        public readonly TError Error;

        private Result(TValue result)
        {
            Success = true;
            Value = result;
            Error = default(TError);
        }

        private Result(TError error)
        {
            Success = false;
            Value = default(TValue);
            Error = error;
        }

        public static Result<TValue, TError> ValueResult(TValue value)
        {
            return new Result<TValue, TError>(value);
        }

        public static Result<TValue, TError> ErrorResult(TError error)
        {
            return new Result<TValue, TError>(error);
        }
    }
}