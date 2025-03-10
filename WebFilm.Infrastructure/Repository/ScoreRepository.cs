using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.Points;
using WebFilm.Core.Enitites.Semesters;
using WebFilm.Core.Interfaces.Repository;

namespace WebFilm.Infrastructure.Repository
{
    public class ScoreRepository : BaseRepository<int, Scores>, IScoreRepository
    {
        public ScoreRepository(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
