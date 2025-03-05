using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebFilm.Core.Enitites.Subject;
using WebFilm.Core.Enitites.User;
using WebFilm.Core.Interfaces.Services;
using WebFilm.Core.Services;

namespace WebFilm.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectsController : BaseController<int, Subjects>
    {
        ISubjectService _subjectService;
        IUserContext _userContext;

        public SubjectsController(ISubjectService subjectService, IUserContext userContext) : base(subjectService)
        {
            _subjectService = subjectService;
            _userContext = userContext;
        }

        [HttpPost("")]
        public IActionResult create(SubjectDTO dto)
        {
            try
            {
                var res = _subjectService.create(dto);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPut("{id}")]
        public IActionResult update(int id, SubjectDTO dto)
        {
            try
            {
                var res = _subjectService.update(id, dto);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult delete(int id)
        {
            try
            {
                var res = _subjectService.delete(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("")]
        public IActionResult getAll()
        {
            try
            {
                var res = _subjectService.findAll();
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
