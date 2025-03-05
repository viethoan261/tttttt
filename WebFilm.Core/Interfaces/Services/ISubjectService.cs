using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.Subject;
using WebFilm.Core.Enitites.User;

namespace WebFilm.Core.Interfaces.Services
{
    public interface ISubjectService : IBaseService<int, Subjects>
    {
        bool create(SubjectDTO dto);
        int update(int id, SubjectDTO dto);
        int delete(int id);
        List<Subjects> findAll();
    }
}
