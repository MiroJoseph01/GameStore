using GameStroe.Mobile.Models;
using GameStroe.Mobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GameStroe.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PublisherDetailPage : ContentPage
    {
        public PublisherDetailPage(Publisher publisher)
        {
            InitializeComponent();
            BindingContext = new PublisherDetailViewModel(publisher);
        }
    }
}