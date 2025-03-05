using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFilm.Core.Enitites.User
{
    public class ChangeProfileDTO
    {
        public string fullName { get; set; }

        public float salary { get; set; }
        
        public string image { get; set; }
    }
}
