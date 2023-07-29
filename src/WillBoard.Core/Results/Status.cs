namespace WillBoard.Core.Results
{
    public readonly struct Status<TError>
    {
        public readonly bool Success;

        public readonly TError Error;

        private Status(TError error)
        {
            Success = false;
            Error = error;
        }

        private Status(bool success)
        {
            Success = success;
            Error = default(TError);
        }

        public static Status<TError> ErrorStatus(TError error)
        {
            return new Status<TError>(error);
        }

        private static Status<TError> successStatus = new Status<TError>(true);

        public static Status<TError> SuccessStatus()
        {
            return successStatus;
        }
    }
}