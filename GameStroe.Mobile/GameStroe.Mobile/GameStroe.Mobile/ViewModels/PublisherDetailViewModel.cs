using GameStroe.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace GameStroe.Mobile.ViewModels
{
    class PublisherDetailViewModel : BaseViewModel
    {
        private string companyName;
        private string id;

        public PublisherDetailViewModel(Publisher publisher)
        {
            Id = publisher.PublisherId;
            CompanyName = publisher.CompanyName;
        }

        public string Id { get => id; set => SetProperty(ref id, value); }
        public string CompanyName { get => companyName; set => SetProperty(ref companyName, value); }

    }
}
