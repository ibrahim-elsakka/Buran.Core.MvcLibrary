using System;
using Buran.Core.MvcLibrary.Repository;

namespace Buran.Core.MvcLibrary.Data.Validation
{
    public class ValidationErrorUtil
    {
        public static bool RaiseError(IValidationDictionary validationDictionary, string fieldName, string errorMessage)
        {
            if (validationDictionary != null)
            {
                validationDictionary.AddError(fieldName, errorMessage);
                return false;
            }
            throw new Exception(errorMessage);
        }
    }
}
