using System;

namespace Project2COP4365
{
    /// <summary>
    /// Represents an advanced candlestick with additional calculated properties
    /// for pattern recognition and analysis.
    /// </summary>
    public class SmartCandlestick : Candlestick
    {
        /// <summary>
        /// Calculates the range of the candlestick (High - Low).
        /// </summary>
        public double Range => High - Low;

        /// <summary>
        /// Calculates the body range of the candlestick, which is the absolute difference between Open and Close.
        /// </summary>
        public double BodyRange => Math.Abs(Open - Close);

        /// <summary>
        /// Calculates the top price of the candlestick, which is the higher of Open and Close.
        /// </summary>
        public double TopPrice => Math.Max(Open, Close);

        /// <summary>
        /// Calculates the bottom price of the candlestick, which is the lower of Open and Close.
        /// </summary>
        public double BottomPrice => Math.Min(Open, Close);

        /// <summary>
        /// Calculates the length of the upper tail, which is the difference between the High and TopPrice.
        /// </summary>
        public double UpperTail => High - TopPrice;

        /// <summary>
        /// Calculates the length of the lower tail, which is the difference between BottomPrice and Low.
        /// </summary>
        public double LowerTail => BottomPrice - Low;

        /// <summary>
        /// Initializes a new instance of the <see cref="SmartCandlestick"/> class, inheriting from Candlestick,
        /// with additional pattern recognition functionality.
        /// </summary>
        /// <param name="date">The date of the candlestick data.</param>
        /// <param name="open">The opening price of the candlestick.</param>
        /// <param name="high">The highest price during the trading period.</param>
        /// <param name="low">The lowest price during the trading period.</param>
        /// <param name="close">The closing price of the candlestick.</param>
        /// <param name="volume">The trading volume associated with this candlestick.</param>
        public SmartCandlestick(DateTime date, double open, double high, double low, double close, double volume)
            : base(date, open, high, low, close, volume) { }

        /// <summary>
        /// Indicates if the candlestick is bullish, meaning the Close price is greater than the Open price.
        /// </summary>
        public bool IsBullish => Close > Open;

        /// <summary>
        /// Indicates if the candlestick is bearish, meaning the Open price is greater than the Close price.
        /// </summary>
        public bool IsBearish => Open > Close;

        /// <summary>
        /// Indicates if the candlestick is neutral, meaning the Open price is equal to the Close price.
        /// </summary>
        public bool IsNeutral => Open == Close;

        /// <summary>
        /// Indicates if the candlestick is a Marubozu pattern, which has no upper or lower tails.
        /// </summary>
        public bool IsMarubozu => UpperTail == 0 && LowerTail == 0;

        /// <summary>
        /// Indicates if the candlestick is a Hammer pattern, identified by a small body and a long lower tail.
        /// </summary>
        public bool IsHammer => BodyRange < Range / 2 && LowerTail > BodyRange;

        /// <summary>
        /// Indicates if the candlestick is a Doji pattern, where the body is very small (Open almost equal to Close).
        /// </summary>
        public bool IsDoji => BodyRange < 0.1 * Range;

        /// <summary>
        /// Indicates if the candlestick is a Dragonfly Doji pattern, a type of Doji where Open is equal to Low.
        /// </summary>
        public bool IsDragonflyDoji => IsDoji && Open == Low;

        /// <summary>
        /// Indicates if the candlestick is a Gravestone Doji pattern, a type of Doji where Open is equal to High.
        /// </summary>
        public bool IsGravestoneDoji => IsDoji && Open == High;

        /// <summary>
        /// Determines the pattern type of the candlestick and returns it as a string.
        /// Logs "Hammer pattern detected" for debugging purposes when a Hammer is identified.
        /// </summary>
        /// <returns>A string indicating the candlestick pattern type (e.g., "Hammer", "Bullish", etc.).</returns>
        public string GetPatternType()
        {
            if (IsMarubozu) return "Marubozu"; // Checks if the pattern is Marubozu
            if (IsHammer)
            {
                Console.WriteLine("Hammer pattern detected for date: " + Date); // Log for debugging
                return "Hammer"; // Identifies the pattern as a Hammer
            }
            if (IsDragonflyDoji) return "Dragonfly Doji"; // Checks if the pattern is Dragonfly Doji
            if (IsGravestoneDoji) return "Gravestone Doji"; // Checks if the pattern is Gravestone Doji
            if (IsDoji) return "Doji"; // Checks if the pattern is Doji
            if (IsBullish) return "Bullish"; // Checks if the candlestick is Bulli
            if (IsBearish) return "Bearish"; // Checks if the candlestick is Bearish
            return "Neutral"; // Default to Neutral if no specific pattern is found
        }
    }
}