using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.Points;
using WebFilm.Core.Enitites.Semesters;

namespace WebFilm.Core.Interfaces.Services
{
    public interface IScoreService : IBaseService<int, Scores>
    {
        bool updateScores(int studentId, int semesterId, List<PointRequest> request);
    }
}
