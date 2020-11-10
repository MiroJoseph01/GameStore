using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Configuration;
using GameStore.BLL.Interfaces;
using GameStore.BLL.Interfaces.Services;
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

        // GET: api/<OrderController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<OrderController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<OrderController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<OrderController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<OrderController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
