using Buran.Core.MvcLibrary.ModelBinders.Model;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Threading.Tasks;

namespace Buran.Core.MvcLibrary.ModelBinders
{
    public class DayMonthYearModelBinder : IModelBinder
    {
        private static int? GetInteger(ModelBindingContext bindingContext, string key)
        {
            if (bindingContext.ValueProvider.ContainsPrefix(bindingContext.ModelName))
            {
                if (string.IsNullOrEmpty(key)) key = bindingContext.ModelName;
                else key = bindingContext.ModelName + "." + key;
            }
            if (string.IsNullOrEmpty(key)) return null;
            var value = bindingContext.ValueProvider.GetValue(key);
            bindingContext.ModelState.SetModelValue(key, value);
            if (value == null)
                return null;
            int? retVal = null;
            try
            {
                retVal = int.Parse(value.ToString());
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

            var day = GetInteger(bindingContext, "Day");
            var month = GetInteger(bindingContext, "Month");
            var year = GetInteger(bindingContext, "Year");

            if (day != null && month != null && year != null)
            {
                bindingContext.Result = ModelBindingResult.Success(new DayMonthYear(year.Value, month.Value, day.Value));
            }
            return Task.CompletedTask;
        }
    }
}