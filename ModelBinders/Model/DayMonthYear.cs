using System;
using System.Collections.Generic;
using System.Text;

namespace Buran.Core.MvcLibrary.ModelBinders.Model
{
    public class DayMonthYear
    {
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }

        public DayMonthYear(int year, int month, int day)
        {
            Year = year;
            Month = month;
            Day = day;
        }

        protected DayMonthYear()
        {
            Year = 1900;
            Month = 1;
            Day = 1;
        }

        public virtual DateTime ToDateTime()
        {
            return new DateTime(Year, Month, Day);
        }
    }
}
