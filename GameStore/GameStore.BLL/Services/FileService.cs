using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using GameStore.BLL.Interfaces;

namespace GameStore.BLL.Services
{
    public class FileService : IFileService
    {
        public HttpResponseMessage CreateFile()
        {
            MemoryStream stream = new MemoryStream();

            UnicodeEncoding uniEncoding = new UnicodeEncoding();

            byte[] binary = uniEncoding.GetBytes("Hello World!");

            stream.Write(binary, 0, binary.Length);

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(stream.ToArray()),
            };

            result.Content.Headers.ContentDisposition =
                new ContentDispositionHeaderValue("attachment")
                {
                    FileName = "game.txt",
                };
            result.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/octet-stream");

            return result;
        }
    }
}
