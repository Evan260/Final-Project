namespace Final_Project
{
    public partial class RentalDetails_Form : Form
    {
        // Properties
        private readonly Rental _currentRental;

        private enum EquipmentColumns
        {
            Equipment,
            DailyRate,
            Days,
            TotalCost
        }

        // Init.
        public RentalDetails_Form(Rental rental)
        {
            InitializeComponent();
            _currentRental = rental;

            SetupEquipmentDataGridView();
            PopulateRentalDetails();
        }
        private void SetupEquipmentDataGridView()
        {
            dgvEquipment.Columns.Add(EquipmentColumns.Equipment.ToString(), "Equipment");
            dgvEquipment.Columns.Add(EquipmentColumns.DailyRate.ToString(), "Daily Rate");
            dgvEquipment.Columns.Add(EquipmentColumns.Days.ToString(), "Days");
            dgvEquipment.Columns.Add(EquipmentColumns.TotalCost.ToString(), "Total Cost");

            dgvEquipment.Columns[EquipmentColumns.Equipment.ToString()].Width = 200;
            dgvEquipment.Columns[EquipmentColumns.DailyRate.ToString()].Width = 100;
            dgvEquipment.Columns[EquipmentColumns.Days.ToString()].Width = 80;
            dgvEquipment.Columns[EquipmentColumns.TotalCost.ToString()].Width = 100;

            dgvEquipment.Columns[EquipmentColumns.DailyRate.ToString()].DefaultCellStyle.Format = "C2";
            dgvEquipment.Columns[EquipmentColumns.TotalCost.ToString()].DefaultCellStyle.Format = "C2";

            dgvEquipment.Columns[EquipmentColumns.DailyRate.ToString()].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvEquipment.Columns[EquipmentColumns.TotalCost.ToString()].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvEquipment.Columns[EquipmentColumns.Days.ToString()].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        // Form event handlers
        private void RentalDetails_Form_Shown(object sender, EventArgs e)
        {
            UpdateActionButtons();
        }

        // Form Population
        private void PopulateRentalDetails()
        {
            try
            {
                PopulateHeaderInfo();
                PopulateCustomerInfo();
                PopulateEquipmentInfo();
                PopulateTimelineInfo();
                PopulateCostInfo();
                PopulateStatusInfo();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error populating rental details", ex.Message);
            }
        }
        private void PopulateHeaderInfo()
        {
            lblRentalIdValue.Text = $"#{_currentRental.ID}";
            lblStatusValue.Text = _currentRental.Status;

            // Set status color
            lblStatusValue.ForeColor = _currentRental.Status switch
            {
                "Active" => Color.FromArgb(40, 199, 111),
                "Overdue" => Color.FromArgb(255, 86, 86),
                "Returned" => Color.FromArgb(180, 180, 180),
                "Extended" => Color.FromArgb(255, 193, 7),
                _ => Color.White,
            };
        }
        private void PopulateCustomerInfo()
        {
            lblCustomerNameValue.Text = _currentRental.CustomerName;
            lblCustomerIdValue.Text = $"#{_currentRental.CustomerID}";

            // TODO: Load additional customer details from database
            // For now, using placeholder information
            lblCustomerPhoneValue.Text = "(403) 555-0123";
            lblCustomerEmailValue.Text = "customer@email.com";
        }
        private void PopulateEquipmentInfo()
        {
            dgvEquipment.Rows.Clear();

            foreach (RentalItem item in _currentRental.Items)
            {
                dgvEquipment.Rows.Add(
                    item.EquipmentName,
                    item.DailyRate,
                    item.Days,
                    item.ItemCost
                );
            }

            lblEquipmentCountValue.Text = _currentRental.Items.Count.ToString();
        }
        private void PopulateTimelineInfo()
        {
            lblRentalDateValue.Text = _currentRental.RentalDate.ToString("MM/dd/yyyy");
            lblExpectedReturnValue.Text = _currentRental.ExpectedReturnDate.ToString("MM/dd/yyyy");

            if (_currentRental.ActualReturnDate.HasValue)
            {
                lblActualReturnValue.Text = _currentRental.ActualReturnDate.Value.ToString("MM/dd/yyyy");
                lblActualReturn.Visible = true;
                lblActualReturnValue.Visible = true;
            }
            else
            {
                lblActualReturn.Visible = false;
                lblActualReturnValue.Visible = false;
            }

            // Calculate rental duration
            DateTime endDate = _currentRental.ActualReturnDate ??
                              (_currentRental.Status == "Active" ? DateTime.Now : _currentRental.ExpectedReturnDate);
            int totalDays = (endDate.Date - _currentRental.RentalDate.Date).Days + 1;
            lblRentalDurationValue.Text = $"{totalDays} day(s)";
        }
        private void PopulateCostInfo()
        {
            decimal subtotal = _currentRental.Items.Sum(item => item.ItemCost);
            lblSubtotalValue.Text = $"${subtotal:F2}";

            // Calculate and display late fees if applicable
            if (_currentRental.IsOverdue && _currentRental.Status != "Returned")
            {
                decimal lateFees = _currentRental.CalculatedLateFee;
                lblLateFeeValue.Text = $"${lateFees:F2}";
                lblLateFee.Visible = true;
                lblLateFeeValue.Visible = true;

                lblTotalCostValue.Text = $"${_currentRental.TotalCost + lateFees:F2}";
            }
            else if (_currentRental.Status == "Returned" && _currentRental.TotalCost > subtotal)
            {
                // Show late fees that were actually charged
                decimal paidLateFees = _currentRental.TotalCost - subtotal;
                lblLateFeeValue.Text = $"${paidLateFees:F2}";
                lblLateFee.Visible = true;
                lblLateFeeValue.Visible = true;

                lblTotalCostValue.Text = $"${_currentRental.TotalCost:F2}";
            }
            else
            {
                lblLateFee.Visible = false;
                lblLateFeeValue.Visible = false;
                lblTotalCostValue.Text = $"${_currentRental.TotalCost:F2}";
            }
        }
        private void PopulateStatusInfo()
        {
            // Clear all status-specific panels
            pnlOverdueInfo.Visible = false;
            pnlReturnedInfo.Visible = false;

            switch (_currentRental.Status)
            {
                case "Overdue":
                    PopulateOverdueInfo();
                    break;
                case "Returned":
                    PopulateReturnedInfo();
                    break;
            }
        }
        private void PopulateOverdueInfo()
        {
            pnlOverdueInfo.Visible = true;

            int overdueDays = _currentRental.DaysOverdue;
            lblOverdueDaysValue.Text = $"{overdueDays} day(s)";

            decimal currentLateFee = _currentRental.CalculatedLateFee;
            lblCurrentLateFeeValue.Text = $"${currentLateFee:F2}";

            // Calculate projected total if returned today
            decimal projectedTotal = _currentRental.TotalCost + currentLateFee;
            lblProjectedTotalValue.Text = $"${projectedTotal:F2}";
        }
        private void PopulateReturnedInfo()
        {
            pnlReturnedInfo.Visible = true;

            if (_currentRental.ActualReturnDate.HasValue)
            {
                bool wasOnTime = _currentRental.ActualReturnDate.Value.Date <= _currentRental.ExpectedReturnDate.Date;
                lblReturnStatusValue.Text = wasOnTime ? "On Time" : "Late Return";
                lblReturnStatusValue.ForeColor = wasOnTime ?
                    Color.FromArgb(40, 199, 111) : Color.FromArgb(255, 86, 86);

                if (!wasOnTime)
                {
                    int lateDays = (_currentRental.ActualReturnDate.Value.Date - _currentRental.ExpectedReturnDate.Date).Days;
                    lblLateDaysValue.Text = $"{lateDays} day(s)";
                    lblLateDays.Visible = true;
                    lblLateDaysValue.Visible = true;
                }
                else
                {
                    lblLateDays.Visible = false;
                    lblLateDaysValue.Visible = false;
                }
            }
        }

        // Event Handlers
        private void BtnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                // TODO: Implement print functionality
                ShowInfoMessage("Print functionality would be implemented here.\n\nThis would generate a detailed rental report including all the information displayed on this form.");
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error printing rental details", ex.Message);
            }
        }
        private void BtnExtendRental_Click(object sender, EventArgs e)
        {
            try
            {
                if (_currentRental.Status == "Active" || _currentRental.Status == "Overdue")
                {
                    ExtendRental_Form extendForm = new(_currentRental);
                    if (extendForm.ShowDialog() == DialogResult.OK)
                    {
                        // Refresh the form with updated rental information
                        PopulateRentalDetails();
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
                if (_currentRental.Status == "Active" || _currentRental.Status == "Overdue" || _currentRental.Status == "Extended")
                {
                    DialogResult result = MessageBox.Show(
                        $"Process return for Rental #{_currentRental.ID}?\n\nThis will mark the rental as returned and apply any applicable late fees.",
                        "Confirm Return",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        // Process the return
                        _currentRental.ActualReturnDate = DateTime.Now;
                        _currentRental.Status = "Returned";

                        // Add late fees if applicable
                        if (_currentRental.ExpectedReturnDate < DateTime.Now.Date)
                        {
                            int overdueDays = (DateTime.Now.Date - _currentRental.ExpectedReturnDate.Date).Days;
                            decimal lateFee = overdueDays * 10.00m;
                            _currentRental.TotalCost += lateFee;
                        }

                        // Refresh the form
                        PopulateRentalDetails();
                        ShowSuccessMessage("Return processed successfully!");
                    }
                }
                else
                {
                    ShowErrorMessage("Invalid Operation", "This rental has already been returned.");
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error processing return", ex.Message);
            }
        }

        // Helper methods
        private static void ShowErrorMessage(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private static void ShowSuccessMessage(string message)
        {
            MessageBox.Show(message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private static void ShowInfoMessage(string message)
        {
            MessageBox.Show(message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void UpdateActionButtons()
        {
            bool canExtend = _currentRental.Status == "Active" || _currentRental.Status == "Overdue";
            bool canReturn = _currentRental.Status == "Active" || _currentRental.Status == "Overdue" || _currentRental.Status == "Extended";

            btnExtendRental.Enabled = canExtend;
            btnProcessReturn.Enabled = canReturn;

            // Hide buttons for returned rentals to clean up the UI
            if (_currentRental.Status == "Returned")
            {
                btnExtendRental.Visible = false;
                btnProcessReturn.Visible = false;
            }
        }
    }
}