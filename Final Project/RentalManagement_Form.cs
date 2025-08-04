using System.Data;

namespace Final_Project
{
    public partial class RentalManagement_Form : Form
    {
        // Properties
        private List<Rental> _allRentals = [];
        private int _selectedRentalId = -1;

        private enum RentalColumns
        {
            ID,
            Customer,
            EquipmentCount,
            RentalDate,
            ExpectedReturn,
            ActualReturn,
            TotalCost,
            Status
        }


        // Init.
        public RentalManagement_Form()
        {
            InitializeComponent();
            SetupForm();
        }
        private void SetupForm()
        {
            SetupDataGridView();
            PopulateFilters();

            ThemeManager.UseImmersiveDarkMode(Handle, true);

            // Set default date range (last 30 days)
            dtpStartDate.Value = DateTime.Now.AddDays(-30);
            dtpEndDate.Value = DateTime.Now;
        }
        private void SetupDataGridView()
        {
            dgvRentals.Columns.Add(RentalColumns.ID.ToString(), "ID");
            dgvRentals.Columns.Add(RentalColumns.Customer.ToString(), "Customer");
            dgvRentals.Columns.Add(RentalColumns.EquipmentCount.ToString(), "Equipment Count");
            dgvRentals.Columns.Add(RentalColumns.RentalDate.ToString(), "Rental Date");
            dgvRentals.Columns.Add(RentalColumns.ExpectedReturn.ToString(), "Expected Return");
            dgvRentals.Columns.Add(RentalColumns.ActualReturn.ToString(), "Actual Return");
            dgvRentals.Columns.Add(RentalColumns.TotalCost.ToString(), "Total Cost");
            dgvRentals.Columns.Add(RentalColumns.Status.ToString(), "Status");

            dgvRentals.Columns[RentalColumns.ID.ToString()].Width = 60;
            dgvRentals.Columns[RentalColumns.Customer.ToString()].Width = 180;
            dgvRentals.Columns[RentalColumns.EquipmentCount.ToString()].Width = 100;
            dgvRentals.Columns[RentalColumns.RentalDate.ToString()].Width = 120;
            dgvRentals.Columns[RentalColumns.ExpectedReturn.ToString()].Width = 120;
            dgvRentals.Columns[RentalColumns.ActualReturn.ToString()].Width = 120;
            dgvRentals.Columns[RentalColumns.TotalCost.ToString()].Width = 100;
            dgvRentals.Columns[RentalColumns.Status.ToString()].Width = 100;

            dgvRentals.Columns[RentalColumns.TotalCost.ToString()].DefaultCellStyle.Format = "C2";
            dgvRentals.Columns[RentalColumns.TotalCost.ToString()].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvRentals.Columns[RentalColumns.RentalDate.ToString()].DefaultCellStyle.Format = "MM/dd/yyyy";
            dgvRentals.Columns[RentalColumns.ExpectedReturn.ToString()].DefaultCellStyle.Format = "MM/dd/yyyy";

            dgvRentals.Columns[RentalColumns.ID.ToString()].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvRentals.Columns[RentalColumns.EquipmentCount.ToString()].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvRentals.Columns[RentalColumns.Status.ToString()].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvRentals.CellFormatting += DgvRentals_CellFormatting;
        }

        // Form event handlers
        private void RentalManagementForm_Load(object sender, EventArgs e)
        {
            try
            {
                LoadRentalData();
                RefreshGrid();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error loading rental data", ex.Message);
            }
        }

        // Data Loading
        private void LoadRentalData()
        {
            // TODO: Replace with actual database call
            // For now, using sample data
            _allRentals = GetSampleRentalData();
        }
        private static List<Rental> GetSampleRentalData() => [
            new Rental
            {
                ID = 1001,
                CustomerID = 1,
                CustomerName = "John Smith",
                RentalDate = DateTime.Now.AddDays(-5),
                ExpectedReturnDate = DateTime.Now.AddDays(2),
                TotalCost = 175.00m,
                Status = "Active",
                Items =
                [
                    new() { EquipmentID = 1, EquipmentName = "Circular Saw", DailyRate = 25.00m, Days = 7 },
                    new() { EquipmentID = 3, EquipmentName = "Air Compressor", DailyRate = 40.00m, Days = 7 }
                ]
            },
            new Rental
            {
                ID = 1002,
                CustomerID = 2,
                CustomerName = "Sarah Johnson",
                RentalDate = DateTime.Now.AddDays(-10),
                ExpectedReturnDate = DateTime.Now.AddDays(-3),
                TotalCost = 315.00m,
                Status = "Overdue",
                Items =
                [
                    new() { EquipmentID = 2, EquipmentName = "Lawn Mower", DailyRate = 35.00m, Days = 9 }
                ]
            },
            new Rental
            {
                ID = 1003,
                CustomerID = 4,
                CustomerName = "Lisa Brown",
                RentalDate = DateTime.Now.AddDays(-15),
                ExpectedReturnDate = DateTime.Now.AddDays(-10),
                ActualReturnDate = DateTime.Now.AddDays(-8),
                TotalCost = 342.00m,
                Status = "Returned",
                Items =
                [
                    new RentalItem { EquipmentID = 4, EquipmentName = "Generator", DailyRate = 60.00m, Days = 6 }
                ]
            },
            new Rental
            {
                ID = 1004,
                CustomerID = 5,
                CustomerName = "David Davis",
                RentalDate = DateTime.Now.AddDays(-3),
                ExpectedReturnDate = DateTime.Now.AddDays(4),
                TotalCost = 140.00m,
                Status = "Active",
                Items =
                [
                    new() { EquipmentID = 5, EquipmentName = "Nail Gun", DailyRate = 20.00m, Days = 7 }
                ]
            },
            new Rental
            {
                ID = 1005,
                CustomerID = 6,
                CustomerName = "Jennifer Garcia",
                RentalDate = DateTime.Now.AddDays(-7),
                ExpectedReturnDate = DateTime.Now.AddDays(7),
                TotalCost = 357.00m,
                Status = "Extended",
                Items =
                [
                    new() { EquipmentID = 6, EquipmentName = "Drill Press", DailyRate = 30.00m, Days = 14 },
                    new() { EquipmentID = 7, EquipmentName = "Hedge Trimmer", DailyRate = 18.00m, Days = 7 }
                ]
            }
        ];
        private void RefreshGrid()
        {
            try
            {
                dgvRentals.Rows.Clear();

                List<Rental> filteredRentals = ApplyFilters();

                foreach (Rental rental in filteredRentals)
                {
                    string actualReturnText = rental.ActualReturnDate?.ToString("MM/dd/yyyy") ?? "Not Returned";

                    dgvRentals.Rows.Add(
                        rental.ID,
                        rental.CustomerName,
                        rental.Items.Count,
                        rental.RentalDate,
                        rental.ExpectedReturnDate,
                        actualReturnText,
                        rental.TotalCost,
                        rental.Status
                    );
                }

                UpdateButtonStates();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error refreshing rental list", ex.Message);
            }
        }

        // Filtering
        private void PopulateFilters()
        {
            // Status filter
            cmbStatus.Items.Clear();
            cmbStatus.Items.Add("All Rentals");
            cmbStatus.Items.Add("Active");
            cmbStatus.Items.Add("Overdue");
            cmbStatus.Items.Add("Returned");
            cmbStatus.Items.Add("Extended");
            cmbStatus.SelectedIndex = 0;
        }
        private List<Rental> ApplyFilters()
        {
            IEnumerable<Rental> filtered = _allRentals.AsEnumerable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                string searchTerm = txtSearch.Text.ToLower();
                filtered = filtered.Where(r =>
                    r.ID.ToString().Contains(searchTerm) ||
                    r.CustomerName.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase) ||
                    r.Items.Any(i => i.EquipmentName.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase)));
            }

            // Apply status filter
            if (cmbStatus.SelectedIndex > 0)
            {
                string selectedStatus = cmbStatus.SelectedItem.ToString();
                filtered = filtered.Where(r => r.Status == selectedStatus);
            }

            // Apply date range filter
            filtered = filtered.Where(r =>
                r.RentalDate.Date >= dtpStartDate.Value.Date &&
                r.RentalDate.Date <= dtpEndDate.Value.Date);

            return filtered.OrderByDescending(r => r.RentalDate).ToList();
        }

        // Event Handlers
        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            RefreshGrid();
        }
        private void CmbStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshGrid();
        }
        private void DtpStartDate_ValueChanged(object sender, EventArgs e)
        {
            if (dtpStartDate.Value > dtpEndDate.Value)
            {
                dtpEndDate.Value = dtpStartDate.Value;
            }
            RefreshGrid();
        }
        private void DtpEndDate_ValueChanged(object sender, EventArgs e)
        {
            if (dtpEndDate.Value < dtpStartDate.Value)
            {
                dtpStartDate.Value = dtpEndDate.Value;
            }
            RefreshGrid();
        }
        private void DgvRentals_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvRentals.SelectedRows.Count > 0)
            {
                _selectedRentalId = Convert.ToInt32(dgvRentals.SelectedRows[0].Cells["ID"].Value);
            }
            else
            {
                _selectedRentalId = -1;
            }

            UpdateButtonStates();
        }
        private void DgvRentals_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                BtnViewDetails_Click(sender, e);
            }
        }
        private void BtnNewRental_Click(object sender, EventArgs e)
        {
            try
            {
                NewRental_Form newRentalForm = new();
                if (newRentalForm.ShowDialog() == DialogResult.OK)
                {
                    // Refresh the rental list
                    LoadRentalData();
                    RefreshGrid();
                    ShowSuccessMessage("New rental created successfully!");
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error creating new rental", ex.Message);
            }
        }
        private void BtnExtendRental_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedRentalId <= 0) return;

                Rental? selectedRental = _allRentals.FirstOrDefault(r => r.ID == _selectedRentalId);
                if (selectedRental != null && (selectedRental.Status == "Active" || selectedRental.Status == "Overdue"))
                {
                    ExtendRental_Form extendForm = new(selectedRental);
                    if (extendForm.ShowDialog() == DialogResult.OK)
                    {
                        // Refresh the rental list
                        LoadRentalData();
                        RefreshGrid();
                        ShowSuccessMessage("Rental extended successfully!");
                    }
                }
                else
                {
                    ShowErrorMessage("Invalid Operation", "Only active or overdue rentals can be extended.");
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error extending rental", ex.Message);
            }
        }
        private void BtnProcessReturn_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedRentalId <= 0) { return; }

                Rental? selectedRental = _allRentals.FirstOrDefault(r => r.ID == _selectedRentalId);
                if (selectedRental != null && (selectedRental.Status == "Active" || selectedRental.Status == "Overdue" || selectedRental.Status == "Extended"))
                {
                    DialogResult result = MessageBox.Show(
                        $"Process return for Rental #{selectedRental.ID}?\n\nCustomer: {selectedRental.CustomerName}\nEquipment Count: {selectedRental.Items.Count}",
                        "Confirm Return",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        // TODO: Implement actual database update
                        selectedRental.ActualReturnDate = DateTime.Now;
                        selectedRental.Status = "Returned";

                        // Calculate any late fees if overdue
                        if (selectedRental.ExpectedReturnDate < DateTime.Now.Date)
                        {
                            int overdueDays = (DateTime.Now.Date - selectedRental.ExpectedReturnDate.Date).Days;
                            decimal lateFee = overdueDays * 10.00m; // $10 per day late fee
                            selectedRental.TotalCost += lateFee;

                            ShowSuccessMessage($"Return processed successfully!\nLate fee applied: ${lateFee:F2} ({overdueDays} days overdue)");
                        }
                        else
                        {
                            ShowSuccessMessage("Return processed successfully!");
                        }

                        RefreshGrid();
                    }
                }
                else
                {
                    ShowErrorMessage("Invalid Operation", "Only active, overdue, or extended rentals can be returned.");
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error processing return", ex.Message);
            }
        }
        private void BtnViewDetails_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedRentalId <= 0) { return; }

                Rental? selectedRental = _allRentals.FirstOrDefault(r => r.ID == _selectedRentalId);
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
        private void DgvRentals_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvRentals.Columns[e.ColumnIndex].Name == "Status")
            {
                switch (e.Value?.ToString())
                {
                    case "Active":
                        e.CellStyle.ForeColor = Color.FromArgb(40, 199, 111);
                        e.CellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                        break;
                    case "Overdue":
                        e.CellStyle.ForeColor = Color.FromArgb(255, 86, 86);
                        e.CellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                        break;
                    case "Returned":
                        e.CellStyle.ForeColor = Color.FromArgb(180, 180, 180);
                        e.CellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
                        break;
                    case "Extended":
                        e.CellStyle.ForeColor = Color.FromArgb(255, 193, 7);
                        e.CellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                        break;
                }
            }
            else if (dgvRentals.Columns[e.ColumnIndex].Name == "Expected Return")
            {
                if (DateTime.TryParse(e.Value?.ToString(), out DateTime expectedReturn))
                {
                    Rental? rental = _allRentals.FirstOrDefault(r => r.ID == Convert.ToInt32(dgvRentals.Rows[e.RowIndex].Cells[RentalColumns.ID.ToString()].Value));
                    if (rental != null && rental.Status == "Active" && expectedReturn < DateTime.Now.Date)
                    {
                        e.CellStyle.BackColor = Color.FromArgb(80, 255, 86, 86);
                        e.CellStyle.ForeColor = Color.FromArgb(255, 86, 86);
                        e.CellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                    }
                }
            }
        }

        // Helper Methods
        private void UpdateButtonStates()
        {
            bool hasSelection = _selectedRentalId > 0;

            btnViewDetails.Enabled = hasSelection;

            if (hasSelection)
            {
                Rental? selectedRental = _allRentals.FirstOrDefault(r => r.ID == _selectedRentalId);
                if (selectedRental != null)
                {
                    // Enable/disable buttons based on rental status
                    btnExtendRental.Enabled = selectedRental.Status == "Active" || selectedRental.Status == "Overdue";
                    btnProcessReturn.Enabled = selectedRental.Status == "Active" || selectedRental.Status == "Overdue" || selectedRental.Status == "Extended";
                }
                else
                {
                    btnExtendRental.Enabled = false;
                    btnProcessReturn.Enabled = false;
                }
            }
            else
            {
                btnExtendRental.Enabled = false;
                btnProcessReturn.Enabled = false;
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

    public class Rental
    {
        public int ID { get; set; }
        public int CustomerID { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DateTime RentalDate { get; set; }
        public DateTime ExpectedReturnDate { get; set; }
        public DateTime? ActualReturnDate { get; set; }
        public decimal TotalCost { get; set; }
        public string Status { get; set; } = "Active";  // Active, Overdue, Returned, Extended
        public List<RentalItem> Items { get; set; } = [];
        public bool IsOverdue => Status == "Active" && ExpectedReturnDate < DateTime.Now.Date;
        public int DaysOverdue
        {
            get
            {
                if (!IsOverdue) { return 0; }
                return (DateTime.Now.Date - ExpectedReturnDate.Date).Days;
            }
        }
        public decimal CalculatedLateFee => DaysOverdue * 10.00m;  // $10 per day late fee
    }

    public class RentalItem
    {
        public int EquipmentID { get; set; }
        public string EquipmentName { get; set; } = string.Empty;
        public decimal DailyRate { get; set; }
        public int Days { get; set; }
        public decimal ItemCost => DailyRate * Days;
        public DateTime RentalDate { get; set; }
        public DateTime ExpectedReturnDate { get; set; }
    }
}