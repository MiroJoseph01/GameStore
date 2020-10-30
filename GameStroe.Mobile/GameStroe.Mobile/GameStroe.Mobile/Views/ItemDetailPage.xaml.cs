using System.ComponentModel;
using Xamarin.Forms;
using GameStroe.Mobile.ViewModels;

namespace GameStroe.Mobile.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}