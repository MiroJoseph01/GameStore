using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using GameStore.BLL.Interfaces;
using GameStore.BLL.Interfaces.Services;
using GameStore.BLL.Models;
using GameStore.BLL.Payments;
using GameStore.BLL.Payments.PaymentStrategies;
using GameStore.Web.Util;
using GameStore.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;

namespace GameStore.Web.Controllers
{
    [CustomController("Basket")]
    public class BasketController : Controller
    {
        private readonly IGameService _gameService;
        private readonly IOrderService _orderService;
        private readonly IShipperService _shipperService;
        private readonly IMapper _mapper;
        private readonly IPaymentContext _paymentContext;
        private readonly IConfiguration _iConfiguration;

        public BasketController(
            IGameService gameService,
            IOrderService orderService,
            IMapper mapper,
            IPaymentContext paymentContext,
            IConfiguration configuration,
            IShipperService shipperService)
        {
            _gameService = gameService;
            _orderService = orderService;
            _mapper = mapper;
            _paymentContext = paymentContext;
            _iConfiguration = configuration;
            _shipperService = shipperService;
        }

        [HttpGet]
        [Route("/busket")]
        public IActionResult ViewOrder()
        {
            var orders = _mapper
                .Map<IEnumerable<BasketViewModel>>(_orderService.GetOrdersByCustomerId(Constants.UserId));

            var result = new GeneralBasketViewModel
            {
                Orders = orders,
            };

            return View(result);
        }

        [HttpPost]
        [Route("/busket")]
        public IActionResult ViewOrder(FilterOrdersViewModel model)
        {
            if (model.MinDate > model.MaxDate)
            {
                ModelState.AddModelError("", "Min Date can not be bigger then Max Date");
            }

            if (model.MaxDate == DateTime.MinValue || model.MinDate == DateTime.MinValue)
            {
                ModelState.AddModelError("", "Fields are required");
            }

            if (!ModelState.IsValid)
            {
                var invalidResult = new GeneralBasketViewModel
                {
                    FilterModel = model,
                    Orders = _mapper
                        .Map<IEnumerable<BasketViewModel>>(_orderService.GetOrdersByCustomerId(Constants.UserId)),
                };

                return View(invalidResult);
            }

            var result = new GeneralBasketViewModel
            {
                FilterModel = model,
                Orders = _mapper.Map<IEnumerable<BasketViewModel>>(
                    _orderService.FilterByDate(model.MinDate, model.MaxDate, model.CustomerId)),
            };

            return View(result);
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

            var success = _orderService.AddOrderDetail(Constants.UserId, game.Key);

            if (!success)
            {
                return RedirectToAction("ViewGameDetails", "Game", new { key });
            }

            return RedirectToAction(nameof(ViewOrder));
        }

        [HttpGet]
        [Route("shippment/{id}")]
        public IActionResult ViewDetails(string id)
        {
            var order = _orderService.GetOrderById(id);

            var orderToView = _mapper.Map<BasketViewModel>(order);

            orderToView.ShipOptions = _shipperService
                .GetAllShippers()
                .ToList()
                .Select(x => new SelectListItem
                {
                    Value = x.ShipperID,
                    Text = x.CompanyName,
                })
                .ToList();
            orderToView.ShipOptions.Insert(0, new SelectListItem
            {
                Text = "No selected shipper",
                Value = "-1",
            });

            return View(orderToView);
        }

        [HttpPost]
        [Route("shippment/{id}")]
        public IActionResult ViewDetails(BasketViewModel model, string id)
        {
            if (model.ShipVia == "-1")
            {
                ModelState.AddModelError("ShipVia", "Select shipper from the list");
            }

            if (!ModelState.IsValid)
            {
                model.ShipOptions = _shipperService
                .GetAllShippers()
                .ToList()
                .Select(x => new SelectListItem
                {
                    Value = x.ShipperID,
                    Text = x.CompanyName,
                })
                .ToList();

                model.ShipOptions.Insert(0, new SelectListItem
                {
                    Text = "No selected shipper",
                    Value = "-1",
                });

                return View(model);
            }

            _orderService.UpdateOrder(UpdateSelectedOrderFields(model));

            return RedirectToAction(nameof(ViewPayment), new { id });
        }

        [HttpGet]
        [Route("/payment/{id}")]
        public IActionResult ViewPayment(string id)
        {
            if (_orderService.OrderIsPaid(id))
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
            if (_orderService.OrderIsPaid(id))
            {
                return RedirectToAction(nameof(ViewPaidPage), new { id });
            }

            _paymentContext.SetStrategy(new BankPaymentStrategy(_orderService));

            PaymentInfo info = _paymentContext.ProcessPayment(id);

            return info.FileStreamResult;
        }

        [HttpGet]
        [Route("/payment/ibox/{id}")]
        public IActionResult PayWithIBox(string id)
        {
            if (_orderService.OrderIsPaid(id))
            {
                return RedirectToAction(nameof(ViewPaidPage), new { id });
            }

            _paymentContext.SetStrategy(new IBoxPaymentStrategy(_orderService));

            _paymentContext.ProcessPayment(id);

            return Redirect(_iConfiguration[Constants.IBoxPage]);
        }

        [HttpGet]
        [Route("/payment/visa/{id}")]
        public IActionResult PayWithCard(string id)
        {
            if (_orderService.OrderIsPaid(id))
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

            _paymentContext.ProcessPayment(id);

            return RedirectToAction(nameof(ViewSuccessfulPaymentPage));
        }

        [HttpGet]
        [Route("/busket/delete/{id}")]
        public IActionResult DeleteDetail(string id)
        {
            _orderService.GetOrderDetailById(id);
            _orderService.DeleteOrderDetail(new OrderDetail { OrderDetailId = id });

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

        private Order UpdateSelectedOrderFields(BasketViewModel model)
        {
            var result = _orderService.GetOrderById(model.OrderId);

            result.OrderDate = DateTime.Now;
            result.ShipVia = model.ShipVia;
            result.ShipName = model.ShipName;
            result.ShipAddress = model.ShipAddress;
            result.ShipCity = model.ShipCity;
            result.ShipRegion = model.ShipRegion;
            result.ShipPostalCode = model.ShipPostalCode;
            result.ShipCountry = model.ShipCountry;

            return result;
        }
    }
}
