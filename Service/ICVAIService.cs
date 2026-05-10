using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using BusinessObjects.Models;
namespace Service
{
    public interface ICVAIService
    {
        //Task<AnalysisResult> AnalyzeAsync(string extractedText);
        Task<string> AnalyzeAsync(string mimeType, string base64Image);
        Task<string> SummarizeConversationAsync(List<(string role, string text)> messages);
        Task<string> AIRespond(string summary, string latestMessage);
    }
}
