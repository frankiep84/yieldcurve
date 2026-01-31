using System;
using CsvHelper.Configuration;

namespace ZeroCurveApp.Models
{
    public class YieldCurve
    {
        public DateTime Date { get; set; }

        public double Y1 { get; set; }
        public double Y2 { get; set; }
        public double Y3 { get; set; }
        public double Y5 { get; set; }
        public double Y7 { get; set; }
        public double Y10 { get; set; }
        public double Y15 { get; set; }
        public double Y20 { get; set; }
        public double Y25 { get; set; }
        public double Y30 { get; set; }

        public double Y1Spot { get; set; }
        public double Y2Spot { get; set; }
        public double Y3Spot { get; set; }
        public double Y5Spot { get; set; }
        public double Y7Spot { get; set; }
        public double Y10Spot { get; set; }
        public double Y15Spot { get; set; }
        public double Y20Spot { get; set; }
        public double Y25Spot { get; set; }
        public double Y30Spot { get; set; }

        public double Y1DF { get; set; }
        public double Y2DF { get; set; }
        public double Y3DF { get; set; }
        public double Y5DF { get; set; }
        public double Y7DF { get; set; }
        public double Y10DF { get; set; }
        public double Y15DF { get; set; }
        public double Y20DF { get; set; }
        public double Y25DF { get; set; }
        public double Y30DF { get; set; }

        public double Sum { get; set; }
    }

    public sealed class YieldCurveMap : ClassMap<YieldCurve>
    {
        public YieldCurveMap()
        {
            Map(m => m.Date).Name("Date");
            Map(m => m.Y1).Name("1Y");
            Map(m => m.Y2).Name("2Y");
            Map(m => m.Y3).Name("3Y");
            Map(m => m.Y5).Name("5Y");
            Map(m => m.Y7).Name("7Y");
            Map(m => m.Y10).Name("10Y");
            Map(m => m.Y15).Name("15Y");
            Map(m => m.Y20).Name("20Y");
            Map(m => m.Y25).Name("25Y");
            Map(m => m.Y30).Name("30Y");
        }
    }
}