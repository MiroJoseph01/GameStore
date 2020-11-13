using System;
using System.Diagnostics;
using System.Threading.Tasks;
using GameStroe.Mobile.Models;
using Xamarin.Forms;

namespace GameStroe.Mobile.ViewModels
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public class ItemDetailViewModel : BaseViewModel
    {
        private string itemId;

        private string text;
        private string description;

        private string discount;
        private bool discontinued;
        private short unitsInStock;
        private int views;
        private string date;
        private decimal price;
        private Publisher publisher;
        private string publisherName;

        //reserved
        private string genres;
        private string platforms;

        #region  basic Properties

        public string Id { get; set; }

        public string Text
        {
            get => text;
            set => SetProperty(ref text, value);
        }

        public string Description
        {
            get => description;
            set => SetProperty(ref description, value);
        }

        public string Discount
        {
            get => discount;
            set => SetProperty(ref discount, value);
        }

        public bool Discontinued
        {
            get => discontinued;
            set => SetProperty(ref discontinued, value);
        }

        public short UnitsInStock
        {
            get => unitsInStock;
            set => SetProperty(ref unitsInStock, value);
        }

        public int Views
        {
            get => views;
            set => SetProperty(ref views, value);
        }

        public decimal Price
        {
            get => price;
            set => SetProperty(ref price, value);
        }

        public string Date
        {
            get => date;
            set => SetProperty(ref date, value);
        }

        public Publisher Publisher
        {
            get => publisher;
            set => SetProperty(ref publisher, value);
        }

        public string PublisherName
        {
            get => publisherName;
            set => SetProperty(ref publisherName, value);
        }

        #endregion

        public string ItemId
        {
            get
            {
                return itemId;
            }
            set
            {
                itemId = value;
                LoadItemId(value);
            }
        }

        public async void LoadItemId(string itemId)
        {
            try
            {
                var item = await DataStore.GetItemAsync(itemId);
                Title = item.Name;
                Id = item.Key;
                Text = item.Name;
                Description = item.Description;
                Discount = item.Discount;
                Discontinued = item.Discontinued;
                UnitsInStock = item.UnitsInStock;
                Date = item.Date;
                Price = item.Price;
                Views = item.Views;
                Publisher = item.Publisher;
                PublisherName = item.Publisher is null? "No Publisher" : item.Publisher.CompanyName;    
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }
    }
}
