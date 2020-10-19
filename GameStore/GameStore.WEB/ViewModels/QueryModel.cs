using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GameStore.DAL.Pipeline.Util;

namespace GameStore.Web.ViewModels
{
    public class QueryModel
    {
        public QueryModel()
        {
            PlatformOptions = new List<string>();
            GenresOptions = new List<string>();
            PublisherOptions = new List<string>();
            PageSize = 10;
        }

        public List<string> PlatformOptions { get; set; }

        public List<string> GenresOptions { get; set; }

        public List<string> PublisherOptions { get; set; }

        public OrderOption Filter { get; set; }

        [Range(0, 1000, ErrorMessage = "Invalid Min price. It should be in range from 0 to 1000")]
        public float From { get; set; }

        [Range(0, 1000, ErrorMessage = "Invalid Max price. It should be in range from 0 to 1000")]
        public float To { get; set; }

        public TimePeriod DateFilter { get; set; }

        public string SearchByGameName { get; set; }

        public int PageSize { get; set; }

        public int Page { get; set; }

        public bool IsFiltered { get; set; }
    }
}
