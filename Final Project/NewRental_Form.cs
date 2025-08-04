using System.Data;

namespace Final_Project
{
    public partial class NewRental_Form : Form
    {
        // Properties
        private Customer _selectedCustomer;
        private List<EquipmentItem> _availableEquipment;
        private List<RentalEquipmentItem> _selectedEquipment;
        private List<Category> _categories;

        private enum AvailableEquipmentColumns
        {
            ID,
            Name,
            Category,
            DailyRate
        }

        private enum SelectedEquipmentColumns
        {
            ID,
            Name,
            DailyRate,
            Days,
            Total
        }

        // Getters
        public Rental CreatedRental { get; private set; }

        // Init.
        public NewRental_Form()
        {
            InitializeComponent();
            SetupForm();
        }
        private void SetupForm()
        {
            // Initialize collections
            _availableEquipment = [];
            _selectedEquipment = [];
            _categories = [];

            // Setup data tables
            SetupDataGridViews();

            // Set default dates
            dtpRentalDate.Value = DateTime.Now;
            dtpReturnDate.Value = DateTime.Now.AddDays(7);

            // Initialize form state
            UpdateFormState();
        }
        private void SetupDataGridViews()
        {
            // Available Equipment
            dgvAvailableEquipment.Columns.Add(AvailableEquipmentColumns.ID.ToString(), "ID");
            dgvAvailableEquipment.Columns.Add(AvailableEquipmentColumns.Name.ToString(), "Name");
            dgvAvailableEquipment.Columns.Add(AvailableEquipmentColumns.Category.ToString(), "Category");
            dgvAvailableEquipment.Columns.Add(AvailableEquipmentColumns.DailyRate.ToString(), "Daily Rate");

            dgvAvailableEquipment.Columns[AvailableEquipmentColumns.ID.ToString()].Width = 50;
            dgvAvailableEquipment.Columns[AvailableEquipmentColumns.Name.ToString()].Width = 180;
            dgvAvailableEquipment.Columns[AvailableEquipmentColumns.Category.ToString()].Width = 120;
            dgvAvailableEquipment.Columns[AvailableEquipmentColumns.DailyRate.ToString()].Width = 80;
            dgvAvailableEquipment.Columns[AvailableEquipmentColumns.DailyRate.ToString()].DefaultCellStyle.Format = "C2";
            dgvAvailableEquipment.Columns[AvailableEquipmentColumns.DailyRate.ToString()].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            // Selected Equipment
            dgvSelectedEquipment.Columns.Add(SelectedEquipmentColumns.ID.ToString(), "ID");
            dgvSelectedEquipment.Columns.Add(SelectedEquipmentColumns.Name.ToString(), "Name");
            dgvSelectedEquipment.Columns.Add(SelectedEquipmentColumns.DailyRate.ToString(), "Daily Rate");
            dgvSelectedEquipment.Columns.Add(SelectedEquipmentColumns.Days.ToString(), "Days");
            dgvSelectedEquipment.Columns.Add(SelectedEquipmentColumns.Total.ToString(), "Total");

            dgvSelectedEquipment.Columns[SelectedEquipmentColumns.ID.ToString()].Width = 40;
            dgvSelectedEquipment.Columns[SelectedEquipmentColumns.Name.ToString()].Width = 160;
            dgvSelectedEquipment.Columns[SelectedEquipmentColumns.DailyRate.ToString()].Width = 80;
            dgvSelectedEquipment.Columns[SelectedEquipmentColumns.Days.ToString()].Width = 50;
            dgvSelectedEquipment.Columns[SelectedEquipmentColumns.Total.ToString()].Width = 80;

            dgvSelectedEquipment.Columns[SelectedEquipmentColumns.DailyRate.ToString()].DefaultCellStyle.Format = "C2";
            dgvSelectedEquipment.Columns[SelectedEquipmentColumns.Total.ToString()].DefaultCellStyle.Format = "C2";
            dgvSelectedEquipment.Columns[SelectedEquipmentColumns.DailyRate.ToString()].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvSelectedEquipment.Columns[SelectedEquipmentColumns.Days.ToString()].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvSelectedEquipment.Columns[SelectedEquipmentColumns.Total.ToString()].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        // Form event handlers
        private void NewRentalForm_Load(object sender, EventArgs e)
        {
            try
            {
                LoadCategories();
                LoadAvailableEquipment();
                RefreshAvailableEquipment();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error loading rental form", ex.Message);
            }
        }

        // Data Loading
        private void LoadCategories()
        {
            // TODO: Load from database
            _categories =
            [
                new Category { ID = 10, Name = "Power Tools" },
                new Category { ID = 20, Name = "Yard Equipment" },
                new Category { ID = 30, Name = "Compressors" },
                new Category { ID = 40, Name = "Generators" },
                new Category { ID = 50, Name = "Air Tools" }
            ];

            cmbEquipmentCategory.Items.Clear();
            cmbEquipmentCategory.Items.Add("All Categories");
            foreach (Category category in _categories)
            {
                cmbEquipmentCategory.Items.Add(category.Name);
            }
            cmbEquipmentCategory.SelectedIndex = 0;
        }
        private void LoadAvailableEquipment()
        {
            // TODO: Load from database - only available equipment
            _availableEquipment =
            [
                new EquipmentItem { ID = 1, Name = "Circular Saw", CategoryID = 10, Category = "Power Tools", DailyRate = 25.00m, Status = "Available" },
                new EquipmentItem { ID = 3, Name = "Air Compressor", CategoryID = 30, Category = "Compressors", DailyRate = 40.00m, Status = "Available" },
                new EquipmentItem { ID = 4, Name = "Generator", CategoryID = 40, Category = "Generators", DailyRate = 60.00m, Status = "Available" },
                new EquipmentItem { ID = 6, Name = "Drill Press", CategoryID = 10, Category = "Power Tools", DailyRate = 30.00m, Status = "Available" },
                new EquipmentItem { ID = 8, Name = "Pressure Washer", CategoryID = 20, Category = "Yard Equipment", DailyRate = 35.00m, Status = "Available" },
                new EquipmentItem { ID = 9, Name = "Jackhammer", CategoryID = 10, Category = "Power Tools", DailyRate = 45.00m, Status = "Available" },
                new EquipmentItem { ID = 10, Name = "Air Nailer", CategoryID = 50, Category = "Air Tools", DailyRate = 20.00m, Status = "Available" }
            ];
        }
        private void RefreshAvailableEquipment()
        {
            try
            {
                dgvAvailableEquipment.Rows.Clear();
                List<EquipmentItem> filteredEquipment = GetFilteredAvailableEquipment();

                foreach (EquipmentItem item in filteredEquipment)
                {
                    dgvAvailableEquipment.Rows.Add(
                        item.ID,
                        item.Name,
                        item.Category,
                        item.DailyRate
                    );
                }

                UpdateFormState();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error refreshing equipment list", ex.Message);
            }
        }
        private List<EquipmentItem> GetFilteredAvailableEquipment()
        {
            IEnumerable<EquipmentItem> filtered = _availableEquipment.Where(e => e.Status == "Available").AsEnumerable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(txtEquipmentSearch.Text))
            {
                string searchTerm = txtEquipmentSearch.Text.ToLower();
                filtered = filtered.Where(e =>
                    e.Name.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase) ||
                    e.Category.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase));
            }

            // Apply category filter
            if (cmbEquipmentCategory.SelectedIndex > 0)
            {
                string selectedCategory = cmbEquipmentCategory.SelectedItem.ToString();
                filtered = filtered.Where(e => e.Category == selectedCategory);
            }

            // Exclude already selected equipment
            List<int> selectedIds = _selectedEquipment.Select(s => s.EquipmentID).ToList();
            filtered = filtered.Where(e => !selectedIds.Contains(e.ID));

            return filtered.ToList();
        }
        private void RefreshSelectedEquipment()
        {
            try
            {
                dgvSelectedEquipment.Rows.Clear();

                foreach (RentalEquipmentItem item in _selectedEquipment)
                {
                    dgvSelectedEquipment.Rows.Add(
                        item.EquipmentID,
                        item.EquipmentName,
                        item.DailyRate,
                        item.RentalDays,
                        item.TotalCost
                    );
                }

                UpdateCostCalculation();
                UpdateFormState();
                RefreshAvailableEquipment(); // Refresh to remove selected items
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error refreshing selected equipment", ex.Message);
            }
        }

        // Customer Selection

        private void BtnSelectCustomer_Click(object sender, EventArgs e)
        {
            try
            {
                CustomerSelection_Form customerSelectionForm = new();
                if (customerSelectionForm.ShowDialog() == DialogResult.OK)
                {
                    _selectedCustomer = customerSelectionForm.SelectedCustomer;
                    UpdateCustomerDisplay();
                    UpdateFormState();
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error selecting customer", ex.Message);
            }
        }
        private void UpdateCustomerDisplay()
        {
            if (_selectedCustomer != null)
            {
                txtSelectedCustomer.Text = $"{_selectedCustomer.FirstName} {_selectedCustomer.LastName}";

                if (_selectedCustomer.IsBanned)
                {
                    lblCustomerStatusValue.Text = "BANNED";
                    lblCustomerStatusValue.ForeColor = Color.FromArgb(255, 86, 86);
                }
                else
                {
                    lblCustomerStatusValue.Text = "Active";
                    lblCustomerStatusValue.ForeColor = Color.FromArgb(40, 199, 111);
                }

                lblCustomerDiscountValue.Text = $"{_selectedCustomer.DiscountPercent}%";

                UpdateCostCalculation();
            }
            else
            {
                txtSelectedCustomer.Text = "";
                lblCustomerStatusValue.Text = "No customer selected";
                lblCustomerStatusValue.ForeColor = Color.FromArgb(180, 180, 180);
                lblCustomerDiscountValue.Text = "0%";
            }
        }

        // Equipment Management
        private void BtnAddToRental_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvAvailableEquipment.SelectedRows.Count == 0) return;

                DataGridViewRow selectedRow = dgvAvailableEquipment.SelectedRows[0];
                int equipmentId = Convert.ToInt32(selectedRow.Cells[AvailableEquipmentColumns.ID.ToString()].Value);

                EquipmentItem? equipment = _availableEquipment.FirstOrDefault(e => e.ID == equipmentId);
                if (equipment != null)
                {
                    RentalEquipmentItem rentalItem = new()
                    {
                        EquipmentID = equipment.ID,
                        EquipmentName = equipment.Name,
                        DailyRate = equipment.DailyRate,
                        RentalDays = (int)numRentalDays.Value
                    };

                    _selectedEquipment.Add(rentalItem);
                    RefreshSelectedEquipment();
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error adding equipment to rental", ex.Message);
            }
        }
        private void BtnRemoveItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvSelectedEquipment.SelectedRows.Count == 0) { return; }

                DataGridViewRow selectedRow = dgvSelectedEquipment.SelectedRows[0];
                int equipmentId = Convert.ToInt32(selectedRow.Cells[AvailableEquipmentColumns.ID.ToString()].Value);

                _selectedEquipment.RemoveAll(s => s.EquipmentID == equipmentId);
                RefreshSelectedEquipment();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error removing equipment from rental", ex.Message);
            }
        }

        // Date and Cost Calculations
        private void NumRentalDays_ValueChanged(object sender, EventArgs e)
        {
            // Update return date based on rental days
            dtpReturnDate.Value = dtpRentalDate.Value.AddDays((double)numRentalDays.Value);

            // Update rental days for all selected equipment
            foreach (RentalEquipmentItem item in _selectedEquipment)
            {
                item.RentalDays = (int)numRentalDays.Value;
            }

            RefreshSelectedEquipment();
        }
        private void DtpRentalDate_ValueChanged(object sender, EventArgs e)
        {
            // Ensure rental date is not in the past
            if (dtpRentalDate.Value.Date < DateTime.Now.Date)
            {
                dtpRentalDate.Value = DateTime.Now.Date;
            }

            // Update return date
            dtpReturnDate.Value = dtpRentalDate.Value.AddDays((double)numRentalDays.Value);
            UpdateFormState();
        }
        private void DtpReturnDate_ValueChanged(object sender, EventArgs e)
        {
            // Calculate rental days based on date difference
            if (dtpReturnDate.Value > dtpRentalDate.Value)
            {
                int days = (dtpReturnDate.Value.Date - dtpRentalDate.Value.Date).Days;
                if (days > 0 && days <= 365)
                {
                    numRentalDays.Value = days;
                }
            }
        }
        private void UpdateCostCalculation()
        {
            try
            {
                decimal subtotal = _selectedEquipment.Sum(s => s.TotalCost);
                decimal discountPercent = _selectedCustomer?.DiscountPercent ?? 0;
                decimal discountAmount = subtotal * (discountPercent / 100);
                decimal totalCost = subtotal - discountAmount;

                lblSubtotalValue.Text = $"${subtotal:F2}";
                lblDiscountAmountValue.Text = $"${discountAmount:F2}";
                lblTotalCostValue.Text = $"${totalCost:F2}";
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error calculating costs", ex.Message);
            }
        }

        // Event Handlers

        private void TxtEquipmentSearch_TextChanged(object sender, EventArgs e)
        {
            RefreshAvailableEquipment();
        }
        private void CmbEquipmentCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshAvailableEquipment();
        }
        private void DgvAvailableEquipment_SelectionChanged(object sender, EventArgs e)
        {
            btnAddRental.Enabled = dgvAvailableEquipment.SelectedRows.Count > 0;
        }
        private void DgvAvailableEquipment_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && btnAddRental.Enabled)
            {
                BtnAddToRental_Click(sender, e);
            }
        }
        private void DgvSelectedEquipment_SelectionChanged(object sender, EventArgs e)
        {
            btnRemoveItem.Enabled = dgvSelectedEquipment.SelectedRows.Count > 0;
        }
        private void BtnCreateRental_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateRentalData()) { return; }

                if (CreateRental())
                {
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error creating rental", ex.Message);
            }
        }
        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        // Helper Methods
        private bool ValidateRentalData()
        {
            // Check if customer is selected
            if (_selectedCustomer == null)
            {
                ShowErrorMessage("Validation Error", "Please select a customer for this rental.");
                return false;
            }

            // Check if customer is banned
            if (_selectedCustomer.IsBanned)
            {
                ShowErrorMessage("Customer Banned",
                    $"{_selectedCustomer.FirstName} {_selectedCustomer.LastName} is currently banned and cannot rent equipment.");
                return false;
            }

            // Check if equipment is selected
            if (_selectedEquipment.Count == 0)
            {
                ShowErrorMessage("Validation Error", "Please select at least one equipment item for this rental.");
                return false;
            }

            // Check rental dates
            if (dtpRentalDate.Value.Date < DateTime.Now.Date)
            {
                ShowErrorMessage("Validation Error", "Rental date cannot be in the past.");
                return false;
            }

            if (dtpReturnDate.Value <= dtpRentalDate.Value)
            {
                ShowErrorMessage("Validation Error", "Return date must be after rental date.");
                return false;
            }

            return true;
        }
        private bool CreateRental()
        {
            try
            {
                // Generate new rental ID
                int rentalId = GenerateNextRentalID();

                // Calculate final costs
                decimal subtotal = _selectedEquipment.Sum(s => s.TotalCost);
                decimal discountAmount = subtotal * (_selectedCustomer.DiscountPercent / 100);
                decimal totalCost = subtotal - discountAmount;

                // Create rental object
                CreatedRental = new Rental
                {
                    ID = rentalId,
                    CustomerID = _selectedCustomer.ID,
                    CustomerName = _selectedCustomer.FullName,
                    RentalDate = dtpRentalDate.Value,
                    ExpectedReturnDate = dtpReturnDate.Value,
                    TotalCost = totalCost,
                    Status = "Active",
                    Items = _selectedEquipment.Select(s => new RentalItem
                    {
                        EquipmentID = s.EquipmentID,
                        EquipmentName = s.EquipmentName,
                        DailyRate = s.DailyRate,
                        Days = s.RentalDays,
                        RentalDate = dtpRentalDate.Value,
                        ExpectedReturnDate = dtpReturnDate.Value
                    }).ToList()
                };

                // TODO: Save to database
                // For now, this will be handled by the calling form

                ShowSuccessMessage($"Rental #{rentalId} created successfully!\n\nCustomer: {_selectedCustomer.FullName}\nTotal Cost: ${totalCost:F2}\nEquipment Items: {_selectedEquipment.Count}");

                return true;
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error creating rental", ex.Message);
                return false;
            }
        }
        private static int GenerateNextRentalID()
        {
            // TODO: Get next ID from database
            // For now, generate a random ID starting from 1000
            Random random = new();
            return random.Next(1000, 9999);
        }
        private void UpdateFormState()
        {
            // Enable Create Rental button only if conditions are met
            btnCreateRental.Enabled = _selectedCustomer != null &&
                                    !_selectedCustomer.IsBanned &&
                                    _selectedEquipment.Count > 0;
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

    public class RentalEquipmentItem
    {
        public int EquipmentID { get; set; }
        public string EquipmentName { get; set; } = string.Empty;
        public decimal DailyRate { get; set; }
        public int RentalDays { get; set; }
        public decimal TotalCost => DailyRate * RentalDays;
    }
}