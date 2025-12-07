using Application.Services.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SftpController : ControllerBase
    {
        private readonly ISftpService _sftpService;
        private readonly IManufactureService _manufactureService;

        public SftpController(ISftpService sftpService, IManufactureService manufactureService)
        {
            _sftpService = sftpService;
            _manufactureService = manufactureService;
        }

        [HttpPut("uploadBatchXml")]
        public async Task<IActionResult> UploadBatchXmlToSftp()
        {

            // XML-i al
            var xmlContent = await _manufactureService.GetBatchForManufacturingAsXMLAsync();

            // SFTP-yə göndər
            var result = await _sftpService.UploadXmlToSftpAsync(xmlContent);

            if (result)
            {
                return Ok(new
                {
                    Success = true,
                    Message = "XML uğurla SFTP serverinə yükləndi"
                });
            }
            else
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "XML SFTP serverinə yüklənə bilmədi"
                });
            }

        }

        [HttpGet("testConnection")]
        public async Task<IActionResult> TestSftpConnection()
        {
            var result = await _sftpService.TestConnectionAsync();

            return result
                ? Ok(new { Connected = true, Message = "SFTP serverinə bağlantı uğurludur" })
                : StatusCode(500, new { Connected = false, Message = "SFTP serverinə bağlantı uğursuzdur" });
        }


        [HttpGet("listFiles")]
        public async Task<IActionResult> ListSftpFiles()
        {
            var files = await _sftpService.ListFilesAsync();
            return Ok(files);

        }
    }
}
