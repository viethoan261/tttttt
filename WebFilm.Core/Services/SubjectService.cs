using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.Subject;
using WebFilm.Core.Enitites.User;
using WebFilm.Core.Exceptions;
using WebFilm.Core.Interfaces.Repository;
using WebFilm.Core.Interfaces.Services;

namespace WebFilm.Core.Services
{
    public class SubjectService : BaseService<int, Subjects>, ISubjectService
    {
        ISubjectRepository _subjectRepository;
        IUserContext _userContext;
        private readonly IConfiguration _configuration;

        public SubjectService(ISubjectRepository subjectRepository,
            IConfiguration configuration,
            IUserContext userContext) : base(subjectRepository)
        {
            _configuration = configuration;
            _subjectRepository = subjectRepository;
            _userContext = userContext;
        }

        public bool create(SubjectDTO dto)
        {
            string role = _userContext.Role;
            if (!"ADMIN".Equals(role))
            {
                throw new ServiceException(Resources.Resource.Not_Permission);
            }

            return _subjectRepository.create(dto);
        }

        public int delete(int id)
        {
            string role = _userContext.Role;
            if (!"ADMIN".Equals(role))
            {
                throw new ServiceException(Resources.Resource.Not_Permission);
            }

            return _subjectRepository.Delete(id);
        }

        public List<Subjects> findAll()
        {
            return _subjectRepository.GetAll().ToList();
        }

        public int update(int id, SubjectDTO dto)
        {
            string role = _userContext.Role;
            if (!"ADMIN".Equals(role))
            {
                throw new ServiceException(Resources.Resource.Not_Permission);
            }

            return _subjectRepository.update(id, dto);
        }
    }
}
