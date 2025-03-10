using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Interfaces.Repository;
using WebFilm.Core.Interfaces.Services;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.InkML;

namespace WebFilm.Core.Services
{
    public class ExportService : IExportService
    {
        IScoreRepository _scoreRepository;
        ISemesterSubjectRepository _semesterSubjectRepository;
        ISubjectRepository _subjectRepository;
        ISemesterRepository _semesterRepository;
        IUserRepository _userRepository;
        IUserContext _userContext;
        private readonly IConfiguration _configuration;

        public ExportService(IScoreRepository scoreRepository, ISemesterSubjectRepository semesterSubjectRepository, ISubjectRepository subjectRepository, 
            IConfiguration configuration,
            IUserContext userContext, IUserRepository userRepository, ISemesterRepository semesterRepository)
        {
            _configuration = configuration;
            _scoreRepository = scoreRepository;
            _semesterSubjectRepository = semesterSubjectRepository;
            _subjectRepository = subjectRepository;
            _userContext = userContext;
            _userRepository = userRepository;
            _semesterRepository = semesterRepository;
        }

        public byte[] export(int exportType, int fileType, string className)
        {
            /*var students = _userRepository.getAllStudents(className);

            using var workbook = new XLWorkbook();
            int year = DateTime.Today.Year;
            var semesters = _semesterRepository.GetAll().Where(int year)

            foreach (var student in students)
            {
                var semesters = _sc
                var semesters = _context.Semesters
                    .Where(s => _context.Scores.Any(sc => sc.StudentId == student.Id && sc.SemesterId == s.Id))
                    .ToList();

                var worksheet = workbook.Worksheets.Add(student.Name);

                int row = 1;
                worksheet.Cell(row, 1).Value = "Semester";
                worksheet.Cell(row, 2).Value = "Subject";
                worksheet.Cell(row, 3).Value = "Score";
                row++;

                foreach (var semester in semesters)
                {
                    var scores = _context.Scores
                        .Where(sc => sc.StudentId == student.Id && sc.SemesterId == semester.Id)
                        .Join(_context.SemesterSubjects, sc => sc.SubjectId, ss => ss.SubjectId, (sc, ss) => new
                        {
                            Semester = semester.Name,
                            SubjectName = ss.Subject.Name,
                            Score = sc.ScoreValue
                        }).ToList();

                    foreach (var score in scores)
                    {
                        worksheet.Cell(row, 1).Value = score.Semester;
                        worksheet.Cell(row, 2).Value = score.SubjectName;
                        worksheet.Cell(row, 3).Value = score.Score;
                        row++;
                    }
                }

                worksheet.Columns().AdjustToContents();
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream)
            return stream.ToArray(); */
            return null;
        }
    }
}
