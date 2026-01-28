using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using MathNet.Numerics.Interpolation;

// Class om een instrument (looptijd + rate) op te slaan
class Instrument
{
    public double Maturity { get; set; }
    public double SwapRate { get; set; }
    public double DiscountFactor { get; set; }
    public double ZeroRate { get; set; }
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
        curves["eur"] = LoadMarketData("eur_swap_curve.txt");
        curves["usd"] = LoadMarketData("usd_swap_curve.txt");
        
        // Thereafter the methode to calculate the zero curve is called
        BuildZeroCurve(curves["eur"]);
        BuildZeroCurve(curves["usd"]);

        // Call interpolation routine
        //var targetMaturities = new List<double> {0.25, 0.5, 1, 2, 3, 4, 5, 7, 10, 15, 20, 30 };
        var targetMaturities = Enumerable.Range(1, 30).Select(i => (double)i).ToList();
        var newCurve = InterpolateCurve(curves["eur"], targetMaturities);
 
        // Thereafter the results are plotted
        Console.WriteLine("Maturity\tZeroRate");
        //zeroDict = curves["Euribor"].ToDictionary(curves => curves.Maturity, curves => curves.ZeroRate);
        //var oisZeroCurve = curves["eur"].ToDictionary(col => col.Maturity, col => col.ZeroRate);
        foreach (var z in newCurve)
        {
            Console.WriteLine($"{z.Maturity}\t\t{z.ZeroRate:P4}");
        }

        // Finally the results are written to a text file
        WriteMarketData("output.txt", newCurve);
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
                SwapRate = double.Parse(parts[1], CultureInfo.InvariantCulture)
            });
        }

        return list;
    }

    // Function to write market data to a text file
    static void WriteMarketData(string file, List<Instrument> instruments)
    {
        using var writer = new StreamWriter(file);
        foreach (var inst in instruments)
        {
            string line =
                inst.Maturity.ToString(CultureInfo.InvariantCulture) + "," +
                inst.SwapRate.ToString(CultureInfo.InvariantCulture);

            writer.WriteLine(line);
        }
    }

    // Function to build the zero curve from the list of instruments
     static void BuildZeroCurve(List<Instrument> instruments)
    {
        var zeroRates = new Dictionary<double, double>();
        var discountFactors = new Dictionary<double, double>();

        foreach (var inst in instruments)
        {
            double T = inst.Maturity;
            double r = inst.SwapRate;

            // Simpele jaarlijkse compounding: DF = 1/(1+r)^T
            double df = 1.0 / Math.Pow(1.0 + r, T);

            // Zero rate = (1/DF)^(1/T) -1
            double zero = Math.Pow(df, -1.0 / T) - 1.0;

            discountFactors[T] = df;
                 
            // Store the zero rates in the instrument for potential further use
            inst.DiscountFactor = double.Parse(df.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
            inst.ZeroRate = double.Parse(zero.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
        }

    }

    // Function for linear interpolation of (zero) curves
    public static List<Instrument> InterpolateCurve(List<Instrument> originalCurve, List<double> targetMaturities)
    {
        List<Instrument> result = new();

        foreach (double m in targetMaturities)
        {
            double rate = InterpolateZeroRate(originalCurve, m);

            // For now the discountfactor and swaprate are set equal to the zero rate
            result.Add(new Instrument
            {
                Maturity = m,
                ZeroRate = rate,
                DiscountFactor = rate,
                SwapRate = rate
            });
        }

        return result;
    }


    // Function for linear interpolation of (zero) rates
    public static double InterpolateZeroRate(IReadOnlyList<Instrument> instruments, double maturity)
    {
        if (instruments == null || instruments.Count < 2)
            throw new ArgumentException("Curve must at least have two points.");

        // Zorg dat curve gesorteerd is
        instruments = instruments.OrderBy(p => p.Maturity).ToList();

        // Extrapolate left
        if (maturity <= instruments[0].Maturity)
        {
            return Linear(
                instruments[0].Maturity, instruments[0].ZeroRate,
                instruments[1].Maturity, instruments[1].ZeroRate,
                maturity);
        }

        // Extrapolate right
        if (maturity >= instruments[^1].Maturity)
        {
            return Linear(
                instruments[^2].Maturity, instruments[^2].ZeroRate,
                instruments[^1].Maturity, instruments[^1].ZeroRate,
                maturity);
        }

        // Interpolate within the range
        for (int i = 0; i < instruments.Count - 1; i++)
        {
            if (maturity >= instruments[i].Maturity &&
                maturity <= instruments[i + 1].Maturity)
            {
                return Linear(
                    instruments[i].Maturity, instruments[i].ZeroRate,
                    instruments[i + 1].Maturity, instruments[i + 1].ZeroRate,
                    maturity);
            }
        }

        throw new Exception("Interpolatation failed.");
    }

    // Auxiliary function for linear interpolation
    public static double Linear(double x0, double y0, double x1, double y1, double x)
        {
            return y0 + (x - x0) * (y1 - y0) / (x1 - x0);
        }

    }
