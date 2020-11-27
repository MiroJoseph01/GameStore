using System;
using System.Collections.Generic;
using GameStore.DAL.Entities;
using GameStore.DAL.Interfaces.Repositories;

namespace GameStore.DAL.Repositories.Facade
{
    public class OrderRepositoryFacade : IOrderRepositoryFacade
    {
        private readonly IOrderRepository _firstSourceOrderRepository;
        private readonly IMongoOrderRepository _secondSourceOrderRepository;

        public OrderRepositoryFacade(
            IOrderRepository sqlOrderRepository,
            IMongoOrderRepository mongoOrderRepository)
        {
            _firstSourceOrderRepository = sqlOrderRepository;
            _secondSourceOrderRepository = mongoOrderRepository;
        }

        public void Create(Order entity)
        {
            _firstSourceOrderRepository.Create(entity);
        }

        public void Delete(string id)
        {
            if (_firstSourceOrderRepository.IsPresent(id))
            {
                var entity = _firstSourceOrderRepository.GetById(id);

                _firstSourceOrderRepository.Delete(id);
            }
            else
            {
                throw new InvalidOperationException($"Can not delete order with id: {id}");
            }
        }

        public IEnumerable<Order> GetAll()
        {
            //changes
            var sqlOrders = _firstSourceOrderRepository.GetAll();
            var mongoOrders = _secondSourceOrderRepository.GetAll();

            var result = new List<Order>();

            result.AddRange(sqlOrders);
            result.AddRange(mongoOrders);

            return sqlOrders;
        }

        public IEnumerable<Order> GetByCustomerId(string customerId)
        {
            //changes
            var sqlOrders = _firstSourceOrderRepository.GetByCustomerId(customerId);
            var mongoOrders = _secondSourceOrderRepository.GetAll();

            var result = new List<Order>();

            result.AddRange(sqlOrders);
            result.AddRange(mongoOrders);

            return sqlOrders;
        }

        public Order GetById(string id)
        {
            var order = _firstSourceOrderRepository.GetById(id);

            if (order != null && order.IsRemoved)
            {
                return null;
            }

            if (order is null)
            {
                order = _secondSourceOrderRepository.GetById(id);
            }

            return order;
        }

        public bool IsPresent(string id)
        {
            return _firstSourceOrderRepository.IsPresent(id) || _secondSourceOrderRepository.IsPresent(id);
        }

        public void Update(string id, Order entity)
        {
            if (_firstSourceOrderRepository.IsPresent(id))
            {
                var oldOrder = _firstSourceOrderRepository.GetById(id);

                _firstSourceOrderRepository.Update(id, entity);
            }
            else
            {
                throw new InvalidOperationException($"Can not update order with id: {id}");
            }
        }
    }
}
