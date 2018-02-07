using Buran.Core.MvcLibrary.ModelBinders.Model;
using Buran.MvcLibrary2.ModelBinders;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;

namespace Buran.Core.MvcLibrary.ModelBinders.Providers
{
    public class Decimal2ModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (context.Metadata.UnderlyingOrModelType == typeof(Decimal2))
            {
                return new Decimal2ModelBinder();
            }
            return null;
        }
    }
}
