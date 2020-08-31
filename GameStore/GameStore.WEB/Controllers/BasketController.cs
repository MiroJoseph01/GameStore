using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using GameStore.BLL.Interfaces;
using GameStore.BLL.Models;
using GameStore.Web.Util;
using GameStore.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Web.Controllers
{
    public class BasketController : Controller
    {
        private readonly IGameService _gameService;
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;

        public BasketController(
            IGameService gameService,
            IOrderService orderService,
            IMapper mapper)
        {
            _gameService = gameService;
            _orderService = orderService;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("/busket")]
        public IActionResult ViewOrder()
        {
            var orders = _mapper
                .Map<IEnumerable<BasketViewModel>>(
                _orderService.GetOrdersByCustomerId(Constants.UserId));

            var orderToView = orders
                .FirstOrDefault(x => x.OrderStatus == Constants.OpenStatus);

            return View(orderToView);
        }

        [HttpGet]
        [Route("game/{key}/buy")]
        public IActionResult AddDetail(string key)
        {
            if (!_gameService.IsPresent(key))
            {
                return NotFound();
            }

            Game game = _gameService.GetGameByKey(key);
            _orderService.AddOrderDetail(Constants.UserId, key);

            return RedirectToAction(nameof(ViewOrder));
        }
    }
}
