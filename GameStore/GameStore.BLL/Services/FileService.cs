using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using GameStore.BLL.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.BLL.Services
{
    public class FileService : IFileService
    {
        public async Task<IActionResult> CreateFile(ControllerBase controller)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "game.txt");

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }

            memory.Position = 0;

            return controller.File(memory, GetContentType(path), Path.GetFileName(path));
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"},
            };
        }
    }
}
