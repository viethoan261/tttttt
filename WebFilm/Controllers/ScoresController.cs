using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using WebFilm.Core.Enitites.Points;
using WebFilm.Core.Enitites.Semesters;
using WebFilm.Core.Interfaces.Services;
using WebFilm.Core.Services;

namespace WebFilm.Controllers
{

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ScoresController : BaseController<int, Scores>
    {
        IScoreService _scoreService;
        IUserContext _userContext;

        public ScoresController(IScoreService scoreService, IUserContext userContext) : base(scoreService)
        {
            _scoreService = scoreService;
            _userContext = userContext;
        }

        [HttpPost("")]
        public IActionResult update(int semesterId, int studentId, List<PointRequest> request)
        {
            try
            {
                var res = _scoreService.updateScores(studentId, semesterId, request);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
