using System;
using System.Collections.Generic;
using GameStroe.Mobile.ViewModels;
using GameStroe.Mobile.Views;
using Xamarin.Forms;

namespace GameStroe.Mobile
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
        }

    }
}
