using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using WebFilm.Core.Interfaces.Services;

namespace WebFilm.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IExportService _exportService;

        public ReportsController(IExportService exportService)
        {
            _exportService = exportService;
        }

        [HttpGet("export-file")]
        public IActionResult ExportExcel(int exportType, int fileType, string className)
        {
            try
            {
                // Gọi service để xuất dữ liệu
                byte[] fileContents = _exportService.export(exportType, fileType, className);

                // Kiểm tra nếu dữ liệu trống
                if (fileContents == null || fileContents.Length == 0)
                {
                    return NotFound("Không có dữ liệu để xuất.");
                }

                // Xác định prefix của tên file dựa trên exportType
                string prefix;
                switch (exportType)
                {
                    case 1:
                        prefix = "BaoCaoDiem";
                        break;
                    case 2:
                        prefix = "BaoCaoHocLuc";
                        break;
                    case 3:
                        prefix = "BaoCaoThanhTich";
                        break;
                    default:
                        return BadRequest("Giá trị exportType không hợp lệ.");
                }

                // Xác định kiểu file và MIME type dựa trên fileType
                string mimeType;
                string fileExtension;
                switch (fileType)
                {
                    case 1:
                        mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        fileExtension = "xlsx";
                        break;
                    case 2:
                        mimeType = "application/pdf";
                        fileExtension = "pdf";
                        break;
                    default:
                        return BadRequest("Giá trị fileType không hợp lệ.");
                }

                // Tạo tên file
                string fileName = $"{prefix}_{className}_{DateTime.Now:yyyyMMddHHmmss}.{fileExtension}";

                // Trả về file
                return File(fileContents, mimeType, fileName);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và trả về thông báo lỗi
                return StatusCode(500, $"Lỗi khi xuất file: {ex.Message}");
            }
        }
    }
}