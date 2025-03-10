using Dapper;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.Points;
using WebFilm.Core.Enitites.Semesters;
using WebFilm.Core.Interfaces.Repository;

namespace WebFilm.Infrastructure.Repository
{
    public class ScoreRepository : BaseRepository<int, Scores>, IScoreRepository
    {
        public ScoreRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public void delete(int semesterId, int studentId)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = @"delete from `Scores` where studentId = @v_studentId and semesterId = @v_semesterId;";  // Lấy ID của bản ghi vừa chèn vào

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("v_studentId", studentId);
                parameters.Add("v_semesterId", semesterId);

                SqlConnection.Execute(sqlCommand, parameters);
            }
        }
    }
}
