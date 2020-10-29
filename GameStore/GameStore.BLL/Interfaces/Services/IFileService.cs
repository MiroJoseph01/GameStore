using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.BLL.Interfaces.Services
{
    public interface IFileService
    {
        Task<IActionResult> CreateFile(ControllerBase controller);
    }
}
