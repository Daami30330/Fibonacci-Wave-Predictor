namespace Project2COP4365
{
    partial class e
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.datePicker_startDate = new System.Windows.Forms.DateTimePicker();
            this.datePicker_endDate = new System.Windows.Forms.DateTimePicker();
            this.button_loadStockData = new System.Windows.Forms.Button();
            this.button_exit = new System.Windows.Forms.Button();
            this.textBox_instructions = new System.Windows.Forms.TextBox();
            this.chart_candlestick = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.button_update = new System.Windows.Forms.Button();
            this.checkBox_toggleAnnotations_CheckedChanged = new System.Windows.Forms.CheckBox();
            this.chart_volume = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.chart_candlestick)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart_volume)).BeginInit();
            this.SuspendLayout();
            // 
            // datePicker_startDate
            // 
            this.datePicker_startDate.Location = new System.Drawing.Point(194, 690);
            this.datePicker_startDate.Name = "datePicker_startDate";
            this.datePicker_startDate.Size = new System.Drawing.Size(269, 22);
            this.datePicker_startDate.TabIndex = 0;
            this.datePicker_startDate.Value = new System.DateTime(2022, 1, 1, 14, 13, 0, 0);
            this.datePicker_startDate.ValueChanged += new System.EventHandler(this.datePicker_startDate_ValueChanged);
            // 
            // datePicker_endDate
            // 
            this.datePicker_endDate.Location = new System.Drawing.Point(1159, 687);
            this.datePicker_endDate.Name = "datePicker_endDate";
            this.datePicker_endDate.Size = new System.Drawing.Size(270, 22);
            this.datePicker_endDate.TabIndex = 1;
            // 
            // button_loadStockData
            // 
            this.button_loadStockData.Location = new System.Drawing.Point(826, 690);
            this.button_loadStockData.Name = "button_loadStockData";
            this.button_loadStockData.Size = new System.Drawing.Size(128, 23);
            this.button_loadStockData.TabIndex = 2;
            this.button_loadStockData.Text = "Load Stock Data";
            this.button_loadStockData.UseVisualStyleBackColor = true;
            this.button_loadStockData.Click += new System.EventHandler(this.button_loadStockData_Click);
            // 
            // button_exit
            // 
            this.button_exit.Location = new System.Drawing.Point(26, 692);
            this.button_exit.Name = "button_exit";
            this.button_exit.Size = new System.Drawing.Size(75, 23);
            this.button_exit.TabIndex = 3;
            this.button_exit.Text = "Exit";
            this.button_exit.UseVisualStyleBackColor = true;
            this.button_exit.Click += new System.EventHandler(this.button_exit_Click);
            // 
            // textBox_instructions
            // 
            this.textBox_instructions.Location = new System.Drawing.Point(346, 12);
            this.textBox_instructions.Name = "textBox_instructions";
            this.textBox_instructions.Size = new System.Drawing.Size(877, 22);
            this.textBox_instructions.TabIndex = 5;
            this.textBox_instructions.Text = "Please upload a .csv stock file and the form will show you a visual representatio" +
    "n of the data uploaded. Choose multiple stocks if need be!";
            this.textBox_instructions.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // chart_candlestick
            // 
            chartArea1.Name = "ChartArea1";
            this.chart_candlestick.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chart_candlestick.Legends.Add(legend1);
            this.chart_candlestick.Location = new System.Drawing.Point(12, 40);
            this.chart_candlestick.Name = "chart_candlestick";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chart_candlestick.Series.Add(series1);
            this.chart_candlestick.Size = new System.Drawing.Size(1586, 316);
            this.chart_candlestick.TabIndex = 6;
            this.chart_candlestick.Text = "chart1";
            // 
            // button_update
            // 
            this.button_update.Location = new System.Drawing.Point(612, 692);
            this.button_update.Name = "button_update";
            this.button_update.Size = new System.Drawing.Size(128, 23);
            this.button_update.TabIndex = 8;
            this.button_update.Text = "Update";
            this.button_update.UseVisualStyleBackColor = true;
            this.button_update.Click += new System.EventHandler(this.button_update_Click);
            // 
            // checkBox_toggleAnnotations_CheckedChanged
            // 
            this.checkBox_toggleAnnotations_CheckedChanged.Location = new System.Drawing.Point(0, 0);
            this.checkBox_toggleAnnotations_CheckedChanged.Name = "checkBox_toggleAnnotations_CheckedChanged";
            this.checkBox_toggleAnnotations_CheckedChanged.Size = new System.Drawing.Size(104, 24);
            this.checkBox_toggleAnnotations_CheckedChanged.TabIndex = 0;
            // 
            // chart_volume
            // 
            chartArea2.Name = "ChartArea1";
            this.chart_volume.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.chart_volume.Legends.Add(legend2);
            this.chart_volume.Location = new System.Drawing.Point(12, 371);
            this.chart_volume.Name = "chart_volume";
            series2.ChartArea = "ChartArea1";
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            this.chart_volume.Series.Add(series2);
            this.chart_volume.Size = new System.Drawing.Size(1586, 272);
            this.chart_volume.TabIndex = 9;
            this.chart_volume.Text = "chart1";
            // 
            // e
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkOliveGreen;
            this.ClientSize = new System.Drawing.Size(1610, 727);
            this.Controls.Add(this.chart_volume);
            this.Controls.Add(this.checkBox_toggleAnnotations_CheckedChanged);
            this.Controls.Add(this.button_update);
            this.Controls.Add(this.chart_candlestick);
            this.Controls.Add(this.textBox_instructions);
            this.Controls.Add(this.button_exit);
            this.Controls.Add(this.button_loadStockData);
            this.Controls.Add(this.datePicker_endDate);
            this.Controls.Add(this.datePicker_startDate);
            this.Name = "e";
            this.Text = "5";
            this.Load += new System.EventHandler(this.e_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chart_candlestick)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart_volume)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker datePicker_startDate;
        private System.Windows.Forms.DateTimePicker datePicker_endDate;
        private System.Windows.Forms.Button button_loadStockData;
        private System.Windows.Forms.Button button_exit;
        private System.Windows.Forms.TextBox textBox_instructions;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart_candlestick;
        private System.Windows.Forms.Button button_update;
        private System.Windows.Forms.CheckBox checkBox_toggleAnnotations_CheckedChanged;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart_volume;
    }
}

