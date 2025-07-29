using ResourceTrackerBackend.Models;

namespace ResourceTrackerBackend.DAO
{
    public interface IAuthService
    {
        User? GetUserByEmail(string email);
        bool ValidatePassword(User user, string password);
        string GenerateJwtToken(User user);
        bool RegisterUser(User user, string password);
    }
}
