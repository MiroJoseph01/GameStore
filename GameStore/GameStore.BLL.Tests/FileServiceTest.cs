using System.Net.Http;
using GameStore.BLL.Services;
using Xunit;

namespace GameStore.BLL.Tests
{
    public class FileServiceTest
    {
        private readonly FileService _fileService;

        public FileServiceTest()
        {
            _fileService = new FileService();
        }

        [Fact]
        public void CreateFile_ReturnsHttpResponseMessage()
        {
            HttpResponseMessage res = _fileService.CreateFile();

            Assert.IsType<HttpResponseMessage>(res);
        }
    }
}
