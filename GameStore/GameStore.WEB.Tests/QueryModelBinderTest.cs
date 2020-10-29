using GameStore.Web.Util.ModelBinders;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Xunit;

namespace GameStore.Web.Tests
{
    public class QueryModelBinderTest
    {
        private readonly QueryModelBinder _modelBinder;
        private readonly RouteValueDictionary _collection;

        public QueryModelBinderTest()
        {
            _modelBinder = new QueryModelBinder();

            _collection = new RouteValueDictionary
            {
                { "platforms", "browser,play station" },
                { "genres", "Action" },
                { "publishers", "Valve" },
                { "order_filter", "New" },
                { "from", "0" },
                { "to", "0" },
                { "date_filter", "LastYear" },
                { "game", "Dota2" },
                { "page_size", "10" },
                { "page", "1" },
                { "is_filtered", "true" },
            };
        }

        [Fact]
        public void BindModelAsync_PassModelBindingContext_ReturnModel()
        {
            var bindingSource = new BindingSource("", "", false, false);
            var modelBindingContext = new DefaultModelBindingContext();

            modelBindingContext.ValueProvider = new RouteValueProvider(bindingSource, _collection);

            _modelBinder.BindModelAsync(modelBindingContext);

            Assert.True(modelBindingContext.Result.IsModelSet);
        }
    }
}
