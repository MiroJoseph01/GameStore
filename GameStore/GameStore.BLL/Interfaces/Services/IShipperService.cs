using System.Collections.Generic;
using GameStore.BLL.Models;

namespace GameStore.BLL.Interfaces.Services
{
    public interface IShipperService
    {
        Shipper GetShipperById(string id);

        IEnumerable<Shipper> GetAllShippers();
    }
}
