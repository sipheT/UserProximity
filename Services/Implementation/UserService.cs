using UserProximity.Models;
using UserProximity.Services.Interface;

namespace UserProximity.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly List<User> _users = new();

        public IEnumerable<User> GetUsers() => _users;

        public User AddUser(User user)
        {
            user.Id = _users.Count > 0 ? _users.Max(u => u.Id) + 1 : 1;
            _users.Add(user);
            return user;
        }

        public User UpdateUser(int id, User user)
        {
            var existing = _users.FirstOrDefault(u => u.Id == id);
            if (existing == null) return null;
            // Update properties as needed
            existing.Name = user.Name;
            existing.Email = user.Email;
            // etc.
            return existing;
        }

        public bool DeleteUser(int id)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            if (user == null) return false;
            _users.Remove(user);
            return true;
        }
    }
}