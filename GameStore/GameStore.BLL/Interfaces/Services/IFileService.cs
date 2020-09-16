using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.BLL.Interfaces
{
    public interface IFileService
    {
        Task<IActionResult> CreateFile(ControllerBase controller);
    }
}
