using Buran.Core.MvcLibrary.ModelBinders.Model;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Threading.Tasks;

namespace Buran.Core.MvcLibrary.ModelBinders
{
    public class Decimal2ModelBinder : IModelBinder
    {
        private static string GetString(ModelBindingContext bindingContext, string key)
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
            string retVal = "";
            try
            {
                retVal = value.ToString();
            }
            catch
            {

            }
            return retVal;
        }

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

            var p1 = GetInteger(bindingContext, "P1");
            var p2 = GetInteger(bindingContext, "P2");
            var p3 = GetString(bindingContext, "P2");

            if (p1 != null && p2 != null)
            {
                var r = new Decimal2(Convert.ToDecimal(p1 + (p2 / (Math.Pow(10, p3.ToString().Length)))));
                bindingContext.Result = ModelBindingResult.Success(r);
            }
            return Task.CompletedTask;
        }
    }
}