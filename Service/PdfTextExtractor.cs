using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UglyToad.PdfPig;
using System.Text;
namespace Service
{
    public class PdfTextExtractor : IPdfTextExtractor
    {
        public string ExtractText(Stream pdfStream)
        {
            using var ms = new MemoryStream();
            pdfStream.CopyTo(ms);
            ms.Position = 0;


            var sb = new StringBuilder();
            using var document = PdfDocument.Open(ms);
            foreach (var page in document.GetPages())
            {
                sb.AppendLine(page.Text);
            }


            return sb.ToString();
        }
    }

}
