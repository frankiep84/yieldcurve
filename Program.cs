using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper; // For this I wrote in the terminal "dotnet add package CsvHelper"
using ZeroCurveApp.Models;
using ZeroCurveApp.Services;

namespace ZeroCurveApp
{

// When one calls "dotnet run" the main program is started. Where void means nothing 
// is returned (alternative in case one wants something returned -> int etc.)
    class Program
    {
        static void Main()
        {
            var curves = new Dictionary<string, List<Instrument>>();
            curves["eur"] = MarketDataLoader.LoadMarketData("eur_swap_curve.txt");
            curves["usd"] = MarketDataLoader.LoadMarketData("usd_swap_curve.txt");

            using var reader = new StreamReader("dummy_yield_curves_20y_monthly.csv");
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<YieldCurveMap>();
            var yieldCurves = csv.GetRecords<YieldCurve>().ToList();

            ZeroCurveBuilder.BuildZeroCurve(curves["eur"]);
            ZeroCurveBuilder.BuildZeroCurve(curves["usd"]);
            ZeroCurveBuilder.BuildZeroCurveHistorical(yieldCurves);

            var targetMaturities = Enumerable.Range(1, 30).Select(i => (double)i).ToList();
            var newCurve = Interpolator.InterpolateCurve(curves["eur"], targetMaturities);

            Console.WriteLine("Maturity\tZeroRate");
            foreach (var z in newCurve)
            {
                Console.WriteLine($"{z.Maturity}\t\t{z.ZeroRate:P4}");
            }

            MarketDataLoader.WriteMarketData("output.txt", newCurve);
        }
    }
}
