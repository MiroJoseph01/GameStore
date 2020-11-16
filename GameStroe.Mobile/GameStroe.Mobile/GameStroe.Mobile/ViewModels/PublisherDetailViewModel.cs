using GameStroe.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace GameStroe.Mobile.ViewModels
{
    class PublisherDetailViewModel : BaseViewModel
    {
        private string id;
        private string companyName;
        private string contactName;
        private string contactTitle;
        private string address;
        private string city;
        private string region;
        private string postalCode;
        private string country;
        private string phone;
        private string fax;
        private string description;
        private string homePage;

        public PublisherDetailViewModel(Publisher publisher)
        {
            Id = publisher.PublisherId;
            CompanyName = publisher.CompanyName;
            Descrription = publisher.Description;
            ContactName = publisher.ContactName is null ? "No contact name" : publisher.ContactName;
            ContactTitle = publisher.ContactTitle is null ? "No contact title" : publisher.ContactTitle;
            Address = publisher.Address is null ? "No address" : publisher.Address;
            City = publisher.City is null ? "No city" : publisher.City;
            Region = publisher.Region is null ? "No region" : publisher.Region;
            PostalCode = publisher.PostalCode is null ? "No code" : publisher.PostalCode;
            Country = publisher.Country is null ? "No contry" : publisher.Country;
            Phone = publisher.Phone is null ? "No phone" : publisher.Phone;
            Fax = publisher.Fax is null ? "No fax" : publisher.Fax;
            Descrription = publisher.Description is null ? "No desc" : publisher.Description;
            HomePage = publisher.HomePage is null ? "No home page" : publisher.HomePage;
            Title = CompanyName;
        }

        #region Properties
        public string Id { get => id; set => SetProperty(ref id, value); }
        public string CompanyName { get => companyName; set => SetProperty(ref companyName, value); }

        public string ContactName { get => contactName; set => SetProperty(ref contactName, value); }

        public string ContactTitle { get => contactTitle; set => SetProperty(ref contactTitle, value); }

        public string Address { get => address; set => SetProperty(ref address, value); }

        public string City { get => city; set => SetProperty(ref city, value); }

        public string Region { get => region; set => SetProperty(ref region, value); }

        public string PostalCode { get => postalCode; set => SetProperty(ref postalCode, value); }

        public string Country { get => country; set => SetProperty(ref country, value); }

        public string Phone { get => phone; set => SetProperty(ref phone, value); }

        public string Fax { get => fax; set => SetProperty(ref fax, value); }

        public string Descrription { get => description; set => SetProperty(ref description, value); }

        public string HomePage { get => homePage; set => SetProperty(ref homePage, value); }

        #endregion

    }
}
