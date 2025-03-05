using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.Subject;
using WebFilm.Core.Enitites.User;

namespace WebFilm.Core.Interfaces.Repository
{
    public interface ISubjectRepository : IBaseRepository<int, Subjects>
    {
        bool create(SubjectDTO dto);

        int update(int id, SubjectDTO dto);
    }
}
