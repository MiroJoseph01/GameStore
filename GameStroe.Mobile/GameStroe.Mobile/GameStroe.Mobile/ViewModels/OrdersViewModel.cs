using GameStroe.Mobile.Models;
using GameStroe.Mobile.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GameStroe.Mobile.ViewModels
{
    public class OrdersViewModel : BaseViewModel
    {
        private Order _selectedItem;

        public ObservableCollection<Order> Items { get; }
        public Command LoadItemsCommand { get; }
        public Command<Order> ItemTapped { get; }

        public OrdersViewModel()
        {
            Title = "Basket";
            Items = new ObservableCollection<Order>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            ItemTapped = new Command<Order>(OnItemSelected);
        }

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                Items.Clear();
                var items = await OrderDataStore.GetOrdersByCustomerId(Constants.userId);
                foreach (var item in items)
                {
                    Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void OnAppearing()
        {
            IsBusy = true;
            SelectedItem = null;
        }

        public Order SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
                OnItemSelected(value);
            }
        }

        async void OnItemSelected(Order item)
        {
            if (item == null)
                return;

            // This will push the ItemDetailPage onto the navigation stack
            await Shell.Current.GoToAsync($"{nameof(OrderDetailPage)}?{nameof(OrderViewModel.OrderId)}={item.OrderId}");
        }
    }
}
