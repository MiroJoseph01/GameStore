using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using GameStore.BLL.Interfaces;
using GameStore.BLL.Interfaces.Services;
using GameStore.BLL.Models;
using GameStore.BLL.Payments;
using GameStore.Web.Controllers;
using GameStore.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace GameStore.Web.Tests
{
    public class BasketControllerTest
    {
        private const string _customerId = "83763329-8e85-4edf-a65e-83986c70edfb";

        private const string _orderId = "11763329-8e85-4edf-a65e-83986c70edfb";

        private readonly BasketController _basketController;
        private readonly Mock<IGameService> _gameService;
        private readonly Mock<IOrderService> _orderService;
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IPaymentContext> _paymentContext;
        private readonly Mock<IShipperService> _shipperService;

        private readonly List<Order> _orders;
        private readonly List<OrderStatus> _orderStatuses;
        private readonly List<OrderDetail> _orderDetails;
        private readonly List<BasketViewModel> _basketViews;
        private readonly Mock<IConfiguration> _configuration;

        public BasketControllerTest()
        {
            _gameService = new Mock<IGameService>();
            _orderService = new Mock<IOrderService>();
            _mapper = new Mock<IMapper>();
            _paymentContext = new Mock<IPaymentContext>();
            _configuration = new Mock<IConfiguration>();
            _shipperService = new Mock<IShipperService>();

            _basketController = new BasketController(
                _gameService.Object,
                _orderService.Object,
                _mapper.Object,
                _paymentContext.Object,
                _configuration.Object,
                _shipperService.Object);

            _orderStatuses = new List<OrderStatus>
            {
                new OrderStatus
                {
                    OrderStatusId = Guid.NewGuid().ToString(),
                    Status = "Open",
                },
                new OrderStatus
                {
                    OrderStatusId = Guid.NewGuid().ToString(),
                    Status = "Paid",
                },
            };

            _orderDetails = new List<OrderDetail>
            {
                new OrderDetail
                {
                    ProductId = Guid.NewGuid().ToString(),
                    ProductName = "Product Name1",
                    ProductKey = "product_name1",
                    Discount = 0.5f,
                    OrderDetailId = Guid.NewGuid().ToString(),
                    Price = 100,
                    Quantity = 4,
                    OrderId = _orderId,
                },
                new OrderDetail
                {
                    ProductId = Guid.NewGuid().ToString(),
                    ProductName = "Product Name2",
                    ProductKey = "product_name2",
                    Discount = 0.5f,
                    OrderDetailId = Guid.NewGuid().ToString(),
                    Price = 90,
                    Quantity = 3,
                    OrderId = _orderId,
                },
            };

            _orders = new List<Order>
            {
                new Order
                {
                OrderId = Guid.NewGuid().ToString(),
                CustomerId = _customerId,
                Status = "Open",
                OrderStatus = _orderStatuses.First(),
                Total = 10,
                OrderDetails = _orderDetails,
                },
            };

            List<OrderDetailViewModel> orderDetailViews = new List<OrderDetailViewModel>
            {
                 new OrderDetailViewModel
                 {
                    ProductId = _orderDetails.First().ProductId,
                    ProductName = "Product Name1",
                    Discount = 0.5f,
                    OrderDetailId = _orderDetails.First().OrderDetailId.ToString(),
                    Price = 100,
                    Quantity = 4,
                 },

                 new OrderDetailViewModel
                 {
                    ProductId = _orderDetails.Last().ProductId,
                    ProductName = "Product Name2",
                    Discount = 0.5f,
                    OrderDetailId = _orderDetails.Last().OrderDetailId.ToString(),
                    Price = 90,
                    Quantity = 3,
                 },
            };

            _basketViews = new List<BasketViewModel>
            {
                new BasketViewModel
                {
                    OrderId = _orders.First().OrderId.ToString(),
                    OrderDetails = orderDetailViews,
                    CustomerId = _orders.First().CustomerId.ToString(),
                    OrderStatus = _orders.First().OrderStatus.Status,
                    Total = _orders.First().Total,
                },

                new BasketViewModel
                {
                    OrderId = _orders.Last().OrderId.ToString(),
                    OrderDetails = orderDetailViews,
                    CustomerId = _orders.Last().CustomerId.ToString(),
                    OrderStatus = _orders.Last().OrderStatus.Status,
                    Total = _orders.Last().Total,
                },
            };
        }

        [Fact]
        public void ViewOrder_ReturnsOrderWithDetails()
        {
            _orderService
                .Setup(o => o.GetOrdersByCustomerId(It.IsAny<string>()))
                .Returns(_orders);
            _mapper
                .Setup(m => m.Map<IEnumerable<BasketViewModel>>(It.IsAny<IEnumerable<Order>>()))
                .Returns(_basketViews);

            IActionResult result = _basketController.ViewOrder();

            ViewResult view = Assert.IsType<ViewResult>(result);
            var model = Assert
                .IsAssignableFrom<GeneralBasketViewModel>(view.Model);
        }

        [Fact]
        public void AddDetail_PassNonExistingGameKey_ReturnsNotFound()
        {
            _gameService
                .Setup(g => g.GetGameByKey(It.IsAny<string>()))
                .Returns((Game)null);

            var result = _basketController.Buy(It.IsAny<string>());

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void AddDetail_PassGameKey_ReturnsRedirectToOrderAction()
        {
            var game = new Game();

            _gameService
                .Setup(g => g.GetGameByKey(It.IsAny<string>()))
                .Returns(game);

            var result = _basketController.Buy(It.IsAny<string>());

            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void ViewPayment_PassId_ReturnsViewResult()
        {
            var expectedNumberOfPaymentMethods = 3;

            var game = new Game
            {
                Name = "Name",
                UnitsInStock = 10,
            };

            _orderService
                .Setup(o => o.GetAllOrderDetails())
                .Returns(_orderDetails);
            _gameService
                .Setup(g => g.GetGameById(It.IsAny<string>()))
                .Returns(game);

            var payment = new PaymentViewModel
            {
                OrderId = _orderId.ToString(),
                Total = 10,
            };

            var result = _basketController.ViewPayment(_orderId.ToString());

            ViewResult view = Assert.IsType<ViewResult>(result);
            var model = Assert
                .IsAssignableFrom<List<PaymentViewModel>>(view.Model);
            Assert.Equal(expectedNumberOfPaymentMethods, model.Count);
        }

        [Fact]
        public void Bank_PassIdAndTotal_ReturnsFileStreamsResult()
        {
            MemoryStream stream = new MemoryStream();

            var info = new PaymentInfo
            {
                FileStreamResult = new FileStreamResult(stream, "application/pdf"),
                Id = _orderId,
            };

            _paymentContext
                .Setup(p => p.ProcessPayment(It.IsAny<string>()))
                .Returns(info);

            var result = _basketController.PayWithBank(_orderId.ToString());

            Assert.IsType<FileStreamResult>(result);
        }

        [Fact]
        public void Visa_PassIdAndTotal_ReturnsViewResult()
        {
            decimal total = 10;

            var result = _basketController.PayWithCard(_orderId.ToString());

            ViewResult view = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<CreditCardViewModel>(view.Model);
        }

        [Fact]
        public void IBox_PassIdAndTotal_ReturnsRedirect()
        {
            _configuration.Setup(x => x[It.IsAny<string>()]).Returns("https://www.microsoft.com");

            var result = _basketController.PayWithIBox(_orderId.ToString());

            Assert.IsType<RedirectResult>(result);
        }

        [Fact]
        public void Card_PassValidCreditCardViewModel_ReturnsRedirectToAction()
        {
            var creditCard = new CreditCardViewModel
            {
                CardNumber = "1111 1111 1111 1111",
                Date = "12/22",
                CVV = "2222",
                FullName = "Full name",
            };

            _orderService.Setup(o => o.DateIsCorrect(creditCard.Date)).Returns(true);

            var result = _basketController.PayWithCard(creditCard, _orderId.ToString());

            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void Card_PassCreditCardViewModelWitnInvalidData_ReturnViewResult()
        {
            var creditCard = new CreditCardViewModel
            {
                CardNumber = "1111 1111 1111 1111",
                Date = "30/20",
                CVV = "2222",
                FullName = "Full name",
            };

            var result = _basketController.PayWithCard(creditCard, _orderId.ToString());

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Card_PassInvalidCreditCardViewModelWitn_ReturnViewResult()
        {
            var creditCard = new CreditCardViewModel
            {
                CardNumber = "",
                Date = "11/23",
                CVV = "222",
                FullName = "Full name",
            };

            _basketController.ModelState
                .AddModelError("CardNumber", "Card Number is required");

            IActionResult result = _basketController.PayWithCard(creditCard, _orderId.ToString());

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void DeleteDetail_PassDetailId_ReturnsRedirectResult()
        {
            var id = "83763329-8e85-4edf-a65e-83986c70edfb";

            var result = _basketController.DeleteDetail(id);

            _orderService.Verify(o => o.DeleteOrderDetail(It.IsAny<OrderDetail>()));
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void ViewDetails_PassId_ReturnsViewWithForm()
        {
            _orderService.Setup(o => o.GetOrderById(It.IsAny<string>())).Returns(_orders.First());
            _mapper.Setup(m => m.Map<BasketViewModel>(It.IsAny<Order>())).Returns(_basketViews.First());

            var result = _basketController.ViewDetails(It.IsAny<string>());

            ViewResult view = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<BasketViewModel>(view.Model);
        }

        [Fact]
        public void ViewDetails_PassModel_RedirectToAction()
        {
            var correctModel = new BasketViewModel
            {
                CustomerId = Guid.NewGuid().ToString(),
                OrderId = Guid.NewGuid().ToString(),
                OrderDate = DateTime.Now.ToString(),
                Freight = 10,
                ShipCity = "City",
                OrderDetails = null,
                ShipAddress = "Address to ship",
                ShipVia = "1",
            };

            _orderService.Setup(o => o.GetOrderById(It.IsAny<string>())).Returns(_orders.First());
            _shipperService.Setup(s => s.GetAllShippers()).Returns(new List<Shipper>
            {
                new Shipper
                {
                    ShipperID = "1",
                    CompanyName = "Name",
                    Phone = "66-555",
                },
            });

            var result = _basketController.ViewDetails(correctModel, It.IsAny<string>());

            _orderService.Verify(x => x.UpdateOrder(It.IsAny<Order>()));

            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void ViewDetails_PassUnvalidModel_RedirectToAction()
        {
            var incorrectModel = new BasketViewModel
            {
                CustomerId = Guid.NewGuid().ToString(),
                OrderId = Guid.NewGuid().ToString(),
                OrderDate = DateTime.Now.ToString(),
                Freight = 10,
                ShipCity = "City",
                OrderDetails = null,
                ShipAddress = "Address to ship",
                ShipVia = "-1",
            };

            _shipperService.Setup(s => s.GetAllShippers()).Returns(new List<Shipper>
            {
                new Shipper
                {
                    ShipperID = "1",
                    CompanyName = "Name",
                    Phone = "66-555",
                },
            });

            var result = _basketController.ViewDetails(incorrectModel, It.IsAny<string>());

            Assert.IsType<ViewResult>(result);
        }
    }
}
