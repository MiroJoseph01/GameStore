using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using GameStore.BLL.Interfaces;
using GameStore.BLL.Models;
using GameStore.BLL.Payments;
using GameStore.BLL.Payments.PaymentStrategies;
using GameStore.Web.Util;
using GameStore.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace GameStore.Web.Controllers
{
    [CustomController("Basket")]
    public class BasketController : Controller
    {
        private readonly IGameService _gameService;
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;
        private readonly IPaymentContext _paymentContext;
        private readonly IConfiguration _iConfiguration;

        public BasketController(
            IGameService gameService,
            IOrderService orderService,
            IMapper mapper,
            IPaymentContext paymentContext,
            IConfiguration configuration)
        {
            _gameService = gameService;
            _orderService = orderService;
            _mapper = mapper;
            _paymentContext = paymentContext;
            _iConfiguration = configuration;
        }

        [HttpGet]
        [Route("/busket")]
        public IActionResult ViewOrder()
        {
            var orders = _mapper.Map<IEnumerable<BasketViewModel>>(_orderService.GetOrdersByCustomerId(Constants.UserId));

            var ordersToView = orders.Where(x => x.OrderStatus == OrderStatuses.Open || x.OrderStatus == OrderStatuses.NotPaid);

            return View(ordersToView);
        }

        [HttpGet]
        [Route("game/{key}/buy")]
        public IActionResult Buy(string key)
        {
            Game game = _gameService.GetGameByKey(key);

            if (game is null)
            {
                return NotFound();
            }

            _orderService.AddOrderDetail(Constants.UserId, game.Key);

            return RedirectToAction(nameof(ViewOrder));
        }

        [HttpGet]
        [Route("/payment/{id}")]
        public IActionResult ViewPayment(string id)
        {
            if (_orderService.OrderIsPaid(Guid.Parse(id)))
            {
                return RedirectToAction(nameof(ViewPaidPage), new { id });
            }

            var methods = new List<PaymentViewModel>
            {
                new PaymentViewModel
                {
                    OrderId = id,
                    PaymentMethod = "PayWithBank",
                    PaymentName = "Bank",
                },
                new PaymentViewModel
                {
                    OrderId = id,
                    PaymentMethod = "PayWithIBox",
                    PaymentName = "IBox Terminal",
                },
                new PaymentViewModel
                {
                    OrderId = id,
                    PaymentMethod = "PayWithCard",
                    PaymentName = "Visa",
                },
            };
            return View(methods);
        }

        [HttpGet]
        [Route("/payment/bank/{id}")]
        public IActionResult PayWithBank(string id)
        {
            if (_orderService.OrderIsPaid(Guid.Parse(id)))
            {
                return RedirectToAction(nameof(ViewPaidPage), new { id });
            }

            _paymentContext.SetStrategy(new BankPaymentStrategy(_orderService));

            PaymentInfo info = _paymentContext.ProcessPayment(Guid.Parse(id));

            return info.FileStreamResult;
        }

        [HttpGet]
        [Route("/payment/ibox/{id}")]
        public IActionResult PayWithIBox(string id)
        {
            if (_orderService.OrderIsPaid(Guid.Parse(id)))
            {
                return RedirectToAction(nameof(ViewPaidPage), new { id });
            }

            _paymentContext.SetStrategy(new IBoxPaymentStrategy(_orderService));

            _paymentContext.ProcessPayment(Guid.Parse(id));

            return Redirect(_iConfiguration[Constants.IBoxPage]);
        }

        [HttpGet]
        [Route("/payment/visa/{id}")]
        public IActionResult PayWithCard(string id)
        {
            if (_orderService.OrderIsPaid(Guid.Parse(id)))
            {
                return RedirectToAction(nameof(ViewPaidPage), new { id });
            }

            return View(new CreditCardViewModel());
        }

        [HttpPost]
        [Route("/payment/visa/{id}")]
        public IActionResult PayWithCard(CreditCardViewModel creditCard, string id)
        {
            if (!_orderService.DateIsCorrect(creditCard.Date))
            {
                ModelState.AddModelError("Date", "Incorrect date");
            }

            if (!ModelState.IsValid)
            {
                return View(creditCard);
            }

            _paymentContext.SetStrategy(new CardPaymentStrategy(_orderService));

            _paymentContext.ProcessPayment(Guid.Parse(id));

            return RedirectToAction(nameof(ViewSuccessfulPaymentPage));
        }

        [HttpGet]
        [Route("/busket/delete/{id}")]
        public IActionResult DeleteDetail(string id)
        {
            _orderService.GetOrderDetailById(Guid.Parse(id));
            _orderService.DeleteOrderDetail(new OrderDetail { OrderDetailId = Guid.Parse(id) });

            return RedirectToAction(nameof(ViewOrder));
        }

        [HttpGet]
        [Route("/payment/successful")]
        public IActionResult ViewSuccessfulPaymentPage()
        {
            return View();
        }

        [HttpGet]
        [Route("/payment/{id}/successful")]
        public IActionResult ViewPaidPage()
        {
            return View();
        }
    }
}
