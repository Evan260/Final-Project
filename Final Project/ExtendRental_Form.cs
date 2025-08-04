namespace Final_Project
{
    public partial class ExtendRental_Form : Form
    {
        // Properties
        private readonly Rental _currentRental;
        private int _additionalDays = 0;
        private decimal _additionalCost = 0;

        // Init.
        public ExtendRental_Form(Rental rental)
        {
            InitializeComponent();
            _currentRental = rental;
            SetupForm();
        }
        private void SetupForm()
        {
            // Set default extension days
            numAdditionalDays.Value = 7;
            _additionalDays = 7;

            PopulateRentalInfo();
            CalculateExtensionCost();
        }
        private void PopulateRentalInfo()
        {
            try
            {
                // Basic rental information
                lblRentalIdValue.Text = $"#{_currentRental.ID}";
                lblCustomerNameValue.Text = _currentRental.CustomerName;
                lblRentalDateValue.Text = _currentRental.RentalDate.ToString("MM/dd/yyyy");
                lblCurrentReturnDateValue.Text = _currentRental.ExpectedReturnDate.ToString("MM/dd/yyyy");
                lblCurrentTotalValue.Text = $"${_currentRental.TotalCost:F2}";

                // Check if rental is overdue
                if (_currentRental.IsOverdue)
                {
                    lblOverdueWarning.Visible = true;
                    lblOverdueWarning.Text = $"⚠️ This rental is {_currentRental.DaysOverdue} day(s) overdue!";
                    lblOverdueWarning.ForeColor = Color.FromArgb(255, 86, 86);
                }
                else
                {
                    lblOverdueWarning.Visible = false;
                }

                // Equipment list
                txtEquipmentList.Text = string.Join(Environment.NewLine,
                    _currentRental.Items.Select(item => $"• {item.EquipmentName} - ${item.DailyRate:F2}/day"));

                // Set new return date
                dtpNewReturnDate.Value = _currentRental.ExpectedReturnDate.AddDays(_additionalDays);
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error populating rental information", ex.Message);
            }
        }

        // Form event handlers
        private void ExtendRentalForm_Load(object sender, EventArgs e)
        {
            try
            {
                // Set focus to additional days
                numAdditionalDays.Focus();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error loading extend rental form", ex.Message);
            }
        }
        private void ExtendRental_Form_Shown(object sender, EventArgs e)
        {
            UpdateFormState();
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Handle Enter key to extend (if form is valid)
            if (keyData == Keys.Enter && btnExtend.Enabled)
            {
                BtnExtend_Click(this, EventArgs.Empty);
                return true;
            }
            else if (keyData == Keys.Escape)
            {
                BtnCancel_Click(this, EventArgs.Empty);
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        // Cost Calculations
        private void CalculateExtensionCost()
        {
            try
            {
                // Calculate daily rate for all equipment
                decimal dailyRate = _currentRental.Items.Sum(item => item.DailyRate);

                // Calculate additional cost
                _additionalCost = dailyRate * _additionalDays;

                // Calculate new total
                decimal newTotal = _currentRental.TotalCost + _additionalCost;

                // Add late fees if overdue
                decimal lateFees = 0;
                if (_currentRental.IsOverdue)
                {
                    lateFees = _currentRental.CalculatedLateFee;
                    newTotal += lateFees;
                }

                // Update display
                lblDailyRateValue.Text = $"${dailyRate:F2}";
                lblAdditionalCostValue.Text = $"${_additionalCost:F2}";
                lblLateFeeValue.Text = $"${lateFees:F2}";
                lblNewTotalValue.Text = $"${newTotal:F2}";

                // Show/hide late fee section
                if (lateFees > 0)
                {
                    lblLateFee.Visible = true;
                    lblLateFeeValue.Visible = true;
                }
                else
                {
                    lblLateFee.Visible = false;
                    lblLateFeeValue.Visible = false;
                }

                // Update new return date
                dtpNewReturnDate.Value = _currentRental.ExpectedReturnDate.AddDays(_additionalDays);
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error calculating extension cost", ex.Message);
            }
        }

        // Event Handlers
        private void NumAdditionalDays_ValueChanged(object sender, EventArgs e)
        {
            _additionalDays = (int)numAdditionalDays.Value;
            CalculateExtensionCost();
            UpdateFormState();
        }
        private void DtpNewReturnDate_ValueChanged(object sender, EventArgs e)
        {
            // Calculate additional days based on new return date
            if (dtpNewReturnDate.Value > _currentRental.ExpectedReturnDate)
            {
                int days = (dtpNewReturnDate.Value.Date - _currentRental.ExpectedReturnDate.Date).Days;
                if (days > 0 && days <= 365)
                {
                    numAdditionalDays.Value = days;
                }
            }
        }
        private void BtnExtend_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateExtension()) { return; }

                if (ProcessExtension())
                {
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error extending rental", ex.Message);
            }
        }
        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        // Helper Methods
        private bool ValidateExtension()
        {
            // Check minimum extension days
            if (_additionalDays < 1)
            {
                ShowErrorMessage("Validation Error", "Extension must be at least 1 day.");
                numAdditionalDays.Focus();
                return false;
            }

            // Check maximum extension days
            if (_additionalDays > 365)
            {
                ShowErrorMessage("Validation Error", "Extension cannot exceed 365 days.");
                numAdditionalDays.Focus();
                return false;
            }

            // Check if new return date is not in the past
            if (dtpNewReturnDate.Value.Date <= DateTime.Now.Date)
            {
                ShowErrorMessage("Validation Error", "New return date must be in the future.");
                dtpNewReturnDate.Focus();
                return false;
            }

            return true;
        }
        private bool ProcessExtension()
        {
            try
            {
                // Calculate final costs
                decimal dailyRate = _currentRental.Items.Sum(item => item.DailyRate);
                decimal extensionCost = dailyRate * _additionalDays;
                decimal lateFees = _currentRental.IsOverdue ? _currentRental.CalculatedLateFee : 0;
                decimal totalAdditionalCost = extensionCost + lateFees;

                // Show confirmation dialog
                string confirmationMessage = $"Extend Rental #{_currentRental.ID}?\n\n" +
                    $"Customer: {_currentRental.CustomerName}\n" +
                    $"Additional Days: {_additionalDays}\n" +
                    $"Extension Cost: ${extensionCost:F2}\n";

                if (lateFees > 0)
                {
                    confirmationMessage += $"Late Fees: ${lateFees:F2}\n";
                }

                confirmationMessage += $"Total Additional Cost: ${totalAdditionalCost:F2}\n" +
                    $"New Return Date: {dtpNewReturnDate.Value:MM/dd/yyyy}";

                DialogResult result = MessageBox.Show(
                    confirmationMessage,
                    "Confirm Extension",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // TODO: Implement actual database update
                    // Update rental information
                    _currentRental.ExpectedReturnDate = dtpNewReturnDate.Value;
                    _currentRental.TotalCost += totalAdditionalCost;
                    _currentRental.Status = "Extended";

                    // Update individual rental items
                    foreach (RentalItem item in _currentRental.Items)
                    {
                        item.ExpectedReturnDate = dtpNewReturnDate.Value;
                    }

                    ShowSuccessMessage($"Rental extended successfully!\n\nNew return date: {dtpNewReturnDate.Value:MM/dd/yyyy}\nAdditional cost: ${totalAdditionalCost:F2}");
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error processing extension", ex.Message);
                return false;
            }
        }
        private void UpdateFormState()
        {
            // Enable extend button only if valid extension
            btnExtend.Enabled = _additionalDays > 0 && _additionalDays <= 365;
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
}