using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebFilm.Core.Enitites.Semesters;
using WebFilm.Core.Enitites.Subject;
using WebFilm.Core.Interfaces.Services;
using WebFilm.Core.Services;

namespace WebFilm.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SemestersController : BaseController<int, Semesters>
    {
        ISemesterService _semesterService;
        IUserContext _userContext;

        public SemestersController(ISemesterService semesterService, IUserContext userContext) : base(semesterService)
        {
            _semesterService = semesterService;
            _userContext = userContext;
        }

        [HttpPost("")]
        public IActionResult create(SemesterDTO dto)
        {
            try
            {
                var res = _semesterService.create(dto);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
