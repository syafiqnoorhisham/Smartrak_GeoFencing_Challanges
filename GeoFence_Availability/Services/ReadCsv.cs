using GeoFence_Availability.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoFence_Availability.Services
{
    public class ReadCsv
    { 
        public static List<GeoFencePeriod> ReadCsvFile(string filePath)
        {
            var periods = new List<GeoFencePeriod>();
            var lines = File.ReadAllLines(filePath).Skip(1); // Skipping the header line

            foreach (var line in lines)
            {
                var parts = line.Split(',');
                periods.Add(new GeoFencePeriod
                {
                    VehicleId = int.Parse(parts[0]),
                    EnterTime = DateTime.Parse(parts[1], CultureInfo.InvariantCulture),
                    ExitTime = DateTime.Parse(parts[2], CultureInfo.InvariantCulture)
                });
            }

            return periods;
        }
    }
}
