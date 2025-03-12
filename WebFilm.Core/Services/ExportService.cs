using Microsoft.Extensions.Configuration;
using WebFilm.Core.Interfaces.Repository;
using WebFilm.Core.Interfaces.Services;
using ClosedXML.Excel;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Table = iText.Layout.Element.Table;
using iText.IO.Font;
using iText.Kernel.Font;

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
            switch (exportType)
            {
                case 1:
                    return exportType1(className, fileType);
                case 2:
                    return exportType2(className, fileType);
                case 3:
                    return exportType3(className, fileType);

                default:
                    return null;
            }
        }


        private byte[] exportType1(string className, int fileType)
        {
            switch (fileType)
            {
                case 1:
                    return ExportToExcel1(className);
                case 2:
                    return ExportToPDF1(className);
                default:
                    throw new ArgumentException("Invalid file type");
            }
        }

        private byte[] ExportToExcel1(string className)
        {
            // Lấy danh sách sinh viên trong lớp
            var students = _userRepository.getAllStudents(className);

            if ("STUDENT".Equals(_userContext.Role))
            {
                students = students.Where(t => t.id == _userContext.UserID).ToList();
            }

            // Tạo một workbook mới
            using var workbook = new XLWorkbook();

            // Lấy năm hiện tại
            int year = DateTime.Now.Year;

            // Lấy danh sách các học kỳ trong năm hiện tại
            var semesters = _semesterRepository.GetAll().Where(t => t.year == year).ToList();

            // Duyệt qua từng sinh viên
            foreach (var student in students)
            {
                // Tạo một worksheet mới với tên là tên của sinh viên
                var worksheet = workbook.Worksheets.Add(student.fullName);

                // Căn giữa toàn bộ worksheet
                worksheet.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Dòng 1: Tiêu đề "Chi tiết điểm năm học 2025", merge 5 cột, background hồng
                var headerCell = worksheet.Cell(1, 1);
                headerCell.Value = "Chi tiết điểm năm học " + year + " lớp " + className;
                headerCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                headerCell.Style.Fill.BackgroundColor = XLColor.Pink;
                worksheet.Range(1, 1, 1, 4).Merge();

                // Dòng 2: Tên sinh viên
                worksheet.Cell(2, 1).Value = "Tên sinh viên:";
                worksheet.Cell(2, 2).Value = student.fullName;

                // Dòng 3: Mã sinh viên
                worksheet.Cell(3, 1).Value = "Mã sinh viên:";
                worksheet.Cell(3, 2).Value = student.id;

                // Bắt đầu từ dòng 4 để hiển thị dữ liệu
                int row = 4;

                // Biến lưu tổng điểm của cả năm học
                double totalYearScore = 0;
                int totalSubjects = 0;

                // Duyệt qua từng học kỳ
                foreach (var semester in semesters)
                {
                    // Merge các cột để hiển thị tên học kỳ
                    var semesterCell = worksheet.Cell(row, 1);
                    semesterCell.Value = semester.semesterName;
                    semesterCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    semesterCell.Style.Fill.BackgroundColor = XLColor.LightGray;
                    worksheet.Range(row, 1, row, 4).Merge();
                    row++;

                    // Đặt tiêu đề cho các cột
                    worksheet.Cell(row, 1).Value = "Tên môn học";
                    worksheet.Cell(row, 2).Value = "Điểm giữa kỳ";
                    worksheet.Cell(row, 3).Value = "Điểm cuối kỳ";
                    worksheet.Cell(row, 4).Value = "Điểm tổng kết";
                    row++;

                    // Biến lưu tổng điểm của kỳ học
                    double totalSemesterScore = 0;
                    int semesterSubjectsCount = 0;
                    int totalCredit = 0;

                    // Lấy danh sách các môn học trong học kỳ
                    var semesterSubjects = _semesterSubjectRepository.GetAll()
                        .Where(ss => ss.semesterId == semester.id)
                        .ToList();

                    // Duyệt qua từng môn học trong học kỳ
                    foreach (var semesterSubject in semesterSubjects)
                    {
                        // Lấy điểm của sinh viên cho môn học này trong học kỳ
                        var scores = _scoreRepository.GetAll()
                            .Where(sc => sc.studentId == student.id && sc.semesterId == semester.id && sc.subjectId == semesterSubject.subjectId)
                            .ToList();

                        // Lấy tên môn học từ repository môn học
                        var subject = _subjectRepository.GetByID(semesterSubject.subjectId);
                        int credit = subject.creditNumber;
                        totalCredit += credit;

                        // Ghi dữ liệu vào worksheet
                        foreach (var score in scores)
                        {
                            double scorePerSubject = score.midtermScore * 0.4 + score.finalScore * 0.6;
                            
                            worksheet.Cell(row, 1).Value = subject.subjectName;  // Tên môn học
                            worksheet.Cell(row, 2).Value = score.midtermScore;   // Điểm giữa kỳ
                            worksheet.Cell(row, 3).Value = score.finalScore;     // Điểm cuối kỳ
                            worksheet.Cell(row, 4).Value = scorePerSubject; // Điểm tổng kết
                            row++;

                            // Cập nhật tổng điểm của kỳ học
                            totalSemesterScore += scorePerSubject * credit;
                            semesterSubjectsCount++;
                        }
                    }

                    // Thêm hàng tổng kết kỳ
                    if (semesterSubjectsCount > 0)
                    {
                        worksheet.Cell(row, 1).Value = "Tổng kết kỳ";
                        worksheet.Cell(row, 4).Value = Math.Round((totalSemesterScore / totalCredit), 2);
                        worksheet.Range(row, 1, row, 4).Style.Fill.BackgroundColor = XLColor.LightGreen;
                        row++;

                        // Cập nhật tổng điểm của cả năm học
                        totalYearScore += totalSemesterScore / totalCredit;
                        totalSubjects += semesterSubjectsCount;
                    }
                }

                // Thêm hàng tổng kết năm học
                if (totalSubjects > 0)
                {
                    worksheet.Cell(row, 1).Value = "Tổng kết năm học";
                    worksheet.Cell(row, 4).Value = Math.Round(totalYearScore * 0.4 / 2, 2);
                    worksheet.Range(row, 1, row, 4).Style.Fill.BackgroundColor = XLColor.LightBlue;
                }

                // Điều chỉnh độ rộng cột cho phù hợp với nội dung
                worksheet.Columns().AdjustToContents();
            }

            // Lưu workbook vào MemoryStream và trả về dưới dạng mảng byte
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        private byte[] ExportToPDF1(string className)
        {
            // Tạo MemoryStream để lưu file PDF
            MemoryStream baos = new MemoryStream();
            PdfWriter writer = new PdfWriter(baos);
            PdfDocument pdfDocument = new PdfDocument(writer.SetSmartMode(true));
            Document document = new Document(pdfDocument, iText.Kernel.Geom.PageSize.LETTER);

            string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "Arial.ttf"); // Sử dụng font Times New Roman
            PdfFont vietnameseFont = PdfFontFactory.CreateFont(fontPath, PdfEncodings.IDENTITY_H);
            
            // Lấy danh sách sinh viên trong lớp
            var students = _userRepository.getAllStudents(className);
            if ("STUDENT".Equals(_userContext.Role))
            {
                students = students.Where(t => t.id == _userContext.UserID).ToList();
            }

            // Lấy năm hiện tại
            int year = DateTime.Now.Year;

            // Lấy danh sách các học kỳ trong năm hiện tại
            var semesters = _semesterRepository.GetAll().Where(t => t.year == year).ToList();


            /* using var stream = new MemoryStream();
             var writer = new PdfWriter(stream);
             var pdf = new PdfDocument(writer);
             var document = new Document(pdf);*/

            // Duyệt qua từng sinh viên
            document.Add(new Paragraph($"Báo cáo năm học {year}")
                    .SetTextAlignment(TextAlignment.CENTER).SetFont(vietnameseFont));
            foreach (var student in students)
            {
                
                document.Add(new Paragraph($"Mã sinh viên: {student.id}")).SetFont(vietnameseFont);
                document.Add(new Paragraph($"Tên sinh viên: {student.fullName}")).SetFont(vietnameseFont);

                // Biến lưu tổng điểm của cả năm học
                double totalYearScore = 0;
                int totalSubjects = 0;

                // Duyệt qua từng học kỳ
                foreach (var semester in semesters)
                {
                    // Thêm tiêu đề học kỳ
                    document.Add(new Paragraph(semester.semesterName)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetBackgroundColor(iText.Kernel.Colors.ColorConstants.LIGHT_GRAY)).SetFont(vietnameseFont);

                    // Tạo bảng cho các môn học trong học kỳ
                    var table = new Table(UnitValue.CreatePercentArray(new float[] { 3, 2, 2, 2 })).UseAllAvailableWidth();
                    table.AddHeaderCell("Tên môn học");
                    table.AddHeaderCell("Điểm giữa kỳ");
                    table.AddHeaderCell("Điểm cuối kỳ");
                    table.AddHeaderCell("Điểm tổng kết");

                    // Biến lưu tổng điểm của kỳ học
                    double totalSemesterScore = 0;
                    int semesterSubjectsCount = 0;
                    int totalCredit = 0;

                    // Lấy danh sách các môn học trong học kỳ
                    var semesterSubjects = _semesterSubjectRepository.GetAll()
                        .Where(ss => ss.semesterId == semester.id)
                        .ToList();

                    // Duyệt qua từng môn học trong học kỳ
                    foreach (var semesterSubject in semesterSubjects)
                    {
                        // Lấy điểm của sinh viên cho môn học này trong học kỳ
                        var scores = _scoreRepository.GetAll()
                            .Where(sc => sc.studentId == student.id && sc.semesterId == semester.id && sc.subjectId == semesterSubject.subjectId)
                            .ToList();

                        // Lấy tên môn học từ repository môn học
                        var subject = _subjectRepository.GetByID(semesterSubject.subjectId);
                        int credit = subject.creditNumber;
                        totalCredit += credit;

                        // Ghi dữ liệu vào bảng
                        foreach (var score in scores)
                        {
                            double scorePerSubject = score.midtermScore * 0.4 + score.finalScore * 0.6;

                            table.AddCell(subject.subjectName);  // Tên môn học
                            table.AddCell(score.midtermScore.ToString("0.00"));   // Điểm giữa kỳ
                            table.AddCell(score.finalScore.ToString("0.00"));     // Điểm cuối kỳ
                            table.AddCell(scorePerSubject.ToString("0.00")); // Điểm tổng kết

                            // Cập nhật tổng điểm của kỳ học
                            totalSemesterScore += scorePerSubject * credit;
                            semesterSubjectsCount++;
                        }
                    }

                    // Thêm bảng vào document
                    document.Add(table);

                    // Thêm hàng tổng kết kỳ
                    if (semesterSubjectsCount > 0)
                    {
                        double averageSemesterScore = totalSemesterScore / totalCredit;
                        document.Add(new Paragraph($"Tổng kết kỳ: {Math.Round(averageSemesterScore, 2)}")
                            .SetBackgroundColor(iText.Kernel.Colors.ColorConstants.LIGHT_GRAY)).SetFont(vietnameseFont);

                        // Cập nhật tổng điểm của cả năm học
                        totalYearScore += averageSemesterScore;
                        totalSubjects++;
                    }
                }

                // Thêm hàng tổng kết năm học
                if (totalSubjects > 0)
                {
                    double averageYearScore = totalYearScore / totalSubjects;
                    document.Add(new Paragraph($"Tổng kết năm học: {Math.Round(averageYearScore, 2)}")
                        .SetBackgroundColor(iText.Kernel.Colors.ColorConstants.MAGENTA)).SetFont(vietnameseFont);
                }

                // Thêm khoảng cách giữa các sinh viên
                document.Add(new AreaBreak());
            }

            document.SetFont(vietnameseFont);
            // Đóng document
            document.Close();

            // Trả về file PDF dưới dạng byte array
            return baos.ToArray();
        }

        private byte[] exportType2(string className, int fileType)
        {
            switch (fileType)
            {
                case 1:
                    return ExportToExcel2(className);
                case 2:
                    return ExportToPDF2(className);
                default:
                    throw new ArgumentException("Invalid file type");
            }
        }

        private byte[] ExportToExcel2(string className)
        {
            // Lấy danh sách sinh viên trong lớp
            var students = _userRepository.getAllStudents(className);
            if ("STUDENT".Equals(_userContext.Role))
            {
                students = students.Where(t => t.id == _userContext.UserID).ToList();
            }

            // Tạo một workbook mới
            using var workbook = new XLWorkbook();

            // Tạo một worksheet mới với tên là "Học lực"
            var worksheet = workbook.Worksheets.Add("Học lực");

            // Căn giữa toàn bộ worksheet
            worksheet.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Lấy năm hiện tại
            int year = DateTime.Now.Year;

            // Header: "Chi tiết học lực năm học 2025 lớp CNTT"
            var headerCell = worksheet.Cell(1, 1);
            headerCell.Value = $"Chi tiết học lực năm học {year} lớp {className}";
            headerCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerCell.Style.Fill.BackgroundColor = XLColor.Pink;
            worksheet.Range(1, 1, 1, 4).Merge();

            // Đặt tiêu đề cho các cột
            int row = 2;
            worksheet.Cell(row, 1).Value = "Tên sinh viên";
            worksheet.Cell(row, 2).Value = "Mã sinh viên";
            worksheet.Cell(row, 3).Value = "Điểm tổng kết năm";
            worksheet.Cell(row, 4).Value = "Học lực";
            row++;

            // Duyệt qua từng sinh viên
            foreach (var student in students)
            {
                // Lấy danh sách các học kỳ trong năm hiện tại
                var semesters = _semesterRepository.GetAll().Where(t => t.year == year).ToList();

                // Biến lưu tổng điểm của cả năm học
                double totalYearScore = 0;
                int totalSubjects = 0;

                // Duyệt qua từng học kỳ
                foreach (var semester in semesters)
                {
                    // Biến lưu tổng điểm của kỳ học
                    double totalSemesterScore = 0;
                    int semesterSubjectsCount = 0;
                    int totalCredit = 0;

                    // Lấy danh sách các môn học trong học kỳ
                    var semesterSubjects = _semesterSubjectRepository.GetAll()
                        .Where(ss => ss.semesterId == semester.id)
                        .ToList();

                    // Duyệt qua từng môn học trong học kỳ
                    foreach (var semesterSubject in semesterSubjects)
                    {
                        // Lấy điểm của sinh viên cho môn học này trong học kỳ
                        var scores = _scoreRepository.GetAll()
                            .Where(sc => sc.studentId == student.id && sc.semesterId == semester.id && sc.subjectId == semesterSubject.subjectId)
                            .ToList();

                        // Lấy tên môn học từ repository môn học
                        var subject = _subjectRepository.GetByID(semesterSubject.subjectId);
                        int credit = subject.creditNumber;
                        totalCredit += credit;

                        // Tính điểm tổng kết môn học
                        foreach (var score in scores)
                        {
                            double scorePerSubject = score.midtermScore * 0.4 + score.finalScore * 0.6;
                            totalSemesterScore += scorePerSubject * credit;
                            semesterSubjectsCount++;
                        }
                    }

                    // Cập nhật tổng điểm của cả năm học
                    if (semesterSubjectsCount > 0)
                    {
                        totalYearScore += totalSemesterScore / totalCredit;
                        totalSubjects++;
                    }
                }

                // Tính điểm tổng kết năm
                double averageYearScore = totalSubjects > 0 ? totalYearScore / totalSubjects * 0.4 : 0;

                // Xác định học lực
                string academicPerformance;
                if (averageYearScore >= 3.6)
                {
                    academicPerformance = "Xuất sắc";
                }
                else if (averageYearScore >= 3.2)
                {
                    academicPerformance = "Giỏi";
                }
                else if (averageYearScore >= 2.8)
                {
                    academicPerformance = "Khá";
                }
                else if (averageYearScore >= 2.0)
                {
                    academicPerformance = "Trung bình";
                }
                else
                {
                    academicPerformance = "Yếu";
                }

                // Ghi dữ liệu vào worksheet
                worksheet.Cell(row, 1).Value = student.fullName; // Tên sinh viên
                worksheet.Cell(row, 2).Value = student.id; // Mã sinh viên
                worksheet.Cell(row, 3).Value = Math.Round(averageYearScore, 2); // Điểm tổng kết năm
                worksheet.Cell(row, 4).Value = academicPerformance; // Học lực
                row++;
            }

            // Điều chỉnh độ rộng cột cho phù hợp với nội dung
            worksheet.Columns().AdjustToContents();

            // Lưu workbook vào MemoryStream và trả về dưới dạng mảng byte
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        private byte[] ExportToPDF2(string className)
        {
            // Lấy danh sách sinh viên trong lớp
            var students = _userRepository.getAllStudents(className);
            if ("STUDENT".Equals(_userContext.Role))
            {
                students = students.Where(t => t.id == _userContext.UserID).ToList();
            }

            // Tạo MemoryStream để lưu file PDF
            using var stream = new MemoryStream();
            var writer = new PdfWriter(stream);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            // Cấu hình font UTF-8
            string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "times.ttf");
            if (!File.Exists(fontPath))
            {
                throw new FileNotFoundException($"Font file not found: {fontPath}");
            }
            PdfFont font = PdfFontFactory.CreateFont(fontPath, PdfEncodings.IDENTITY_H);

            // Lấy năm hiện tại
            int year = DateTime.Now.Year;

            // Header: "Chi tiết học lực năm học 2025 lớp CNTT"
            document.Add(new Paragraph($"Chi tiết học lực năm học {year} lớp {className}")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(16)
                .SetFont(font)
                .SetBackgroundColor(iText.Kernel.Colors.ColorConstants.PINK));

            // Tạo bảng để hiển thị dữ liệu
            var table = new Table(UnitValue.CreatePercentArray(new float[] { 3, 2, 2, 2 })).UseAllAvailableWidth();
            table.AddHeaderCell(new Paragraph("Tên sinh viên").SetFont(font));
            table.AddHeaderCell(new Paragraph("Mã sinh viên").SetFont(font));
            table.AddHeaderCell(new Paragraph("Điểm tổng kết năm").SetFont(font));
            table.AddHeaderCell(new Paragraph("Học lực").SetFont(font));

            // Duyệt qua từng sinh viên
            foreach (var student in students)
            {
                // Lấy danh sách các học kỳ trong năm hiện tại
                var semesters = _semesterRepository.GetAll().Where(t => t.year == year).ToList();

                // Biến lưu tổng điểm của cả năm học
                double totalYearScore = 0;
                int totalSubjects = 0;

                // Duyệt qua từng học kỳ
                foreach (var semester in semesters)
                {
                    // Biến lưu tổng điểm của kỳ học
                    double totalSemesterScore = 0;
                    int semesterSubjectsCount = 0;
                    int totalCredit = 0;

                    // Lấy danh sách các môn học trong học kỳ
                    var semesterSubjects = _semesterSubjectRepository.GetAll()
                        .Where(ss => ss.semesterId == semester.id)
                        .ToList();

                    // Duyệt qua từng môn học trong học kỳ
                    foreach (var semesterSubject in semesterSubjects)
                    {
                        // Lấy điểm của sinh viên cho môn học này trong học kỳ
                        var scores = _scoreRepository.GetAll()
                            .Where(sc => sc.studentId == student.id && sc.semesterId == semester.id && sc.subjectId == semesterSubject.subjectId)
                            .ToList();

                        // Lấy tên môn học từ repository môn học
                        var subject = _subjectRepository.GetByID(semesterSubject.subjectId);
                        int credit = subject.creditNumber;
                        totalCredit += credit;

                        // Tính điểm tổng kết môn học
                        foreach (var score in scores)
                        {
                            double scorePerSubject = score.midtermScore * 0.4 + score.finalScore * 0.6;
                            totalSemesterScore += scorePerSubject * credit;
                            semesterSubjectsCount++;
                        }
                    }

                    // Cập nhật tổng điểm của cả năm học
                    if (semesterSubjectsCount > 0)
                    {
                        totalYearScore += totalSemesterScore / totalCredit;
                        totalSubjects++;
                    }
                }

                // Tính điểm tổng kết năm
                double averageYearScore = totalSubjects > 0 ? totalYearScore / totalSubjects * 0.4 : 0;

                // Xác định học lực
                string academicPerformance;
                if (averageYearScore >= 3.6)
                {
                    academicPerformance = "Xuất sắc";
                }
                else if (averageYearScore >= 3.2)
                {
                    academicPerformance = "Giỏi";
                }
                else if (averageYearScore >= 2.8)
                {
                    academicPerformance = "Khá";
                }
                else if (averageYearScore >= 2.0)
                {
                    academicPerformance = "Trung bình";
                }
                else
                {
                    academicPerformance = "Yếu";
                }

                // Thêm dữ liệu vào bảng
                table.AddCell(new Paragraph(student.fullName).SetFont(font)); // Tên sinh viên
                table.AddCell(new Paragraph(student.id.ToString()).SetFont(font)); // Mã sinh viên
                table.AddCell(new Paragraph(Math.Round(averageYearScore, 2).ToString()).SetFont(font)); // Điểm tổng kết năm
                table.AddCell(new Paragraph(academicPerformance).SetFont(font)); // Học lực
            }

            // Thêm bảng vào document
            document.Add(table);

            // Đóng document
            document.Close();

            // Trả về file PDF dưới dạng mảng byte
            return stream.ToArray();
        }

        private byte[] exportType3(string className, int fileType)
        {
            switch (fileType)
            {
                case 1:
                    return ExportToExcel3(className);
                case 2:
                    return ExportToPDF3(className);
                default:
                    throw new ArgumentException("Invalid file type");
            }
        }

        private byte[] ExportToExcel3(string className)
        {
            // Lấy danh sách sinh viên trong lớp
            var students = _userRepository.getAllStudents(className);
            if ("STUDENT".Equals(_userContext.Role))
            {
                students = students.Where(t => t.id == _userContext.UserID).ToList();
            }

            // Tạo một workbook mới
            using var workbook = new XLWorkbook();

            // Tạo một worksheet mới với tên là "Thành tích học tập"
            var worksheet = workbook.Worksheets.Add("Thành tích học tập");

            // Căn giữa toàn bộ worksheet
            worksheet.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Lấy năm hiện tại
            int year = DateTime.Now.Year;

            // Header: "Báo cáo thành tích học tập 2025 lớp CNTT"
            var headerCell = worksheet.Cell(1, 1);
            headerCell.Value = $"Báo cáo thành tích học tập {year} lớp {className}";
            headerCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerCell.Style.Fill.BackgroundColor = XLColor.Pink;
            worksheet.Range(1, 1, 1, 4).Merge();

            // Bắt đầu từ dòng 2 để hiển thị dữ liệu
            int row = 2;

            // Duyệt qua từng sinh viên
            foreach (var student in students)
            {
                // Thêm thông tin sinh viên
                worksheet.Cell(row, 1).Value = "Tên sinh viên:";
                worksheet.Cell(row, 2).Value = student.fullName;
                row++;

                worksheet.Cell(row, 1).Value = "Mã sinh viên:";
                worksheet.Cell(row, 2).Value = student.id;
                row++;

                // Lấy danh sách các học kỳ trong năm hiện tại
                var semesters = _semesterRepository.GetAll().Where(t => t.year == year).ToList();

                // Duyệt qua từng học kỳ
                foreach (var semester in semesters)
                {
                    // Thêm tiêu đề học kỳ
                    worksheet.Cell(row, 1).Value = $"{semester.semesterName}";
                    worksheet.Cell(row, 1).Style.Font.Bold = true;
                    worksheet.Range(row, 1, row, 4).Merge();
                    row++;

                    // Danh sách các môn đạt yêu cầu và cần cải thiện
                    var passedSubjects = new List<string>();
                    var failedSubjects = new List<string>();

                    // Lấy danh sách các môn học trong học kỳ
                    var semesterSubjects = _semesterSubjectRepository.GetAll()
                        .Where(ss => ss.semesterId == semester.id)
                        .ToList();

                    // Duyệt qua từng môn học trong học kỳ
                    foreach (var semesterSubject in semesterSubjects)
                    {
                        // Lấy điểm của sinh viên cho môn học này trong học kỳ
                        var scores = _scoreRepository.GetAll()
                            .Where(sc => sc.studentId == student.id && sc.semesterId == semester.id && sc.subjectId == semesterSubject.subjectId)
                            .ToList();

                        // Lấy tên môn học từ repository môn học
                        var subject = _subjectRepository.GetByID(semesterSubject.subjectId);

                        // Tính điểm tổng kết môn học
                        foreach (var score in scores)
                        {
                            double scorePerSubject = score.midtermScore * 0.4 + score.finalScore * 0.6;

                            // Phân loại môn học
                            if (scorePerSubject >= 5.0)
                            {
                                passedSubjects.Add(subject.subjectName);
                            }
                            else
                            {
                                failedSubjects.Add(subject.subjectName);
                            }
                        }
                    }

                    // Thêm thông tin các môn đạt yêu cầu
                    worksheet.Cell(row, 1).Value = "Các môn đạt yêu cầu:";
                    worksheet.Cell(row, 2).Value = string.Join(", ", passedSubjects);
                    row++;

                    // Thêm thông tin các môn cần cải thiện
                    worksheet.Cell(row, 1).Value = "Các môn cần cải thiện:";
                    worksheet.Cell(row, 2).Value = string.Join(", ", failedSubjects);
                    row++;
                }

                // Thêm khoảng cách giữa các sinh viên
                row++;
            }

            // Điều chỉnh độ rộng cột cho phù hợp với nội dung
            worksheet.Columns().AdjustToContents();

            // Lưu workbook vào MemoryStream và trả về dưới dạng mảng byte
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        private byte[] ExportToPDF3(string className)
        {
            // Lấy danh sách sinh viên trong lớp
            var students = _userRepository.getAllStudents(className);
            if ("STUDENT".Equals(_userContext.Role))
            {
                students = students.Where(t => t.id == _userContext.UserID).ToList();
            }

            // Tạo MemoryStream để lưu file PDF
            using var stream = new MemoryStream();
            var writer = new PdfWriter(stream);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            // Cấu hình font UTF-8
            string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "times.ttf");
            if (!File.Exists(fontPath))
            {
                throw new FileNotFoundException($"Font file not found: {fontPath}");
            }
            PdfFont font = PdfFontFactory.CreateFont(fontPath, PdfEncodings.IDENTITY_H);

            // Lấy năm hiện tại
            int year = DateTime.Now.Year;

            // Header: "Báo cáo thành tích học tập 2025 lớp CNTT"
            document.Add(new Paragraph($"Báo cáo thành tích học tập {year} lớp {className}")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(16)
                .SetFont(font)
                .SetBackgroundColor(iText.Kernel.Colors.ColorConstants.PINK));

            // Duyệt qua từng sinh viên
            foreach (var student in students)
            {
                // Thêm thông tin sinh viên
                document.Add(new Paragraph($"Tên sinh viên: {student.fullName}")
                    .SetFont(font));
                document.Add(new Paragraph($"Mã sinh viên: {student.id}")
                    .SetFont(font));

                // Lấy danh sách các học kỳ trong năm hiện tại
                var semesters = _semesterRepository.GetAll().Where(t => t.year == year).ToList();

                // Duyệt qua từng học kỳ
                foreach (var semester in semesters)
                {
                    // Thêm tiêu đề học kỳ
                    document.Add(new Paragraph($"{semester.semesterName}")
                        .SetFont(font)
                        .SetFontSize(14).SimulateBold());
                    // Danh sách các môn đạt yêu cầu và cần cải thiện
                    var passedSubjects = new List<string>();
                    var failedSubjects = new List<string>();

                    // Lấy danh sách các môn học trong học kỳ
                    var semesterSubjects = _semesterSubjectRepository.GetAll()
                        .Where(ss => ss.semesterId == semester.id)
                        .ToList();

                    // Duyệt qua từng môn học trong học kỳ
                    foreach (var semesterSubject in semesterSubjects)
                    {
                        // Lấy điểm của sinh viên cho môn học này trong học kỳ
                        var scores = _scoreRepository.GetAll()
                            .Where(sc => sc.studentId == student.id && sc.semesterId == semester.id && sc.subjectId == semesterSubject.subjectId)
                            .ToList();

                        // Lấy tên môn học từ repository môn học
                        var subject = _subjectRepository.GetByID(semesterSubject.subjectId);

                        // Tính điểm tổng kết môn học
                        foreach (var score in scores)
                        {
                            double scorePerSubject = score.midtermScore * 0.4 + score.finalScore * 0.6;

                            // Phân loại môn học
                            if (scorePerSubject >= 5.0)
                            {
                                passedSubjects.Add(subject.subjectName);
                            }
                            else
                            {
                                failedSubjects.Add(subject.subjectName);
                            }
                        }
                    }

                    /*// Thêm thông tin các môn đạt yêu cầu
                    document.Add(new Paragraph("Các môn đạt yêu cầu:")
                        .SetFont(font).SimulateBold());
                    document.Add(new Paragraph(string.Join(", ", passedSubjects))
                        .SetFont(font));

                    // Thêm thông tin các môn cần cải thiện
                    document.Add(new Paragraph("Các môn cần cải thiện:")
                        .SetFont(font).SimulateBold());
                    document.Add(new Paragraph(string.Join(", ", failedSubjects))
                        .SetFont(font));*/
                    document.Add(new Paragraph()
                        .Add(new Text("Các môn đạt yêu cầu: ").SetFont(font).SimulateBold())
                        .Add(new Text(string.Join(", ", passedSubjects)).SetFont(font)));

                    // Các môn cần cải thiện
                    document.Add(new Paragraph()
                        .Add(new Text("Các môn cần cải thiện: ").SetFont(font).SimulateBold())
                        .Add(new Text(string.Join(", ", failedSubjects)).SetFont(font)));

                    // Thêm khoảng cách giữa các kỳ học
                    document.Add(new Paragraph("\n"));
                }

                // Thêm khoảng cách giữa các sinh viên
                document.Add(new Paragraph("\n"));
            }

            // Đóng document
            document.Close();

            // Trả về file PDF dưới dạng mảng byte
            return stream.ToArray();
        }
    }
}
