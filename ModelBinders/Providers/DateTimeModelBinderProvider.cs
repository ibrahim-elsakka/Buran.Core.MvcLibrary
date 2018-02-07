using Buran.MvcLibrary2.ModelBinders;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;

namespace Buran.Core.MvcLibrary.ModelBinders.Providers
{
    public class DateTimeModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (context.Metadata.UnderlyingOrModelType == typeof(DateTime))
            {
                return new DateTimeModelBinder();
            }
            return null;
        }
    }
}
