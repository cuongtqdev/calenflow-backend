using Azure.Core;
using CalenFlowApp.Models.AI;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace CalenFlowApp.Controllers
{ 
    public class CVController : Controller
    {
        private readonly IPdfTextExtractor _pdfTextExtractor;
        private readonly IOcrHelper _ocrHelper;
        private readonly ICVAIService _aiService;


        public CVController(IPdfTextExtractor pdfTextExtractor, IOcrHelper ocrHelper, ICVAIService aiService)
        {
            _pdfTextExtractor = pdfTextExtractor;
            _ocrHelper = ocrHelper;
            _aiService = aiService;
        }


        [HttpGet]
        public IActionResult Index()
        {
            return View(); // trả view chứa UI mà bạn đã gửi
        }


        [HttpPost("/CV/Analyze")]
        public async Task<IActionResult> AnalyzeFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Vui lòng upload file hợp lệ");

            if (file.Length > 10 * 1024 * 1024)
                return BadRequest("File quá lớn. Vui lòng chọn file < 10MB");

            var ext = Path.GetExtension(file.FileName).ToLower();

            byte[] fileBytes;
            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                fileBytes = ms.ToArray();
            }

            string mimeType = ext switch
            {
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                _ => "application/octet-stream"
            };

            string base64Image = Convert.ToBase64String(fileBytes);

            try
            {
                var analysis = await _aiService.AnalyzeAsync(mimeType, base64Image);
                return Ok(new { success = true, data = analysis });
            }
            catch (Exception ex)
            {
                // log lỗi để debug (ví dụ Serilog, ILogger…)
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> AIRespond([FromBody] ChatRequestDto dto) 
        {
            try
            {
                List<MessageDto> context = dto.Context;
                List<(string role, string text)> formattedContext = context
                    .Select(m => (m.Role.ToLower(), m.Text))
                    .ToList();
                var summary = await _aiService.SummarizeConversationAsync(formattedContext);

                var reply = await _aiService.AIRespond(summary, dto.LatestMessage);

                return Ok(new { success = true, reply });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, error = ex.Message });
            }
        }

    }
}
