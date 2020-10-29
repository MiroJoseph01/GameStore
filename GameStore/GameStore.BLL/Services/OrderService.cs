using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using GameStore.BLL.Interfaces.Services;
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
        private readonly IOrderRepositoryFacade _orderRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IRepository<DbModels.OrderStatus> _orderStatusRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrderService(
            IGameService gameService,
            IOrderRepositoryFacade orderRepository,
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
            var orders = _mapper.Map<IEnumerable<BusinessModels.Order>>(_orderRepository.GetByCustomerId(customerId));

            if (!(orders is null))
            {
                // for each OrderDetail in Order fill properties ProductKey and ProductName from GameService
                foreach (var o in orders)
                {
                    decimal total = 0;
                    foreach (var d in o.OrderDetails)
                    {
                        BusinessModels.Game game = _gameService.GetGameById(d.ProductId);

                        if (game != null)
                        {
                            d.ProductKey = game.Key;
                            d.ProductName = game.Name;
                        }

                        total += d.Price - (d.Price * (decimal)d.Discount);
                    }

                    o.Total = total;
                }
            }

            return orders;
        }

        public BusinessModels.Order AddOrder(BusinessModels.Order order)
        {
            order.OrderId = Guid.NewGuid().ToString();

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

        public BusinessModels.Order GetOrderById(string orderId)
        {
            DbModels.Order orderFromDb = _orderRepository.GetById(orderId);

            var order = _mapper.Map<BusinessModels.Order>(orderFromDb);

            if (!(order is null))
            {
                decimal total = 0;
                foreach (var g in order.OrderDetails)
                {
                    BusinessModels.Game game = _gameService.GetGameById(g.ProductId);
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
                        var game = _gameService.GetGameById(g.ProductId);
                        g.ProductKey = game.Key;
                        g.ProductName = game.Name;
                        total += g.Price - (g.Price * (decimal)g.Discount);
                    }

                    order.Total = total;
                }
            }

            return orders;
        }

        public bool AddOrderDetail(string customerId, string gameKey)
        {
            var game = _gameService.GetGameByKey(gameKey);

            var orderDetail = new BusinessModels.OrderDetail
            {
                ProductId = game.GameId,
                ProductKey = game.Key,
                ProductName = game.Name,
                Price = game.Price,
                Quantity = 1,
                Discount = game.Discount,
            };

            var success = UpdateGameQuantity(orderDetail, -1);

            if (!success)
            {
                return false;
            }

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

            return success;
        }

        public void DeleteOrderDetail(BusinessModels.OrderDetail orderDetail)
        {
            if (!_orderDetailRepository.IsPresent(orderDetail.OrderDetailId))
            {
                throw new ArgumentException("Order Detail doesn't exist or has been already deleted");
            }

            var orderDetailFromDb = _orderDetailRepository
                .GetById(orderDetail.OrderDetailId);

            var game = _gameService.GetGameById(orderDetailFromDb.ProductId);

            _gameService.EditGame(game, orderDetailFromDb.Quantity);

            _orderDetailRepository.Delete(orderDetail.OrderDetailId);

            _unitOfWork.Commit();
        }

        public BusinessModels.OrderDetail UpdateOrderDetail(BusinessModels.OrderDetail orderDetail)
        {
            _orderDetailRepository.Update(orderDetail.OrderDetailId, _mapper.Map<DbModels.OrderDetail>(orderDetail));

            _unitOfWork.Commit();

            return orderDetail;
        }

        public BusinessModels.OrderDetail GetOrderDetailById(string orderDetailId)
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
            orderStatus.OrderStatusId = Guid.NewGuid().ToString();

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

        public BusinessModels.OrderStatus GetOrderStatusById(string orderStatusId)
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
            if (string.IsNullOrWhiteSpace(dateFromCard))
            {
                return false;
            }

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

        public string GenrerateShortPaymentId(string id)
        {
            return id.ToString().Substring(0, 9);
        }

        public void UpdateStatusOfOrder(string paymentId, string status)
        {
            var order = GetOrderById(paymentId);

            var orderStatus = GetOrderStatusByName(status);

            order.OrderStatus = orderStatus;
            order.Status = orderStatus.Status;

            UpdateOrder(order);
        }

        public bool OrderIsPaid(string id)
        {
            return _orderRepository.GetById(id).Status == OrderStatuses.Paid;
        }

        public IEnumerable<BusinessModels.Order> FilterByDate(DateTime minDate, DateTime maxDate, string customerId)
        {
            var activeOrders = GetOrdersByCustomerId(customerId)
                .Where(x => x.Status == "Open" || x.Status == "NotPaid")
                .ToList();

            var orders = GetOrdersByCustomerId(customerId)
                .Where(x => x.Status == OrderStatuses.Paid && x.OrderDate > minDate && x.OrderDate < maxDate)
                .ToList();

            activeOrders.AddRange(orders);

            return activeOrders;
        }

        private bool UpdateGameQuantity(BusinessModels.OrderDetail order, short quantity)
        {
            var game = _gameService.GetGameById(order.ProductId);
            game.UnitsInStock = (short)(game.UnitsInStock - order.Quantity);

            var updatedGame = _gameService.EditGame(game, quantity);

            if (updatedGame is null)
            {
                return false;
            }

            return true;
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
                CustomerId = customerId,
                OrderId = Guid.NewGuid().ToString(),
            };
            orderDetail.OrderId = order.OrderId;

            orderDetail.OrderDetailId = Guid.NewGuid().ToString();

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

            orderDetail.OrderDetailId = Guid.NewGuid().ToString();

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
