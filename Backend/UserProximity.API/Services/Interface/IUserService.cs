using UserProximity.Models;

namespace UserProximity.Services.Interface
{
    public interface IUserService
    {
        IEnumerable<User> GetUsers();
        User AddUser(User user);
        User UpdateUser(int id, User user);
        bool DeleteUser(int id);
    }
}