namespace GymManagementSystem_BLL.Results
{
    public enum ErrorType
    {
        None,
        NotFound,
        Conflict,
        Validation,
        Failure
    }

    public sealed record Error(string Code, string Message, ErrorType Type, string? Field = null)
    {
        public static readonly Error None = new(string.Empty, string.Empty, ErrorType.None);
    }
}
