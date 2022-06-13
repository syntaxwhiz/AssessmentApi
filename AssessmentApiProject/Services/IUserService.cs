using AssessmentApiProject.Models;

namespace AssessmentApiProject.Services
{
    public interface IUserService
    {
        Task<string> AddUser(User user);
        Task<string> UpdateUser(string name, User user);
        Task<string> DeleteUser(string name);
        Task<List<User>> GetAllUsers();
    }
}
