using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Models
{
    public class AnalysisResult
    {
        public string Summary { get; set; }
        public List<string> Strengths { get; set; }
        public List<string> Improvements { get; set; }
        public List<string> Suggestions { get; set; }
        public string Raw { get; set; }
    }
}
