using Final_Project.Classes;
using System.Data;

namespace Final_Project
{
    public partial class CustomerSelection_Form : Form
    {
        // Properties
        private List<Customer> _allCustomers = [];
        private int _selectedCustomerId = -1;

        private enum CustomerColumns
        {
            ID,
            Name,
            Phone,
            Email,
            DiscountPercent,
            Status
        }

        // Getters
        public Customer? SelectedCustomer { get; private set; }

        // Init.
        public CustomerSelection_Form()
        {
            InitializeComponent();

            ThemeManager.UseImmersiveDarkMode(Handle, true);
            SetupDataGridView();
            PopulateFilters();
        }
        private void SetupDataGridView()
        {
            DataGridViewManager.Initialize(dgvCustomers);

            dgvCustomers.Columns.Add(CustomerColumns.ID.ToString(), "ID");
            dgvCustomers.Columns.Add(CustomerColumns.Name.ToString(), "Name");
            dgvCustomers.Columns.Add(CustomerColumns.Phone.ToString(), "Phone");
            dgvCustomers.Columns.Add(CustomerColumns.Email.ToString(), "Email");
            dgvCustomers.Columns.Add(CustomerColumns.DiscountPercent.ToString(), "Discount %");
            dgvCustomers.Columns.Add(CustomerColumns.Status.ToString(), "Status");

            dgvCustomers.Columns[CustomerColumns.ID.ToString()].Width = 60;
            dgvCustomers.Columns[CustomerColumns.Name.ToString()].Width = 180;
            dgvCustomers.Columns[CustomerColumns.Phone.ToString()].Width = 140;
            dgvCustomers.Columns[CustomerColumns.Email.ToString()].Width = 200;
            dgvCustomers.Columns[CustomerColumns.DiscountPercent.ToString()].Width = 80;
            dgvCustomers.Columns[CustomerColumns.Status.ToString()].Width = 80;

            dgvCustomers.Columns[CustomerColumns.ID.ToString()].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvCustomers.Columns[CustomerColumns.DiscountPercent.ToString()].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvCustomers.Columns[CustomerColumns.Status.ToString()].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        // Form event handlers
        private void CustomerSelectionForm_Load(object sender, EventArgs e)
        {
            try
            {
                LoadCustomerData();
                RefreshGrid();

                // Set focus to search box
                txtSearch.Focus();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error loading customers", ex.Message);
            }
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Handle Escape key to cancel
            if (keyData == Keys.Escape)
            {
                BtnCancel_Click(this, EventArgs.Empty);
                return true;
            }
            else if (keyData == Keys.Enter && btnSelect.Enabled)
            {
                BtnSelect_Click(this, EventArgs.Empty);
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        // Data Loading
        private void LoadCustomerData()
        {
            // TODO: Load from database
            // For now, using sample data matching CustomerManagement_Form
            _allCustomers =
            [
                new Customer { ID = 1, FirstName = "John", LastName = "Smith", Phone = "(403) 555-0123", Email = "john.smith@email.com", DiscountPercent = 0, IsActive = true, IsBanned = false },
                new Customer { ID = 2, FirstName = "Sarah", LastName = "Johnson", Phone = "(403) 555-0234", Email = "sarah.j@email.com", DiscountPercent = 10, IsActive = true, IsBanned = false },
                new Customer { ID = 3, FirstName = "Mike", LastName = "Wilson", Phone = "(403) 555-0345", Email = "m.wilson@email.com", DiscountPercent = 0, IsActive = true, IsBanned = true },
                new Customer { ID = 4, FirstName = "Lisa", LastName = "Brown", Phone = "(403) 555-0456", Email = "lisa.brown@email.com", DiscountPercent = 5, IsActive = true, IsBanned = false },
                new Customer { ID = 5, FirstName = "David", LastName = "Davis", Phone = "(403) 555-0567", Email = "d.davis@email.com", DiscountPercent = 0, IsActive = true, IsBanned = false },
                new Customer { ID = 6, FirstName = "Jennifer", LastName = "Garcia", Phone = "(403) 555-0678", Email = "j.garcia@email.com", DiscountPercent = 15, IsActive = true, IsBanned = false },
                new Customer { ID = 7, FirstName = "Robert", LastName = "Miller", Phone = "(403) 555-0789", Email = "rob.miller@email.com", DiscountPercent = 0, IsActive = false, IsBanned = true }
            ];
        }
        private void RefreshGrid()
        {
            try
            {
                dgvCustomers.Rows.Clear();

                List<Customer> filteredCustomers = ApplyFilters();

                foreach (Customer customer in filteredCustomers)
                {
                    string status = customer.IsBanned ? "Banned" : (customer.IsActive ? "Active" : "Inactive");

                    dgvCustomers.Rows.Add(
                        customer.ID,
                        customer.FullName,
                        customer.Phone,
                        customer.Email,
                        customer.DiscountPercent,
                        status
                    );
                }

                UpdateButtonStates();
                lblCustomerCount.Text = $"{filteredCustomers.Count} customers found";
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error refreshing customer list", ex.Message);
            }
        }

        // Filtering
        private void PopulateFilters()
        {
            // Status filter
            cmbStatus.Items.Clear();
            cmbStatus.Items.Add("All Customers");
            cmbStatus.Items.Add("Active Only");
            cmbStatus.Items.Add("Exclude Banned");
            cmbStatus.SelectedIndex = 2; // Default to exclude banned customers
        }
        private List<Customer> ApplyFilters()
        {
            IEnumerable<Customer> filtered = _allCustomers.AsEnumerable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                string searchTerm = txtSearch.Text.ToLower();
                filtered = filtered.Where(c =>
                    c.FirstName.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase) ||
                    c.LastName.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase) ||
                    c.Phone.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase) ||
                    c.Email.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase));
            }

            // Apply status filter
            switch (cmbStatus.SelectedIndex)
            {
                case 1: // Active Only
                    filtered = filtered.Where(c => c.IsActive && !c.IsBanned);
                    break;
                case 2: // Exclude Banned
                    filtered = filtered.Where(c => !c.IsBanned);
                    break;
                    // case 0: All Customers - no additional filter
            }

            return filtered.OrderBy(c => c.LastName).ThenBy(c => c.FirstName).ToList();
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
        private void DgvCustomers_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (dgvCustomers.SelectedRows.Count > 0)
                {
                    DataGridViewRow selectedRow = dgvCustomers.SelectedRows[0];
                    _selectedCustomerId = Convert.ToInt32(selectedRow.Cells[CustomerColumns.ID.ToString()].Value);
                }
                else
                {
                    _selectedCustomerId = -1;
                }

                UpdateButtonStates();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error selecting customer", ex.Message);
            }
        }
        private void DgvCustomers_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                BtnSelect_Click(sender, e);
            }
        }
        private void BtnSelect_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedCustomerId <= 0)
                {
                    ShowErrorMessage("Selection Required", "Please select a customer first.");
                    return;
                }

                Customer? selectedCustomer = _allCustomers.FirstOrDefault(c => c.ID == _selectedCustomerId);
                if (selectedCustomer != null)
                {
                    // Check if customer is banned
                    if (selectedCustomer.IsBanned)
                    {
                        DialogResult result = MessageBox.Show(
                            $"{selectedCustomer.FullName} is currently banned.\n\nAre you sure you want to select this customer?",
                            "Customer Banned",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning);

                        if (result == DialogResult.No)
                        {
                            return;
                        }
                    }

                    SelectedCustomer = selectedCustomer;
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error selecting customer", ex.Message);
            }
        }
        private void BtnCancel_Click(object sender, EventArgs e)
        {
            SelectedCustomer = null;
            DialogResult = DialogResult.Cancel;
            Close();
        }
        private void BtnAddNew_Click(object sender, EventArgs e)
        {
            try
            {
                AddEditCustomer_Form addForm = new();
                if (addForm.ShowDialog() == DialogResult.OK)
                {
                    // Refresh the customer list
                    LoadCustomerData();
                    RefreshGrid();

                    // Select the newly added customer if available
                    if (addForm.Customer != null)
                    {
                        _selectedCustomerId = addForm.Customer.ID;

                        // Find and select the row in the grid
                        foreach (DataGridViewRow row in dgvCustomers.Rows)
                        {
                            if (Convert.ToInt32(row.Cells[CustomerColumns.ID.ToString()].Value) == _selectedCustomerId)
                            {
                                row.Selected = true;
                                dgvCustomers.FirstDisplayedScrollingRowIndex = row.Index;
                                break;
                            }
                        }

                        UpdateButtonStates();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error adding new customer", ex.Message);
            }
        }

        // Helper Methods
        private void UpdateButtonStates()
        {
            bool hasSelection = _selectedCustomerId > 0;
            btnSelect.Enabled = hasSelection;

            // Update selection display
            if (hasSelection)
            {
                Customer? selectedCustomer = _allCustomers.FirstOrDefault(c => c.ID == _selectedCustomerId);
                if (selectedCustomer != null)
                {
                    lblSelectedCustomer.Text = $"Selected: {selectedCustomer.FullName}";

                    // Color code based on status
                    if (selectedCustomer.IsBanned)
                    {
                        lblSelectedCustomer.ForeColor = Color.FromArgb(231, 76, 60);  // Red
                    }
                    else if (!selectedCustomer.IsActive)
                    {
                        lblSelectedCustomer.ForeColor = Color.FromArgb(255, 193, 7);  // Yellow
                    }
                    else
                    {
                        lblSelectedCustomer.ForeColor = Color.FromArgb(40, 199, 111);  // Green
                    }
                }
            }
            else
            {
                lblSelectedCustomer.Text = "No customer selected";
                lblSelectedCustomer.ForeColor = Color.FromArgb(180, 180, 180); // Gray
            }
        }
        private static void ShowErrorMessage(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}