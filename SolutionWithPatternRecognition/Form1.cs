using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static System.Windows.Forms.AxHost;

namespace Project3COP4365
{
    public partial class e : Form
    {
        private string stockFilePath; // Path to the CSV file for stock data
        private DateTime startDate; // Start date for filtering stock data
        private DateTime endDate; // End date for filtering stock data
        private List<SmartCandlestick> candlesticks = new List<SmartCandlestick>(); // List to store candlestick data
        private SmartCandlestick firstSelectedCandlestick = null;
        private SmartCandlestick secondSelectedCandlestick = null;
        private int firstSelectedIndex = -1;
        private int secondSelectedIndex = -1;
        private bool isDragging = false; // Indicates if dragging is in progress
        private Point startPoint; // Starting point of the drag
        private RectangleAnnotation dragRectangle; // Temporary rectangle annotation for visual feedback


        /// <summary>
        /// Default constructor for the form, initializing UI components.
        /// </summary>
        public e()
        {
            InitializeComponent();
            chart_candlestick.MouseClick += chart_candlestick_MouseClick;


        }

        /// <summary>
        /// Constructor with parameters to set the file path, start date, and end date.
        /// Calls the FileHandler method to load and display data on form creation.
        /// </summary>
        /// <param name="filePath">The path to the stock data CSV file.</param>
        /// <param name="start">Start date for filtering data.</param>
        /// <param name="end">End date for filtering data.</param>
        public e(string filePath, DateTime start, DateTime end)
        {
            InitializeComponent();
            stockFilePath = filePath; // Set the file path for the stock data
            startDate = start; // Set the start date for filtering
            endDate = end; // Set the end date for filtering
            FileHandler(); // Load and display data on form creation
        }

        /// <summary>
        /// Event handler for the "Load Stock Data" button.
        /// Allows the user to select one or multiple CSV files and opens a new form for each file.
        /// </summary>
        private void button_loadStockData_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Monthly CSV Files (*-Month.csv)|*-Month.csv|Daily CSV Files (*-Day.csv)|*-Day.csv|Weekly CSV Files (*-Week.csv)|*-Week.csv|All CSV Files (*.csv)|*.csv", // Filtering the files by type
                FilterIndex = 4, // Default to show all CSV files initially
                Multiselect = true // Allow multiple file selection
            };

            // Show the file dialog and check if the user selected a file
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string[] filePaths = openFileDialog.FileNames; // Get the selected file paths
                startDate = datePicker_startDate.Value; // Get start date from date picker
                endDate = datePicker_endDate.Value; // Get end date from date picker

                // Open a new form for each selected file with specified date range
                foreach (string filePath in filePaths)
                {
                    e newForm = new e(filePath, startDate, endDate);
                    newForm.Show();
                }
            }
        }

        /// <summary>
        /// Handles the main sequence of reading, filtering, normalizing, and displaying data.
        /// </summary>
        private void FileHandler()
        {
            readCandlesticksFromFile(); // Load candlestick data from the CSV file
            filterCandlesticks(); // Filter data within specified date range
            NormalizeChart(); // Set Y-axis to fit data range with padding for both charts
            displayChart(); // Render the filtered candlestick data
       
        }

        /// <summary>
        /// Reads the stock data from a CSV file and populates the candlesticks list.
        /// </summary>
        private void readCandlesticksFromFile()
        {
            try
            {
                candlesticks.Clear(); // Clear existing data in candlesticks list

                using (StreamReader reader = new StreamReader(stockFilePath))
                {
                    string line;
                    bool isHeader = true;

                    // Read each line from the file
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (isHeader) // Skip the header row
                        {
                            isHeader = false;
                            continue;
                        }

                        string[] values = line.Split(','); // Split line by comma to extract values
                        DateTime date = DateTime.Parse(values[0]); // Parse date from first column

                        // Create a new SmartCandlestick object and add it to the list
                        var candlestick = new SmartCandlestick(
                            date,
                            Convert.ToDouble(values[1]), // Open price
                            Convert.ToDouble(values[2]), // High price
                            Convert.ToDouble(values[3]), // Low price
                            Convert.ToDouble(values[4]), // Close price
                            Convert.ToDouble(values[5]) // Volume
                        );
                        candlesticks.Add(candlestick); // Add candlestick to the list
                    }
                }
            }
            catch (Exception ex)
            {
                // Display error message if reading fails
                MessageBox.Show("Error reading stock data: " + ex.Message);
            }
        }

        /// <summary>
        /// Filters the candlesticks list to include only data within the start and end date range.
        /// </summary>
        private void filterCandlesticks()
        {
            candlesticks = candlesticks
                .Where(c => c.Date >= startDate && c.Date <= endDate) // Filter based on date
                .ToList();
        }

        /// <summary>
        /// Displays the filtered candlestick data on the chart, adding peaks, valleys, and tooltips for pattern types.
        /// </summary>
        private void displayChart()
        {
            // Clear existing annotations and series to avoid duplicates
            chart_candlestick.Annotations.Clear();
            chart_candlestick.Series.Clear();

            // Create a new series for the candlestick chart
            var candlestickSeries = new System.Windows.Forms.DataVisualization.Charting.Series("Candlestick")
            {
                ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Candlestick,
                XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime, // Changed to DateTime
                ["OpenCloseStyle"] = "Triangle",
                ["ShowOpenClose"] = "Both",
                ["PointWidth"] = "0.8",
                ["PriceUpColor"] = "Green",
                ["PriceDownColor"] = "Red"
            };

            // Loop through each candlestick to add data points and annotations
            for (int i = 1; i < candlesticks.Count - 1; i++)
            {
                var candle = candlesticks[i];
                var previousCandle = candlesticks[i - 1];
                var nextCandle = candlesticks[i + 1];

                // Create a new data point for the candlestick chart
                var dataPoint = new System.Windows.Forms.DataVisualization.Charting.DataPoint();
                dataPoint.SetValueXY(candle.Date, candle.High, candle.Low, candle.Open, candle.Close); // Pass DateTime directly
                candlestickSeries.Points.Add(dataPoint); // Add data point to series

                // Check if current candle is a peak and add annotation if true
                if (candle.High > previousCandle.High && candle.High > nextCandle.High)
                {
                    AddAnnotation("Peak", candle.Date, candle.High, Color.Green);
                }

                // Check if current candle is a valley and add annotation if true
                if (candle.Low < previousCandle.Low && candle.Low < nextCandle.Low)
                {
                    AddAnnotation("Valley", candle.Date, candle.Low, Color.Red);
                }
            }

            // Add the candlestick series to the chart
            chart_candlestick.Series.Add(candlestickSeries);

            // Set additional chart properties for readability and appearance
            double minPrice = candlesticks.Min(c => c.Low);
            double maxPrice = candlesticks.Max(c => c.High);
            double buffer = (maxPrice - minPrice) * 0.02;

            // Set the Y-axis range based on candlestick values
            chart_candlestick.ChartAreas[0].AxisY.Minimum = minPrice - buffer;
            chart_candlestick.ChartAreas[0].AxisY.Maximum = maxPrice + buffer;

            // Set the X-axis formatting to match the candlestick chart
            chart_candlestick.ChartAreas[0].AxisX.LabelStyle.Format = "MM/yyyy"; // Adjust format as needed
            chart_candlestick.ChartAreas[0].AxisX.LabelStyle.Angle = -45; // Rotate labels for readability
            chart_candlestick.ChartAreas[0].AxisX.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Months;
            chart_candlestick.ChartAreas[0].AxisX.Interval = 5; // Set to 5 month intervals for better alignment

            // Ensure grid and other appearance settings are similar to the candlestick chart
            chart_candlestick.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0; // Hide X-axis grid lines
            chart_candlestick.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.Black; // Set Y-axis grid color
            chart_candlestick.Titles.Clear();
            chart_candlestick.Titles.Add($"{Path.GetFileNameWithoutExtension(stockFilePath)} Candlestick Chart");
            chart_candlestick.Legends.Clear(); // Remove legend to maximize chart space
        }


        /// <summary>
        /// Displays the volume data as a column plot, normalized with dynamic Y-axis range
        /// and consistent X-axis interval with the candlestick chart.
        /// </summary>
        private void PlotBeautyVsPrice(List<Tuple<double, int>> beautyVsPrice)
        {
            // Clear the existing chart
            chart_volume.Series.Clear();
            chart_volume.ChartAreas[0].AxisX.Title = "Price";
            chart_volume.ChartAreas[0].AxisY.Title = "Beauty";

            // Create a new series for the Beauty vs. Price plot
            var beautySeries = new System.Windows.Forms.DataVisualization.Charting.Series("Beauty vs Price")
            {
                ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column,
                XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double,
                BorderWidth = 4 // Set the line thickness to 3 (adjust as needed)
            };

            // Add data points to the series
            foreach (var dataPoint in beautyVsPrice)
            {
                beautySeries.Points.AddXY(dataPoint.Item1, dataPoint.Item2);
            }

            // Add the series to the chart
            chart_volume.Series.Add(beautySeries);
        }

        private void AnalyzeCorrelation(List<Tuple<double, int>> beautyVsPrice, int secondSelectedIndex)
        {
            // Find the price with the highest Beauty value
            var maxBeauty = beautyVsPrice.OrderByDescending(b => b.Item2).First();
            double maxBeautyPrice = maxBeauty.Item1;

            // Find the next significant candlestick (Peak or Valley)
            SmartCandlestick nextSignificantCandlestick = null;
            for (int i = secondSelectedIndex + 1; i < candlesticks.Count; i++)
            {
                if (IsPeak(i) || IsValley(i))
                {
                    nextSignificantCandlestick = candlesticks[i];
                    break;
                }
            }

            // Check if a significant candlestick was found
            if (nextSignificantCandlestick != null)
            {
                double nextPrice = IsPeak(candlesticks.IndexOf(nextSignificantCandlestick)) ? nextSignificantCandlestick.High : nextSignificantCandlestick.Low;

                // Display the results
                MessageBox.Show(
                    $"Price with Highest Beauty: {maxBeautyPrice}\n" +
                    $"Next Significant Candlestick Price: {nextPrice}\n" +
                    $"Correlation: {(Math.Abs(maxBeautyPrice - nextPrice) < 1e-2 ? "High" : "Low")}",
                    "Correlation Analysis"
                );
            }
            else
            {
                MessageBox.Show("No significant candlestick (Peak or Valley) found after the selected wave.", "Correlation Analysis");
            }
        }






        /// <summary>
        /// Adds an annotation and a horizontal line on the chart for peaks or valleys.
        /// </summary>
        /// <param name="label">Label for the annotation (e.g., "Peak" or "Valley").</param>
        /// <param name="date">Date associated with the candlestick.</param>
        /// <param name="price">Price level for the annotation.</param>
        /// <param name="color">Color of the annotation and line.</param>
        private void AddAnnotation(string label, DateTime date, double price, Color color)
        {
            var annotation = new System.Windows.Forms.DataVisualization.Charting.TextAnnotation
            {
                Text = label, // Set annotation text
                X = date.ToOADate(), // Set X position based on date
                Y = price, // Set Y position based on price
                ForeColor = color, // Set annotation color
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            chart_candlestick.Annotations.Add(annotation); // Add annotation to chart

            var lineAnnotation = new System.Windows.Forms.DataVisualization.Charting.HorizontalLineAnnotation
            {
                AxisX = chart_candlestick.ChartAreas[0].AxisX,
                AxisY = chart_candlestick.ChartAreas[0].AxisY,
                ClipToChartArea = chart_candlestick.ChartAreas[0].Name,
                IsInfinitive = true, // Make line stretch across chart
                LineColor = color, // Set line color
                LineWidth = 2, // Set line width
                AnchorY = price // Anchor line at specified price level
            };
            chart_candlestick.Annotations.Add(lineAnnotation); // Add line annotation to chart
        }

        /// <summary>
        /// Sets the Y-axis minimum and maximum with a 2% padding to use the full dynamic range of the chart.
        /// </summary>
        private void NormalizeChart()
        {
            if (candlesticks.Count > 0)
            {
                double minY = candlesticks.Min(c => c.Low); // Find minimum low
                double maxY = candlesticks.Max(c => c.High); // Find maximum high
                double padding = (maxY - minY) * 0.02; // Calculate 2% padding

                chart_candlestick.ChartAreas[0].AxisY.Minimum = minY - padding; // Set Y-axis minimum
                chart_candlestick.ChartAreas[0].AxisY.Maximum = maxY + padding; // Set Y-axis maximum
                chart_volume.ChartAreas[0].AxisY.Minimum = 0; // Start volume chart Y-axis from 0 for clarity
            }
        }
        /// <summary>
        /// Highlights the candlestick
        /// </summary>
        private void HighlightCandlestick(int index, string label)
        {
            var annotation = new System.Windows.Forms.DataVisualization.Charting.TextAnnotation
            {
                Text = label,
                X = candlesticks[index].Date.ToOADate(),
                Y = candlesticks[index].High, // Show label above the candlestick
                ForeColor = Color.Blue,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            chart_candlestick.Annotations.Add(annotation);
        }

        /// <summary>
        /// computes the beauty vs price
        /// </summary>
        private List<Tuple<double, int>> ComputeBeautyVsPrice(double lowPrice, double highPrice, List<SmartCandlestick> waveCandlesticks, double tolerance, bool isDownwave = true)
        {
            List<Tuple<double, int>> beautyVsPrice = new List<Tuple<double, int>>();

            // Calculate the range (height) of the wave
            double waveHeight = highPrice - lowPrice;
            double step = waveHeight * 0.05; // 5% step (adjust as needed)
            int numSteps = (int)(waveHeight * 0.25 / step); // Calculate steps for 25% of the wave

            // Loop through each adjusted price and compute Beauty
            for (int i = 0; i <= numSteps; i++)
            {
                // Adjust price
                double adjustedLow = isDownwave ? lowPrice - i * step : lowPrice + i * step;
                double adjustedHigh = isDownwave ? highPrice : highPrice + i * step;

                // Recompute Fibonacci levels
                var fibonacciLevels = CalculateFibonacciLevels(adjustedLow, adjustedHigh);

                // Compute Beauty
                int beauty = CalculateBeauty(waveCandlesticks, fibonacciLevels, tolerance);

                // Add adjusted price and Beauty to the list
                beautyVsPrice.Add(new Tuple<double, int>(adjustedLow, beauty));
            }

            return beautyVsPrice;
        }


        private void ValidateWave()
        {
            if (firstSelectedCandlestick != null && secondSelectedCandlestick != null)
            {
                if (firstSelectedCandlestick.Date < secondSelectedCandlestick.Date)
                {
                    MessageBox.Show("Valid wave selected!");

                    // Debug: Print details of the selected candlesticks
                    Console.WriteLine($"First Selected: Date = {firstSelectedCandlestick.Date}, Open = {firstSelectedCandlestick.Open}, High = {firstSelectedCandlestick.High}, Low = {firstSelectedCandlestick.Low}, Close = {firstSelectedCandlestick.Close}");
                    Console.WriteLine($"Second Selected: Date = {secondSelectedCandlestick.Date}, Open = {secondSelectedCandlestick.Open}, High = {secondSelectedCandlestick.High}, Low = {secondSelectedCandlestick.Low}, Close = {secondSelectedCandlestick.Close}");


                    // Get High and Low prices of the wave
                    double lowPrice = Math.Min(firstSelectedCandlestick.Low, secondSelectedCandlestick.Low);
                    double highPrice = Math.Max(firstSelectedCandlestick.High, secondSelectedCandlestick.High);

                    // Get all candlesticks in the wave
                    var waveCandlesticks = candlesticks
                        .Where(c => c.Date >= firstSelectedCandlestick.Date && c.Date <= secondSelectedCandlestick.Date)
                        .ToList();

                    // Display Fibonacci levels
                    DisplayFibonacciLevels(lowPrice, highPrice);

                    // Add the rectangle annotation for the wave
                    AddWaveRectangle(lowPrice, highPrice, firstSelectedCandlestick.Date, secondSelectedCandlestick.Date);

                    // Add confirmation dots for each candlestick
                    double tolerance = 0.5; // Adjust as needed
                    AddConfirmationDots(waveCandlesticks, CalculateFibonacciLevels(lowPrice, highPrice), tolerance);

                    // Display the Beauty of the wave
                    DisplayWaveBeauty(waveCandlesticks, lowPrice, highPrice);

                    // Compute Beauty vs. Price
                    var beautyVsPrice = ComputeBeautyVsPrice(lowPrice, highPrice, waveCandlesticks, tolerance);

                    // Plot Beauty vs. Price
                    PlotBeautyVsPrice(beautyVsPrice);

                    // Perform Correlation Analysis
                    AnalyzeCorrelation(beautyVsPrice, candlesticks.IndexOf(secondSelectedCandlestick));

                    // Display the pattern type for the first and second candlesticks
                    string firstPattern = firstSelectedCandlestick.GetPatternType();
                    string secondPattern = secondSelectedCandlestick.GetPatternType();
                    MessageBox.Show(
                        $"First Candlestick Pattern: {firstPattern}\n" +
                        $"Second Candlestick Pattern: {secondPattern}",
                        "Selected Candlestick Patterns"
                    );
                }
                else
                {
                    MessageBox.Show("Invalid wave. The second candlestick must come after the first.");
                    ResetSelection();
                }
            }
        }




        private void AddWaveRectangle(double lowPrice, double highPrice, DateTime startDate, DateTime endDate)
        {
            var rectangleAnnotation = new System.Windows.Forms.DataVisualization.Charting.RectangleAnnotation
            {
                AxisX = chart_candlestick.ChartAreas[0].AxisX,
                AxisY = chart_candlestick.ChartAreas[0].AxisY,
                ClipToChartArea = chart_candlestick.ChartAreas[0].Name,
                X = startDate.ToOADate(),
                Y = highPrice,
                Width = endDate.ToOADate() - startDate.ToOADate(), // Horizontal span (dates)
                Height = highPrice - lowPrice, // Vertical span (prices)
                LineColor = Color.Black,
                LineWidth = 2,
                BackColor = Color.FromArgb(50, Color.LightBlue) // Semi-transparent fill
            };

            chart_candlestick.Annotations.Add(rectangleAnnotation);
        }




        /// <summary>
        /// Add confirmation dots
        /// </summary>
        private void AddConfirmationDots(List<SmartCandlestick> waveCandlesticks, List<double> fibonacciLevels, double tolerance)
        {
            foreach (var candlestick in waveCandlesticks)
            {
                foreach (var level in fibonacciLevels)
                {
                    // Check for confirmation with tolerance
                    if (Math.Abs(candlestick.Open - level) <= tolerance ||
                        Math.Abs(candlestick.High - level) <= tolerance ||
                        Math.Abs(candlestick.Low - level) <= tolerance ||
                        Math.Abs(candlestick.Close - level) <= tolerance)
                    {
                        var dotAnnotation = new System.Windows.Forms.DataVisualization.Charting.TextAnnotation
                        {
                            Text = "●", // Represent the dot
                            AxisX = chart_candlestick.ChartAreas[0].AxisX,
                            AxisY = chart_candlestick.ChartAreas[0].AxisY,
                            ClipToChartArea = chart_candlestick.ChartAreas[0].Name,
                            X = candlestick.Date.ToOADate(),
                            Y = level,
                            ForeColor = Color.Red,
                            Font = new Font("Arial", 8, FontStyle.Bold)
                        };

                        chart_candlestick.Annotations.Add(dotAnnotation);
                    }
                }
            }
        }

        /// <summary>
        /// Calaculates the beauty
        /// </summary>
        private int CalculateBeauty(List<SmartCandlestick> waveCandlesticks, List<double> fibonacciLevels, double tolerance)
        {
            int beauty = 0;

            foreach (var candlestick in waveCandlesticks)
            {
                foreach (var level in fibonacciLevels)
                {
                    // Check if any OHLC value confirms the level
                    if (Math.Abs(candlestick.Open - level) <= tolerance ||
                        Math.Abs(candlestick.High - level) <= tolerance ||
                        Math.Abs(candlestick.Low - level) <= tolerance ||
                        Math.Abs(candlestick.Close - level) <= tolerance)
                    {
                        beauty++;
                    }
                }
            }

            return beauty;
        }

        /// <summary>
        /// Display the wave beauty
        /// </summary>
        private void DisplayWaveBeauty(List<SmartCandlestick> waveCandlesticks, double lowPrice, double highPrice)
        {
            var fibonacciLevels = CalculateFibonacciLevels(lowPrice, highPrice);
            double tolerance = 0.5; // Adjust tolerance as needed
            int beauty = CalculateBeauty(waveCandlesticks, fibonacciLevels, tolerance);

            // Display the result
            MessageBox.Show($"The Beauty of the selected wave is: {beauty}");
        }


        /// <summary>
        /// Resets the selection
        /// </summary>
        private void ResetSelection()
        {
            firstSelectedCandlestick = null;
            secondSelectedCandlestick = null;
            firstSelectedIndex = -1;
            secondSelectedIndex = -1;
            chart_candlestick.Annotations.Clear(); // Clear annotations
        }

        /// <summary>
        /// Sees if a  candlestick is a peak
        /// </summary>
        private bool IsPeak(int index)
        {
            if (index > 0 && index < candlesticks.Count - 1)
            {
                return candlesticks[index].High > candlesticks[index - 1].High &&
                       candlesticks[index].High > candlesticks[index + 1].High;
            }
            return false;
        }
        /// <summary>
        /// Sees if a candlestick is a valley
        /// </summary>
        private bool IsValley(int index)
        {
            if (index > 0 && index < candlesticks.Count - 1)
            {
                return candlesticks[index].Low < candlesticks[index - 1].Low &&
                       candlesticks[index].Low < candlesticks[index + 1].Low;
            }
            return false;
        }

        /// <summary>
        /// Caculates the Fibonacci levels
        /// </summary>
        private List<double> CalculateFibonacciLevels(double lowPrice, double highPrice)
        {
            // Define Fibonacci percentages
            double[] percentages = { 0, 0.236, 0.382, 0.5, 0.618, 0.764, 1 };

            // Calculate levels based on percentages
            return percentages.Select(p => lowPrice + (p * (highPrice - lowPrice))).ToList();
        }

        /// <summary>
        /// Displays the Fibonacci levels
        /// </summary>
        private void DisplayFibonacciLevels(double lowPrice, double highPrice)
        {
            // Remove any previous Fibonacci annotations
            chart_candlestick.Annotations
                .Where(a => a is HorizontalLineAnnotation || a is TextAnnotation)
                .ToList()
                .ForEach(a => chart_candlestick.Annotations.Remove(a));

            // Calculate Fibonacci levels
            var fibonacciLevels = CalculateFibonacciLevels(lowPrice, highPrice);

            foreach (var level in fibonacciLevels)
            {
                // Add a horizontal line for each Fibonacci level
                var lineAnnotation = new HorizontalLineAnnotation
                {
                    AxisX = chart_candlestick.ChartAreas[0].AxisX,
                    AxisY = chart_candlestick.ChartAreas[0].AxisY,
                    ClipToChartArea = chart_candlestick.ChartAreas[0].Name,
                    LineColor = Color.Blue,
                    LineWidth = 2,
                    AnchorY = level, // Y-coordinate for the Fibonacci level
                    X = firstSelectedCandlestick.Date.ToOADate(), // Constrain within the rectangle
                    Width = secondSelectedCandlestick.Date.ToOADate() - firstSelectedCandlestick.Date.ToOADate()
                };
                chart_candlestick.Annotations.Add(lineAnnotation);

                // Add a text label for each level
                var textAnnotation = new TextAnnotation
                {
                    Text = $"{level:F2}", // Display the level value with 2 decimals
                    AxisX = chart_candlestick.ChartAreas[0].AxisX,
                    AxisY = chart_candlestick.ChartAreas[0].AxisY,
                    ClipToChartArea = chart_candlestick.ChartAreas[0].Name,
                    X = firstSelectedCandlestick.Date.ToOADate(), // Align label to rectangle
                    Y = level,
                    ForeColor = Color.Black,
                    Font = new Font("Arial", 8)
                };
                chart_candlestick.Annotations.Add(textAnnotation);
            }
        }






        /// <summary>
        /// Event handler for the Exit button to close the form.
        /// </summary>
        private void button_exit_Click(object sender, EventArgs e)
        {
            this.Close(); // Close the form
        }

        /// <summary>
        /// Event handler for the Update button to refresh the data based on new date range.
        /// </summary>
        private void button_update_Click(object sender, EventArgs e)
        {
            startDate = datePicker_startDate.Value; // Update start date from date picker
            endDate = datePicker_endDate.Value; // Update end date from date picker
            FileHandler(); // Reload and display data with updated dates
        }

        private void datePicker_startDate_ValueChanged(object sender, EventArgs e)
        {
            // No specific action required for date picker change event
        }

        private void e_Load(object sender, EventArgs e)
        {
            // No specific action required on form load
        }



        private void let(object sender, EventArgs e)
        {

        }

        private void chart_candlestick_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Convert mouse X-coordinate to chart X-axis (date)
                double mouseXValue = chart_candlestick.ChartAreas[0].AxisX.PixelPositionToValue(e.X);
                DateTime clickedDate = DateTime.FromOADate(mouseXValue);

                // Find the nearest candlestick based on the clicked date
                var nearestCandlestick = candlesticks
                    .OrderBy(c => Math.Abs((c.Date - clickedDate).TotalDays))
                    .FirstOrDefault();

                if (nearestCandlestick != null)
                {
                    if (firstSelectedCandlestick == null)
                    {
                        firstSelectedCandlestick = nearestCandlestick;
                        Console.WriteLine($"First Selected: {firstSelectedCandlestick.Date}");
                    }
                    else if (secondSelectedCandlestick == null)
                    {
                        secondSelectedCandlestick = nearestCandlestick;
                        Console.WriteLine($"Second Selected: {secondSelectedCandlestick.Date}");
                        ValidateWave(); // Trigger wave validation after both selections
                    }
                    else
                    {
                        ResetSelection(); // Reset if both candlesticks are already selected
                    }
                }
            }
        }



        private void SelectWave(DateTime startDate, DateTime endDate)
        {
            // Find candlesticks within the selected range
            var selectedCandlesticks = candlesticks
                .Where(c => c.Date >= startDate && c.Date <= endDate)
                .ToList();

            if (selectedCandlesticks.Count >= 2)
            {
                firstSelectedCandlestick = selectedCandlesticks.First();
                secondSelectedCandlestick = selectedCandlesticks.Last();

                // Highlight the selected wave
                AddWaveRectangle(firstSelectedCandlestick.Low, secondSelectedCandlestick.High,
                                 firstSelectedCandlestick.Date, secondSelectedCandlestick.Date);

                // Validate the selected wave and process accordingly
                ValidateWave();
            }
            else
            {
                MessageBox.Show("Invalid wave selection. Please select at least two candlesticks.");
                ResetSelection();
            }
        }

        private void textBox_instructions_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
