using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Final_Project
{
    public partial class AddEditCustomer_Form : Form
    {
        // Properties
        private readonly bool _isEditMode = false;

        // Getters
        public Customer Customer { get; private set; }

        // Constructor for Add mode
        public AddEditCustomer_Form()
        {
            InitializeComponent();
            SetupAddMode();
            ThemeManager.UseImmersiveDarkMode(Handle, true);
        }

        // Constructor for Edit mode
        public AddEditCustomer_Form(Customer customer)
        {
            InitializeComponent();

            Customer = customer;
            _isEditMode = true;
            SetupEditMode();
            ThemeManager.UseImmersiveDarkMode(Handle, true);
        }

        private void SetupAddMode()
        {
            lblTitle.Text = "Add Customer";
            btnSave.Text = "Add Customer";

            // Generate next customer ID
            txtCustomerID.Text = GenerateNextCustomerID().ToString();
        }
        private void SetupEditMode()
        {
            lblTitle.Text = "Edit Customer";
            btnSave.Text = "Update Customer";
        }

        // Form event handlers
        private void AddEditCustomerForm_Load(object sender, EventArgs e)
        {
            try
            {
                PopulateStatusDropdown();

                if (_isEditMode && Customer != null)
                {
                    PopulateFormWithCustomer();
                }

                // Set focus to first name
                txtFirstName.Focus();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error loading form", ex.Message);
            }
        }
        private void AddEditCustomer_Form_Shown(object sender, EventArgs e)
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

        private void PopulateStatusDropdown()
        {
            cmbStatus.Items.Clear();
            cmbStatus.Items.Add("Active");
            cmbStatus.Items.Add("Inactive");

            if (!_isEditMode)
            {
                cmbStatus.SelectedIndex = 0; // Default to Active
            }
        }
        private void PopulateFormWithCustomer()
        {
            if (Customer == null) return;

            txtCustomerID.Text = Customer.ID.ToString();
            txtFirstName.Text = Customer.FirstName;
            txtLastName.Text = Customer.LastName;
            txtPhone.Text = Customer.Phone;
            txtEmail.Text = Customer.Email;
            numDiscountPercent.Value = Customer.DiscountPercent;

            // Set status (exclude banned status as that's handled separately)
            cmbStatus.SelectedItem = Customer.IsActive ? "Active" : "Inactive";
        }

        // Validation
        private void ValidateForm(object sender, EventArgs e)
        {
            errorProvider.Clear();
            bool isValid = true;

            // Validate first name
            if (string.IsNullOrWhiteSpace(txtFirstName.Text))
            {
                errorProvider.SetError(txtFirstName, "First name is required");
                isValid = false;
            }
            else if (txtFirstName.Text.Length < 2)
            {
                errorProvider.SetError(txtFirstName, "First name must be at least 2 characters");
                isValid = false;
            }
            else if (!IsValidName(txtFirstName.Text))
            {
                errorProvider.SetError(txtFirstName, "First name can only contain letters, spaces, hyphens, and apostrophes");
                isValid = false;
            }

            // Validate last name
            if (string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                errorProvider.SetError(txtLastName, "Last name is required");
                isValid = false;
            }
            else if (txtLastName.Text.Length < 2)
            {
                errorProvider.SetError(txtLastName, "Last name must be at least 2 characters");
                isValid = false;
            }
            else if (!IsValidName(txtLastName.Text))
            {
                errorProvider.SetError(txtLastName, "Last name can only contain letters, spaces, hyphens, and apostrophes");
                isValid = false;
            }

            // Validate phone
            if (string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                errorProvider.SetError(txtPhone, "Phone number is required");
                isValid = false;
            }
            else if (!IsValidPhoneNumber(txtPhone.Text))
            {
                errorProvider.SetError(txtPhone, "Please enter a valid phone number (e.g., (403) 555-0123)");
                isValid = false;
            }

            // Validate email
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                errorProvider.SetError(txtEmail, "Email address is required");
                isValid = false;
            }
            else if (!IsValidEmail(txtEmail.Text))
            {
                errorProvider.SetError(txtEmail, "Please enter a valid email address");
                isValid = false;
            }

            // Validate status selection
            if (cmbStatus.SelectedItem == null)
            {
                errorProvider.SetError(cmbStatus, "Please select a status");
                isValid = false;
            }

            // Validate discount percent (already constrained by NumericUpDown)
            if (numDiscountPercent.Value < 0 || numDiscountPercent.Value > 100)
            {
                errorProvider.SetError(numDiscountPercent, "Discount must be between 0% and 100%");
                isValid = false;
            }

            btnSave.Enabled = isValid;
        }
        private static bool IsValidName(string name)
        {
            // Allow letters, spaces, hyphens, and apostrophes
            return NameRegex().IsMatch(name.Trim());
        }
        private static bool IsValidPhoneNumber(string phone)
        {
            // Remove all non-digits
            string digits = PhoneRegex().Replace(phone, "");

            // Check if it's 10 or 11 digits (with or without country code)
            return digits.Length == 10 || digits.Length == 11;
        }
        private static bool IsValidEmail(string email)
        {
            try
            {
                MailAddress addr = new(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        // Event Handlers
        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateFormData()) return;

                if (_isEditMode)
                {
                    UpdateExistingCustomer();
                }
                else
                {
                    CreateNewCustomer();
                }

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error saving customer", ex.Message);
            }
        }
        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
        private void TxtPhone_Leave(object sender, EventArgs e)
        {
            // Auto-format phone number
            FormatPhoneNumber();
        }

        // Customer Operations
        private void CreateNewCustomer()
        {
            Customer newCustomer = new()
            {
                ID = int.Parse(txtCustomerID.Text),
                FirstName = txtFirstName.Text.Trim(),
                LastName = txtLastName.Text.Trim(),
                Phone = txtPhone.Text.Trim(),
                Email = txtEmail.Text.Trim().ToLower(),
                DiscountPercent = (int)numDiscountPercent.Value,
                IsActive = cmbStatus.SelectedItem?.ToString() == "Active",
                IsBanned = false,
                CreatedDate = DateTime.Now
            };

            // TODO: Save to database
            // For now, this would be handled by the calling form
            Customer = newCustomer;
        }
        private void UpdateExistingCustomer()
        {
            if (Customer != null)
            {
                Customer.FirstName = txtFirstName.Text.Trim();
                Customer.LastName = txtLastName.Text.Trim();
                Customer.Phone = txtPhone.Text.Trim();
                Customer.Email = txtEmail.Text.Trim().ToLower();
                Customer.DiscountPercent = (int)numDiscountPercent.Value;
                Customer.IsActive = cmbStatus.SelectedItem?.ToString() == "Active";
                // Note: IsBanned is not modified here as it's handled separately
            }

            // TODO: Update in database
            // For now, this would be handled by the calling form
        }
        private bool ValidateFormData()
        {
            // Check email uniqueness
            if (!_isEditMode && EmailExists(txtEmail.Text.Trim()))
            {
                ShowErrorMessage("Validation Error",
                    "A customer with this email address already exists. Please use a different email address.");
                txtEmail.Focus();
                return false;
            }

            // Check if email changed in edit mode
            if (_isEditMode && Customer != null &&
                !string.Equals(Customer.Email, txtEmail.Text.Trim(), StringComparison.OrdinalIgnoreCase) &&
                EmailExists(txtEmail.Text.Trim()))
            {
                ShowErrorMessage("Validation Error",
                    "A customer with this email address already exists. Please use a different email address.");
                txtEmail.Focus();
                return false;
            }

            return true;
        }
        private bool EmailExists(string email)
        {
            // TODO: Check database for existing email
            // For now, return false (no duplicates)
            return false;
        }
        private int GenerateNextCustomerID()
        {
            // TODO: Get next ID from database
            // For now, return a random ID for demonstration
            Random random = new();
            return random.Next(1000, 9999);
        }

        // Helper Methods

        private void FormatPhoneNumber()
        {
            string phone = txtPhone.Text;
            if (string.IsNullOrWhiteSpace(phone)) return;

            // Remove all non-digits
            string digits = NonDigitsRegex().Replace(phone, "");

            // Format as (XXX) XXX-XXXX if 10 digits
            if (digits.Length == 10)
            {
                txtPhone.Text = $"({digits.Substring(0, 3)}) {digits.Substring(3, 3)}-{digits.Substring(6, 4)}";
            }
            // Format as +1 (XXX) XXX-XXXX if 11 digits and starts with 1
            else if (digits.Length == 11 && digits.StartsWith('1'))
            {
                txtPhone.Text = $"+1 ({digits.Substring(1, 3)}) {digits.Substring(4, 3)}-{digits.Substring(7, 4)}";
            }
        }
        private static void ShowErrorMessage(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        [GeneratedRegex(@"\D")]
        private static partial Regex PhoneRegex();

        [GeneratedRegex(@"^[a-zA-Z\s\-']+$")]
        private static partial Regex NameRegex();

        [GeneratedRegex(@"\D")]
        private static partial Regex NonDigitsRegex();
    }
}