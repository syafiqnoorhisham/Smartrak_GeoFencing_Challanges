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
    private const double BusinessHoursPerDay = 8.5; // 8.5 hours expressed in tenths of hours
    private const double BusinessDaysPerWeek = 5;
    private const double TotalBusinessHoursPerWeek = BusinessHoursPerDay* BusinessDaysPerWeek;
    private const int TotalVehicles = 12;

    public static void Main()
    {
        string csvFilePath = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\data\GeofencePeriods.csv");

        var geofencePeriods = ReadCsv.ReadCsvFile(csvFilePath);

        Console.WriteLine("+-------------------------+---------------------------------------------------------------------------------------+");
        Console.WriteLine("| Number of vehicles sold | Number of hours per week during which no vehicles are available (inside the geofence) |");
        Console.WriteLine("+-------------------------+---------------------------------------------------------------------------------------+");

        for (int soldVehicles = 0; soldVehicles <= TotalVehicles; soldVehicles++)
        {
            int unavailableIntervals = Calculate.CalculateUnavailableIntervals(geofencePeriods);
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
