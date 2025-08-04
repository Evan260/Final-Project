using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.WinForms;
using SkiaSharp;
using System.Globalization;
using System.Text;

namespace Final_Project
{
    public partial class Reports_Form : Form
    {
        // Properties
        private CartesianChart _revenueChart;
        private PieChart _categoryChart;
        private CartesianChart _customerChart;
        private List<ReportData> _reportData = [];
        private string _currentReportType = "Sales Overview";
        private readonly Random _random = new();

        // Init.
        public Reports_Form()
        {
            InitializeComponent();
            SetupForm();
        }
        private void SetupForm()
        {
            // Setup date range (default last 30 days)
            dtpStartDate.Value = DateTime.Now.AddDays(-30);
            dtpEndDate.Value = DateTime.Now;

            ThemeManager.UseImmersiveDarkMode(Handle, true);

            PopulateReportTypes();
            InitializeCharts();
        }
        private void InitializeCharts()
        {
            // Revenue Trend Chart (Line Chart)
            _revenueChart = new CartesianChart
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(45, 45, 45)
            };

            Panel revenueChartPanel = new() { Dock = DockStyle.Fill };
            revenueChartPanel.Controls.Add(_revenueChart);
            pnlRevenueChart.Controls.Add(revenueChartPanel);

            // Category Distribution Chart (Pie Chart)
            _categoryChart = new PieChart
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(45, 45, 45)
            };

            Panel categoryChartPanel = new() { Dock = DockStyle.Fill };
            categoryChartPanel.Controls.Add(_categoryChart);
            pnlCategoryChart.Controls.Add(categoryChartPanel);

            // Customer Activity Chart (Bar Chart)
            _customerChart = new CartesianChart
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(45, 45, 45)
            };

            Panel customerChartPanel = new() { Dock = DockStyle.Fill };
            customerChartPanel.Controls.Add(_customerChart);
            pnlCustomerChart.Controls.Add(customerChartPanel);
        }

        // Form event handlers
        private void ReportsForm_Load(object sender, EventArgs e)
        {
            try
            {
                LoadReportData();
                UpdateCharts();
                UpdateSummaryStats();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error loading report data", ex.Message);
            }
        }

        // Data Loading
        private void PopulateReportTypes()
        {
            cmbReportType.Items.Clear();
            cmbReportType.Items.Add("Sales Overview");
            cmbReportType.Items.Add("Sales by Date");
            cmbReportType.Items.Add("Sales by Customer");
            cmbReportType.Items.Add("Equipment by Category");
            cmbReportType.Items.Add("Rental Trends");
            cmbReportType.Items.Add("Customer Analysis");
            cmbReportType.SelectedIndex = 0;
        }
        private void LoadReportData()
        {
            // TODO: Replace with actual database queries
            // For now, using sample data
            _reportData = GenerateSampleReportData();
        }
        private List<ReportData> GenerateSampleReportData()
        {
            List<ReportData> data = [];

            // Generate daily data for the date range
            for (DateTime date = dtpStartDate.Value.Date; date <= dtpEndDate.Value.Date; date = date.AddDays(1))
            {
                // Skip some days to simulate realistic business patterns
                if (date.DayOfWeek == DayOfWeek.Sunday) continue;

                int dailyRentals = _random.Next(2, 8);
                decimal dailyRevenue = dailyRentals * (50 + _random.Next(0, 150));

                data.Add(new ReportData
                {
                    Date = date,
                    Revenue = dailyRevenue,
                    RentalCount = dailyRentals,
                    Category = GetRandomCategory(_random),
                    Customer = GetRandomCustomer(_random)
                });
            }

            return data;
        }
        private static string GetRandomCategory(Random random)
        {
            string[] categories = ["Power Tools", "Yard Equipment", "Compressors", "Generators", "Air Tools"];
            return categories[random.Next(categories.Length)];
        }
        private static string GetRandomCustomer(Random random)
        {
            string[] customers = ["John Smith", "Sarah Johnson", "Mike Wilson", "Lisa Brown", "David Davis", "Jennifer Garcia"];
            return customers[random.Next(customers.Length)];
        }

        // Chart Updates
        private void UpdateCharts()
        {
            try
            {
                switch (_currentReportType)
                {
                    case "Sales Overview":
                        UpdateOverviewCharts();
                        break;
                    case "Sales by Date":
                        UpdateSalesByDateChart();
                        break;
                    case "Sales by Customer":
                        UpdateSalesByCustomerChart();
                        break;
                    case "Equipment by Category":
                        UpdateEquipmentByCategoryChart();
                        break;
                    case "Rental Trends":
                        UpdateRentalTrendsChart();
                        break;
                    case "Customer Analysis":
                        UpdateCustomerAnalysisChart();
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error updating charts", ex.Message);
            }
        }
        private void UpdateOverviewCharts()
        {
            if (_revenueChart == null) { return; }

            // Revenue trend chart
            var dailyRevenue = _reportData
                .GroupBy(r => r.Date)
                .Select(g => new { Date = g.Key, Revenue = g.Sum(r => r.Revenue), Count = g.Sum(r => r.RentalCount) })
                .OrderBy(r => r.Date)
                .ToList();

            _revenueChart.Series =
            [
                new LineSeries<decimal>
                {
                    Values = dailyRevenue.Select(r => r.Revenue).ToArray(),
                    Name = "Revenue",
                    Stroke = new SolidColorPaint(SKColors.LimeGreen) { StrokeThickness = 3 },
                    Fill = null,
                    GeometrySize = 8
                }
            ];

            // Category distribution
            var categoryData = _reportData
                .GroupBy(r => r.Category)
                .Select(g => new { Category = g.Key, Count = g.Sum(r => r.RentalCount) })
                .ToList();

            SKColor[] colors = [SKColors.Purple, SKColors.Green, SKColors.Orange, SKColors.Red, SKColors.Blue];

            List<ISeries> pieSeries = [];
            for (int i = 0; i < categoryData.Count; i++)
            {
                pieSeries.Add(new PieSeries<int>
                {
                    Values = [categoryData[i].Count],
                    Name = categoryData[i].Category,
                    Fill = new SolidColorPaint(colors[i % colors.Length])
                });
            }
            _categoryChart.Series = pieSeries;

            // Customer activity
            var customerData = _reportData
                .GroupBy(r => r.Customer)
                .Select(g => new { Customer = g.Key, Revenue = g.Sum(r => r.Revenue) })
                .OrderByDescending(c => c.Revenue)
                .Take(10)
                .ToList();

            _customerChart.Series =
            [
                new ColumnSeries<decimal>
                {
                    Values = customerData.Select(c => c.Revenue).ToArray(),
                    Name = "Customer Revenue",
                    Fill = new SolidColorPaint(SKColors.DodgerBlue)
                }
            ];
        }
        private void UpdateSalesByDateChart()
        {
            var dailyRevenue = _reportData
                .GroupBy(r => r.Date)
                .Select(g => new { Date = g.Key, Revenue = g.Sum(r => r.Revenue), Count = g.Sum(r => r.RentalCount) })
                .OrderBy(r => r.Date)
                .ToList();

            _revenueChart.Series =
            [
                new LineSeries<decimal>
                {
                    Values = dailyRevenue.Select(r => r.Revenue).ToArray(),
                    Name = "Revenue",
                    Stroke = new SolidColorPaint(SKColors.Green) { StrokeThickness = 3 },
                    Fill = null,
                    GeometrySize = 6
                },
                new LineSeries<int>
                {
                    Values = dailyRevenue.Select(r => r.Count).ToArray(),
                    Name = "Rental Count",
                    Stroke = new SolidColorPaint(SKColors.Orange) { StrokeThickness = 3 },
                    Fill = null,
                    GeometrySize = 6
                }
            ];
        }
        private void UpdateSalesByCustomerChart()
        {
            var customerData = _reportData
                .GroupBy(r => r.Customer)
                .Select(g => new { Customer = g.Key, Revenue = g.Sum(r => r.Revenue), Count = g.Sum(r => r.RentalCount) })
                .OrderByDescending(c => c.Revenue)
                .Take(15)
                .ToList();

            _customerChart.Series =
            [
                new ColumnSeries<decimal>
                {
                    Values = customerData.Select(c => c.Revenue).ToArray(),
                    Name = "Revenue by Customer",
                    Fill = new SolidColorPaint(SKColors.SteelBlue)
                }
            ];
        }
        private void UpdateEquipmentByCategoryChart()
        {
            // Equipment distribution and utilization by category
            var categoryData = _reportData
                .GroupBy(r => r.Category)
                .Select(g => new { Category = g.Key, Count = g.Sum(r => r.RentalCount), Revenue = g.Sum(r => r.Revenue) })
                .ToList();

            SKColor[] colors = [SKColors.Purple, SKColors.Green, SKColors.Orange, SKColors.Red, SKColors.Blue];

            List<ISeries> pieSeries = [];
            for (int i = 0; i < categoryData.Count; i++)
            {
                pieSeries.Add(new PieSeries<int>
                {
                    Values = [categoryData[i].Count],
                    Name = categoryData[i].Category,
                    Fill = new SolidColorPaint(colors[i % colors.Length])
                });
            }
            _categoryChart.Series = pieSeries;

            _revenueChart.Series =
            [
                new ColumnSeries<decimal>
                {
                    Values = categoryData.Select(c => c.Revenue).ToArray(),
                    Name = "Revenue by Category",
                    Fill = new SolidColorPaint(SKColors.MediumSeaGreen)
                }
            ];
        }
        private void UpdateRentalTrendsChart()
        {
            // Weekly rental trends
            var weeklyData = _reportData
                .GroupBy(r => GetWeekOfYear(r.Date))
                .Select(g => new { Week = g.Key, Count = g.Sum(r => r.RentalCount), Revenue = g.Sum(r => r.Revenue) })
                .OrderBy(w => w.Week)
                .ToList();

            _revenueChart.Series =
            [
                new ColumnSeries<int>
                {
                    Values = weeklyData.Select(w => w.Count).ToArray(),
                    Name = "Weekly Rentals",
                    Fill = new SolidColorPaint(SKColors.MediumPurple)
                }
            ];
        }
        private void UpdateCustomerAnalysisChart()
        {
            // Customer retention and frequency analysis
            var customerFrequency = _reportData
                .GroupBy(r => r.Customer)
                .Select(g => new { Customer = g.Key, Frequency = g.Count(), TotalSpent = g.Sum(r => r.Revenue) })
                .OrderByDescending(c => c.Frequency)
                .Take(10)
                .ToList();

            _customerChart.Series =
            [
                new ColumnSeries<int>
                {
                    Values = customerFrequency.Select(c => c.Frequency).ToArray(),
                    Name = "Rental Frequency",
                    Fill = new SolidColorPaint(SKColors.Coral)
                }
            ];
        }
        private static int GetWeekOfYear(DateTime date)
        {
            System.Globalization.CultureInfo culture = CultureInfo.CurrentCulture;
            System.Globalization.Calendar calendar = culture.Calendar;
            return calendar.GetWeekOfYear(date, culture.DateTimeFormat.CalendarWeekRule, culture.DateTimeFormat.FirstDayOfWeek);
        }

        // Summary Statistics
        private void UpdateSummaryStats()
        {
            try
            {
                List<ReportData> filteredData = GetFilteredData();

                // Total revenue
                decimal totalRevenue = filteredData.Sum(r => r.Revenue);
                lblTotalRevenueValue.Text = $"${totalRevenue:N0}";

                // Total rentals
                int totalRentals = filteredData.Sum(r => r.RentalCount);
                lblTotalRentalsValue.Text = totalRentals.ToString();

                // Average rental value
                decimal avgRental = totalRentals > 0 ? totalRevenue / totalRentals : 0;
                lblAvgRentalValue.Text = $"${avgRental:F0}";

                // Most popular category
                string popularCategory = filteredData
                    .GroupBy(r => r.Category)
                    .OrderByDescending(g => g.Sum(r => r.RentalCount))
                    .FirstOrDefault()?.Key ?? "None";
                lblPopularCategoryValue.Text = popularCategory;
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error updating summary statistics", ex.Message);
            }
        }
        private List<ReportData> GetFilteredData()
        {
            return _reportData.Where(r => r.Date >= dtpStartDate.Value.Date && r.Date <= dtpEndDate.Value.Date).ToList();
        }

        // Event Handlers
        private void CmbReportType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                _currentReportType = cmbReportType.SelectedItem?.ToString() ?? "Sales Overview";
                UpdateCharts();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error changing report type", ex.Message);
            }
        }
        private void DtpStartDate_ValueChanged(object sender, EventArgs e)
        {
            RefreshData();
        }
        private void DtpEndDate_ValueChanged(object sender, EventArgs e)
        {
            RefreshData();
        }
        private void BtnRefreshData_Click(object sender, EventArgs e)
        {
            RefreshData();
        }
        private void BtnExportData_Click(object sender, EventArgs e)
        {
            try
            {
                ExportReportData();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error exporting data", ex.Message);
            }
        }

        // Helper Methods
        private void RefreshData()
        {
            try
            {
                LoadReportData();
                UpdateCharts();
                UpdateSummaryStats();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error refreshing data", ex.Message);
            }
        }
        private void ExportReportData()
        {
            using SaveFileDialog saveDialog = new()
            {
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                Title = "Export Report Data",
                FileName = $"Report_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
            };

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                StringBuilder csv = new();
                csv.AppendLine("Date,Revenue,RentalCount,Category,Customer");

                foreach (ReportData item in GetFilteredData())
                {
                    csv.AppendLine($"{item.Date:yyyy-MM-dd},{item.Revenue},{item.RentalCount},{item.Category},{item.Customer}");
                }

                File.WriteAllText(saveDialog.FileName, csv.ToString());
                ShowSuccessMessage("Report data exported successfully!");
            }
        }
        private static void ShowErrorMessage(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private static void ShowSuccessMessage(string message)
        {
            MessageBox.Show(message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    public class ReportData
    {
        public DateTime Date { get; set; }
        public decimal Revenue { get; set; }
        public int RentalCount { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Customer { get; set; } = string.Empty;
    }
}