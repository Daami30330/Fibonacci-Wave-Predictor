using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Represents a single candlestick in a candlestick chart, storing date, open, high, low, close, and volume values.
/// </summary>
public class Candlestick
{
    /// <summary>
    /// Gets or sets the date of the candlestick.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Gets or sets the opening price of the candlestick.
    /// </summary>
    public double Open { get; set; }

    /// <summary>
    /// Gets or sets the highest price of the candlestick.
    /// </summary>
    public double High { get; set; }

    /// <summary>
    /// Gets or sets the lowest price of the candlestick.
    /// </summary>
    public double Low { get; set; }

    /// <summary>
    /// Gets or sets the closing price of the candlestick.
    /// </summary>
    public double Close { get; set; }

    /// <summary>
    /// Gets or sets the volume associated with the candlestick.
    /// </summary>
    public double Volume { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Candlestick"/> class with specified date, open, high, low, close, and volume values.
    /// </summary>
    /// <param name="date">The date of the candlestick.</param>
    /// <param name="open">The opening price of the candlestick.</param>
    /// <param name="high">The highest price of the candlestick.</param>
    /// <param name="low">The lowest price of the candlestick.</param>
    /// <param name="close">The closing price of the candlestick.</param>
    /// <param name="volume">The volume associated with the candlestick.</param>
    public Candlestick(DateTime date, double open, double high, double low, double close, double volume)
    {
        Date = date;  // Set the date for this candlestick
        Open = open;  // Set the opening price
        High = high;  // Set the highest price of the day
        Low = low;    // Set the lowest price of the day
        Close = close; // Set the closing price
        Volume = volume; // Set the trading volume
    }
}

