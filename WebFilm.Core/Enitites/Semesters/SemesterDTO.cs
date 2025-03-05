using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFilm.Core.Enitites.Semesters
{
    public class SemesterDTO
    {
        public string semesterName { get; set; }
        public int year { get; set; }

        public List<int> subjectIds { get; set; }
    }
}
