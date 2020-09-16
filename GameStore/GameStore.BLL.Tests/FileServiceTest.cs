using System.Net.Http;
using System.Threading.Tasks;
using GameStore.BLL.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
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
        public void CreateFile_ReturnsFile()
        {
            var res = _fileService.CreateFile(It.IsAny<ControllerBase>());

            Assert.IsType<Task<IActionResult>>(res);
        }
    }
}
