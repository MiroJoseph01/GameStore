using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using GameStroe.Mobile.Models;
using GameStroe.Mobile.ViewModels.SupportModels;
using Xamarin.Forms;

namespace GameStroe.Mobile.ViewModels
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public class ItemDetailViewModel : BaseViewModel
    {
        private string itemId;

        private string text;
        private string description;

        private short unitsInStock;
        private int views;
        private string date;
        private decimal price;
        private Publisher publisher;
        private string publisherName;

        private string genres;
        private string platforms;

        private List<Comment> comments;
        private List<ImageSupportModel> images;

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

        public string Genres
        {
            get => genres;
            set => SetProperty(ref genres, value);
        }

        public string Platforms
        {
            get => platforms;
            set => SetProperty(ref platforms, value);
        }

        public List<ImageSupportModel> Images
        {
            get => images;
            set => SetProperty(ref images, value);
        }

        public List<Comment> Comments
        {
            get => comments;
            set => SetProperty(ref comments, value);
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
                UnitsInStock = item.UnitsInStock;
                Date = item.Date;
                Price = item.Price;
                Views = item.Views;
                Publisher = item.Publisher;
                PublisherName = item.Publisher is null? "No Publisher" : item.Publisher.CompanyName;
                Genres = (item.Genres is null || item.Genres.Count == 0) ?
                    "No genres" : String.Join("| ", item.Genres.Select(x=>x.GenreName).ToList());
                Platforms = (item.Platforms is null || item.Platforms.Count == 0) ?
                    "No platforms" : String.Join(" | ", item.Platforms.Select(x => x.PlatformName).ToList());
                Images = item.Images is null || item.Images.Count == 0 ?
                    new List<ImageSupportModel> { new ImageSupportModel { Url = "game.png" } } 
                    : item.Images.Select(x => new ImageSupportModel { Url = x }).ToList();
                Comments = item.Comments.ToList();
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }
    }
}
