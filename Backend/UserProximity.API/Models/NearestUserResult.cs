using UserProximity.Models;

namespace UserProximity.API.Models
{
    public class NearestUserResult
    {
        public string Hotel { get; set; }
        public User NearestUser { get; set; }
    }
}
