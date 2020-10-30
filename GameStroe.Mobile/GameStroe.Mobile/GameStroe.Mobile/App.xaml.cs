using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using GameStroe.Mobile.Services;
using GameStroe.Mobile.Views;

namespace GameStroe.Mobile
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
