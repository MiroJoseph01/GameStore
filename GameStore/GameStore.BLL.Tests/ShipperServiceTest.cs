using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using GameStore.BLL.Services;
using GameStore.DAL.Interfaces.Repositories;
using Moq;
using Xunit;
using BusinessModels = GameStore.BLL.Models;
using DbModels = GameStore.DAL.Entities.MongoEntities;

namespace GameStore.BLL.Tests
{
    public class ShipperServiceTest
    {
        private readonly ShipperService _shipperService;
        private readonly Mock<IMongoShipperRepository> _shipperRepository;
        private readonly Mock<IMapper> _mapper;

        private readonly List<DbModels.Shipper> _shippersFromDb;
        private readonly List<BusinessModels.Shipper> _shippers;

        public ShipperServiceTest()
        {
            _shipperRepository = new Mock<IMongoShipperRepository>();
            _mapper = new Mock<IMapper>();

            _shipperService = new ShipperService(_shipperRepository.Object, _mapper.Object);

            _shippersFromDb = new List<DbModels.Shipper>
            {
                new DbModels.Shipper
                {
                    ShipperID = "1",
                    CompanyName = "Shipper",
                    Phone = "55-666",
                },
            };

            _shippers = new List<BusinessModels.Shipper>
            {
                new BusinessModels.Shipper
                {
                    ShipperID = "1",
                    CompanyName = "Shipper",
                    Phone = "55-666",
                },
            };
        }

        [Fact]
        public void GetAllShippers_ReturnsListOfShippers()
        {
            _shipperRepository.Setup(s => s.GetAll()).Returns(_shippersFromDb);
            _mapper.Setup(m => m.Map<IEnumerable<BusinessModels.Shipper>>(_shippersFromDb)).Returns(_shippers);

            var result = _shipperService.GetAllShippers();

            Assert.IsAssignableFrom<IEnumerable<BusinessModels.Shipper>>(result);
            Assert.Equal(_shippersFromDb.Count, result.ToList().Count);
        }

        [Fact]
        public void GetShipperById_ReturnsShipper()
        {
            _shipperRepository.Setup(s => s.GetById(It.IsAny<string>())).Returns(_shippersFromDb.First());
            _mapper.Setup(m => m.Map<BusinessModels.Shipper>(_shippersFromDb.First())).Returns(_shippers.First());

            var result = _shipperService.GetShipperById(It.IsAny<string>());

            Assert.IsType<BusinessModels.Shipper>(result);
        }
    }
}
