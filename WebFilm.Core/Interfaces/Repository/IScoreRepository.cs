using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.Points;
using WebFilm.Core.Enitites.Semesters;

namespace WebFilm.Core.Interfaces.Repository
{
    public interface IScoreRepository : IBaseRepository<int, Scores>
    {
    }
}
