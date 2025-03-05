using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.Semesters;
using WebFilm.Core.Enitites.Subject;

namespace WebFilm.Core.Interfaces.Repository
{
    public interface ISemesterRepository : IBaseRepository<int, Semesters>
    {
        int create(SemesterDTO dto);

        int update(int id, SemesterDTO dto);
    }
}
