using System.Data;

namespace Final_Project
{
    public partial class CategoryManagement_Form : Form
    {
        // Properties
        private List<Category> _allCategories = [];
        private int _selectedCategoryId = -1;
        private bool _isEditMode = false;

        private enum CategoryColumns
        {
            ID,
            Name,
            Description,
            EquipmentCount
        }


        // Init.
        public CategoryManagement_Form()
        {
            InitializeComponent();

            ThemeManager.UseImmersiveDarkMode(Handle, true);
            SetupDataGridView();
            ClearForm();
        }
        private void SetupDataGridView()
        {
            dgvCategories.Columns.Add(CategoryColumns.ID.ToString(), "ID");
            dgvCategories.Columns.Add(CategoryColumns.Name.ToString(), "Name");
            dgvCategories.Columns.Add(CategoryColumns.Description.ToString(), "Description");
            dgvCategories.Columns.Add(CategoryColumns.EquipmentCount.ToString(), "Equipment Count");

            dgvCategories.Columns[CategoryColumns.ID.ToString()].Width = 60;
            dgvCategories.Columns[CategoryColumns.Name.ToString()].Width = 120;
            dgvCategories.Columns[CategoryColumns.Description.ToString()].Width = 180;
            dgvCategories.Columns[CategoryColumns.EquipmentCount.ToString()].Width = 80;

            dgvCategories.Columns[CategoryColumns.ID.ToString()].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvCategories.Columns[CategoryColumns.EquipmentCount.ToString()].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        // Form event handlers
        private void CategoryManagementForm_Load(object sender, EventArgs e)
        {
            try
            {
                LoadCategoryData();
                RefreshGrid();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error loading category data", ex.Message);
            }
        }

        // Data Loading
        private void LoadCategoryData()
        {
            // TODO: Replace with actual database call
            // For now, using sample data from project requirements
            _allCategories = GetSampleCategoryData();
        }
        private static List<Category> GetSampleCategoryData()
        {
            return
            [
                new Category { ID = 10, Name = "Power Tools", Description = "Electric and battery powered tools", EquipmentCount = 15 },
                new Category { ID = 20, Name = "Yard Equipment", Description = "Lawn mowers, trimmers, and garden tools", EquipmentCount = 12 },
                new Category { ID = 30, Name = "Compressors", Description = "Air compressors and related equipment", EquipmentCount = 8 },
                new Category { ID = 40, Name = "Generators", Description = "Portable and stationary power generators", EquipmentCount = 6 },
                new Category { ID = 50, Name = "Air Tools", Description = "Pneumatic tools and accessories", EquipmentCount = 10 }
            ];
        }
        private void RefreshGrid()
        {
            try
            {
                dgvCategories.Rows.Clear();


                foreach (Category? category in _allCategories.OrderBy(c => c.ID))
                {
                    dgvCategories.Rows.Add(
                        category.ID,
                        category.Name,
                        category.Description,
                        category.EquipmentCount
                    );
                }

                UpdateButtonStates();
                UpdateEquipmentCountLabel();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error refreshing category list", ex.Message);
            }
        }

        // Form Management
        private void ClearForm()
        {
            _isEditMode = false;
            _selectedCategoryId = -1;

            lblFormTitle.Text = "Add Category";
            numCategoryID.Value = GetNextCategoryID();
            numCategoryID.Enabled = true;
            txtCategoryName.Text = "";
            txtDescription.Text = "";

            btnSave.Enabled = false;
            txtCategoryName.Focus();
        }
        private void LoadCategoryToForm(Category category)
        {
            _isEditMode = true;
            _selectedCategoryId = category.ID;

            lblFormTitle.Text = "Edit Category";
            numCategoryID.Value = category.ID;
            numCategoryID.Enabled = false;  // Don't allow ID changes when editing
            txtCategoryName.Text = category.Name;
            txtDescription.Text = category.Description;

            ValidateForm(null, null);
        }
        private int GetNextCategoryID()
        {
            if (_allCategories.Count == 0)
            {
                return 10;
            }

            int maxId = _allCategories.Max(c => c.ID);
            return ((maxId / 10) + 1) * 10;  // Round up to next 10
        }

        // Event Handlers
        private void DgvCategories_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCategories.SelectedRows.Count > 0)
            {
                _selectedCategoryId = Convert.ToInt32(dgvCategories.SelectedRows[0].Cells[CategoryColumns.ID.ToString()].Value);
            }
            else
            {
                _selectedCategoryId = -1;
            }

            UpdateButtonStates();
            UpdateEquipmentCountLabel();
        }
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            ClearForm();
        }
        private void BtnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedCategoryId <= 0) { return; }

                Category? selectedCategory = _allCategories.FirstOrDefault(c => c.ID == _selectedCategoryId);
                if (selectedCategory != null)
                {
                    LoadCategoryToForm(selectedCategory);
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error loading category for editing", ex.Message);
            }
        }
        private void BtnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedCategoryId <= 0)
                {
                    return;
                }

                Category? selectedCategory = _allCategories.FirstOrDefault(c => c.ID == _selectedCategoryId);
                if (selectedCategory != null)
                {
                    // Check if category has equipment
                    if (selectedCategory.EquipmentCount > 0)
                    {
                        ShowErrorMessage("Cannot Delete Category",
                            $"Cannot delete '{selectedCategory.Name}' because it contains {selectedCategory.EquipmentCount} equipment item(s).\n\nRemove or reassign all equipment before deleting this category.");
                        return;
                    }

                    DialogResult result = MessageBox.Show(
                        $"Are you sure you want to delete the category '{selectedCategory.Name}'?\n\nThis action cannot be undone.",
                        "Confirm Delete",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        // TODO: Implement actual database deletion
                        _allCategories.RemoveAll(c => c.ID == _selectedCategoryId);

                        RefreshGrid();
                        ClearForm();
                        ShowSuccessMessage("Category deleted successfully!");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error deleting category", ex.Message);
            }
        }
        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateFormData()) { return; }

                int categoryId = (int)numCategoryID.Value;
                string categoryName = txtCategoryName.Text.Trim();
                string description = txtDescription.Text.Trim();

                if (_isEditMode)
                {
                    // Update existing category
                    Category? existingCategory = _allCategories.FirstOrDefault(c => c.ID == _selectedCategoryId);
                    if (existingCategory != null)
                    {
                        existingCategory.Name = categoryName;
                        existingCategory.Description = description;
                        ShowSuccessMessage("Category updated successfully!");
                    }
                }
                else
                {
                    // Add new category
                    Category newCategory = new()
                    {
                        ID = categoryId,
                        Name = categoryName,
                        Description = description,
                        EquipmentCount = 0
                    };

                    _allCategories.Add(newCategory);
                    ShowSuccessMessage("Category added successfully!");
                }

                RefreshGrid();
                ClearForm();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error saving category", ex.Message);
            }
        }
        private void BtnCancel_Click(object sender, EventArgs e)
        {
            ClearForm();
        }
        private void ValidateForm(object sender, EventArgs e)
        {
            bool isValid = true;

            // Check if category name is provided
            if (string.IsNullOrWhiteSpace(txtCategoryName.Text))
            {
                isValid = false;
            }

            // Check if category ID already exists (for new categories)
            if (!_isEditMode)
            {
                int categoryId = (int)numCategoryID.Value;
                if (_allCategories.Any(c => c.ID == categoryId))
                {
                    isValid = false;
                }
            }

            btnSave.Enabled = isValid;
        }

        // Validation
        private bool ValidateFormData()
        {
            // Validate category name
            if (string.IsNullOrWhiteSpace(txtCategoryName.Text))
            {
                ShowErrorMessage("Validation Error", "Category name is required.");
                txtCategoryName.Focus();
                return false;
            }

            // Check for duplicate category ID (for new categories)
            if (!_isEditMode)
            {
                int categoryId = (int)numCategoryID.Value;
                if (_allCategories.Any(c => c.ID == categoryId))
                {
                    ShowErrorMessage("Validation Error", $"Category ID {categoryId} already exists.");
                    numCategoryID.Focus();
                    return false;
                }
            }

            // Check for duplicate category name
            string categoryName = txtCategoryName.Text.Trim();
            Category? duplicateName = _allCategories.FirstOrDefault(c =>
                c.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase) &&
                c.ID != _selectedCategoryId);

            if (duplicateName != null)
            {
                ShowErrorMessage("Validation Error", $"A category named '{categoryName}' already exists.");
                txtCategoryName.Focus();
                return false;
            }

            return true;
        }

        // Helper Methods
        private void UpdateButtonStates()
        {
            bool hasSelection = _selectedCategoryId > 0;

            btnEdit.Enabled = hasSelection;

            if (hasSelection)
            {
                Category? selectedCategory = _allCategories.FirstOrDefault(c => c.ID == _selectedCategoryId);
                // Only allow deletion if category has no equipment
                btnDelete.Enabled = selectedCategory != null && selectedCategory.EquipmentCount == 0;
            }
            else
            {
                btnDelete.Enabled = false;
            }
        }
        private void UpdateEquipmentCountLabel()
        {
            if (_selectedCategoryId > 0)
            {
                Category? selectedCategory = _allCategories.FirstOrDefault(c => c.ID == _selectedCategoryId);
                if (selectedCategory != null)
                {
                    lblEquipmentCount.Text = $"{selectedCategory.EquipmentCount} equipment item(s)";

                    if (selectedCategory.EquipmentCount > 0)
                    {
                        lblEquipmentCount.ForeColor = Color.FromArgb(255, 193, 7);
                    }
                    else
                    {
                        lblEquipmentCount.ForeColor = Color.FromArgb(180, 180, 180);
                    }
                }
            }
            else
            {
                lblEquipmentCount.Text = "";
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

    public class Category
    {
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int EquipmentCount { get; set; } = 0;
    }
}