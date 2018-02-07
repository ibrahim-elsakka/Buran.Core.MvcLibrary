using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Buran.Core.MvcLibrary.ModelBinders
{
    public class DecimalModelBinder : IModelBinder
    {
        private static Decimal? GetA(ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, value);
            if (value == null)
                return null;
            Decimal? retVal = null;
            try
            {
                var cu = new CultureInfo("tr-TR");
                retVal = Decimal.Parse(value.ToString(), cu);
            }
            catch
            {

            }
            return retVal;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
                throw new ArgumentNullException(nameof(bindingContext));
            var ay = GetA(bindingContext);
            if (ay != null)
            {
                bindingContext.Result = ModelBindingResult.Success(ay);
            }
            return Task.CompletedTask;
        }
    }
}