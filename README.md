# SmartCandlestick Analyzer

A Windows Forms application that visualizes stock data using candlestick charts and analyzes market patterns like Doji, Hammer, Marubozu, Peaks, and Valleys.

## Features

- Load daily, weekly, or monthly stock data from Yahoo Finance `.csv` files
- Display OHLC candlestick charts and volume column plots
- Detect bullish, bearish, and neutral patterns
- Automatically identify and annotate Peaks (green) and Valleys (red)
- Use dynamic Y-axis normalization for clear visuals
- Multi-chart support — open new charts in separate windows
- Built with `SmartCandlestick` class extending traditional OHLC modeling

## 🛠 Technologies

- **Language:** C#
- **Framework:** .NET Framework (Windows Forms)
- **Data Binding:** Built-in WinForms data binding with custom models
- **Charting:** System.Windows.Forms.DataVisualization
- **UI Tools:** Windows Forms with modular UI elements

## Project Structure

- `SmartCandlestick.cs`: Custom class with range and pattern detection
- `Form_Main.cs`: Main chart and logic for file input, filtering, and rendering
- `ChartHelpers.cs`: Methods for drawing annotations and normalizing charts
- `Stock Data/`: Folder containing sample CSVs from Yahoo Finance

## How to Run

1. Open solution in **Visual Studio**
2. Restore NuGet packages (if needed)
3. Click **Start / Run**
4. Use the OpenFileDialog to select a `.csv` stock data file

> Example file names: `AAPL-Day.csv`, `GOOG-Week.csv`, `MSFT-Month.csv`

##  Example Output

<h2>📈 Example Output</h2>
<p align="center">
  <img src="Candlestick.png" alt="Candlestick chart sample" width="600">
</p>


## Learnings & Highlights

- Designed a full-featured stock analysis GUI with real-time updates
- Used object-oriented inheritance to model financial data
- Implemented a clean, user-friendly interface with pattern detection
- Followed best practices in UI naming conventions and code commenting

## Stock CSV Format

Expected input:
Date,Open,High,Low,Close,Adj Close,Volume
2023-01-03,125.07,130.00,124.17,129.41,129.41,98931200
...


## License

This project is open-source and available under the MIT License.
