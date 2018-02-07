using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Buran.Core.MvcLibrary.Repository
{
    public class ModelStateWrapper : IValidationDictionary
    {
        public readonly ModelStateDictionary ModelState;

        public ModelStateWrapper(ModelStateDictionary modelState)
        {
            ModelState = modelState;
        }

        public void AddError(string key, string errorMessage)
        {
            ModelState.AddModelError(key, errorMessage);
        }

        public bool IsValid
        {
            get { return ModelState.IsValid; }
        }
    }
}