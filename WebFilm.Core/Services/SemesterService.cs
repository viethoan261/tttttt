using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.Semesters;
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
        IUserContext _userContext;
        private readonly IConfiguration _configuration;

        public SemesterService(ISemesterRepository semesterRepository, ISemesterSubjectRepository semesterSubjectRepository,
            IConfiguration configuration,
            IUserContext userContext) : base(semesterRepository)
        {
            _configuration = configuration;
            _semesterRepository = semesterRepository;
            _semesterSubjectRepository = semesterSubjectRepository;
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

        public List<Semesters> findAll()
        {
            throw new NotImplementedException();
        }
    }
}
