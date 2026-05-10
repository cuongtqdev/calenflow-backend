using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using BusinessObjects.Models;
using System.Text.RegularExpressions;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Logging;
namespace Service
{
    public class CVAIService : ICVAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "";
        private readonly ILogger<CVAIService> _logger;

        public CVAIService(HttpClient httpClient, ILogger<CVAIService> logger)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", true, true).Build();
            _httpClient = httpClient;
            _logger = logger;
        }
        public async Task<string> AnalyzeAsync(string mime_type, string base64Image)
        {
            //var userPrompt = $@"Bạn là chuyên gia tuyển dụng, trả lời ngắn gọn, rõ ràng, phân loại điểm mạnh, điểm cần cải thiện, và gợi ý cụ thể, nếu bạn thấy đã ổn rồi thì chỉ cần trả lời cv của user đã ổn.
            //            Nếu không phải là CV thì trả lời 'This is not a CV'.
            //            Phân tích CV này và trả kết quả dưới dạng JSON với các trường: summary, strengths[], improvements[], suggestions[].  Chỉ trả JSON hợp lệ và bằng tiếng việt:";
            var userPrompt = $@"Bạn là chuyên gia tuyển dụng, trả lời ngắn gọn, rõ ràng, phân loại điểm mạnh, điểm cần cải thiện, và gợi ý cụ thể, nếu bạn thấy đã ổn rồi thì chỉ cần trả lời cv của user đã ổn.
                        Nếu không phải là CV thì trả lời 'This is not a CV'.
                        Phân tích CV bằng tiếng việt với các lưu ý sau:
                        + Mở đầu sẽ là phân tích cấu trúc cv với cấu trúc mẫu sau: 🔍 **Phân tích cấu trúc CV:**\n\n✅ **Điểm mạnh:**\n• Layout rõ ràng, dễ đọc\n• Thông tin liên hệ đầy đủ\n• Sử dụng font chữ chuyên nghiệp\n\n⚠️ **Cần cải thiện:**\n• <span class='highlight-negative'>Thiếu summary/objective section</span>\n• <span class='highlight-negative'>Không có keywords quan trọng cho ATS</span>
                        + Tiếp theo là Phân tích nội dung cv với cấu trúc mẫu sau:📊 **Phân tích nội dung:**\n\n✅ **Điểm mạnh:**\n• <span class='highlight-positive'>Kinh nghiệm làm việc được trình bày chi tiết</span>\n• <span class='highlight-positive'>Kỹ năng technical phù hợp với vị trí</span>\n• Học vấn rõ ràng\n\n⚠️ **Cần cải thiện:**\n• <span class='highlight-negative'>Thiếu số liệu cụ thể về thành tích</span>\n• <span class='highlight-negative'>Mô tả công việc chưa highlight impact</span>
                        + Tiếp theo là Gợi ý cải thiện cv với cấu trúc mẫu sau:💡 **Gợi ý cải thiện:**\n\n1. <span class='highlight-suggestion'>Thêm Professional Summary</span> ở đầu CV (2-3 câu tóm tắt)\n2. <span class='highlight-suggestion'>Sử dụng action verbs</span>: 'Achieved', 'Implemented', 'Led'\n3. <span class='highlight-suggestion'>Thêm metrics</span>: '20% increase', '50+ projects'\n4. <span class='highlight-suggestion'>Tối ưu keywords</span> cho ATS systems\n5. <span class='highlight-suggestion'>Thêm section Projects</span> nếu có
                        + Tiếp theo là Đánh giá tổng thể cv với cấu trúc mẫu sau:🎯 **Đánh giá tổng thể:** 7.5/10\n\nCV của bạn có foundation tốt nhưng cần một số cải thiện để nổi bật hơn. Hãy áp dụng các gợi ý trên để tăng cơ hội được nhà tuyển dụng chú ý!\n\n✨ Bây giờ bạn có thể chat với tôi để hỏi thêm về bất kỳ phần nào của CV!";

            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={_apiKey}";
            var payload = new
            {
                contents = new[]
                {
                    new {
                        parts = new object[]
                        {
                            new { text = $"{userPrompt}" },
                            new { inline_data = new {
                                            mimeType = mime_type,
                                            data = base64Image
                                        }
                                }
                        }
                    }
                }
            };
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            int maxRetries = 5;
            int delay = 1000; // 1s

            for (int attempt = 0; attempt < maxRetries; attempt++)
            {
                var response = await _httpClient.PostAsync(url, content);
                var body = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    //if (ExtractAndFormat(body).IndexOf("Lỗi khi xử lý JSON") < 0)
                    //{
                    //    return ExtractAndFormat(body);
                    //}
                    using var doc = JsonDocument.Parse(body);
                    var root = doc.RootElement;

                    var reply = root
                        .GetProperty("candidates")[0]
                        .GetProperty("content")
                        .GetProperty("parts")[0]
                        .GetProperty("text")
                        .GetString();
                    return reply?.Trim() ?? "⚠️ Không có phản hồi từ AI.";
                }
                if ((int)response.StatusCode == 503)
                {
                    await Task.Delay(delay);
                    delay *= 2; // exponential backoff
                    continue;
                }

                throw new Exception($"Gemini API error: {response.StatusCode} - {body}");
            }
            return null;
        }
        public string ExtractAndFormat(string json)
        {
            try
            {
                using var doc = JsonDocument.Parse(json);
                var text = doc.RootElement
                              .GetProperty("candidates")[0]
                              .GetProperty("content")
                              .GetProperty("parts")[0]
                              .GetProperty("text")
                              .GetString();

                string cleaned = Regex.Replace(text, "```json|```", "").Trim();
                if (cleaned.Contains("This is not a CV"))
                {
                    return "Đây không phải CV";
                }
                var jsonElement = JsonDocument.Parse(cleaned).RootElement;

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };

                return JsonSerializer.Serialize(jsonElement, options);
            }
            catch (Exception ex)
            {
                return "Lỗi khi xử lý JSON: " + ex.Message;
            }
        }
        public async Task<string> SummarizeConversationAsync(List<(string role, string text)> messages)
        {
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={_apiKey}";

            var prompt = "Tóm tắt ngắn gọn hội thoại sau (dưới 150 từ, bằng tiếng Việt):\n\n";
            foreach (var msg in messages)
            {
                prompt += $"{msg.role.ToUpper()}: {msg.text}\n";
            }

            var payload = new
            {
                contents = new[]
                {
                    new {
                        parts = new object[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            var json = JsonSerializer.Serialize(payload);
            var response = await _httpClient.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));
            var body = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(body);
            return doc.RootElement
                      .GetProperty("candidates")[0]
                      .GetProperty("content")
                      .GetProperty("parts")[0]
                      .GetProperty("text")
                      .GetString();
        }

        public async Task<string> AIRespond(string summary, string latestMessage)
        {
            try
            {
                var userPrompt = $@"Bạn là một trợ lý AI. Đây là tóm tắt của cuộc trò chuyện trước đó: 
                                    '{summary}'

                                    Người dùng vừa gửi thêm tin nhắn: 
                                    '{latestMessage}'

                                    Hãy trả lời ngắn gọn, tự nhiên và hữu ích. 
                                    Nếu thấy liên quan đến CV thì đưa phân tích, nếu không thì phản hồi đúng ngữ cảnh.";

                var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={_apiKey}";

                var payload = new
                {
                    contents = new[]
                    {
                        new {
                            parts = new object[]
                            {
                                new { text = userPrompt }
                            }
                        }
                    }
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(url, content);
                var body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Gemini API Error: {StatusCode} - {Body}", response.StatusCode, body);
                    throw new Exception($"Gemini API error: {response.StatusCode} - {body}");
                }

                using var doc = JsonDocument.Parse(body);
                var root = doc.RootElement;

                var reply = root
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();

                return reply?.Trim() ?? "⚠️ Không có phản hồi từ AI.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gọi Gemini API.");
                return $"❌ Lỗi khi phân tích: {ex.Message}";
            }
        }

    }
}
