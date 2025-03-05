using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFilm.Core.Enitites.Subject
{
    public class SubjectDTO
    {
        public string subjectCode { get; set; }

        public string? subjectName { get; set; }

        public int creditNumber { get; set; }

        public string desscription { get; set; }
    }
}
