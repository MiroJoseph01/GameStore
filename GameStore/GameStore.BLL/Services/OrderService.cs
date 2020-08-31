using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using GameStore.BLL.Interfaces;
using GameStore.DAL.Interfaces;
using GameStore.DAL.Interfaces.Repositories;
using BusinessModels = GameStore.BLL.Models;
using DbModels = GameStore.DAL.Entities;

namespace GameStore.BLL.Services
{
    public class OrderService : IOrderService
    {
        private const string OpenStatus = "Open";

        private readonly IGameService _gameService;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IRepository<DbModels.OrderStatus> _orderStatusRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrderService(
            IGameService gameService,
            IOrderRepository orderRepository,
            IOrderDetailRepository orderDetailRepository,
            IRepository<DbModels.OrderStatus> orderStatusRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _gameService = gameService;
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
            _orderStatusRepository = orderStatusRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public IEnumerable<BusinessModels.Order> GetOrdersByCustomerId(
            string customerId)
        {
            Guid id = Guid.Parse(customerId);

            var orders = _mapper.Map<IEnumerable<BusinessModels.Order>>(
                    _orderRepository.GetByCustomerId(id));

            if (!(orders is null))
            {
                // for each OrderDetail in Order fill properties ProductKey and ProductName from GameService
                foreach (var o in orders)
                {
                    foreach (var d in o.OrderDetails)
                    {
                        BusinessModels.Game game = _gameService
                            .GetGameById(Guid.Parse(d.ProductId));

                        d.ProductKey = game.Key;
                        d.ProductName = game.Name;
                    }
                }
            }

            return orders;
        }

        public BusinessModels.Order AddOrder(BusinessModels.Order order)
        {
            _orderRepository.Create(_mapper.Map<DbModels.Order>(order));

            _unitOfWork.Commit();

            return order;
        }

        public void DeleteOrder(BusinessModels.Order order)
        {
            if (!_orderRepository.IsPresent(order.OrderId))
            {
                throw new ArgumentException(
                    "Order doesn't exist or has been already deleted");
            }

            _orderRepository.Delete(order.OrderId);

            DbModels.Order orderFromDb = _orderRepository
                .GetById(order.OrderId);

            foreach (DbModels.OrderDetail d in orderFromDb.OrderDetails)
            {
                _orderDetailRepository.Delete(d.OrderDetailId);
            }

            _unitOfWork.Commit();
        }

        public BusinessModels.Order UpdateOrder(BusinessModels.Order order)
        {
            _orderRepository
                .Update(order.OrderId, _mapper.Map<DbModels.Order>(order));

            _unitOfWork.Commit();

            return order;
        }

        public BusinessModels.Order GetOrderById(Guid orderId)
        {
            DbModels.Order orderFromDb = _orderRepository.GetById(orderId);

            var order = _mapper.Map<BusinessModels.Order>(orderFromDb);

            return order;
        }

        public IEnumerable<BusinessModels.Order> GetAllOrders()
        {
            var orders = _mapper
                .Map<IEnumerable<BusinessModels.Order>>(
                _orderRepository.GetAll());

            return orders;
        }

        public void AddOrderDetail(string customerId, string gameKey)
        {
            var game = _gameService.GetGameByKey(gameKey);

            var orderDetail = new BusinessModels.OrderDetail
            {
                ProductId = game.GameId.ToString(),
                ProductKey = game.Key,
                ProductName = game.Name,
                Price = game.Price,
                Quantity = 1,
            };

            BusinessModels.Order order = GetOrdersByCustomerId(customerId)
                .FirstOrDefault(x => x.Status == OpenStatus);

            if (order is null)
            {
                SaveOrderWithDetail(customerId, orderDetail);
            }
            else
            {
                AddDetailToOrder(order, orderDetail);
            }
        }

        public void DeleteOrderDetail(BusinessModels.OrderDetail orderDetail)
        {
            if (!_orderDetailRepository.IsPresent(orderDetail.OrderDetailId))
            {
                throw new ArgumentException(
                    "Order Detail doesn't exist or has been already deleted");
            }

            _orderDetailRepository.Delete(orderDetail.OrderDetailId);

            _unitOfWork.Commit();
        }

        public BusinessModels.OrderDetail UpdateOrderDetail(
            BusinessModels.OrderDetail orderDetail)
        {
            _orderDetailRepository
                .Update(orderDetail.OrderDetailId, _mapper
                .Map<DbModels.OrderDetail>(orderDetail));

            _unitOfWork.Commit();

            return orderDetail;
        }

        public BusinessModels.OrderDetail GetOrderDetailById(Guid orderDetailId)
        {
            DbModels.OrderDetail orderDetailFromDb = _orderDetailRepository
                .GetById(orderDetailId);

            var orderDetail = _mapper
                .Map<BusinessModels.OrderDetail>(orderDetailFromDb);

            return orderDetail;
        }

        public IEnumerable<BusinessModels.OrderDetail> GetAllOrderDetails()
        {
            var orderDetails = _mapper
                .Map<IEnumerable<BusinessModels.OrderDetail>>(
                    _orderDetailRepository.GetAll());

            return orderDetails;
        }

        public BusinessModels.OrderStatus AddOrderStatus(
            BusinessModels.OrderStatus orderStatus)
        {
            _orderStatusRepository
                .Create(_mapper.Map<DbModels.OrderStatus>(orderStatus));

            _unitOfWork.Commit();

            return orderStatus;
        }

        public void DeleteOrderStatus(BusinessModels.OrderStatus orderStatus)
        {
            if (!_orderStatusRepository.IsPresent(orderStatus.OrderStatusId))
            {
                throw new ArgumentException(
                    "Order Status doesn't exist or has been already deleted");
            }

            _orderStatusRepository.Delete(orderStatus.OrderStatusId);

            _unitOfWork.Commit();
        }

        public BusinessModels.OrderStatus UpdateOrderStatus(
            BusinessModels.OrderStatus orderStatus)
        {
            _orderStatusRepository
                .Update(
                orderStatus.OrderStatusId,
                _mapper.Map<DbModels.OrderStatus>(orderStatus));

            _unitOfWork.Commit();

            return orderStatus;
        }

        public BusinessModels.OrderStatus GetOrderStatusById(Guid orderStatusId)
        {
            DbModels.OrderStatus orderStatusFromDb = _orderStatusRepository
                .GetById(orderStatusId);

            var orderStatus = _mapper
                .Map<BusinessModels.OrderStatus>(orderStatusFromDb);

            return orderStatus;
        }

        public IEnumerable<BusinessModels.OrderStatus> GetAllOrderStatuses()
        {
            var orderStatuses = _mapper
                .Map<IEnumerable<BusinessModels.OrderStatus>>(
                    _orderStatusRepository.GetAll());

            return orderStatuses;
        }

        private void SaveOrderWithDetail(
            string customerId,
            BusinessModels.OrderDetail orderDetail)
        {
            BusinessModels.Order order = new BusinessModels.Order
            {
                Status = OpenStatus,
                OrderDetails = new List<BusinessModels.OrderDetail>
                {
                    orderDetail,
                },
                CustomerId = Guid.Parse(customerId),
                OrderId = Guid.NewGuid(),
            };
            orderDetail.OrderId = order.OrderId;

            _orderRepository.Create(_mapper.Map<DbModels.Order>(order));

            _unitOfWork.Commit();
        }

        private void AddDetailToOrder(
            BusinessModels.Order order,
            BusinessModels.OrderDetail orderDetail)
        {
            var detail = _orderDetailRepository
                .GetOrderDetailByOrderIdAndProductId(
                    order.OrderId,
                    orderDetail.ProductId);

            if (detail is null)
            {
                CreateNewOrderDetail(order, orderDetail);
            }
            else
            {
                UpdateOrderDetail(order, detail);
            }
        }

        private void CreateNewOrderDetail(
            BusinessModels.Order order,
            BusinessModels.OrderDetail orderDetail)
        {
            orderDetail.OrderId = order.OrderId;

            _orderDetailRepository
                .Create(
                    _mapper.Map<DbModels.OrderDetail>(orderDetail));

            _unitOfWork.Commit();
        }

        private void UpdateOrderDetail(
            BusinessModels.Order order,
            DbModels.OrderDetail detail)
        {
            detail.OrderId = order.OrderId;
            detail.Price = detail.Price + (detail.Price / detail.Quantity);
            detail.Quantity += 1;

            _orderDetailRepository.Update(
                detail.OrderDetailId,
                detail);

            _unitOfWork.Commit();
        }
    }
}
