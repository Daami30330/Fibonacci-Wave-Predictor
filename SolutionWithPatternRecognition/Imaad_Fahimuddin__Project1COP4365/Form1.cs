using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Project2COP4365
{
    public partial class e : Form
    {
        private string stockFilePath;  // Path to the CSV file for stock data
        private DateTime startDate;    // Start date for filtering stock data
        private DateTime endDate;      // End date for filtering stock data
        private List<Candlestick> candlesticks = new List<Candlestick>(); // List to store candlestick data
        public e()
        {
            InitializeComponent();
            // Optionally, set some default values or display an instruction
            // that the user should load data for the charts to appear.
        }

        public e(string filePath, DateTime start, DateTime end)
        {
            InitializeComponent(); // Initialize the form

            stockFilePath = filePath;
            startDate = start;
            endDate = end;

            LoadStockData(); // Automatically load data when the form is created
        }

        

        // Event handler for the "Load Stock Data" button
        private void button_loadStockData_Click(object sender, EventArgs e)
        {
            // Open file dialog for selecting stock CSV files
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv",
                Multiselect = true // Allow multiple files to be selected
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Get all selected file paths
                string[] filePaths = openFileDialog.FileNames;
                startDate = datePicker_startDate.Value;
                endDate = datePicker_endDate.Value;

                // Load data for each selected file
                foreach (string filePath in filePaths)
                {
                    // Create a new form for each stock file with the specified date range
                    e newForm = new e(filePath, startDate, endDate);
                    newForm.Show(); // Show each form as a separate window
                }
            }
        }

        // Method to load and display the stock data
        private void LoadStockData()
        {
            try
            {
                candlesticks.Clear(); // Clear existing data

                using (StreamReader reader = new StreamReader(stockFilePath))
                {
                    string line;
                    bool isHeader = true;

                    while ((line = reader.ReadLine()) != null)
                    {
                        if (isHeader)
                        {
                            isHeader = false;
                            continue;
                        }

                        string[] values = line.Split(',');
                        DateTime date = DateTime.Parse(values[0]);

                        if (date >= startDate && date <= endDate)
                        {
                            var candlestick = new Candlestick(
                                date,
                                Convert.ToDouble(values[1]),
                                Convert.ToDouble(values[2]),
                                Convert.ToDouble(values[3]),
                                Convert.ToDouble(values[4]),
                                Convert.ToDouble(values[5])
                            );
                            candlesticks.Add(candlestick);
                        }
                    }
                }

                // Set up the candlestick chart
                chart_candlestick.Series.Clear();
                var candlestickSeries = new System.Windows.Forms.DataVisualization.Charting.Series("Candlestick");
                candlestickSeries.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Candlestick;
                candlestickSeries.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.String;
                candlestickSeries["OpenCloseStyle"] = "Triangle";
                candlestickSeries["ShowOpenClose"] = "Both";
                candlestickSeries["PointWidth"] = "0.8";
                candlestickSeries["PriceUpColor"] = "Green";
                candlestickSeries["PriceDownColor"] = "Red";

                foreach (var candle in candlesticks)
                {
                    var dataPoint = new System.Windows.Forms.DataVisualization.Charting.DataPoint();
                    dataPoint.SetValueXY(candle.Date.ToString(), candle.High, candle.Low, candle.Open, candle.Close);
                    candlestickSeries.Points.Add(dataPoint);
                }
                chart_candlestick.Series.Add(candlestickSeries);
                chart_candlestick.ChartAreas[0].AxisX.LabelStyle.Angle = -45;
                chart_candlestick.ChartAreas[0].AxisX.Interval = 5;
                chart_candlestick.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;
                chart_candlestick.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.Black;
                chart_candlestick.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.Black;
                chart_candlestick.Titles.Clear();
                chart_candlestick.Titles.Add($"{Path.GetFileNameWithoutExtension(stockFilePath)} Candlestick Chart");

                // Set up the volume chart
                chart_volume.Series.Clear();
                var volumeSeries = new System.Windows.Forms.DataVisualization.Charting.Series("Volume");
                volumeSeries.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column;
                volumeSeries.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.String;

                foreach (var candle in candlesticks)
                {
                    volumeSeries.Points.AddXY(candle.Date.ToString(), candle.Volume);
                }
                chart_volume.Series.Add(volumeSeries);
                chart_volume.ChartAreas[0].AxisX.LabelStyle.Angle = -45;
                chart_volume.ChartAreas[0].AxisX.Interval = 5;
                chart_volume.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;
                chart_volume.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.Black;
                chart_volume.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.Black;
                chart_volume.Titles.Clear();
                chart_volume.Titles.Add($"{Path.GetFileNameWithoutExtension(stockFilePath)} Volume Chart");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading stock data: " + ex.Message);
            }
        }

        // Event handler for the "Exit" button
        private void button_exit_Click(object sender, EventArgs e) // Exit button
        {
            this.Close(); // Close the form
        }

        private void e_Load(object sender, EventArgs e)
        {
            // Set focus to the button when the form loads
            button_loadStockData.Focus();
        }

        private void button_update_Click(object sender, EventArgs e)
        {
            // Update the date range for the stock data
            startDate = datePicker_startDate.Value;
            endDate = datePicker_endDate.Value;
            LoadStockData();
        }

    }
}
