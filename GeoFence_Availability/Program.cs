using GeoFence_Availability.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using GeoFence_Availability.Services;
using GeoFence_Availability.Helper;

class Program
{
    private const double TotalBusinessHoursPerWeek = 42.5; // 42.5 hours expressed in tenths of hours
    private const int BusinessHoursPerDay = 85; // 8.5 hours expressed in tenths of hours
    private const int IntervalLengthInMinutes = 15;
    private const int TotalVehicles = 12;

    public static void Main()
    { 
        string csvFilePath = "C:\\Users\\LENOVO X1\\Downloads\\coding_challenge\\GeofencePeriods_local.csv";
        var geofencePeriods = ReadCsv.ReadCsvFile(csvFilePath);
        var availabilityTable = Calculate.CalculateAvailability(geofencePeriods);

        Console.WriteLine("+-------------------------+---------------------------------------------------------------------------------------+");
        Console.WriteLine("| Number of vehicles sold | Number of hours per week during which no vehicles are available (inside the geofence) |");
        Console.WriteLine("+-------------------------+---------------------------------------------------------------------------------------+");

        for (int soldVehicles = 0; soldVehicles <= TotalVehicles; soldVehicles++)
        {
            int unavailableIntervals = Calculate.CalculateUnavailableIntervals(geofencePeriods, soldVehicles);
            double unavailableHours = unavailableIntervals / 4;
            double unavailableHoursbyweek = unavailableHours / 5;
            double individualunavailableIntervals = (TotalBusinessHoursPerWeek - unavailableHoursbyweek)/TotalVehicles;
            if (soldVehicles > 0)
            {
                unavailableHoursbyweek = (individualunavailableIntervals * soldVehicles) + unavailableHoursbyweek;
            }
            Console.WriteLine($"| {soldVehicles,23} | {unavailableHoursbyweek,85:F2} |");
        }

        Console.WriteLine("+-------------------------+---------------------------------------------------------------------------------------+");
    }

  
}
