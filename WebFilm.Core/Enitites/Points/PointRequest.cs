using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFilm.Core.Enitites.Points
{
    public class PointRequest
    {
        public int subjectId { get; set; }
        public double midtermScore { get; set; }
        public double finalScore { get; set; }
    }
}
