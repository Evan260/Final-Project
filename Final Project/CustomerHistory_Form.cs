using Final_Project.Classes;
using System.Data;

namespace Final_Project
{
    public partial class CustomerHistory_Form : Form
    {
        // Properties
        private readonly Customer _currentCustomer;
        private List<Rental> _customerRentals;
        private int _selectedRentalId = -1;

        private enum HistoryColumns
        {
            RentalID,
            RentalDate,
            ReturnDate,
            EquipmentCount,
            TotalCost,
            Status
        }

        // Init.
        public CustomerHistory_Form(Customer customer)
        {
            InitializeComponent();

            _currentCustomer = customer;
            ThemeManager.UseImmersiveDarkMode(Handle, true);
            SetupDataGridView();
            PopulateCustomerInfo();
        }
        private void SetupDataGridView()
        {
            DataGridViewManager.Initialize(dgvHistory);

            dgvHistory.Columns.Add(HistoryColumns.RentalID.ToString(), "Rental ID");
            dgvHistory.Columns.Add(HistoryColumns.RentalDate.ToString(), "Rental Date");
            dgvHistory.Columns.Add(HistoryColumns.ReturnDate.ToString(), "Return Date");
            dgvHistory.Columns.Add(HistoryColumns.EquipmentCount.ToString(), "Equipment Count");
            dgvHistory.Columns.Add(HistoryColumns.TotalCost.ToString(), "Total Cost");
            dgvHistory.Columns.Add(HistoryColumns.Status.ToString(), "Status");

            dgvHistory.Columns[HistoryColumns.RentalID.ToString()].Width = 80;
            dgvHistory.Columns[HistoryColumns.RentalDate.ToString()].Width = 120;
            dgvHistory.Columns[HistoryColumns.ReturnDate.ToString()].Width = 120;
            dgvHistory.Columns[HistoryColumns.EquipmentCount.ToString()].Width = 120;
            dgvHistory.Columns[HistoryColumns.TotalCost.ToString()].Width = 100;
            dgvHistory.Columns[HistoryColumns.Status.ToString()].Width = 100;

            dgvHistory.Columns[HistoryColumns.TotalCost.ToString()].DefaultCellStyle.Format = "C2";
            dgvHistory.Columns[HistoryColumns.RentalDate.ToString()].DefaultCellStyle.Format = "MM/dd/yyyy";
            dgvHistory.Columns[HistoryColumns.ReturnDate.ToString()].DefaultCellStyle.Format = "MM/dd/yyyy";

            dgvHistory.Columns[HistoryColumns.TotalCost.ToString()].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvHistory.Columns[HistoryColumns.EquipmentCount.ToString()].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        // Form event handlers
        private void CustomerHistoryForm_Load(object sender, EventArgs e)
        {
            try
            {
                LoadRentalHistory();
                RefreshGrid();
                UpdateSummaryStats();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error loading customer history", ex.Message);
            }
        }

        // Data Loading
        private void LoadRentalHistory()
        {
            // TODO: Load from database
            // For now, using sample data
            _customerRentals = GenerateSampleRentalHistory();
        }
        private List<Rental> GenerateSampleRentalHistory()
        {
            // Generate sample rental history for the customer
            return
            [
                new() {
                    ID = 1001,
                    CustomerID = _currentCustomer.ID,
                    CustomerName = _currentCustomer.FullName,
                    RentalDate = DateTime.Now.AddDays(-30),
                    ExpectedReturnDate = DateTime.Now.AddDays(-25),
                    ActualReturnDate = DateTime.Now.AddDays(-24),
                    TotalCost = 150.00m,
                    Status = "Returned",
                    Items =
                    [
                        new RentalItem { EquipmentName = "Circular Saw", DailyRate = 25.00m, Days = 5 },
                        new RentalItem { EquipmentName = "Safety Glasses", DailyRate = 5.00m, Days = 5 }
                    ]
                },
                new() {
                    ID = 1015,
                    CustomerID = _currentCustomer.ID,
                    CustomerName = _currentCustomer.FullName,
                    RentalDate = DateTime.Now.AddDays(-15),
                    ExpectedReturnDate = DateTime.Now.AddDays(-10),
                    ActualReturnDate = DateTime.Now.AddDays(-8),
                    TotalCost = 275.00m,
                    Status = "Returned",
                    Items =
                    [
                        new RentalItem { EquipmentName = "Air Compressor", DailyRate = 45.00m, Days = 5 },
                        new RentalItem { EquipmentName = "Nail Gun", DailyRate = 10.00m, Days = 5 }
                    ]
                },
                new() {
                    ID = 1023,
                    CustomerID = _currentCustomer.ID,
                    CustomerName = _currentCustomer.FullName,
                    RentalDate = DateTime.Now.AddDays(-5),
                    ExpectedReturnDate = DateTime.Now.AddDays(2),
                    ActualReturnDate = null,
                    TotalCost = 320.00m,
                    Status = "Active",
                    Items =
                    [
                        new RentalItem { EquipmentName = "Excavator", DailyRate = 80.00m, Days = 7}
                    ]
                }
            ];
        }
        private void RefreshGrid()
        {
            try
            {
                dgvHistory.Rows.Clear();

                foreach (Rental rental in _customerRentals.OrderByDescending(r => r.RentalDate))
                {
                    dgvHistory.Rows.Add(
                        rental.ID,
                        rental.RentalDate,
                        rental.ActualReturnDate ?? rental.ExpectedReturnDate,
                        rental.Items.Count,
                        rental.TotalCost,
                        rental.Status
                    );
                }

                UpdateButtonStates();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error refreshing rental history", ex.Message);
            }
        }

        // Form Population
        private void PopulateCustomerInfo()
        {
            lblCustomerNameValue.Text = _currentCustomer.FullName;
            lblCustomerIdValue.Text = $"#{_currentCustomer.ID}";
            lblCustomerPhoneValue.Text = _currentCustomer.Phone;
            lblCustomerEmailValue.Text = _currentCustomer.Email;
            lblCustomerStatusValue.Text = _currentCustomer.StatusText;

            // Set status color
            lblCustomerStatusValue.ForeColor = _currentCustomer.StatusText switch
            {
                "Active" => Color.FromArgb(40, 199, 111),
                "Banned" => Color.FromArgb(231, 76, 60),
                _ => Color.FromArgb(180, 180, 180),
            };
            if (_currentCustomer.DiscountPercent > 0)
            {
                lblDiscountValue.Text = $"{_currentCustomer.DiscountPercent}%";
                lblDiscountValue.ForeColor = Color.FromArgb(255, 193, 7);
            }
            else
            {
                lblDiscountValue.Text = "None";
                lblDiscountValue.ForeColor = Color.FromArgb(180, 180, 180);
            }
        }
        private void UpdateSummaryStats()
        {
            // Total rentals
            lblTotalRentalsValue.Text = _customerRentals.Count.ToString();

            // Total spent
            decimal totalSpent = _customerRentals.Sum(r => r.TotalCost);
            lblTotalSpentValue.Text = totalSpent.ToString("C2");

            // Active rentals
            int activeRentals = _customerRentals.Count(r => r.Status == "Active" || r.Status == "Overdue");
            lblActiveRentalsValue.Text = activeRentals.ToString();

            // Last rental date
            if (_customerRentals.Count != 0)
            {
                DateTime lastRental = _customerRentals.Max(r => r.RentalDate);
                lblLastRentalValue.Text = lastRental.ToString("MM/dd/yyyy");
            }
            else
            {
                lblLastRentalValue.Text = "Never";
            }
        }

        // Event Handlers
        private void DgvHistory_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (dgvHistory.SelectedRows.Count > 0)
                {
                    DataGridViewRow selectedRow = dgvHistory.SelectedRows[0];
                    _selectedRentalId = Convert.ToInt32(selectedRow.Cells[HistoryColumns.RentalID.ToString()].Value);
                }
                else
                {
                    _selectedRentalId = -1;
                }

                UpdateButtonStates();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error selecting rental", ex.Message);
            }
        }
        private void BtnViewDetails_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedRentalId <= 0) return;

                Rental? selectedRental = _customerRentals.FirstOrDefault(r => r.ID == _selectedRentalId);
                if (selectedRental != null)
                {
                    RentalDetails_Form detailsForm = new(selectedRental);
                    detailsForm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error viewing rental details", ex.Message);
            }
        }
        private void BtnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                // TODO: Implement printing functionality
                ShowInfoMessage("Print functionality will be implemented in a future update.");
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error printing customer history", ex.Message);
            }
        }
        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                LoadRentalHistory();
                RefreshGrid();
                UpdateSummaryStats();
                ShowInfoMessage("Customer history refreshed successfully.");
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error refreshing customer history", ex.Message);
            }
        }

        // Helper Methods
        private void UpdateButtonStates()
        {
            bool hasSelection = _selectedRentalId > 0;
            btnViewDetails.Enabled = hasSelection;
        }
        private static void ShowErrorMessage(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private static void ShowInfoMessage(string message)
        {
            MessageBox.Show(message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}