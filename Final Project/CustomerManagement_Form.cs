using Final_Project.Classes;
using System.Data;

namespace Final_Project
{
    public partial class CustomerManagement_Form : Form
    {
        // Properties
        private List<Customer> _allCustomers = [];
        private int _selectedCustomerId = -1;

        private enum CustomerColumns
        {
            ID,
            FirstName,
            LastName,
            Phone,
            Email,
            DiscountPercent,
            Status
        }

        // Init.
        public CustomerManagement_Form()
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
            dgvCustomers.Columns.Add(CustomerColumns.FirstName.ToString(), "First Name");
            dgvCustomers.Columns.Add(CustomerColumns.LastName.ToString(), "Last Name");
            dgvCustomers.Columns.Add(CustomerColumns.Phone.ToString(), "Phone");
            dgvCustomers.Columns.Add(CustomerColumns.Email.ToString(), "Email");
            dgvCustomers.Columns.Add(CustomerColumns.DiscountPercent.ToString(), "Discount %");
            dgvCustomers.Columns.Add(CustomerColumns.Status.ToString(), "Status");

            dgvCustomers.Columns[CustomerColumns.ID.ToString()].Width = 60;
            dgvCustomers.Columns[CustomerColumns.FirstName.ToString()].Width = 120;
            dgvCustomers.Columns[CustomerColumns.LastName.ToString()].Width = 120;
            dgvCustomers.Columns[CustomerColumns.Phone.ToString()].Width = 130;
            dgvCustomers.Columns[CustomerColumns.Email.ToString()].Width = 200;
            dgvCustomers.Columns[CustomerColumns.DiscountPercent.ToString()].Width = 80;
            dgvCustomers.Columns[CustomerColumns.Status.ToString()].Width = 100;

            dgvCustomers.Columns[CustomerColumns.ID.ToString()].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvCustomers.Columns[CustomerColumns.DiscountPercent.ToString()].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvCustomers.Columns[CustomerColumns.Status.ToString()].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvCustomers.CellFormatting += DgvCustomers_CellFormatting;
        }


        // Form event handlers
        private void CustomerManagementForm_Load(object sender, EventArgs e)
        {
            try
            {
                LoadCustomerData();
                RefreshGrid();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error loading customer data", ex.Message);
            }
        }

        // Event handlers
        private void DgvCustomers_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvCustomers.Columns[e.ColumnIndex].Name == "Status")
            {
                if (e.Value?.ToString() == "Banned")
                {
                    e.CellStyle.ForeColor = Color.FromArgb(255, 86, 86);
                    e.CellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                }
                else if (e.Value?.ToString() == "Active")
                {
                    e.CellStyle.ForeColor = Color.FromArgb(40, 199, 111);
                    e.CellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                }
            }
        }
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
            if (dgvCustomers.SelectedRows.Count > 0)
            {
                _selectedCustomerId = Convert.ToInt32(dgvCustomers.SelectedRows[0].Cells["ID"].Value);
            }
            else
            {
                _selectedCustomerId = -1;
            }

            UpdateButtonStates();
        }
        private void DgvCustomers_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                BtnEdit_Click(sender, e);
            }
        }
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                AddEditCustomer_Form addForm = new();
                if (addForm.ShowDialog() == DialogResult.OK)
                {
                    // Refresh the customer list
                    LoadCustomerData();
                    RefreshGrid();
                    ShowSuccessMessage("Customer added successfully!");
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error adding customer", ex.Message);
            }
        }
        private void BtnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedCustomerId <= 0) return;

                Customer? selectedCustomer = _allCustomers.FirstOrDefault(c => c.ID == _selectedCustomerId);
                if (selectedCustomer != null)
                {
                    AddEditCustomer_Form editForm = new(selectedCustomer);
                    if (editForm.ShowDialog() == DialogResult.OK)
                    {
                        // Refresh the customer list
                        LoadCustomerData();
                        RefreshGrid();
                        ShowSuccessMessage("Customer updated successfully!");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error editing customer", ex.Message);
            }
        }
        private void BtnBan_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedCustomerId <= 0) return;

                Customer? selectedCustomer = _allCustomers.FirstOrDefault(c => c.ID == _selectedCustomerId);
                if (selectedCustomer != null)
                {
                    DialogResult result = MessageBox.Show(
                        $"Are you sure you want to ban {selectedCustomer.FirstName} {selectedCustomer.LastName}?\n\nThey will not be able to rent equipment until unbanned.",
                        "Confirm Ban",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        // TODO: Implement actual database update
                        selectedCustomer.IsBanned = true;

                        RefreshGrid();
                        ShowSuccessMessage($"{selectedCustomer.FirstName} {selectedCustomer.LastName} has been banned!");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error banning customer", ex.Message);
            }
        }
        private void BtnUnban_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedCustomerId <= 0) { return; }

                Customer? selectedCustomer = _allCustomers.FirstOrDefault(c => c.ID == _selectedCustomerId);
                if (selectedCustomer != null)
                {
                    DialogResult result = MessageBox.Show(
                        $"Unban {selectedCustomer.FirstName} {selectedCustomer.LastName}?\n\nThey will be able to rent equipment again.",
                        "Confirm Unban",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        // TODO: Implement actual database update
                        selectedCustomer.IsBanned = false;

                        RefreshGrid();
                        ShowSuccessMessage($"{selectedCustomer.FirstName} {selectedCustomer.LastName} has been unbanned!");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error unbanning customer", ex.Message);
            }
        }
        private void BtnViewHistory_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedCustomerId <= 0) { return; }

                Customer? selectedCustomer = _allCustomers.FirstOrDefault(c => c.ID == _selectedCustomerId);
                if (selectedCustomer != null)
                {
                    CustomerHistory_Form historyForm = new(selectedCustomer);
                    historyForm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error viewing customer history", ex.Message);
            }
        }

        // Data Loading
        private void LoadCustomerData()
        {
            // TODO: Replace with actual database call
            // For now, using sample data
            _allCustomers = GetSampleCustomerData();
        }
        private static List<Customer> GetSampleCustomerData()
        {
            return
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
                    dgvCustomers.Rows.Add(
                        customer.ID,
                        customer.FirstName,
                        customer.LastName,
                        customer.Phone,
                        customer.Email,
                        customer.DiscountPercent,
                        customer.StatusText
                    );
                }

                UpdateButtonStates();
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
            cmbStatus.Items.Add("Active");
            cmbStatus.Items.Add("Banned");
            cmbStatus.Items.Add("Inactive");
            cmbStatus.SelectedIndex = 0;
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
            if (cmbStatus.SelectedIndex > 0)
            {
                string selectedStatus = cmbStatus.SelectedItem.ToString();
                switch (selectedStatus)
                {
                    case "Active":
                        filtered = filtered.Where(c => c.IsActive && !c.IsBanned);
                        break;
                    case "Banned":
                        filtered = filtered.Where(c => c.IsBanned);
                        break;
                    case "Inactive":
                        filtered = filtered.Where(c => !c.IsActive);
                        break;
                }
            }

            return filtered.ToList();
        }

        // Helper Methods
        private void UpdateButtonStates()
        {
            bool hasSelection = _selectedCustomerId > 0;

            btnEdit.Enabled = hasSelection;
            btnViewHistory.Enabled = hasSelection;

            if (hasSelection)
            {
                Customer? selectedCustomer = _allCustomers.FirstOrDefault(c => c.ID == _selectedCustomerId);
                if (selectedCustomer != null)
                {
                    // Show Ban or Unban button based on current status
                    if (selectedCustomer.IsBanned)
                    {
                        btnBan.Visible = false;
                        btnUnban.Visible = true;
                        btnUnban.Enabled = true;
                    }
                    else
                    {
                        btnBan.Visible = true;
                        btnUnban.Visible = false;
                        btnBan.Enabled = true;
                    }
                }
            }
            else
            {
                btnBan.Enabled = false;
                btnUnban.Enabled = false;
                btnBan.Visible = true;
                btnUnban.Visible = false;
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

    public class Customer
    {
        public int ID { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int DiscountPercent { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public bool IsBanned { get; set; } = false;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string FullName => $"{FirstName} {LastName}";
        public string StatusText
        {
            get
            {
                if (IsBanned) { return "Banned"; }
                if (IsActive) { return "Active"; }
                return "Inactive";
            }
        }
    }
}