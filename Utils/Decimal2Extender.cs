using System.Globalization;
using System.Linq;
using Buran.Core.MvcLibrary.ModelBinders.Model;

namespace Buran.Core.MvcLibrary.Utils
{
    public static class Decimal2Extender
    {
        public struct DecimalPart
        {
            public string Integral;
            public string Fraction;

            public DecimalPart(string p1, string p2)
            {
                Integral = p1;
                Fraction = p2;
            }
        }

        public static DecimalPart BreakPart(this Decimal2 decimalValue)
        {
            if (decimalValue != null)
            {
                var result = decimalValue.Value.ToString(CultureInfo.InvariantCulture).Split('.');
                if (result.Count() == 2)
                {
                    var d = int.Parse(result[0]);
                    var f = int.Parse(result[1]);
                    var fractionSplit = new DecimalPart
                    {
                        Integral = d.ToString("N0"),
                        Fraction = f < 10 ? "0" + f.ToString("N0") : f.ToString("N0")
                    };
                    return fractionSplit;
                }
            }
            return new DecimalPart("0", "0");
        }
    }
}
