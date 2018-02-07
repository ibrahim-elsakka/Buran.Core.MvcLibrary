using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Buran.MvcLibrary2.ModelBinders
{
    public class DateTimeModelBinder : IModelBinder
    {
        private static DateTime? GetA(ModelBindingContext bindingContext, string key)
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
            DateTime? retVal = null;
            try
            {
                var cu = new CultureInfo("tr-TR");
                retVal = DateTime.Parse(value.ToString(), cu);
            }
            catch
            {

            }
            return retVal;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null) throw new ArgumentNullException("bindingContext");
            var dateTimeAttempt = GetA(bindingContext, "");

            if (dateTimeAttempt != null)
            {
                bindingContext.Result = ModelBindingResult.Success(dateTimeAttempt.Value);
                return Task.CompletedTask;
            }
            var dateAttempt = GetA(bindingContext, "Date");
            var timeAttempt = GetA(bindingContext, "TimeOfDay");
            if (dateAttempt != null && timeAttempt != null)
            {
                bindingContext.Result = ModelBindingResult.Success(new DateTime(dateAttempt.Value.Year,
                    dateAttempt.Value.Month,
                    dateAttempt.Value.Day,
                    timeAttempt.Value.Hour,
                    timeAttempt.Value.Minute,
                    timeAttempt.Value.Second));
            }
            else
            {
                bindingContext.Result = ModelBindingResult.Success(dateAttempt ?? timeAttempt);
            }
            return Task.CompletedTask;
        }
    }
}