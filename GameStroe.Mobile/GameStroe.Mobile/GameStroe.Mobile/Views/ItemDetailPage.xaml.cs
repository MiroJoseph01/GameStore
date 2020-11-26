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

        private async void OnCommentsButtonClick(object sender, EventArgs args)
        {
            if (_itemDetail.Comments is null|| _itemDetail.Comments.Count == 0)
            {
                var button = (Button)sender;
                button.BackgroundColor = Color.Gray;
            }
            else
            {
                await Navigation.PushAsync(new CommentsViewModel(_itemDetail.Comments));
            }
        }
    }
}