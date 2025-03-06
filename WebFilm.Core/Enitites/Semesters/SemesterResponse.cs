using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.Subject;

namespace WebFilm.Core.Enitites.Semesters
{
    public class SemesterResponse
    {
        public string semesterName { get; set; }
        public int year { get; set; }

        public List<Subjects> subjects { get; set; }
    }
}
