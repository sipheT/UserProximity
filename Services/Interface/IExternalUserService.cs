using UserProximity.Models;

namespace UserProximity.Services.Interface
{
    public interface IExternalUserService
    {
        Task<IEnumerable<User>> GetUsers();
    }
}