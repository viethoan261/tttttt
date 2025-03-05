using WebFilm.Core.Enitites;
using WebFilm.Core.Enitites.Staff;
using WebFilm.Core.Enitites.User;

namespace WebFilm.Core.Interfaces.Services
{
    public interface IUserService : IBaseService<int, Users>
    {
        Users GetUserByID(int userID);

        Users Signup(UserDTO user);

        Dictionary<string, object> Login(string username, string password);

        Users changeProfile(string username, ChangeProfileDTO dto);

        Users getProfile(string username);

        bool deleteStaff(int id);

        List<Users> getAllStaff();
    }
}
