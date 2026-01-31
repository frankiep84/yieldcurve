using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using ZeroCurveApp.Models;

namespace ZeroCurveApp.Services
{
    public static class MarketDataLoader
    {
        public static List<Instrument> LoadMarketData(string file)
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

        public static void WriteMarketData(string file, List<Instrument> instruments)
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
    }
}