using Dapper;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.Semesters;
using WebFilm.Core.Enitites.Subject;
using WebFilm.Core.Enitites.User;
using WebFilm.Core.Interfaces.Repository;

namespace WebFilm.Infrastructure.Repository
{
    public class SemesterRepository : BaseRepository<int, Semesters>, ISemesterRepository
    {
        public SemesterRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public int create(SemesterDTO dto)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = @"
            INSERT INTO Semesters (semesterName, year, CreatedDate, ModifiedDate)
            VALUES (@v_semesterName, @v_year, NOW(), NOW());
            SELECT LAST_INSERT_ID();";  // Lấy ID của bản ghi vừa chèn vào

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("v_semesterName", dto.semesterName);
                parameters.Add("v_year", dto.year);

                var insertedId = connection.ExecuteScalar<int>(sqlCommand, parameters); // Trả về ID vừa insert

                return insertedId;
            }
        }

        public int update(int id, SemesterDTO dto)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = $@"Update `Semesters` set semesterName = @v_semesterName, year = @v_year, ModifiedDate = NOW() where id = @v_id;";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("v_semesterName", dto.semesterName);
                parameters.Add("v_year", dto.year);
                parameters.Add("v_id", id);

                var affectedRows = SqlConnection.Execute(sqlCommand, parameters);

                return affectedRows;
            }
        }
    }
}
