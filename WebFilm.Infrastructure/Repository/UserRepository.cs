using Dapper;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using Newtonsoft.Json;
using WebFilm.Core.Enitites;
using WebFilm.Core.Enitites.User;
using WebFilm.Core.Interfaces.Repository;

namespace WebFilm.Infrastructure.Repository
{
    public class UserRepository : BaseRepository<int, Users>, IUserRepository
    {
        public UserRepository(IConfiguration configuration) : base(configuration)
        {
        }
        #region Method
        public Users GetUserByID(int userID)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = "SELECT * FROM Users WHERE ID = @v_UserID";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("v_UserID", userID);
                var user = SqlConnection.QueryFirstOrDefault<Users>(sqlCommand, parameters);

                //Trả dữ liệu về client
                SqlConnection.Close();
                return user;
            }
        }

        public Users Signup(UserDTO user)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = $@"INSERT INTO Users (Fullname, UserName, Password, Role, CreatedDate, ModifiedDate)
                                              VALUES (@v_FullName, @v_UserName, @v_Password, @v_Role, NOW(), NOW());";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("v_UserName", user.username);
                parameters.Add("v_Password", user.password);
                parameters.Add("v_FullName", user.fullName);
                parameters.Add("v_Role", user.role);
                var affectedRows = SqlConnection.Execute(sqlCommand, parameters);

                if (affectedRows > 0)
                {
                    var res = SqlConnection.QueryFirstOrDefault<Users>("SELECT * FROM Users WHERE UserName = @v_UserName", parameters);
                    SqlConnection.Close();
                    return res;
                }

                //Trả dữ liệu về client
                SqlConnection.Close();
                return null;
            }
        }

        public Users Login(string username)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = $"SELECT * FROM Users WHERE UserName = @v_UserName";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@v_UserName", username);
                var res = SqlConnection.QueryFirstOrDefault<Users>(sqlCommand, parameters);

                //Trả dữ liệu về client
                SqlConnection.Close();
                return res;
            }
        }

        public bool CheckDuplicateUsername(string username)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCheck = "SELECT * FROM Users WHERE UserName = @v_UserName";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@v_UserName", username);

                var res = SqlConnection.QueryFirstOrDefault<Users>(sqlCheck, parameters);
                if (res != null)
                {
                    SqlConnection.Close();
                    return true;
                }
                SqlConnection.Close();
                return false;
            }
        }

        public Users getUserByUsername(string username)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = "SELECT * FROM User WHERE  UserName = @v_UserName";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("v_UserName", username);
                var user = SqlConnection.QueryFirstOrDefault<Users>(sqlCommand, parameters);

                //Trả dữ liệu về client
                SqlConnection.Close();
                return user;
            }
        }

        public Users changeProfile(string username, ChangeProfileDTO dto)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = $@"Update `Users` set fullname = @v_fullname, salary = @v_salary, image = @v_image, ModifiedDate = NOW() where username = @v_username;";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("v_fullname", dto.fullName);
                parameters.Add("v_salary", dto.salary);
                parameters.Add("v_image", dto.image);
                parameters.Add("v_username", username);

                var affectedRows = SqlConnection.Execute(sqlCommand, parameters);

                if (affectedRows > 0)
                {
                    var res = SqlConnection.QueryFirstOrDefault<Users>("SELECT * FROM `Users` WHERE username = @v_username", parameters);
                    SqlConnection.Close();
                    return res;
                }
                //Trả dữ liệu về client
                SqlConnection.Close();
                return null;
            }
        }
        #endregion
    }
}
