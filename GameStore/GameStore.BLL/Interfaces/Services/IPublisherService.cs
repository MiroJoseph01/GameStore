using System.Collections.Generic;
using GameStore.BLL.Models;

namespace GameStore.BLL.Interfaces.Services
{
    public interface IPublisherService
    {
        IEnumerable<Publisher> GetAllPublishers();

        Publisher GetPublisherByName(string publisherName);

        void DeletePublisher(Publisher publisher);

        Publisher UpdatePublisher(Publisher publisher);

        Publisher GetPublisherById(string publisherId);

        Publisher CreatePublisher(Publisher publisher);

        bool IsPresent(string name);
    }
}
