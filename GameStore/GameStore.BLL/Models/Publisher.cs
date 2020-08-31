using System;

namespace GameStore.BLL.Models
{
    public class Publisher
    {
        public Guid PublisherId { get; set; }

        public string CompanyName { get; set; }

        public string Description { get; set; }

        public string HomePage { get; set; }
    }
}
