using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.Points;
using WebFilm.Core.Enitites.Semesters;
using WebFilm.Core.Interfaces.Repository;
using WebFilm.Core.Interfaces.Services;

namespace WebFilm.Core.Services
{
    public class ScoreService : BaseService<int, Scores>, IScoreService
    {
        IScoreRepository _scoreRepository;
        ISemesterSubjectRepository _semesterSubjectRepository;
        ISubjectRepository _subjectRepository;
        IUserContext _userContext;
        private readonly IConfiguration _configuration;

        public ScoreService(IScoreRepository scoreRepository, ISemesterSubjectRepository semesterSubjectRepository, ISubjectRepository subjectRepository,
            IConfiguration configuration,
            IUserContext userContext) : base(scoreRepository)
        {
            _configuration = configuration;
            _scoreRepository = scoreRepository;
            _semesterSubjectRepository = semesterSubjectRepository;
            _subjectRepository = subjectRepository;
            _userContext = userContext;
        }

        public bool updateScores(int studentId, int semesterId, List<PointRequest> request)
        {
            if (request == null || request.Count == 0)
            {
                return false;
            }

            _scoreRepository.delete(semesterId, studentId);

            foreach (PointRequest pointRequest in request)
            {
                Scores score = new Scores();

                score.studentId = studentId;
                score.semesterId = semesterId;
                score.subjectId = pointRequest.subjectId;
                score.midtermScore = pointRequest.midtermScore;
                score.finalScore = pointRequest.finalScore;
                score.createdDate = DateTime.Now;
                score.modifiedDate = DateTime.Now;

                _scoreRepository.Add(score);
            }

            return true;
        }
    }
}
