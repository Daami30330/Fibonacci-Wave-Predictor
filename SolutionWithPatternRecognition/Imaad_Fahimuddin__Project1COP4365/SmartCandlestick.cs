using System;

namespace Project2COP4365
{
    public class SmartCandlestick : Candlestick
    {
        public double Range => High - Low;
        public double BodyRange => Math.Abs(Open - Close);
        public double TopPrice => Math.Max(Open, Close);
        public double BottomPrice => Math.Min(Open, Close);
        public double UpperTail => High - TopPrice;
        public double LowerTail => BottomPrice - Low;

        public SmartCandlestick(DateTime date, double open, double high, double low, double close, double volume)
            : base(date, open, high, low, close, volume) { }

        public bool IsBullish => Close > Open;
        public bool IsBearish => Open > Close;
        public bool IsNeutral => Open == Close;

        public bool IsMarubozu => UpperTail == 0 && LowerTail == 0;
        public bool IsHammer => BodyRange < Range / 2 && LowerTail > BodyRange;
        public bool IsDoji => BodyRange < 0.1 * Range;
        public bool IsDragonflyDoji => IsDoji && Open == Low;
        public bool IsGravestoneDoji => IsDoji && Open == High;

        // Additional methods for peak and valley detection can be added later
    }
}
