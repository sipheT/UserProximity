using UserProximity.API.Models;

namespace UserProximity.API.Services.Interface
{
    public interface INearestUserService
    {
        Task<IEnumerable<NearestUserResult>> GetNearestUsersAsync();
    }
}
