using Dapper;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.Semesters;
using WebFilm.Core.Enitites.SemesterSubject;
using WebFilm.Core.Interfaces.Repository;

namespace WebFilm.Infrastructure.Repository
{
    public class SemesterSubjectRepository : BaseRepository<int, SemesterSubject>, ISemesterSubjectRepository
    {
        public SemesterSubjectRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public bool create(int sesmesterId, int subjectId)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = $@"INSERT INTO SemesterSubject (semesterId, subjectId, CreatedDate, ModifiedDate)
                                              VALUES (@v_semesterId, @v_subjectId, NOW(), NOW());";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("v_semesterId", sesmesterId);
                parameters.Add("v_subjectId", subjectId);
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
    }
}
