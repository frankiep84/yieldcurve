using System;

namespace ZeroCurveApp.Models
{
    public class Instrument
    {
        public double Maturity { get; set; }
        public double SwapRate { get; set; }
        public double DiscountFactor { get; set; }
        public double ZeroRate { get; set; }
    }
}