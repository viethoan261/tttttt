using WebFilm.Core.Enitites;
using WebFilm.Core.Enitites.Staff;
using WebFilm.Core.Enitites.User;

namespace WebFilm.Core.Interfaces.Repository
{
    public interface IUserRepository : IBaseRepository<int, Users>
    {
        Users GetUserByID(int staffID);

        Users Signup(UserDTO staff);

        Users Login(string username);

        Users changeProfile(string username, ChangeProfileDTO dto);

        Users getUserByUsername(string username);

        bool CheckDuplicateUsername(string username);

    }
}
