using System;
using System.Collections.Generic;
using ZeroCurveApp.Models;

namespace ZeroCurveApp.Services
{
    public static class ZeroCurveBuilder
    {
        public static void BuildZeroCurve(List<Instrument> instruments)
        {
            var zeroRates = new Dictionary<double, double>();
            var discountFactors = new Dictionary<double, double>();

            foreach (var inst in instruments)
            {
                double T = inst.Maturity;
                double r = inst.SwapRate;

                // Simple annual compounding: DF = 1/(1+r)^T
                double df = 1.0 / Math.Pow(1.0 + r, T);

                // Zero rate = (1/DF)^(1/T) -1
                double zero = Math.Pow(df, -1.0 / T) - 1.0;

                discountFactors[T] = df;

                inst.DiscountFactor = double.Parse(df.ToString(System.Globalization.CultureInfo.InvariantCulture), System.Globalization.CultureInfo.InvariantCulture);
                inst.ZeroRate = double.Parse(zero.ToString(System.Globalization.CultureInfo.InvariantCulture), System.Globalization.CultureInfo.InvariantCulture);
            }

        }

        public static void BuildZeroCurveHistorical(List<YieldCurve> yieldCurves)
        {
            // Helper local functions
            static double DF(double rate, double T) => 1.0 / Math.Pow(1.0 + rate, T);
            static double ZeroFromDF(double df, double T) => Math.Pow(df, -1.0 / T) - 1.0;

            foreach (var curve in yieldCurves)
            {
                DateTime date = curve.Date;

                if (curve.Y1 != 0.0)
                {
                    curve.Y1DF = DF(curve.Y1, 1.0);
                    curve.Y1Spot = ZeroFromDF(curve.Y1DF, 1.0);
                }

                if (curve.Y2 != 0.0)
                {
                    curve.Y2DF = DF(curve.Y2, 2.0);
                    curve.Y2Spot = ZeroFromDF(curve.Y2DF, 2.0);
                }

                if (curve.Y3 != 0.0)
                {
                    curve.Y3DF = DF(curve.Y3, 3.0);
                    curve.Y3Spot = ZeroFromDF(curve.Y3DF, 3.0);
                }

                if (curve.Y5 != 0.0)
                {
                    curve.Y5DF = DF(curve.Y5, 5.0);
                    curve.Y5Spot = ZeroFromDF(curve.Y5DF, 5.0);
                }

                if (curve.Y7 != 0.0)
                {
                    curve.Y7DF = DF(curve.Y7, 7.0);
                    curve.Y7Spot = ZeroFromDF(curve.Y7DF, 7.0);
                }

                if (curve.Y10 != 0.0)
                {
                    curve.Y10DF = DF(curve.Y10, 10.0);
                    curve.Y10Spot = ZeroFromDF(curve.Y10DF, 10.0);
                }

                if (curve.Y15 != 0.0)
                {
                    curve.Y15DF = DF(curve.Y15, 15.0);
                    curve.Y15Spot = ZeroFromDF(curve.Y15DF, 15.0);
                }

                if (curve.Y20 != 0.0)
                {
                    curve.Y20DF = DF(curve.Y20, 20.0);
                    curve.Y20Spot = ZeroFromDF(curve.Y20DF, 20.0);
                }

                if (curve.Y25 != 0.0)
                {
                    curve.Y25DF = DF(curve.Y25, 25.0);
                    curve.Y25Spot = ZeroFromDF(curve.Y25DF, 25.0);
                }

                if (curve.Y30 != 0.0)
                {
                    curve.Y30DF = DF(curve.Y30, 30.0);
                    curve.Y30Spot = ZeroFromDF(curve.Y30DF, 30.0);
                }

                curve.Sum = curve.Y1 + curve.Y2 + curve.Y3 + curve.Y5 + curve.Y7 + curve.Y10 + curve.Y15 + curve.Y20 + curve.Y25 + curve.Y30;
            }
        }
    }
}