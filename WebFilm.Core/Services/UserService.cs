using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebFilm.Core.Enitites;
using WebFilm.Core.Enitites.Staff;
using WebFilm.Core.Enitites.User;
using WebFilm.Core.Exceptions;
using WebFilm.Core.Interfaces.Repository;
using WebFilm.Core.Interfaces.Services;

namespace WebFilm.Core.Services
{
    public class UserService : BaseService<int, Users>, IUserService
    {
        IUserRepository _userRepository;
        IUserContext _userContext;
        private readonly IConfiguration _configuration;

        public UserService(IUserRepository userRepository,
            IConfiguration configuration,
            IUserContext userContext) : base(userRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _userContext = userContext;
        }

        #region Method
        public Users GetUserByID(int userID)
        {
            var user = _userRepository.GetUserByID(userID);
            return user;
        }

        public Users Signup(UserDTO user)
        {
            //// username can not duplicate
            string role = _userContext.Role;
            if (!"ADMIN".Equals(role))
            {
                throw new ServiceException(Resources.Resource.Not_Permission);
            }

            var isDuplicateUsername = _userRepository.CheckDuplicateUsername(user.username);
            if (isDuplicateUsername)
            {
                throw new ServiceException(Resources.Resource.Error_Duplicate_UserName);
            }

            user.password = BCrypt.Net.BCrypt.HashPassword(user.password);
            var res = _userRepository.Signup(user);

            return res;
        }

        public Dictionary<string, object> Login(string username, string password)
        {
            var userDto = _userRepository.Login(username);
            if (userDto != null)
            {
              
              var isPasswordCorrect = BCrypt.Net.BCrypt.Verify(password, userDto.password);
                if (isPasswordCorrect)
                {
                    var token = GenarateToken(userDto);
                    return new Dictionary<string, object>()
                    {
                        { "token", token }
                    };
                }
            }
            throw new ServiceException("Thông tin tài khoản hoặc mật khẩu không chính xác");
        }

        private string GenarateToken(Users user)
        {
            // Authenticate user credentials and get the user's claims
            var claims = new List<Claim>
            {
                new Claim("id", user.id.ToString()),
                new Claim("username", user.username),
                new Claim("Role", user.role),
                new Claim("fullname", user.fullname ?? ""),
                // Add any other user claims as needed
            };

            // Generate a symmetric security key using your secret key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));

            // Create a signing credentials object using the key
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Set token expiration time
            var expires = DateTime.UtcNow.AddDays(30);

            // Create a JWT token
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: credentials
            );

            // Serialize the token to a string
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return "Bearer " + tokenString;
        }

        public Users changeProfile(string username, ChangeProfileDTO dto)
        {
            var staff = _userRepository.changeProfile(username, dto);
            if (staff != null)
            {
                return staff;
            }
            throw new ServiceException("User không khả dụng");
        }

        public bool deleteStaff(int id)
        {
            string role = _userContext.Role;
            if ("STAFF".Equals(role))
            {
                throw new ServiceException(Resources.Resource.Not_Permission);
            }
            int res = _userRepository.Delete(id);
            if (res == 1)
            {
                return true;
            } else
            {
                throw new ServiceException("Staff không khả dụng");
            }
        }

        public List<Users> getAllStaff()
        {
            string role = _userContext.Role;
            if ("STAFF".Equals(role))
            {
                throw new ServiceException(Resources.Resource.Not_Permission);
            }
            return _userRepository.GetAll().ToList();
        }

        public Users getProfile(string username)
        {
            var profile = _userRepository.GetAll().Where(p => p.username.Equals(username)).FirstOrDefault();

            return profile;
        }
        #endregion
    }
}
