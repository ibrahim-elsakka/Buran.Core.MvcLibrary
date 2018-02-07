using System;
using System.ComponentModel.DataAnnotations;

namespace Buran.Core.MvcLibrary.Data.Validation
{
    public class RequiredIfAttribute : ValidationAttribute
    {
        private string PropertyName { get; set; }
        private object DesiredValue { get; set; }

        public RequiredIfAttribute(String propertyName, Object desiredvalue)
        {
            PropertyName = propertyName;
            DesiredValue = desiredvalue;
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            var instance = context.ObjectInstance;
            var type = instance.GetType();
            var proprtyvalue = type.GetProperty(PropertyName).GetValue(instance, null);
            if (proprtyvalue == null && DesiredValue == null)
                return ValidationResult.Success;
            return proprtyvalue.Equals(DesiredValue)
                       ? new ValidationResult(ErrorMessage)
                       : ValidationResult.Success;
        }
    }
}
