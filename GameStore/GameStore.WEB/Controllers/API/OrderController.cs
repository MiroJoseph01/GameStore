using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.Configuration;
using GameStore.BLL.Interfaces;
using GameStore.BLL.Interfaces.Services;
using GameStore.BLL.Models;
using GameStore.Web.ApiModels;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GameStore.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IGameService _gameService;
        private readonly IOrderService _orderService;
        private readonly IShipperService _shipperService;
        private readonly IMapper _mapper;
        private readonly IPaymentContext _paymentContext;
        private readonly IConfiguration _iConfiguration;

        public OrderController(
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

        // GET: api/<OrderController/5
        [HttpGet("{id}/{getById}")]
        public IActionResult Get(string id, bool getById)
        {
            var shippers = _shipperService.GetAllShippers().Select(x => new ShipperViewModel
            {
                ShipperId = x.ShipperID,
                ShipperName = x.CompanyName,
            }).ToList();

            if (getById)
            {
                var order = _mapper.Map<OrderViewModel>(_orderService.GetOrderById(id));
                order.ShipOptions = shippers;

                return Ok(order);
            }
            else
            {
                var orders = _mapper
                    .Map<IEnumerable<OrderViewModel>>(_orderService.GetOrdersByCustomerId(id));

                orders.Select(x => x.ShipOptions = shippers);

                return Ok(orders);
            }
        }

        // GET api/<OrderController>
        [HttpGet]
        public IActionResult Get()
        {
            var shippers = _shipperService.GetAllShippers().Select(x => new ShipperViewModel
            {
                ShipperId = x.ShipperID,
                ShipperName = x.CompanyName,
            }).ToList();

            var orders = _mapper
                    .Map<IEnumerable<OrderViewModel>>(_orderService.GetAllOrders());

            orders.Select(x => x.ShipOptions = shippers);

            return Ok(orders);

            return Ok();
        }

        // POST api/<OrderController>
        [HttpPost("{gameKey}/{userId}")]
        public IActionResult Post(string gameKey, string userId)
        {
            var game = _gameService.GetGameByKey(gameKey);

            if (game is null)
            {
                return NotFound();
            }

            var success = _orderService.AddOrderDetail(userId, game.Key);

            if (!success)
            {
                return BadRequest();
            }

            return Ok();
        }

        // DELETE order detail not order!
        // DELETE api/<OrderController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            _orderService.GetOrderDetailById(id);
            _orderService.DeleteOrderDetail(new OrderDetail { OrderDetailId = id });

            return Ok();
        }
    }
}
