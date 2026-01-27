using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using MathNet.Numerics.Interpolation;

// Class om een instrument (looptijd + rate) op te slaan
class Instrument
{
    public double Maturity { get; set; }
    public double Rate { get; set; }
}

// When one calls "dotnet run" the main program is started. Where void means nothing 
// is returned (alternative in case one wants something returned -> int etc.)
class Program
{
    static void Main()
    {
        // In Main the first methode to run is the one that reads the marketdata 
        // for the file (euriborCurve = LoadMarketData("euribor_curve.txt");)
        var curves = new Dictionary<string, List<Instrument>>(); 
        curves["euribor"] = LoadMarketData("euribor_curve.txt");
        curves["ois"] = LoadMarketData("ois_curve.txt");
        
        // Thereafter the methode to calculate the zero curve is called
        var euriborZeroCurve = BuildZeroCurve(curves["euribor"]);
        var oisZeroCurve = BuildZeroCurve(curves["ois"]);

        // Thereafter the results are plotted
        Console.WriteLine("Maturity\tZeroRate");
        foreach (var z in oisZeroCurve)
        {
            Console.WriteLine($"{z.Key}\t\t{z.Value:P4}");
        }
    }

    // Function to read the text file into a list of instruments
    static List<Instrument> LoadMarketData(string file)
    {
        var list = new List<Instrument>();

        if (!File.Exists(file))
        {
            Console.WriteLine($"Fout: bestand '{file}' niet gevonden!");
            return list;
        }

        foreach (var line in File.ReadAllLines(file))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            var parts = line.Split(',');

            if (parts.Length != 2) continue;

            list.Add(new Instrument
            {
                Maturity = double.Parse(parts[0], CultureInfo.InvariantCulture),
                Rate = double.Parse(parts[1], CultureInfo.InvariantCulture)
            });
        }

        return list;
    }

    // Function to build the zero curve from the list of instruments
    static Dictionary<double, double> BuildZeroCurve(List<Instrument> instruments)
    {
        var zeroRates = new Dictionary<double, double>();
        var discountFactors = new Dictionary<double, double>();

        foreach (var inst in instruments)
        {
            double T = inst.Maturity;
            double r = inst.Rate;

            // Simpele jaarlijkse compounding: DF = 1/(1+r)^T
            double df = 1.0 / Math.Pow(1.0 + r, T);

            // Zero rate = (1/DF)^(1/T) -1
            double zero = Math.Pow(df, -1.0 / T) - 1.0;

            discountFactors[T] = df;
            zeroRates[T] = zero;
        }

        return zeroRates;
    }

    // Function to build the zero curve from the list of instruments
    //static
}
