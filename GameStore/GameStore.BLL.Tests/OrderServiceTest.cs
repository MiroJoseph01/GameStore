using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using GameStore.BLL.Interfaces.Services;
using GameStore.BLL.Payments;
using GameStore.BLL.Services;
using GameStore.DAL.Interfaces;
using GameStore.DAL.Interfaces.Repositories;
using Moq;
using Xunit;
using BusinessModels = GameStore.BLL.Models;
using DbModels = GameStore.DAL.Entities;

namespace GameStore.BLL.Tests
{
    public class OrderServiceTest
    {
        private readonly string _customerId = Guid.NewGuid().ToString();

        private readonly OrderService _orderService;
        private readonly Mock<IGameService> _gameService;
        private readonly Mock<IOrderRepositoryFacade> _orderRepository;
        private readonly Mock<IOrderDetailRepository> _orderDetailRepository;
        private readonly Mock<IRepository<DbModels.OrderStatus>> _orderStatusRepository;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IMapper> _mapper;

        private readonly List<BusinessModels.Order> _orders;
        private readonly List<DbModels.Order> _ordersFromDb;

        private readonly List<BusinessModels.OrderDetail> _orderDetails;
        private readonly List<DbModels.OrderDetail> _orderDetailsFromDb;

        private readonly List<BusinessModels.OrderStatus> _orderStatuses;
        private readonly List<DbModels.OrderStatus> _orderStatusesFromDb;

        public OrderServiceTest()
        {
            _gameService = new Mock<IGameService>();
            _orderRepository = new Mock<IOrderRepositoryFacade>();
            _orderDetailRepository = new Mock<IOrderDetailRepository>();
            _orderStatusRepository = new Mock<IRepository<DbModels.OrderStatus>>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _mapper = new Mock<IMapper>();

            _orderService = new OrderService(
                _gameService.Object,
                _orderRepository.Object,
                _orderDetailRepository.Object,
                _orderStatusRepository.Object,
                _unitOfWork.Object,
                _mapper.Object);

            _orderDetails = new List<BusinessModels.OrderDetail>
            {
                new BusinessModels.OrderDetail
                {
                    OrderDetailId = Guid.NewGuid().ToString(),
                    ProductName = "Product Name 1",
                    Price = 10,
                    Discount = 0.5f,
                    ProductId = Guid.NewGuid().ToString(),
                    ProductKey = "product_key_1",
                    Quantity = 1,
                },

                new BusinessModels.OrderDetail
                {
                    OrderDetailId = Guid.NewGuid().ToString(),
                    ProductName = "Product Name 2",
                    Price = 20,
                    Discount = 0.5f,
                    ProductId = Guid.NewGuid().ToString(),
                    ProductKey = "product_key_2",
                    Quantity = 4,
                },

                new BusinessModels.OrderDetail
                {
                    OrderDetailId = Guid.NewGuid().ToString(),
                    ProductName = "Product Name 3",
                    Price = 20,
                    Discount = 0.5f,
                    ProductId = Guid.NewGuid().ToString(),
                    ProductKey = "product_key_3",
                    Quantity = 5,
                },
            };

            _orderDetailsFromDb = new List<DbModels.OrderDetail>
            {
                new DbModels.OrderDetail
                {
                    OrderDetailId = _orderDetails.First().OrderDetailId,
                    Price = 10,
                    Discount = 0.5f,
                    ProductId = _orderDetails.First().ProductId,
                    Quantity = 1,
                },

                new DbModels.OrderDetail
                {
                    OrderDetailId = _orderDetails.ElementAt(1).OrderDetailId,
                    Price = 20,
                    Discount = 0.5f,
                    ProductId = _orderDetails.ElementAt(1).ProductId,
                    Quantity = 4,
                },

                new DbModels.OrderDetail
                {
                    OrderDetailId = _orderDetails.Last().OrderDetailId,
                    Price = 20,
                    Discount = 0.5f,
                    ProductId = _orderDetails.Last().ProductId,
                    Quantity = 5,
                },
            };

            _orders = new List<BusinessModels.Order>
            {
                new BusinessModels.Order
                {
                    OrderId = Guid.NewGuid().ToString(),
                    Status = "Open",
                    CustomerId = _customerId,
                    OrderDetails = _orderDetails,
                },

                new BusinessModels.Order
                {
                    OrderId = Guid.NewGuid().ToString(),
                    Status = "Submitted",
                    CustomerId = _customerId,
                    OrderDetails =
                    new List<BusinessModels.OrderDetail> { _orderDetails.Last() },
                },
            };

            _ordersFromDb = new List<DbModels.Order>
            {
                new DbModels.Order
                {
                    OrderId = _orders.First().OrderId,
                    Status = OrderStatuses.Open,
                    CustomerId = _customerId,
                    OrderDetails = _orderDetailsFromDb,
                },

                new DbModels.Order
                {
                    OrderId = _orders.Last().OrderId,
                    Status = OrderStatuses.Open,
                    CustomerId = _customerId,
                },
            };

            _orderStatuses = new List<BusinessModels.OrderStatus>
            {
                new BusinessModels.OrderStatus
                {
                    OrderStatusId = Guid.NewGuid().ToString(),
                    Status = "Open",
                },
            };

            _orderStatusesFromDb = new List<DbModels.OrderStatus>
            {
                new DbModels.OrderStatus
                {
                    OrderStatusId = Guid.NewGuid().ToString(),
                    Status = "Open",
                },
            };
        }

        [Fact]
        public void AddOrderDetail_PassCustomerIdWithNonExistingOrderAndOrderDetail_VerifyCreatingOrderAndOrderDetail()
        {
            var game = new BusinessModels.Game
            {
                Key = "game key",
                Name = "game name",
                UnitsInStock = 1,
                GameId = "Game Id",
            };
            _orderRepository
                .Setup(o => o.GetByCustomerId(_customerId))
                .Returns((IEnumerable<DbModels.Order>)null);
            _mapper
                .Setup(m => m.Map<DbModels.Order>(It.IsAny<BusinessModels.Order>()))
                .Returns(_ordersFromDb.First());
            _mapper
                .Setup(m => m.Map<DbModels.OrderDetail>(_orderDetails.First()))
                .Returns(_orderDetailsFromDb.First());
            _gameService
                .Setup(g => g.GetGameByKey(It.IsAny<string>()))
                .Returns(game);
            _gameService
                .Setup(g => g.GetGameById(It.IsAny<string>()))
                .Returns(game);
            _gameService
                .Setup(g => g.EditGame(It.IsAny<BusinessModels.Game>(), It.IsAny<short>()))
                .Returns(game);

            _orderService.AddOrderDetail(_customerId.ToString(), game.Key);

            _orderRepository.Verify(o => o.Create(It.IsAny<DbModels.Order>()));
        }

        [Fact]
        public void AddOrderDetail_PassCustomerIdWithExistingOrderAndNonExistingOrderDetail_VerifyCreatingOrderDetail()
        {
            var game = new BusinessModels.Game
            {
                Key = "game key",
                Name = "game name",
                GameId = "Game Id",
            };
            _orderRepository
                .Setup(o => o.GetByCustomerId(_customerId))
                .Returns(_ordersFromDb);
            _mapper
                .Setup(m => m.
                    Map<IEnumerable<BusinessModels.Order>>(
                        It.IsAny<IEnumerable<DbModels.Order>>()))
                .Returns(new List<BusinessModels.Order> { _orders.Last() });
            _mapper
                .Setup(m => m.Map<DbModels.OrderDetail>(_orderDetails.Last()));
            _gameService
                .Setup(g => g.GetGameByKey(It.IsAny<string>()))
                .Returns(game);
            _gameService
                .Setup(g => g.GetGameById(It.IsAny<string>()))
                .Returns(game);

            _orderService.AddOrderDetail(_customerId.ToString(), game.Key);
        }

        [Fact]
        public void AddOrderDetail_PassCustomerIdWithExistingOrderAndOrderDetail_VerifyEditingOrderDetail()
        {
            BusinessModels.Game game = new BusinessModels.Game
            {
                Key = "game key",
                Name = "game name",
                GameId = Guid.NewGuid().ToString(),
                UnitsInStock = 10,
            };

            BusinessModels.OrderDetail detail = new BusinessModels.OrderDetail
            {
                OrderDetailId = Guid.NewGuid().ToString(),
                OrderId = _orders.First().OrderId,
                ProductName = "Product Name 1",
                Price = 10,
                Discount = 0.5f,
                ProductId = game.GameId.ToString(),
                ProductKey = "product_key_1",
                Quantity = 1,
            };

            DbModels.OrderDetail detailFromDb = new DbModels.OrderDetail
            {
                OrderDetailId = detail.OrderDetailId,
                OrderId = _orders.First().OrderId,
                Price = 10,
                Discount = 0.5f,
                ProductId = game.GameId.ToString(),
                Quantity = 1,
            };

            List<BusinessModels.Order> orders = new List<BusinessModels.Order>
            {
                new BusinessModels.Order
                {
                    OrderId = Guid.NewGuid().ToString(),
                    Status = "Open",
                    CustomerId = _customerId,
                    OrderDetails = new List<BusinessModels.OrderDetail>
                    {
                        detail,
                    },
                },
            };

            _orderDetailRepository
                .Setup(o => o.GetOrderDetailByOrderIdAndProductId(
                        It.IsAny<string>(),
                        It.IsAny<string>()))
                .Returns(detailFromDb);
            _mapper
                .Setup(m => m
                    .Map<IEnumerable<BusinessModels.Order>>(
                        It.IsAny<IEnumerable<DbModels.Order>>()))
                .Returns(orders);
            _mapper
                .Setup(m => m.Map<DbModels.OrderDetail>(_orderDetails.First()));
            _gameService
                .Setup(g => g.GetGameByKey(It.IsAny<string>()))
                .Returns(game);
            _gameService
                .Setup(g => g.EditGame(It.IsAny<BusinessModels.Game>(), It.IsAny<short>()))
                .Returns(game);

            _gameService.Setup(g => g.GetGameById(It.IsAny<string>())).Returns(game);

            _orderService.AddOrderDetail(_customerId.ToString(), game.Key);

            _orderDetailRepository
                .Verify(o => o
                    .Update(It.IsAny<string>(), It.IsAny<DbModels.OrderDetail>()));
        }

        [Fact]
        public void GetOrdersByCustomerId_PassCustomerId_ReturnsListOfOrders()
        {
            BusinessModels.Game game = new BusinessModels.Game
            {
                Key = "game key",
                Name = "game name",
            };

            _orderRepository
                .Setup(o => o.GetByCustomerId(_customerId))
                .Returns(_ordersFromDb);
            _mapper
                .Setup(m => m
                    .Map<IEnumerable<BusinessModels.Order>>(
                        It.IsAny<IEnumerable<DbModels.Order>>()))
                .Returns(_orders);
            _mapper
                .Setup(m => m
                    .Map<IEnumerable<BusinessModels.OrderDetail>>(
                        It.IsAny<IEnumerable<DbModels.OrderDetail>>()))
                .Returns(_orderDetails);

            _gameService.Setup(g => g.GetGameById(It.IsAny<string>())).Returns(game);

            int result = _orderService.GetOrdersByCustomerId(_customerId.ToString()).Count();

            Assert.Equal(_orders.Count, result);
        }

        [Fact]
        public void GetOrdersByCustomerId_PassNonexistingCustomerId_ReturnsNull()
        {
            _mapper
                .Setup(m => m.
                    Map<IEnumerable<BusinessModels.Order>>(
                        It.IsAny<IEnumerable<DbModels.Order>>()))
                .Returns((IEnumerable<BusinessModels.Order>)null);

            IEnumerable<BusinessModels.Order> result = _orderService
                .GetOrdersByCustomerId(Guid.NewGuid().ToString());

            Assert.Null(result);
        }

        [Fact]
        public void AddOrder_PassOrderModel_ReturnsOrder()
        {
            _mapper
                .Setup(m => m
                    .Map<DbModels.Order>(It.IsAny<BusinessModels.Order>()))
                .Returns(_ordersFromDb.First());

            BusinessModels.Order result = _orderService
                .AddOrder(_orders.First());

            _orderRepository.Verify(g => g.Create(It.IsAny<DbModels.Order>()));
            Assert.NotNull(result);
        }

        [Fact]
        public void DeleteOrder_PassOrderModel_VerifyDeleting()
        {
            _orderRepository
                .Setup(o => o.IsPresent(It.IsAny<string>()))
                .Returns(true);
            _orderRepository
                .Setup(g => g.GetById(It.IsAny<string>()))
                .Returns(_ordersFromDb.First());

            _orderService.DeleteOrder(_orders.First());

            _orderRepository.Verify(g => g.Delete(_orders.First().OrderId));
        }

        [Fact]
        public void DeleteOrder_PassNonExistingOrderModel_ThrowsException()
        {
            DbModels.Order order = _ordersFromDb.First();
            order.IsRemoved = true;

            _orderRepository.Setup(g => g.GetById(It.IsAny<string>())).Returns(order);

            Assert.Throws<ArgumentException>(() => _orderService
                .DeleteOrder(_orders.First()));

            order = null;

            Assert.Throws<ArgumentException>(() => _orderService
                .DeleteOrder(_orders.First()));
        }

        [Fact]
        public void UpdateOrder_PassOrderModel_ReturnsOrder()
        {
            _mapper
                .Setup(m => m
                    .Map<DbModels.Order>(It.IsAny<BusinessModels.Order>()))
                .Returns(_ordersFromDb.First());

            BusinessModels.Order result = _orderService
                .UpdateOrder(_orders.First());

            _orderRepository
                .Verify(c => c.Update(It.IsAny<string>(), It.IsAny<DbModels.Order>()));

            Assert.NotNull(result);
        }

        [Fact]
        public void GetOrderById_PassOrderId_ReturnsOrder()
        {
            var game = new BusinessModels.Game
            {
                Key = "Key",
                Name = "Name",
                Price = 10.0M,
            };

            _orderRepository
                .Setup(c => c.GetById(It.IsAny<string>()))
                .Returns(_ordersFromDb.First());
            _gameService
                .Setup(g => g.GetGameById(It.IsAny<string>()))
                .Returns(game);
            _mapper
                .Setup(m => m
                    .Map<BusinessModels.Order>(It.IsAny<DbModels.Order>()))
                .Returns(_orders.First());

            BusinessModels.Order result = _orderService
                .GetOrderById(_orders.First().OrderId);

            Assert.NotNull(result);
        }

        [Fact]
        public void GetOrderById_PassNonExistingOrderId_ReturnsNull()
        {
            DbModels.Order order = _ordersFromDb.First();
            order.IsRemoved = true;

            BusinessModels.Order result = _orderService
                .GetOrderById(_orders.First().OrderId);

            Assert.Null(result);
        }

        [Fact]
        public void GetAllOrders_ReturnsListOfOrders()
        {
            var game = new BusinessModels.Game
            {
                Key = "Key",
                Name = "Name",
                Price = 10.0M,
            };

            _orderRepository.Setup(p => p.GetAll()).Returns(_ordersFromDb);
            _gameService
                .Setup(g => g.GetGameById(It.IsAny<string>()))
                .Returns(game);
            _mapper
                .Setup(m => m
                    .Map<IEnumerable<BusinessModels.Order>>(_ordersFromDb))
                .Returns(_orders);

            int result = _orderService.GetAllOrders().Count();

            Assert.Equal(_orders.Count(), result);
        }

        [Fact]
        public void GetAllOrders_ReturnsEmptyList()
        {
            List<DbModels.Order> ordersFromDb = new List<DbModels.Order>();
            List<BusinessModels.Order> orders =
                new List<BusinessModels.Order>();

            _orderRepository
                .Setup(g => g.GetAll())
                .Returns(ordersFromDb);
            _mapper
                .Setup(m => m
                    .Map<IEnumerable<BusinessModels.Order>>(
                        It.IsAny<IEnumerable<DbModels.Order>>()))
                .Returns(orders);

            IEnumerable<BusinessModels.Order> result = _orderService
                .GetAllOrders();

            Assert.Empty(result);
        }

        [Fact]
        public void DeleteOrderDetail_PassOrderDetailModel_VerifyDeleting()
        {
            var game = new BusinessModels.Game
            {
                UnitsInStock = 0,
            };

            _orderDetailRepository
                .Setup(o => o.IsPresent(It.IsAny<string>()))
                .Returns(true);
            _orderDetailRepository
                .Setup(g => g.GetById(It.IsAny<string>()))
                .Returns(_orderDetailsFromDb.First());
            _gameService
                .Setup(g => g.GetGameById(It.IsAny<string>()))
                .Returns(game);
            _orderService
                .DeleteOrderDetail(_orderDetails.First());

            _orderDetailRepository.Verify(g => g.Delete(_orderDetails.First().OrderDetailId));
        }

        [Fact]
        public void DeleteOrderDetail_PassNonExistingOrderDetailModel()
        {
            DbModels.OrderDetail orderDetail = _orderDetailsFromDb.First();
            orderDetail.IsRemoved = true;

            _orderDetailRepository
                .Setup(g => g.GetById(It.IsAny<string>()))
                .Returns(orderDetail);

            Assert
                .Throws<ArgumentException>(() => _orderService.DeleteOrderDetail(_orderDetails.First()));

            orderDetail = null;

            Assert
                .Throws<ArgumentException>(() => _orderService.DeleteOrderDetail(_orderDetails.First()));
        }

        [Fact]
        public void UpdateOrderDetail_PassOrderDetailModel_ReturnsOrderetail()
        {
            _mapper
                .Setup(m => m
                    .Map<DbModels.OrderDetail>(
                        It.IsAny<BusinessModels.OrderDetail>()))
                .Returns(_orderDetailsFromDb.First());

            BusinessModels.OrderDetail result = _orderService
                .UpdateOrderDetail(_orderDetails.First());

            _orderDetailRepository
                .Verify(c => c
                    .Update(It.IsAny<string>(), It.IsAny<DbModels.OrderDetail>()));

            Assert.NotNull(result);
        }

        [Fact]
        public void GetOrderDetailById_PassOrderDetailId_ReturnsOrderDetail()
        {
            _orderDetailRepository
                .Setup(o => o.IsPresent(It.IsAny<string>()))
                .Returns(true);
            _orderDetailRepository
                .Setup(c => c.GetById(It.IsAny<string>()))
                .Returns(_orderDetailsFromDb.First());
            _mapper
                .Setup(m => m
                    .Map<BusinessModels.OrderDetail>(
                        It.IsAny<DbModels.OrderDetail>()))
                .Returns(_orderDetails.First());

            BusinessModels.OrderDetail result = _orderService
                .GetOrderDetailById(_orderDetails.First().OrderDetailId);

            Assert.NotNull(result);
        }

        [Fact]
        public void GetOrderDetailById_PassNonExistingOrderDetailId_ReturnsNull()
        {
            DbModels.OrderDetail orderDetail = _orderDetailsFromDb.First();
            orderDetail.IsRemoved = true;

            BusinessModels.OrderDetail result = _orderService
                .GetOrderDetailById(_orderDetails.First().OrderDetailId);

            Assert.Null(result);
        }

        [Fact]
        public void GetAllOrderDetails_ReturnsListOfOrderDetails()
        {
            _orderDetailRepository
                .Setup(p => p.GetAll())
                .Returns(_orderDetailsFromDb);
            _mapper
                .Setup(m => m
                    .Map<IEnumerable<BusinessModels.OrderDetail>>(
                        _orderDetailsFromDb))
                .Returns(_orderDetails);

            int result = _orderService.GetAllOrderDetails().Count();

            Assert.Equal(_orderDetails.Count(), result);
        }

        [Fact]
        public void GetAllOrderDetails_ReturnsEmptyList()
        {
            List<DbModels.OrderDetail> orderDetailsFromDb =
                new List<DbModels.OrderDetail>();
            List<BusinessModels.OrderDetail> orderDetails =
                new List<BusinessModels.OrderDetail>();

            _orderDetailRepository
                .Setup(g => g.GetAll())
                .Returns(orderDetailsFromDb);
            _mapper
                .Setup(m => m
                    .Map<IEnumerable<BusinessModels.OrderDetail>>(
                        It.IsAny<IEnumerable<DbModels.OrderDetail>>()))
                .Returns(orderDetails);

            IEnumerable<BusinessModels.OrderDetail> result = _orderService
                .GetAllOrderDetails();

            Assert.Empty(result);
        }

        [Fact]
        public void AddOrderStatus_PassOrderStatusModel_ReturnsOrderStatus()
        {
            _mapper
                .Setup(m => m
                    .Map<DbModels.OrderStatus>(
                        It.IsAny<BusinessModels.OrderStatus>()))
                .Returns(_orderStatusesFromDb.First());

            BusinessModels.OrderStatus result = _orderService
                .AddOrderStatus(_orderStatuses.First());

            _orderStatusRepository.Verify(g => g.Create(It.IsAny<DbModels.OrderStatus>()));
            Assert.NotNull(result);
        }

        [Fact]
        public void DeleteOrderStatus_PassOrderStatusModel_VerifyDeleting()
        {
            _orderStatusRepository.Setup(o => o.IsPresent(It.IsAny<string>())).Returns(true);
            _orderStatusRepository
                .Setup(g => g.GetById(It.IsAny<string>()))
                .Returns(_orderStatusesFromDb.First());
            _orderService.DeleteOrderStatus(_orderStatuses.First());

            _orderStatusRepository.Verify(g => g.Delete(_orderStatuses.First().OrderStatusId));
        }

        [Fact]
        public void DeleteOrderStatus_PassNonExistingOrderStatusModel()
        {
            DbModels.OrderStatus orderStatus = _orderStatusesFromDb.First();
            orderStatus.IsRemoved = true;

            _orderStatusRepository
                .Setup(g => g.GetById(It.IsAny<string>()))
                .Returns(orderStatus);

            Assert
                .Throws<ArgumentException>(() => _orderService.DeleteOrderStatus(_orderStatuses.First()));

            orderStatus = null;

            Assert
                .Throws<ArgumentException>(() => _orderService.DeleteOrderStatus(_orderStatuses.First()));
        }

        [Fact]
        public void UpdateOrderStatus_PassOrderStatusModel_ReturnsOrderStatus()
        {
            _mapper
                .Setup(m => m
                    .Map<DbModels.OrderStatus>(
                        It.IsAny<BusinessModels.OrderStatus>()))
                .Returns(_orderStatusesFromDb.First());

            BusinessModels.OrderStatus result = _orderService
                .UpdateOrderStatus(_orderStatuses.First());

            _orderStatusRepository
                .Verify(c => c
                    .Update(It.IsAny<string>(), It.IsAny<DbModels.OrderStatus>()));
            Assert.NotNull(result);
        }

        [Fact]
        public void GetOrderStatusByName_PassOrderStatusId_ReturnsOrderStatus()
        {
            _orderStatusRepository
                .Setup(c => c.GetAll())
                .Returns(_orderStatusesFromDb);
            _mapper
                .Setup(m => m
                    .Map<BusinessModels.OrderStatus>(
                        It.IsAny<DbModels.OrderStatus>()))
                .Returns(_orderStatuses.First());

            BusinessModels.OrderStatus result =
                _orderService.GetOrderStatusByName(_orderStatuses.First().Status);

            Assert.NotNull(result);
        }

        [Fact]
        public void GetOrderStatusByName_PassNonExistingOrderStatusId_ReturnsNull()
        {
            DbModels.OrderStatus genre = _orderStatusesFromDb.First();
            genre.IsRemoved = true;

            BusinessModels.OrderStatus result =
                _orderService.GetOrderStatusByName(_orderStatuses.First().Status);

            Assert.Null(result);
        }

        [Fact]
        public void GetAllOrderStatuses_ReturnsListOfOrderStatuses()
        {
            _orderStatusRepository
                .Setup(p => p.GetAll())
                .Returns(_orderStatusesFromDb);

            _mapper
                .Setup(m => m
                    .Map<IEnumerable<BusinessModels.OrderStatus>>(
                        _orderStatusesFromDb))
                .Returns(_orderStatuses);

            int result = _orderService.GetAllOrderStatuses().Count();

            Assert.Equal(_orderStatuses.Count(), result);
        }

        [Fact]
        public void GetAllorderStatuses_ReturnsEmptyList()
        {
            List<DbModels.OrderStatus> orderStatusesFromDb =
                new List<DbModels.OrderStatus>();
            List<BusinessModels.OrderStatus> orderStatuses =
                new List<BusinessModels.OrderStatus>();

            _orderStatusRepository
                .Setup(g => g.GetAll())
                .Returns(orderStatusesFromDb);

            _mapper
                .Setup(m => m
                    .Map<IEnumerable<BusinessModels.OrderStatus>>(
                        It.IsAny<IEnumerable<DbModels.OrderStatus>>()))
                .Returns(orderStatuses);

            IEnumerable<BusinessModels.OrderStatus> result = _orderService
                .GetAllOrderStatuses();

            Assert.Empty(result);
        }
    }
}
