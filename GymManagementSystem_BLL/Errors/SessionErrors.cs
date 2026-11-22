using GymManagementSystem_BLL.Results;

namespace GymManagementSystem_BLL.Errors
{
    public static class SessionErrors
    {
        public static readonly Error NotFound =
            new("Session.NotFound", "Session not found.", ErrorType.NotFound);
    }
}
