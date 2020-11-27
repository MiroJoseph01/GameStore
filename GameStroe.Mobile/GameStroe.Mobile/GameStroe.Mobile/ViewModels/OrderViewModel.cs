using GameStroe.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace GameStroe.Mobile.ViewModels
{
    [QueryProperty(nameof(OrderId), nameof(OrderId))]
    public class OrderViewModel : BaseViewModel
    {
        private string orderId;

        private string customerId;
        private string orderStatus;

        private float total;
        private string orderDate;
        private string shipName;
        private string shipAddress;
        private List<Shipper> shippers;
        private ObservableCollection<OrderDetail> details;

        public OrderViewModel()
        {
            OrderDetails = new ObservableCollection<OrderDetail>();
        }

        public string OrderId
        {
            get
            {
                return orderId;
            }
            set
            {
                orderId = value;
                LoadItemId(value);
            }
        }

        #region  basic Properties

        public string Id { get; set; }

        public string CustomerId
        {
            get => customerId;
            set => SetProperty(ref customerId, value);
        }

        public string OrderStatus
        {
            get => orderStatus;
            set => SetProperty(ref orderStatus, value);
        }

        public float Total
        {
            get => total;
            set => SetProperty(ref total, value);
        }

        public string OrderDate
        {
            get => orderDate;
            set => SetProperty(ref orderDate, value);
        }

        public string ShipName
        {
            get => shipName;
            set => SetProperty(ref shipName, value);
        }

        public string ShipAddress
        {
            get => shipAddress;
            set => SetProperty(ref shipAddress, value);
        }

        public List<Shipper> Shippers
        {
            get => shippers;
            set => SetProperty(ref shippers, value);
        }

        public ObservableCollection<OrderDetail> OrderDetails
        {
            get => details;
            set => SetProperty(ref details, value);
        }

        #endregion

        public async void LoadItemId(string orderId)
        {
            try
            {
                var item = await OrderDataStore.GetOrderById(orderId);
                Title = "Order";
                Id = item.OrderId;
                CustomerId = item.CustomerId;
                OrderStatus = item.OrderStatus;
                Total = (float)item.Total;
                OrderDate = item.OrderDate;
                ShipName = item.ShipName is null ? "No selected shipper" : item.ShipName; ;
                ShipAddress = item.ShipAddress is null ? "No address yet" : item.ShipAddress;
                Shippers = item.ShipOptions;

                IsBusy = true;

                try
                {
                    OrderDetails.Clear();
                    foreach (var i in item.OrderDetails)
                    {
                        OrderDetails.Add(i);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                finally
                {
                    IsBusy = false;
                }
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }
    }
}
