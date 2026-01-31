using System;
using System.Collections.Generic;
using System.Linq;
using ZeroCurveApp.Models;

namespace ZeroCurveApp.Services
{
    public static class Interpolator
    {
        public static List<Instrument> InterpolateCurve(List<Instrument> originalCurve, List<double> targetMaturities)
        {
            List<Instrument> result = new();

            foreach (double m in targetMaturities)
            {
                double rate = InterpolateZeroRate(originalCurve, m);

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

        public static double InterpolateZeroRate(IReadOnlyList<Instrument> instruments, double maturity)
        {
            if (instruments == null || instruments.Count < 2)
                throw new ArgumentException("Curve must at least have two points.");

            instruments = instruments.OrderBy(p => p.Maturity).ToList();

            if (maturity <= instruments[0].Maturity)
            {
                return Linear(
                    instruments[0].Maturity, instruments[0].ZeroRate,
                    instruments[1].Maturity, instruments[1].ZeroRate,
                    maturity);
            }

            if (maturity >= instruments[^1].Maturity)
            {
                return Linear(
                    instruments[^2].Maturity, instruments[^2].ZeroRate,
                    instruments[^1].Maturity, instruments[^1].ZeroRate,
                    maturity);
            }

            for (int i = 0; i < instruments.Count - 1; i++)
            {
                if (maturity >= instruments[i].Maturity && maturity <= instruments[i + 1].Maturity)
                {
                    return Linear(
                        instruments[i].Maturity, instruments[i].ZeroRate,
                        instruments[i + 1].Maturity, instruments[i + 1].ZeroRate,
                        maturity);
                }
            }

            throw new Exception("Interpolation failed.");
        }

        public static double Linear(double x0, double y0, double x1, double y1, double x)
        {
            return y0 + (x - x0) * (y1 - y0) / (x1 - x0);
        }
    }
}