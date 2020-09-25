using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using GameStore.BLL.Interfaces;
using GameStore.BLL.Payments;
using GameStore.DAL.Interfaces;
using GameStore.DAL.Interfaces.Repositories;
using BusinessModels = GameStore.BLL.Models;
using DbModels = GameStore.DAL.Entities;

namespace GameStore.BLL.Services
{
    public class OrderService : IOrderService
    {
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

            var orders = _mapper.Map<IEnumerable<BusinessModels.Order>>(_orderRepository.GetByCustomerId(id));

            if (!(orders is null))
            {
                // for each OrderDetail in Order fill properties ProductKey and ProductName from GameService
                foreach (var o in orders)
                {
                    decimal total = 0;
                    foreach (var d in o.OrderDetails)
                    {
                        BusinessModels.Game game = _gameService.GetGameById(Guid.Parse(d.ProductId));

                        d.ProductKey = game.Key;
                        d.ProductName = game.Name;
                        total += d.Price - (d.Price * (decimal)d.Discount);
                    }

                    o.Total = total;
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
                throw new ArgumentException("Order doesn't exist or has already been deleted");
            }

            _orderRepository.Delete(order.OrderId);

            DbModels.Order orderFromDb = _orderRepository.GetById(order.OrderId);

            foreach (DbModels.OrderDetail d in orderFromDb.OrderDetails)
            {
                _orderDetailRepository.Delete(d.OrderDetailId);
            }

            _unitOfWork.Commit();
        }

        public BusinessModels.Order UpdateOrder(BusinessModels.Order order)
        {
            _orderRepository.Update(order.OrderId, _mapper.Map<DbModels.Order>(order));

            _unitOfWork.Commit();

            return order;
        }

        public BusinessModels.Order GetOrderById(Guid orderId)
        {
            DbModels.Order orderFromDb = _orderRepository.GetById(orderId);

            var order = _mapper.Map<BusinessModels.Order>(orderFromDb);

            if (!(order is null))
            {
                decimal total = 0;
                foreach (var g in order.OrderDetails)
                {
                    BusinessModels.Game game = _gameService.GetGameById(Guid.Parse(g.ProductId));
                    g.ProductKey = game.Key;
                    g.ProductName = game.Name;
                    total += g.Price - (g.Price * (decimal)g.Discount);
                }

                order.Total = total;
            }

            return order;
        }

        public IEnumerable<BusinessModels.Order> GetAllOrders()
        {
            var orders = _mapper.Map<IEnumerable<BusinessModels.Order>>(_orderRepository.GetAll());
            if (!(orders is null))
            {
                foreach (var order in orders)
                {
                    decimal total = 0;
                    foreach (var g in order.OrderDetails)
                    {
                        var game = _gameService.GetGameById(Guid.Parse(g.ProductId));
                        g.ProductKey = game.Key;
                        g.ProductName = game.Name;
                        total += g.Price - (g.Price * (decimal)g.Discount);
                    }

                    order.Total = total;
                }
            }

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
                Discount = game.Discount,
            };

            BusinessModels.Order order = GetOrdersByCustomerId(customerId)
                .FirstOrDefault(x => x.Status == OrderStatuses.Open);

            if (order is null)
            {
                SaveOrderWithDetail(customerId, orderDetail);
            }
            else
            {
                AddDetailToOrder(order, orderDetail);
            }

            UpdateGameQuantity(orderDetail);
        }

        public void DeleteOrderDetail(BusinessModels.OrderDetail orderDetail)
        {
            if (!_orderDetailRepository.IsPresent(orderDetail.OrderDetailId))
            {
                throw new ArgumentException("Order Detail doesn't exist or has been already deleted");
            }

            var orderDetailFromDb = _orderDetailRepository
                .GetById(orderDetail.OrderDetailId);

            var game = _gameService.GetGameById(Guid.Parse(orderDetailFromDb.ProductId));
            game.UnitsInStock += orderDetailFromDb.Quantity;

            _gameService.EditGame(game);

            _orderDetailRepository.Delete(orderDetail.OrderDetailId);

            _unitOfWork.Commit();
        }

        public BusinessModels.OrderDetail UpdateOrderDetail(BusinessModels.OrderDetail orderDetail)
        {
            _orderDetailRepository.Update(orderDetail.OrderDetailId, _mapper.Map<DbModels.OrderDetail>(orderDetail));

            _unitOfWork.Commit();

            return orderDetail;
        }

        public BusinessModels.OrderDetail GetOrderDetailById(Guid orderDetailId)
        {
            DbModels.OrderDetail orderDetailFromDb = _orderDetailRepository.GetById(orderDetailId);

            var orderDetail = _mapper.Map<BusinessModels.OrderDetail>(orderDetailFromDb);

            return orderDetail;
        }

        public IEnumerable<BusinessModels.OrderDetail> GetAllOrderDetails()
        {
            var orderDetails = _mapper.Map<IEnumerable<BusinessModels.OrderDetail>>(_orderDetailRepository.GetAll());

            return orderDetails;
        }

        public BusinessModels.OrderStatus AddOrderStatus(
            BusinessModels.OrderStatus orderStatus)
        {
            _orderStatusRepository.Create(_mapper.Map<DbModels.OrderStatus>(orderStatus));

            _unitOfWork.Commit();

            return orderStatus;
        }

        public void DeleteOrderStatus(BusinessModels.OrderStatus orderStatus)
        {
            if (!_orderStatusRepository.IsPresent(orderStatus.OrderStatusId))
            {
                throw new ArgumentException("Order Status doesn't exist or has been already deleted");
            }

            _orderStatusRepository.Delete(orderStatus.OrderStatusId);

            _unitOfWork.Commit();
        }

        public BusinessModels.OrderStatus UpdateOrderStatus(BusinessModels.OrderStatus orderStatus)
        {
            _orderStatusRepository.Update(orderStatus.OrderStatusId, _mapper.Map<DbModels.OrderStatus>(orderStatus));

            _unitOfWork.Commit();

            return orderStatus;
        }

        public BusinessModels.OrderStatus GetOrderStatusById(Guid orderStatusId)
        {
            DbModels.OrderStatus orderStatusFromDb = _orderStatusRepository.GetById(orderStatusId);

            var orderStatus = _mapper.Map<BusinessModels.OrderStatus>(orderStatusFromDb);

            return orderStatus;
        }

        public BusinessModels.OrderStatus GetOrderStatusByName(string orderStatus)
        {
            DbModels.OrderStatus orderStatusFromDB = _orderStatusRepository
                .GetAll()
                .FirstOrDefault(x => x.Status.Equals(orderStatus));

            if (orderStatusFromDB is null)
            {
                return null;
            }

            return _mapper.Map<BusinessModels.OrderStatus>(orderStatusFromDB);
        }

        public IEnumerable<BusinessModels.OrderStatus> GetAllOrderStatuses()
        {
            var statuses = _mapper.Map<IEnumerable<BusinessModels.OrderStatus>>(_orderStatusRepository.GetAll());

            return statuses;
        }

        public bool DateIsCorrect(string dateFromCard)
        {
            var date = dateFromCard.Split('/');
            var month = short.Parse(date[0]);
            var year = short.Parse(date[1]);

            var nowYear = short.Parse(
                DateTime
                .Now.Year
                .ToString()
                .Substring(DateTime.Now.Year.ToString().Length - 2));

            if ((month > 12 || month < 1)
                || (year == nowYear && month <= DateTime.Now.Month)
                || (year < nowYear))
            {
                return false;
            }

            return true;
        }

        public string GenrerateShortPaymentId(Guid id)
        {
            return id.ToString().Substring(0, 9);
        }

        public void UpdateStatusOfOrder(Guid paymentId, string status)
        {
            var order = GetOrderById(paymentId);

            var orderStatus = GetOrderStatusByName(status);

            order.OrderStatus = orderStatus;
            order.Status = orderStatus.Status;

            UpdateOrder(order);
        }

        public bool OrderIsPaid(Guid id)
        {
            return _orderRepository.GetById(id).Status == OrderStatuses.Paid;
        }

        private void UpdateGameQuantity(BusinessModels.OrderDetail order)
        {
            var game = _gameService.GetGameById(Guid.Parse(order.ProductId));
            game.UnitsInStock = (short)(game.UnitsInStock - order.Quantity);
            _gameService.EditGame(game);
        }

        private bool SaveOrderWithDetail(
            string customerId,
            BusinessModels.OrderDetail orderDetail)
        {
            BusinessModels.Order order = new BusinessModels.Order
            {
                Status = OrderStatuses.Open,
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

            return true;
        }

        private void AddDetailToOrder(BusinessModels.Order order, BusinessModels.OrderDetail orderDetail)
        {
            var detail = _orderDetailRepository.GetOrderDetailByOrderIdAndProductId(order.OrderId, orderDetail.ProductId);

            if (detail is null)
            {
                CreateNewOrderDetail(order, orderDetail);
            }
            else
            {
                UpdateOrderDetail(order, detail);
            }
        }

        private void CreateNewOrderDetail(BusinessModels.Order order, BusinessModels.OrderDetail orderDetail)
        {
            orderDetail.OrderId = order.OrderId;

            _orderDetailRepository.Create(_mapper.Map<DbModels.OrderDetail>(orderDetail));

            _unitOfWork.Commit();
        }

        private void UpdateOrderDetail(BusinessModels.Order order, DbModels.OrderDetail detail)
        {
            detail.OrderId = order.OrderId;
            detail.Price += detail.Price / detail.Quantity;
            detail.Quantity += 1;

            _orderDetailRepository.Update(detail.OrderDetailId, detail);

            _unitOfWork.Commit();
        }
    }
}
