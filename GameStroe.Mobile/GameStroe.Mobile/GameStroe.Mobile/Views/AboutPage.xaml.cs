using GameStroe.Mobile.ViewModels;
using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GameStroe.Mobile.Views
{
    public partial class AboutPage : ContentPage
    {
        OrdersViewModel _viewModel;

        public AboutPage()
        {
            InitializeComponent();

            BindingContext = _viewModel = new OrdersViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
    }
}