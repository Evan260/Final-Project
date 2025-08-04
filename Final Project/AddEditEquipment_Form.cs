namespace Final_Project
{
    public partial class AddEditEquipment_Form : Form
    {
        // Properties
        private readonly bool _isEditMode = false;
        private List<Category> _categories;

        // Getters
        public EquipmentItem Equipment { get; private set; }

        // Constructor for Add mode
        public AddEditEquipment_Form()
        {
            InitializeComponent();
            SetupAddMode();
        }

        // Constructor for Edit mode
        public AddEditEquipment_Form(EquipmentItem equipment)
        {
            InitializeComponent();

            Equipment = equipment;
            _isEditMode = true;
            ThemeManager.UseImmersiveDarkMode(Handle, true);
            SetupEditMode();
        }

        // Init.
        private void SetupAddMode()
        {
            lblTitle.Text = "Add Equipment";
            btnSave.Text = "Add Equipment";

            // Generate next equipment ID
            txtEquipmentID.Text = GenerateNextEquipmentID().ToString();

            // Set default status
            cmbStatus.Visible = false;
            lblStatus.Visible = false;
        }
        private void SetupEditMode()
        {
            lblTitle.Text = "Edit Equipment";
            btnSave.Text = "Update Equipment";

            // Show status field for editing
            cmbStatus.Visible = true;
            lblStatus.Visible = true;
        }

        // Form event handlers
        private void AddEditEquipmentForm_Load(object sender, EventArgs e)
        {
            try
            {
                LoadCategories();
                PopulateStatusDropdown();

                if (_isEditMode && Equipment != null)
                {
                    PopulateFormWithEquipment();
                }

                // Set focus to equipment name
                txtEquipmentName.Focus();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error loading form", ex.Message);
            }
        }
        private void AddEditEquipment_Form_Shown(object sender, EventArgs e)
        {
            ValidateForm(null, null);
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Handle Enter key to save (if form is valid)
            if (keyData == Keys.Enter && btnSave.Enabled)
            {
                BtnSave_Click(this, EventArgs.Empty);
                return true;
            }
            else if (keyData == Keys.Escape)
            {
                BtnCancel_Click(this, EventArgs.Empty);
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        // Data Loading

        private void LoadCategories()
        {
            // TODO: Load from database
            // For now, using hardcoded categories from project requirements
            _categories =
            [
                new() { ID = 10, Name = "Power Tools" },
                new() { ID = 20, Name = "Yard Equipment" },
                new() { ID = 30, Name = "Compressors" },
                new() { ID = 40, Name = "Generators" },
                new() { ID = 50, Name = "Air Tools" }
            ];

            cmbCategory.Items.Clear();
            foreach (Category category in _categories)
            {
                cmbCategory.Items.Add(category);
            }

            cmbCategory.DisplayMember = "Name";
            cmbCategory.ValueMember = "ID";
        }
        private void PopulateStatusDropdown()
        {
            cmbStatus.Items.Clear();
            cmbStatus.Items.Add("Available");
            cmbStatus.Items.Add("Rented");
            cmbStatus.Items.Add("Damaged");
            cmbStatus.Items.Add("Sold");

            if (!_isEditMode)
            {
                cmbStatus.SelectedIndex = 0; // Default to Available
            }
        }
        private void PopulateFormWithEquipment()
        {
            if (Equipment == null) return;

            txtEquipmentID.Text = Equipment.ID.ToString();
            txtEquipmentName.Text = Equipment.Name;
            txtDescription.Text = Equipment.Description;
            numDailyRate.Value = Equipment.DailyRate;

            // Select the appropriate category
            Category? category = _categories.FirstOrDefault(c => c.ID == Equipment.CategoryID);
            if (category != null)
            {
                cmbCategory.SelectedItem = category;
            }

            // Set status
            cmbStatus.SelectedItem = Equipment.Status;
        }

        // Validation
        private void ValidateForm(object sender, EventArgs e)
        {
            errorProvider.Clear();
            bool isValid = true;

            // Validate equipment name
            if (string.IsNullOrWhiteSpace(txtEquipmentName.Text))
            {
                errorProvider.SetError(txtEquipmentName, "Equipment name is required");
                isValid = false;
            }
            else if (txtEquipmentName.Text.Length < 2)
            {
                errorProvider.SetError(txtEquipmentName, "Equipment name must be at least 2 characters");
                isValid = false;
            }

            // Validate category selection
            if (cmbCategory.SelectedItem == null)
            {
                errorProvider.SetError(cmbCategory, "Please select a category");
                isValid = false;
            }

            // Validate description
            if (string.IsNullOrWhiteSpace(txtDescription.Text))
            {
                errorProvider.SetError(txtDescription, "Description is required");
                isValid = false;
            }
            else if (txtDescription.Text.Length < 5)
            {
                errorProvider.SetError(txtDescription, "Description must be at least 5 characters");
                isValid = false;
            }

            // Validate daily rate
            if (numDailyRate.Value <= 0)
            {
                errorProvider.SetError(numDailyRate, "Daily rate must be greater than $0");
                isValid = false;
            }

            // Validate status (only in edit mode)
            if (_isEditMode && cmbStatus.SelectedItem == null)
            {
                errorProvider.SetError(cmbStatus, "Please select a status");
                isValid = false;
            }

            btnSave.Enabled = isValid;
        }

        // Event Handlers
        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateFormData()) { return; }

                if (_isEditMode)
                {
                    UpdateExistingEquipment();
                }
                else
                {
                    CreateNewEquipment();
                }

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error saving equipment", ex.Message);
            }
        }
        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        // Data Operations
        private void CreateNewEquipment()
        {
            Category? selectedCategory = (Category)cmbCategory.SelectedItem;

            EquipmentItem newEquipment = new()
            {
                ID = int.Parse(txtEquipmentID.Text),
                Name = txtEquipmentName.Text.Trim(),
                CategoryID = selectedCategory.ID,
                Category = selectedCategory.Name,
                Description = txtDescription.Text.Trim(),
                DailyRate = numDailyRate.Value,
                Status = "Available"
            };

            // TODO: Save to database
            // For now, this would be handled by the calling form
            Equipment = newEquipment;
        }
        private void UpdateExistingEquipment()
        {
            Category? selectedCategory = (Category)cmbCategory.SelectedItem;

            if (Equipment != null)
            {
                Equipment.Name = txtEquipmentName.Text.Trim();
                Equipment.CategoryID = selectedCategory.ID;
                Equipment.Category = selectedCategory.Name;
                Equipment.Description = txtDescription.Text.Trim();
                Equipment.DailyRate = numDailyRate.Value;
                Equipment.Status = cmbStatus.SelectedItem?.ToString() ?? "Available";
            }

            // TODO: Update in database
            // For now, this would be handled by the calling form
        }
        private bool ValidateFormData()
        {
            // Check equipment name uniqueness
            if (!_isEditMode && EquipmentNameExists(txtEquipmentName.Text.Trim()))
            {
                ShowErrorMessage("Validation Error",
                    "An equipment item with this name already exists. Please choose a different name.");
                txtEquipmentName.Focus();
                return false;
            }

            // Additional business logic validation
            if (_isEditMode && cmbStatus.SelectedItem?.ToString() == "Rented")
            {
                DialogResult result = MessageBox.Show(
                    "This equipment is currently rented. Are you sure you want to save these changes?",
                    "Equipment Currently Rented",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.No)
                {
                    return false;
                }
            }

            return true;
        }
        private bool EquipmentNameExists(string name)
        {
            // TODO: Check database for existing equipment name
            // For now, return false (no duplicates)
            return false;
        }
        private int GenerateNextEquipmentID()
        {
            // TODO: Get next ID from database
            // For now, return a random ID for demonstration
            Random random = new();
            return random.Next(1000, 9999);
        }

        // Methods
        private static void ShowErrorMessage(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}