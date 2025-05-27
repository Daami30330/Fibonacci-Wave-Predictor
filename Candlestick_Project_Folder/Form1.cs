using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Project2COP4365
{
    public partial class e : Form
    {
        private string stockFilePath; // Path to the CSV file for stock data
        private DateTime startDate; // Start date for filtering stock data
        private DateTime endDate; // End date for filtering stock data
        private List<SmartCandlestick> candlesticks = new List<SmartCandlestick>(); // List to store candlestick data

        /// <summary>
        /// Default constructor for the form, initializing UI components.
        /// </summary>
        public e()
        {
            InitializeComponent();
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
            displayVolumeChart(); // Render the volume data as a column plot
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
        private void displayVolumeChart()
        {
            // Clear any existing series and annotations from the volume chart
            chart_volume.Series.Clear();
            chart_volume.Annotations.Clear();

            // Create a new series for the volume chart with column chart type
            var volumeSeries = new System.Windows.Forms.DataVisualization.Charting.Series("Volume")
            {
                ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column,
                XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime,
                Color = Color.Blue
            };

            // Add data points to the series
            foreach (var candle in candlesticks)
            {
                var dataPoint = new System.Windows.Forms.DataVisualization.Charting.DataPoint
                {
                    XValue = candle.Date.ToOADate(), // Use Date as X-axis value
                    YValues = new[] { candle.Volume } // Use Volume as Y-axis value
                };
                volumeSeries.Points.Add(dataPoint); // Add the data point to the series
            }

            // Add the volume series to the chart
            chart_volume.Series.Add(volumeSeries);

            // Calculate minimum and maximum volume for normalization with a buffer
            double minVolume = candlesticks.Min(c => c.Volume);
            double maxVolume = candlesticks.Max(c => c.Volume);
            double buffer = (maxVolume - minVolume) * 0.02;

            // Set the Y-axis range based on volume values with padding for better visualization
            chart_volume.ChartAreas[0].AxisY.Minimum = Math.Max(0, minVolume - buffer);
            chart_volume.ChartAreas[0].AxisY.Maximum = maxVolume + buffer;

            // Set the X-axis formatting to match the candlestick chart
            chart_volume.ChartAreas[0].AxisX.LabelStyle.Format = "MM/yyyy"; // Adjust format as needed
            chart_volume.ChartAreas[0].AxisX.LabelStyle.Angle = -45; // Rotate labels for readability
            chart_volume.ChartAreas[0].AxisX.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Months;
            chart_volume.ChartAreas[0].AxisX.Interval = 5; // Set to 1 month intervals for better alignment

            // Ensure grid and other appearance settings are similar to the candlestick chart
            chart_volume.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0; // Hide X-axis grid lines
            chart_volume.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.Black; // Set Y-axis grid color
            chart_volume.Titles.Clear();
            chart_volume.Titles.Add($"{Path.GetFileNameWithoutExtension(stockFilePath)} Volume Chart");
            chart_volume.Legends.Clear(); // Remove legend to maximize chart space
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
    }
}
