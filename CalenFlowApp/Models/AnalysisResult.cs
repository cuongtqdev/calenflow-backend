namespace CalenFlowApp.Models
{
    public class AnalysisResult
    {
        public string Summary { get; set; }
        public List<string> Strengths { get; set; } = new();
        public List<string> Improvements { get; set; } = new();
        public string RawAIResponse { get; set; }
    }
}
