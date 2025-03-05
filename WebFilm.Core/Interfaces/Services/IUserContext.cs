using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.User;

namespace WebFilm.Core.Interfaces.Services
{
    public interface IUserContext
    {
        int UserID { get; }
        string UserName { get; }
        string Role { get; }
        string FullName { get; }
    }
}
