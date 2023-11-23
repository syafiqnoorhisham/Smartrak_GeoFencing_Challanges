using GeoFence_Availability.Helper;
using GeoFence_Availability.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoFence_Availability.Services
{

    public class Calculate
    {
        
        public static Dictionary<int, List<Tuple<DateTime, DateTime>>> CalculateAvailability(List<GeoFencePeriod> geofencePeriods)
        {
            // Group periods by vehicle ID and then create a sorted list of intervals for each vehicle
            var groupedPeriods = geofencePeriods.GroupBy(p => p.VehicleId)
                .ToDictionary(g => g.Key, g => g.Select(p => Tuple.Create(p.EnterTime, p.ExitTime)).OrderBy(t => t.Item1).ToList());

            return groupedPeriods;
        }

        public static int CalculateUnavailableIntervals(List<GeoFencePeriod> geofencePeriods, int soldVehicles)
        {
            int unavailableIntervals = 0;
            var weekIntervals = PopulateWeekInterval.GenerateBusinessWeekIntervals();
            var groupedByWeek = geofencePeriods
                .OrderBy(p => p.EnterTime) // Sort by oldest date first
                .GroupBy(p => CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(p.EnterTime, CalendarWeekRule.FirstDay, DayOfWeek.Monday));


            // For each interval, check if there are enough vehicles available
            foreach (var weekGroup in groupedByWeek)
            {
                var carAvailabilityList = weekGroup
                    .SelectMany(gp => CalculateCarAvailability(weekGroup.ToList(), weekIntervals))
                    .GroupBy(ca => new { ca.Day, ca.Time })
                    .Select(g => new CarAvailability
                    {
                        Day = g.Key.Day,
                        Time = g.Key.Time,
                        Date = g.Any(ca => ca.Date.HasValue) ? g.First(ca => ca.Date.HasValue).Date : null,
                        NumberOfCarsAvailable = g.FirstOrDefault()?.NumberOfCarsAvailable
                    })
                    .ToList();

                foreach (var availability in carAvailabilityList)
                {
                    if (availability.NumberOfCarsAvailable == 0)
                    {
                        unavailableIntervals++;
                    }
                }
            }

            return unavailableIntervals;
        }

        public static List<CarAvailability> CalculateCarAvailability(List<GeoFencePeriod> geofencePeriods, List<Tuple<DayOfWeek, TimeSpan, DateOnly>> weekIntervals)
        {
            var carAvailabilityList = new List<CarAvailability>();
            var availabilityTable = geofencePeriods.GroupBy(p => p.VehicleId)
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var intervalTuple in weekIntervals)
            {
                var day = intervalTuple.Item1;
                var time = intervalTuple.Item2;
                var date = intervalTuple.Item3;
               

                var matchingPeriod = geofencePeriods.FirstOrDefault(gp =>
                   gp.EnterTime.DayOfWeek == day);
                DateOnly? intervalDate = matchingPeriod != null ? DateOnly.FromDateTime(matchingPeriod.EnterTime) : (DateOnly?)null;

                var availableVehicleIds = availabilityTable
                    .Where(kv => kv.Value.Any(period =>
                        period.EnterTime.DayOfWeek == day &&
                        period.EnterTime.TimeOfDay <= time &&
                        period.ExitTime.TimeOfDay > time &&
                        DateOnly.FromDateTime(period.EnterTime) == intervalDate))  // Match the date as well
                    .Select(kv => kv.Key)
                    .ToList();

               


                carAvailabilityList.Add(new CarAvailability
                {
                    Day = day,
                    Time = time,
                    Date = intervalDate,
                    NumberOfCarsAvailable = availableVehicleIds.Count
                });
            }

            return carAvailabilityList;
        }
    }
 }

