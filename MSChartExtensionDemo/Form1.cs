﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace MSChartExtensionDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            PlotData();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            // Make axes intervals nicer (fewer decimals)
            const string format = "{0.0000}"; // four decimals
            ChartArea a = chart1.ChartAreas[0];
            a.AxisX.LabelStyle.Format = format;
            a.AxisY.LabelStyle.Format = format;
            chart1.EnableZoomAndPanControls(ChartCursorSelected, ChartCursorMoved);
        }
        private void PlotData()
        {
            int DataSizeBase = 10; //Increase this number to plot more points

            Series Ser1 = chart1.Series[0];
            for (int x = 0; x < (10 * DataSizeBase); x++)
                Ser1.Points.AddXY(Math.PI * 0.1 * x, Math.Sin(Math.PI * 0.1 * x));

            Series Ser2 = chart1.Series[1];
            for (int x = 0; x < (5 * DataSizeBase); x++)
                Ser2.Points.AddXY(Math.PI * 0.2 * x, Math.Cos(Math.PI * 0.2 * x));
        }

        private void ClearData()
        {
            foreach (Series ptrSeries in chart1.Series)
                ptrSeries.ClearPoints();
        }

        private void btnPlot_Click(object sender, EventArgs e)
        {
            ClearData();
            StartStopWatch();
            PlotData();
            Application.DoEvents();
            CheckStopWatch("Plot datas");
        }

        private void btnClearDataFast_Click(object sender, EventArgs e)
        {
            StartStopWatch();
            ClearData();
            Application.DoEvents();
            CheckStopWatch("Clear datas");
        }

        private void btnClearDataSlow_Click(object sender, EventArgs e)
        {
            StartStopWatch();
            foreach (Series ptrSeries in chart1.Series)
                ptrSeries.Points.Clear();
            Application.DoEvents();
            CheckStopWatch("Clear datas");
        }

        System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
        private readonly frmInfo _frmInfo = new frmInfo();
        private void StartStopWatch() { watch.Restart(); }
        private void CheckStopWatch(string message)
        {
            watch.Stop();
            MessageBox.Show(message + " took " + watch.ElapsedMilliseconds.ToString() + "ms");
        }

        private void ChartCursorSelected(RectangleF selectionBox)
        {
            double xMin = selectionBox.Left, yMin = selectionBox.Bottom, xRange = selectionBox.Width, yRange = selectionBox.Height;
            txtChartSelect.Text = xMin.ToString("F4") + ", " + yMin.ToString("F4");

            // Display points nearest to selection if user wants
            // If info popup is already open, just update its information
            if (cboxNearestPoint.Checked)
            {
                IDictionary<string, IEnumerable<DataPoint>> nearestPoints = 
                    chart1.NearestPoints(xMin, yMin, epsilonCalculator: EpsilonsFromSeries);
                nearestPoints = chart1.PointsWithin(selectionBox);
                var s = new StringBuilder();
                foreach (var pair in nearestPoints)
                {
                    s.Append(pair.Key);
                    s.Append(": ");
                    s.AppendLine(string.Join(",", pair.Value));
                }

                _frmInfo.Closing -= OnFrmInfoOnClosing;
                _frmInfo.Closing += OnFrmInfoOnClosing;
                _frmInfo.rtboxMain.Text = s.ToString();
                _frmInfo.Show();
            }
        }

        private Tuple<double, double> EpsilonsFromSeries(Series series)
        {
            // Helper to find the range of values for a particular axis' points
            Func<string, double> range = axis =>
                {
                    DataPoint max = series.Points.FindMaxByValue(axis);
                    DataPoint min = series.Points.FindMinByValue(axis);
                    return max.GetValueByName(axis) - min.GetValueByName(axis);
                };

            // In this case (because we know the shape of the data beforehand), 
            //   be more precise in X than in Y
            return new Tuple<double, double>(0.01*range("X"), 0.05*range("Y"));
        }

        private void OnFrmInfoOnClosing(object sender, CancelEventArgs args)
        {
            args.Cancel = true;
            _frmInfo.Hide();
        }

        private void ChartCursorMoved(double x, double y)
        {
            txtChartValue.Text = x.ToString("F4") + ", " + y.ToString("F4");
        }

        private void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text.StartsWith("Item"))
            {
                ToolStripMenuItem ptrMenu = (ToolStripMenuItem) e.ClickedItem;
                if (ptrMenu.HasDropDownItems) return;
                MessageBox.Show(ptrMenu.Text);
            }
        }

        private void item11ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Test");
        }

        private void item12ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Test2");
        }

        private void item13ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Test3");
        }

        private void item14ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Test4");
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            chart1.DrawHorizontalLine(0.5, Color.Green, lineWidth: 3, lineStyle: ChartDashStyle.DashDot);
            chart1.DrawVerticalLine(750, Color.Orange, lineWidth: 3, lineStyle: ChartDashStyle.Dot);
            chart1.DrawRectangle(1000, -0.3, 500, 0.6, Color.Lime, lineWidth: 2);
            chart1.DrawLine(1500, 2000, -1, 1, Color.Pink, lineWidth: 2);
            chart1.AddText("Test chart message", 1000, 0.3, Color.White, textStyle: TextStyle.Shadow);
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            chart1.Annotations.Clear();
        }
    }
}
