using Xamarin.Forms;
using GameStroe.Mobile.ViewModels;
using System;
using GameStroe.Mobile.Services.Interfaces;
using System.Threading;

namespace GameStroe.Mobile.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        private readonly ItemDetailViewModel _itemDetail;

        private IOrderDataStore OrderDataStore => DependencyService.Get<IOrderDataStore>();

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
                button.Text = "No publisher";
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
                button.Text = "No comments";
            }
            else
            {
                await Navigation.PushAsync(new CommentsViewModel(_itemDetail.Comments));
            }
        }

        private void OnBuyClick(object sender, EventArgs args)
        {
            if (_itemDetail.UnitsInStock<1)
            {
                var button = (Button)sender;
                button.BackgroundColor = Color.Gray;
                button.Text = "No items left";
            }
            else
            {
                OrderDataStore.AddDetail(_itemDetail.Id, Constants.userId);
                Navigation.PushAsync(new AboutPage());
            }
        }

        private void OnImageClick(object sender, EventArgs args)
        {
            var button = (Image)sender;
            image.Source = button.Source;
            image.HorizontalOptions = LayoutOptions.CenterAndExpand;
            image.VerticalOptions = LayoutOptions.CenterAndExpand;
            image.IsVisible = true;
            main.RowDefinitions[0].Height = new GridLength(1, GridUnitType.Star);
            child.IsVisible = false;
        }

        private void OnImageSwipe(object sender, EventArgs args)
        {
            child.IsVisible = true;
            main.RowDefinitions[0].Height = new GridLength(900);
            image.IsVisible = false;
        }
    }
}