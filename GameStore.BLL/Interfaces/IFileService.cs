using System.Net.Http;

namespace GameStore.BLL.Interfaces
{
    public interface IFileService
    {
        HttpResponseMessage CreateFile();
    }
}
