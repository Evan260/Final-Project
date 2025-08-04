using System.Data;

namespace Final_Project
{
    public partial class EquipmentManagement_Form : Form
    {
        // Properties
        private List<EquipmentItem> _allEquipment = [];
        private int _selectedEquipmentId = -1;

        private enum EquipmentColumns
        {
            ID,
            Name,
            Category,
            Description,
            DailyRate,
            Status
        }


        // Init.
        public EquipmentManagement_Form()
        {
            InitializeComponent();

            ThemeManager.UseImmersiveDarkMode(Handle, true);
            SetupDataGridView();
            PopulateFilters();
        }
        private void SetupDataGridView()
        {
            // Add columns
            dgvEquipment.Columns.Add(EquipmentColumns.ID.ToString(), "ID");
            dgvEquipment.Columns.Add(EquipmentColumns.Name.ToString(), "Name");
            dgvEquipment.Columns.Add(EquipmentColumns.Category.ToString(), "Category");
            dgvEquipment.Columns.Add(EquipmentColumns.Description.ToString(), "Description");
            dgvEquipment.Columns.Add(EquipmentColumns.DailyRate.ToString(), "Daily Rate");
            dgvEquipment.Columns.Add(EquipmentColumns.Status.ToString(), "Status");

            // Set column widths
            dgvEquipment.Columns[EquipmentColumns.ID.ToString()].Width = 60;
            dgvEquipment.Columns[EquipmentColumns.Name.ToString()].Width = 180;
            dgvEquipment.Columns[EquipmentColumns.Category.ToString()].Width = 120;
            dgvEquipment.Columns[EquipmentColumns.Description.ToString()].Width = 250;
            dgvEquipment.Columns[EquipmentColumns.DailyRate.ToString()].Width = 100;
            dgvEquipment.Columns[EquipmentColumns.Status.ToString()].Width = 100;

            // Format currency
            dgvEquipment.Columns[EquipmentColumns.DailyRate.ToString()].DefaultCellStyle.Format = "C2";
            dgvEquipment.Columns[EquipmentColumns.DailyRate.ToString()].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            // Center align certain columns
            dgvEquipment.Columns[EquipmentColumns.ID.ToString()].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvEquipment.Columns[EquipmentColumns.Status.ToString()].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        // Form event handlers
        private void EquipmentManagementForm_Load(object sender, EventArgs e)
        {
            try
            {
                LoadEquipmentData();
                RefreshGrid();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error loading equipment data", ex.Message);
            }
        }

        // Data Loading
        private void LoadEquipmentData()
        {
            // TODO: Replace with actual database call
            // For now, using sample data
            _allEquipment = GetSampleEquipmentData();
        }
        private static List<EquipmentItem> GetSampleEquipmentData()
        {
            return
            [
                new EquipmentItem { ID = 1, Name = "Circular Saw", CategoryID = 10, Category = "Power Tools", Description = "Professional grade circular saw", DailyRate = 25.00m, Status = "Available" },
                new EquipmentItem { ID = 2, Name = "Lawn Mower", CategoryID = 20, Category = "Yard Equipment", Description = "Self-propelled gas mower", DailyRate = 35.00m, Status = "Rented" },
                new EquipmentItem { ID = 3, Name = "Air Compressor", CategoryID = 30, Category = "Compressors", Description = "50 gallon air compressor", DailyRate = 40.00m, Status = "Available" },
                new EquipmentItem { ID = 4, Name = "Generator", CategoryID = 40, Category = "Generators", Description = "5000W portable generator", DailyRate = 60.00m, Status = "Available" },
                new EquipmentItem { ID = 5, Name = "Nail Gun", CategoryID = 50, Category = "Air Tools", Description = "Pneumatic finish nailer", DailyRate = 20.00m, Status = "Damaged" },
                new EquipmentItem { ID = 6, Name = "Drill Press", CategoryID = 10, Category = "Power Tools", Description = "Bench drill press", DailyRate = 30.00m, Status = "Available" },
                new EquipmentItem { ID = 7, Name = "Hedge Trimmer", CategoryID = 20, Category = "Yard Equipment", Description = "Electric hedge trimmer", DailyRate = 18.00m, Status = "Rented" }
            ];
        }
        private void RefreshGrid()
        {
            try
            {
                dgvEquipment.Rows.Clear();
                List<EquipmentItem> filteredEquipment = ApplyFilters();

                foreach (EquipmentItem item in filteredEquipment)
                {
                    dgvEquipment.Rows.Add(
                        item.ID,
                        item.Name,
                        item.Category,
                        item.Description,
                        item.DailyRate,
                        item.Status
                    );
                }

                UpdateButtonStates();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error refreshing equipment list", ex.Message);
            }
        }

        // Filtering
        private void PopulateFilters()
        {
            // Category filter
            cmbCategory.Items.Clear();
            cmbCategory.Items.Add("All Categories");
            cmbCategory.Items.Add("Power Tools");
            cmbCategory.Items.Add("Yard Equipment");
            cmbCategory.Items.Add("Compressors");
            cmbCategory.Items.Add("Generators");
            cmbCategory.Items.Add("Air Tools");
            cmbCategory.SelectedIndex = 0;

            // Status filter
            cmbStatus.Items.Clear();
            cmbStatus.Items.Add("All Status");
            cmbStatus.Items.Add("Available");
            cmbStatus.Items.Add("Rented");
            cmbStatus.Items.Add("Damaged");
            cmbStatus.Items.Add("Sold");
            cmbStatus.SelectedIndex = 0;
        }
        private List<EquipmentItem> ApplyFilters()
        {
            IEnumerable<EquipmentItem> filtered = _allEquipment.AsEnumerable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                string searchTerm = txtSearch.Text.ToLower();
                filtered = filtered.Where(e =>
                    e.Name.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase) ||
                    e.Description.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase) ||
                    e.Category.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase));
            }

            // Apply category filter
            if (cmbCategory.SelectedIndex > 0)
            {
                string selectedCategory = cmbCategory.SelectedItem.ToString();
                filtered = filtered.Where(e => e.Category == selectedCategory);
            }

            // Apply status filter
            if (cmbStatus.SelectedIndex > 0)
            {
                string selectedStatus = cmbStatus.SelectedItem.ToString();
                filtered = filtered.Where(e => e.Status == selectedStatus);
            }

            return filtered.ToList();
        }

        // Event Handlers
        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            RefreshGrid();
        }
        private void CmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshGrid();
        }
        private void CmbStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshGrid();
        }
        private void DgvEquipment_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvEquipment.SelectedRows.Count > 0)
            {
                _selectedEquipmentId = Convert.ToInt32(dgvEquipment.SelectedRows[0].Cells[EquipmentColumns.ID.ToString()].Value);
            }
            else
            {
                _selectedEquipmentId = -1;
            }

            UpdateButtonStates();
        }
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                AddEditEquipment_Form addForm = new();
                if (addForm.ShowDialog() == DialogResult.OK)
                {
                    // Refresh the equipment list
                    LoadEquipmentData();
                    RefreshGrid();
                    ShowSuccessMessage("Equipment added successfully!");
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error adding equipment", ex.Message);
            }
        }
        private void BtnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedEquipmentId <= 0) return;

                EquipmentItem? selectedEquipment = _allEquipment.FirstOrDefault(e => e.ID == _selectedEquipmentId);
                if (selectedEquipment != null)
                {
                    AddEditEquipment_Form editForm = new(selectedEquipment);
                    if (editForm.ShowDialog() == DialogResult.OK)
                    {
                        // Refresh the equipment list
                        LoadEquipmentData();
                        RefreshGrid();
                        ShowSuccessMessage("Equipment updated successfully!");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error editing equipment", ex.Message);
            }
        }
        private void BtnRemove_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedEquipmentId <= 0) { return; }

                EquipmentItem? selectedEquipment = _allEquipment.FirstOrDefault(e => e.ID == _selectedEquipmentId);
                if (selectedEquipment != null)
                {
                    DialogResult result = MessageBox.Show(
                        $"Are you sure you want to remove '{selectedEquipment.Name}' from inventory?\n\nThis action cannot be undone.",
                        "Confirm Removal",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        // TODO: Implement actual database deletion
                        // For now, just remove from list
                        _allEquipment.RemoveAll(e => e.ID == _selectedEquipmentId);

                        RefreshGrid();
                        ShowSuccessMessage("Equipment removed successfully!");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error removing equipment", ex.Message);
            }
        }
        private void BtnMarkDamaged_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedEquipmentId <= 0) return;

                EquipmentItem? selectedEquipment = _allEquipment.FirstOrDefault(e => e.ID == _selectedEquipmentId);
                if (selectedEquipment != null)
                {
                    DialogResult result = MessageBox.Show(
                        $"Mark '{selectedEquipment.Name}' as damaged?\n\nThis will make it unavailable for rental.",
                        "Mark as Damaged",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        // TODO: Implement actual database update
                        selectedEquipment.Status = "Damaged";

                        RefreshGrid();
                        ShowSuccessMessage("Equipment marked as damaged!");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error marking equipment as damaged", ex.Message);
            }
        }

        // Helper Methods
        private void UpdateButtonStates()
        {
            bool hasSelection = _selectedEquipmentId > 0;

            btnEdit.Enabled = hasSelection;
            btnRemove.Enabled = hasSelection;

            // Only allow marking as damaged if equipment is available or rented
            if (hasSelection)
            {
                EquipmentItem? selectedEquipment = _allEquipment.FirstOrDefault(e => e.ID == _selectedEquipmentId);
                btnMarkDamaged.Enabled = selectedEquipment != null &&
                    (selectedEquipment.Status == "Available" || selectedEquipment.Status == "Rented");
            }
            else
            {
                btnMarkDamaged.Enabled = false;
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

    public class EquipmentItem
    {
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CategoryID { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal DailyRate { get; set; }
        public string Status { get; set; } = "Available";  // Available, Rented, Damaged, Sold
    }
}