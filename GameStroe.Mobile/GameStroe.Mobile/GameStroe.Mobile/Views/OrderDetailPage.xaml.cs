using GameStroe.Mobile.Services.Interfaces;
using GameStroe.Mobile.ViewModels;
using System;
using System.Threading;
using Xamarin.Forms;

namespace GameStroe.Mobile.Views
{
    public partial class OrderDetailPage : ContentPage
    {
        private readonly OrderViewModel _orderDetail;
        private IOrderDataStore OrderDataStore => DependencyService.Get<IOrderDataStore>();

        public OrderDetailPage()
        {
            InitializeComponent();
            _orderDetail = new OrderViewModel();
            BindingContext = _orderDetail;
        }

        public void OnPayClicked(object sender, EventArgs args)
        {
            if (_orderDetail.OrderStatus == "Open")
            {
                OrderDataStore.Pay(_orderDetail.Id);
                Navigation.PopAsync();
            }
            else
            {
                var button = (ToolbarItem)sender;
                button.Text = "Paid";
            }
        }

        public void OnDeleteClick(object sender, EventArgs args)
        {
            if(_orderDetail.OrderStatus == "Open")
            {
                var data = ((Button)sender).BindingContext as string;
                OrderDataStore.DeleteDetail(data);
                Thread.Sleep(1000);
                _orderDetail.LoadItemId(_orderDetail.Id);
            }
            else
            {
                var button = (Button)sender;
                button.BackgroundColor = Color.Gray;
                button.Text = "Impossible";
            }
            
        }
    }
}