using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Tesseract;
using System.IO;
namespace Service
{
    public class OcrHelper : IOcrHelper
    {
        private readonly string _tessDataPath;


        public OcrHelper(IConfiguration configuration)
        {
            // Bạn có thể cấu hình tessdata path trong appsettings, ví dụ: "./tessdata"
            _tessDataPath = configuration["Tesseract:TessDataPath"]
                  ?? @"C:\Program Files\Tesseract-OCR\tessdata";
        }


        public string ExtractTextFromImage(Stream imageStream)
        {
            // Tesseract requires a seekable stream; copy to memory.
            using var ms = new MemoryStream();
            imageStream.CopyTo(ms);
            ms.Position = 0;


            using var engine = new TesseractEngine(_tessDataPath, "eng", EngineMode.Default);
            using var img = Pix.LoadFromMemory(ms.ToArray());
            using var page = engine.Process(img);
            return page.GetText();
        }
    }
}
