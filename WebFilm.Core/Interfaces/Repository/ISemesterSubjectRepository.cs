using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.Semesters;
using WebFilm.Core.Enitites.SemesterSubject;

namespace WebFilm.Core.Interfaces.Repository
{
    public interface ISemesterSubjectRepository : IBaseRepository<int, SemesterSubject>
    {
        bool create(int sesmesterId, int subjectId);
    }
}
