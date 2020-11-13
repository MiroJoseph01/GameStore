using System.ComponentModel;
using Xamarin.Forms;
using GameStroe.Mobile.ViewModels;
using System;

namespace GameStroe.Mobile.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        private readonly ItemDetailViewModel _itemDetail;
        public ItemDetailPage()
        {
            InitializeComponent();
            _itemDetail = new ItemDetailViewModel();

            BindingContext = _itemDetail;
        }

        private async void OnPublisherButtonClick(object sender, EventArgs args)
        {
            if (_itemDetail.Publisher is null)
            {
                var button = (Button)sender;
                button.BackgroundColor = Color.Gray;
            }
            else
            {
                await Navigation.PushAsync(new PublisherDetailPage(_itemDetail.Publisher));
            }
        }
    }
}