using Dapper;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.Subject;
using WebFilm.Core.Enitites.User;
using WebFilm.Core.Interfaces.Repository;

namespace WebFilm.Infrastructure.Repository
{
    public class SubjectRepository : BaseRepository<int, Subjects>, ISubjectRepository
    {
        public SubjectRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public bool create(SubjectDTO dto)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = $@"INSERT INTO Subjects (subjectCode, subjectName, creditNumber, desscription, CreatedDate, ModifiedDate)
                                              VALUES (@v_subjectCode, @v_subjectName, @v_creditNumber, @v_desscription, NOW(), NOW());";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("v_subjectCode", dto.subjectCode);
                parameters.Add("v_subjectName", dto.subjectName);
                parameters.Add("v_creditNumber", dto.creditNumber);
                parameters.Add("v_desscription", dto.description);
                var affectedRows = SqlConnection.Execute(sqlCommand, parameters);

                if (affectedRows > 0)
                {
                    return true;
                }

                //Trả dữ liệu về client
                SqlConnection.Close();
                return false;
            }
        }

        public int update(int id, SubjectDTO dto)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = $@"Update `Subjects` set subjectCode = @v_subjectCode, subjectName = @v_subjectName, creditNumber = @v_creditNumber, description = @v_description, ModifiedDate = NOW() where id = @v_id;";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("v_subjectCode", dto.subjectCode);
                parameters.Add("v_subjectName", dto.subjectName);
                parameters.Add("v_creditNumber", dto.creditNumber);
                parameters.Add("v_description", dto.description);
                parameters.Add("v_id", id);

                var affectedRows = SqlConnection.Execute(sqlCommand, parameters);

                return affectedRows;
            }
        }
    }
}
