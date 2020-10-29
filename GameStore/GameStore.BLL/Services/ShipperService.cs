using System.Collections.Generic;
using AutoMapper;
using GameStore.BLL.Interfaces.Services;
using GameStore.BLL.Models;
using GameStore.DAL.Interfaces.Repositories;

namespace GameStore.BLL.Services
{
    public class ShipperService : IShipperService
    {
        private readonly IMongoShipperRepository _shipperRepository;
        private readonly IMapper _mapper;

        public ShipperService(IMongoShipperRepository shipperRepository, IMapper mapper)
        {
            _shipperRepository = shipperRepository;
            _mapper = mapper;
        }

        public IEnumerable<Shipper> GetAllShippers()
        {
            var shippers = _shipperRepository.GetAll();

            return _mapper.Map<IEnumerable<Shipper>>(shippers);
        }

        public Shipper GetShipperById(string id)
        {
            var shipper = _shipperRepository.GetById(id);

            return _mapper.Map<Shipper>(shipper);
        }
    }
}
