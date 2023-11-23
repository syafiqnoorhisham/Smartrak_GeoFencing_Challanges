using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoFence_Availability.Models
{
    public class GeoFencePeriod
    {
        public int VehicleId { get; set; }
        public DateTime EnterTime { get; set; }
        public DateTime ExitTime { get; set; }

    }
}
