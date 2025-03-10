using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using WebFilm.Controllers;
using WebFilm.Core.Enitites;
using WebFilm.Core.Enitites.Staff;
using WebFilm.Core.Enitites.User;
using WebFilm.Core.Interfaces.Services;
using WebFilm.Core.Services;

namespace WebFilm.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : BaseController<int, Users>
    {
        #region Field
        IUserService _userService;
        IUserContext _userContext;

        #endregion

        #region Contructor
        public UsersController(IUserService userService, IUserContext userContext) : base(userService)
        {
            _userService = userService;
            _userContext = userContext;
        }
        #endregion

        #region Method

        [HttpPost("signup")]
        public IActionResult Signup(UserDTO user)
        {
            try
            {
                var res = _userService.Signup(user);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login(LoginDTO dto)
        {
            try
            {
                var res = _userService.Login(dto.username, dto.password);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("{username}/profile")]
        public IActionResult changeProfile(string username, ChangeProfileDTO dto)
        {
            try
            {
                var res = _userService.changeProfile(username, dto);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("{username}/profile")]
        public IActionResult getProfile(string username)
        {
            try
            {
                var res = _userService.getProfile(username);
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
                var res = _userService.getAllStaff();
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult deleteStaff(int id)
        {
            try
            {
                var res = _userService.deleteStaff(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("import-students")]
        public IActionResult importStudent(IFormFile file)
        {
            try
            {
                 _userService.importStudent(file);
                return Ok("Import successfully!!");
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("students")]
        public IActionResult getAllStudent(int semesterId, String className)
        {
            try
            {
                var res = _userService.getAllStudents(semesterId, className);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        #endregion
    }
}
