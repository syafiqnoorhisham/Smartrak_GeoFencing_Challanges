using GeoFence_Availability.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoFence_Availability.Helper
{
    public class PopulateWeekInterval
    {
        private const int IntervalLengthInMinutes = 15;
        public static List<Tuple<DayOfWeek, TimeSpan, DateOnly>> GenerateBusinessWeekIntervals(IGrouping<int,GeoFencePeriod> weekgroup)
        {
            var weekIntervals = new List<Tuple<DayOfWeek, TimeSpan, DateOnly>>();
            TimeSpan businessDayStart = new TimeSpan(8, 30, 0); // Business hours start at 8:30
            TimeSpan businessDayEnd = new TimeSpan(17, 0, 0); // Business hours end at 17:00 (5:00 pm)
            var earliestEnterTime = weekgroup.Min(gp => gp.EnterTime);
            DateOnly current = DateOnly.FromDateTime(earliestEnterTime);

            // Find the start of the week (Monday)
            int daysUntilMonday = DayOfWeek.Monday - current.DayOfWeek;
            DateOnly startOfWeek = current.AddDays(daysUntilMonday);

            // Loop over each day of the week
            foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
            {
                // Skip weekends
                if (day == DayOfWeek.Saturday || day == DayOfWeek.Sunday)
                    continue;

                DateOnly date = startOfWeek;

                // Advance the date to the correct day of the week
                while (date.DayOfWeek != day)
                {
                    date = date.AddDays(1);
                }

                // Calculate the intervals within the business hours for each day
                for (TimeSpan interval = businessDayStart; interval < businessDayEnd; interval = interval.Add(TimeSpan.FromMinutes(IntervalLengthInMinutes)))
                {
                    weekIntervals.Add(new Tuple<DayOfWeek, TimeSpan, DateOnly>(day, interval, date));
                }
            }

            return weekIntervals;
        }

    }
}
