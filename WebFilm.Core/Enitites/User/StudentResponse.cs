using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.Points;

namespace WebFilm.Core.Enitites.User
{
    public class StudentResponse
    {
        public int id { get; set; }
        public string fullName { get; set; }
        public string className { get; set; }

        public List<Scores>? scores { get; set; }
    }
}
