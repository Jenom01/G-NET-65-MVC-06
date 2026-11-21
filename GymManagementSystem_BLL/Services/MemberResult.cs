namespace GymManagementSystem_BLL.Services
{
    public enum MemberResult
    {
        Success,
        EmailExists,
        PhoneExists,
        NotFound,
        HasActiveBookings
    }
}
