using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameStore.DAL.Pipeline.Util;
using GameStore.Web.ViewModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GameStore.Web.Util.ModelBinders
{
    public class QueryModelBinder : IModelBinder
    {
        private const string Platforms = "platforms";
        private const string Genres = "genres";
        private const string Publishers = "publishers";
        private const string OrderFilter = "order_filter";
        private const string From = "from";
        private const string To = "to";
        private const string DateFilter = "date_filter";
        private const string SeacrchByName = "game";
        private const string PageSize = "page_size";
        private const string PageNumber = "page";
        private const string IsFiltered = "is_filtered";
        private const string DefaultOrderFilter = "New";
        private const string DefaultDateFilter = "AllTime";

        private ModelBindingContext _modelBindingContext;

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            _modelBindingContext = bindingContext;

            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            bindingContext.Result = ModelBindingResult.Success(new QueryModel
            {
                PlatformOptions = GetListOfItems(Platforms),
                GenresOptions = GetListOfItems(Genres),
                PublisherOptions = GetListOfItems(Publishers),
                Filter = GetOrder(OrderFilter, DefaultOrderFilter),
                DateFilter = GetDate(DateFilter, DefaultDateFilter),
                To = GetValueOfItem(To, 0f),
                From = GetValueOfItem(From, 0f),
                SearchByGameName = GetValueOfItem(SeacrchByName),
                PageSize = GetPageSize(PageSize, 10),
                Page = GetValueOfItem(PageNumber, 1),
                IsFiltered = GetValueOfItem(IsFiltered, false),
            });

            return Task.CompletedTask;
        }

        private List<string> GetListOfItems(string name)
        {
            var stringName = _modelBindingContext
                .ValueProvider
                .GetValue(name)
                .FirstValue;

            List<string> list = null;

            if (!string.IsNullOrEmpty(stringName))
            {
                list = stringName.Split('-').ToList();
            }

            return list;
        }

        private dynamic GetValueOfItem(string name, dynamic defaultValue = null)
        {
            string stringValue = _modelBindingContext
                .ValueProvider
                .GetValue(name)
                .FirstValue;

            var value = defaultValue;

            if (defaultValue is float && !string.IsNullOrEmpty(stringValue))
            {
                value = Convert.ToSingle(stringValue);
            }

            if ((defaultValue is string || defaultValue is null) && !string.IsNullOrEmpty(stringValue))
            {
                value = stringValue;
            }

            if (defaultValue is int && !string.IsNullOrEmpty(stringValue))
            {
                value = Convert.ToInt32(stringValue);
            }

            if (defaultValue is bool && stringValue == "true")
            {
                value = true;
            }

            return value;
        }

        private int GetPageSize(string name, int defaultValue = 10)
        {
            var stringName = _modelBindingContext
               .ValueProvider
               .GetValue(name)
               .FirstValue;

            var result = defaultValue;

            if (!string.IsNullOrWhiteSpace(stringName))
            {
                if (stringName.Equals("All"))
                {
                    result = -1;
                }
                else
                {
                    result = Convert.ToInt32(stringName);
                }
            }

            return result;
        }

        private OrderOption GetOrder(string name, string defaultValue)
        {
            var stringName = _modelBindingContext
                .ValueProvider
                .GetValue(name)
                .FirstValue;

            if (string.IsNullOrWhiteSpace(stringName))
            {
                return EnumHelper.GetValueFromDescription<OrderOption>(defaultValue);
            }

            return EnumHelper.GetValueFromDescription<OrderOption>(stringName);
        }

        private TimePeriod GetDate(string name, string defaultValue)
        {
            var stringName = _modelBindingContext
                .ValueProvider
                .GetValue(name)
                .FirstValue;

            if (string.IsNullOrWhiteSpace(stringName))
            {
                return EnumHelper.GetValueFromDescription<TimePeriod>(defaultValue);
            }

            return EnumHelper.GetValueFromDescription<TimePeriod>(stringName);
        }
    }
}
