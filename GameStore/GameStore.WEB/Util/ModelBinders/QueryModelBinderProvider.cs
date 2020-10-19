using GameStore.Web.ViewModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GameStore.Web.Util.ModelBinders
{
    public class QueryModelBinderProvider : IModelBinderProvider
    {
        private readonly IModelBinder _binder = new QueryModelBinder();

        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            return context.Metadata.ModelType ==
                typeof(QueryModel) ? _binder : null;
        }
    }
}
