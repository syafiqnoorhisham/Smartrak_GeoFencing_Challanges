using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoFence_Availability.Models
{
    public class CarAvailability
    {
        public DayOfWeek Day { get; set; }
        public TimeSpan Time { get; set; }
        public DateOnly? Date { get; set; }
        public int? NumberOfCarsAvailable { get; set; }
    }
}
