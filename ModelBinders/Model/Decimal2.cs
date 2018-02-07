using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace Buran.Core.MvcLibrary.ModelBinders.Model
{
    [ComplexType]
    [ModelBinder(BinderType = typeof(Decimal2ModelBinder))]
    public class Decimal2
    {
        public decimal Value { get; set; }

        public Decimal2()
        {
        }
        public Decimal2(decimal val)
        {
            Value = val;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}