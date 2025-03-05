using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFilm.Core.Enitites.SemesterSubject
{
    public class SemesterSubject : BaseEntity
    {
        public int semesterId { get; set; }
        public int subjectId { get; set; }
    }
}
