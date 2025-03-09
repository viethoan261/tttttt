using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;
using OpenCvSharp;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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
                new Claim("fullname", user.fullName ?? ""),
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

        public void importStudent(IFormFile file)
        {
            string role = _userContext.Role;
            if (!"ADMIN".Equals(role))
            {
                throw new ServiceException(Resources.Resource.Not_Permission);
            }

            if (file == null || file.Length == 0)
                throw new ServiceException("File is empty");

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Thêm dòng này

            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    int rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        UserDTO student = new UserDTO();

                        student.fullName = worksheet.Cells[row, 1].Text;
                        student.username = worksheet.Cells[row, 2].Text;
                        student.password = BCrypt.Net.BCrypt.HashPassword(worksheet.Cells[row, 3].Text);
                        student.role = "STUDENT";
                        student.className = worksheet.Cells[row, 4].Text;

                        var isDuplicateUsername = _userRepository.CheckDuplicateUsername(student.username);
                        if (isDuplicateUsername)
                        {
                            continue;
                        }

                        _userRepository.Signup(student);
                    }
                }
            }
        }

        public byte[] ExportStudent()
        {
            string role = _userContext.Role;
            if (!"ADMIN".Equals(role))
            {
                throw new ServiceException(Resources.Resource.Not_Permission);
            }

            List<Users> students = _userRepository.GetAll().Where(t => t.role.Equals("STUDENT")).ToList();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Students");
                worksheet.Cells[1, 1].Value = "Full Name";
                worksheet.Cells[1, 2].Value = "Username";
                worksheet.Cells[1, 3].Value = "Password";

                for (int i = 0; i < students.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = students[i].fullName;
                    worksheet.Cells[i + 2, 2].Value = students[i].username;
                    worksheet.Cells[i + 2, 3].Value = students[i].password;
                }

                return package.GetAsByteArray();
            }
        }

        #endregion
    }
}
