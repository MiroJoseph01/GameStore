using GameStroe.Mobile.Models;
using GameStroe.Mobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GameStroe.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PublisherDetailPage : ContentPage
    {
        private readonly Publisher _publisher;

        public PublisherDetailPage(Publisher publisher)
        {
            InitializeComponent();
            BindingContext = new PublisherDetailViewModel(publisher);
            _publisher = publisher;
        }

        private async void OnImageButtonPublisherClicked(object sender, EventArgs args)
        {
            try
            {
                await Browser.OpenAsync(_publisher.HomePage, BrowserLaunchMode.SystemPreferred);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}