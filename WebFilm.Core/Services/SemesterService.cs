using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.Semesters;
using WebFilm.Core.Enitites.SemesterSubject;
using WebFilm.Core.Enitites.Subject;
using WebFilm.Core.Exceptions;
using WebFilm.Core.Interfaces.Repository;
using WebFilm.Core.Interfaces.Services;

namespace WebFilm.Core.Services
{
    public class SemesterService : BaseService<int, Semesters>, ISemesterService
    {
        ISemesterRepository _semesterRepository;
        ISemesterSubjectRepository _semesterSubjectRepository;
        ISubjectRepository _subjectRepository;
        IUserContext _userContext;
        private readonly IConfiguration _configuration;

        public SemesterService(ISemesterRepository semesterRepository, ISemesterSubjectRepository semesterSubjectRepository, ISubjectRepository subjectRepository,
            IConfiguration configuration,
            IUserContext userContext) : base(semesterRepository)
        {
            _configuration = configuration;
            _semesterRepository = semesterRepository;
            _semesterSubjectRepository = semesterSubjectRepository;
            _subjectRepository = subjectRepository;
            _userContext = userContext;
        }

        public bool create(SemesterDTO dto)
        {
            string role = _userContext.Role;
            if (!"ADMIN".Equals(role))
            {
                throw new ServiceException(Resources.Resource.Not_Permission);
            }

            if (dto.subjectIds.Count == 0)
            {
                throw new ServiceException(Resources.Resource.Error_Exception);
            }

            int semesterIdNew = _semesterRepository.create(dto);

            foreach (int subjectId in dto.subjectIds) {
                _semesterSubjectRepository.create(semesterIdNew, subjectId);
            }

            return true;
        }

        public int update(int id, SemesterDTO dto)
        {
            throw new NotImplementedException();
        }

        public int delete(int id)
        {
            throw new NotImplementedException();
        }

        public List<SemesterResponse> findAll()
        {
            List<SemesterResponse> res = new List<SemesterResponse>();

            List<Semesters> semesters = _semesterRepository.GetAll().ToList();

            foreach (var semester in semesters)
            {
                SemesterResponse sr = new SemesterResponse();
                sr.semesterName = semester.semesterName;
                sr.year = semester.year;


                List<int> semesterSubjectIds = _semesterSubjectRepository.GetAll().Where(t => t.semesterId == semester.id).Select(u => u.subjectId).ToList();
                List<Subjects> subjects = _subjectRepository.GetAll().Where(t => semesterSubjectIds.Contains(t.id)).ToList();

                sr.subjects = subjects;

                res.Add(sr);
            }

            return res;


            throw new NotImplementedException();
        }
    }
}
