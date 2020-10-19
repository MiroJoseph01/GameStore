using System;
using System.Collections.Generic;
using GameStore.BLL.Models;

namespace GameStore.BLL.Interfaces
{
    public interface IPublisherService
    {
        IEnumerable<Publisher> GetAllPublishers();

        Publisher GetPublisherByName(string publisherName);

        void DeletePublisher(Publisher publisher);

        Publisher UpdatePublisher(Publisher publisher);

        Publisher GetPublisherById(Guid publisherId);

        Publisher CreatePublisher(Publisher publisher);

        bool IsPresent(string name);
    }
}
