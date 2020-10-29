using System.Collections.Generic;
using GameStore.DAL.Pipeline.Util;

namespace GameStore.DAL.Pipeline
{
    public class FilterModel
    {
        public FilterModel()
        {
            PlatformOptions = new List<string>();
            GenresOptions = new List<string>();
            PublisherOptions = new List<string>();
            DateFilter = TimePeriod.AllTime;
            Take = 10;
        }

        public List<string> PlatformOptions { get; set; }

        public List<string> GenresOptions { get; set; }

        public List<string> PublisherOptions { get; set; }

        public OrderOption Filter { get; set; }

        public decimal From { get; set; }

        public decimal To { get; set; }

        public TimePeriod DateFilter { get; set; }

        public string SearchByGameName { get; set; }

        public int Take { get; set; }

        public int Skip { get; set; }
    }
}
