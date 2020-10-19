using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using GameStore.BLL.Interfaces;
using GameStore.DAL.Pipeline.Util;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GameStore.Web.ViewModels
{
    public class QueryViewModel
    {
        public QueryViewModel(IGameService gameService)
        {
            var orderOptions = gameService.GetOrderOptions();
            var timePeriod = gameService.GetTimePeriods();

            Filters = orderOptions.Values
                .Select(x => new SelectListItem
                {
                    Text = x.Text,
                    Value = x.Name,
                })
                .ToList();

            DateOptions = timePeriod.Values
                .Select(x => new SelectListItem
                {
                    Text = x.Text,
                    Value = x.Name,
                })
                .ToList();

            NumberOfItemsPerPage = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = "10",
                    Text = "10",
                },

                new SelectListItem
                {
                    Value = "20",
                    Text = "20",
                },

                new SelectListItem
                {
                    Value = "50",
                    Text = "50",
                },

                new SelectListItem
                {
                    Value = "100",
                    Text = "100",
                },

                new SelectListItem
                {
                    Value = "All",
                    Text = "All",
                },
            };

            PageSize = "10";
        }

        [Display(Name = "Platforms")]
        public List<SelectListItem> PlatformOptions { get; set; }

        [Display(Name = "Genres")]
        public List<SelectListItem> GenresOptions { get; set; }

        [Display(Name = "Publisher")]
        public List<SelectListItem> PublisherOptions { get; set; }

        [Display(Name = "Order")]
        public List<SelectListItem> Filters { get; set; }

        public OrderOption Filter { get; set; }

        [Display(Name = "Min price")]
        public float From { get; set; }

        [Display(Name = "Max price")]
        public float To { get; set; }

        [Display(Name = "Date Filters")]
        public List<SelectListItem> DateOptions { get; set; }

        public TimePeriod DateFilter { get; set; }

        [Display(Name = "Search by name")]
        public string SearchByGameName { get; set; }

        [Display(Name = "Number of items per page")]
        public List<SelectListItem> NumberOfItemsPerPage { get; set; }

        public string PageSize { get; set; }

        public int Page { get; set; }

        public bool IsFiltered { get; set; }
    }
}
