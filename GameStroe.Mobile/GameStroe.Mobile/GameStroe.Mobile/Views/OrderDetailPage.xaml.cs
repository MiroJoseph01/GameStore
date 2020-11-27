using GameStroe.Mobile.Services.Interfaces;
using GameStroe.Mobile.ViewModels;
using System;

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
    }
}