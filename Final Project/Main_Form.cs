namespace Final_Project
{
    public partial class Main_Form : Form
    {
        // Init.
        public Main_Form()
        {
            InitializeComponent();
            SetupFormStyle();
        }
        private void SetupFormStyle()
        {
            // Set form properties for modern look
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        }
        private void LoadDashboardStatistics()
        {
            try
            {
                // TODO: Replace with actual database calls
                // For now, using placeholder data for the prototype

                // Active Rentals Count
                int activeRentals = GetActiveRentalsCount();
                lblActiveRentalsValue.Text = activeRentals.ToString();

                // Total Equipment Count
                int totalEquipment = GetTotalEquipmentCount();
                lblTotalEquipmentValue.Text = totalEquipment.ToString();

                // Total Customers Count
                int totalCustomers = GetTotalCustomersCount();
                lblTotalCustomersValue.Text = totalCustomers.ToString();

                // Today's Revenue
                decimal todayRevenue = GetTodayRevenue();
                lblTodayRevenueValue.Text = $"${todayRevenue:F0}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading dashboard statistics: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Form event handlers
        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadDashboardStatistics();
        }

        // Event handlers
        private void TileEquipment_Click(object sender, EventArgs e)
        {
            try
            {
                // Open Equipment Management Form
                EquipmentManagement_Form equipmentForm = new();
                equipmentForm.ShowDialog();

                // Refresh dashboard stats when returning
                LoadDashboardStatistics();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error opening Equipment Management", ex.Message);
            }
        }
        private void TileCustomers_Click(object sender, EventArgs e)
        {
            try
            {
                // Open Customer Management Form
                CustomerManagement_Form customerForm = new();
                customerForm.ShowDialog();

                // Refresh dashboard stats when returning
                LoadDashboardStatistics();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error opening Customer Management", ex.Message);
            }
        }
        private void TileRentals_Click(object sender, EventArgs e)
        {
            try
            {
                // Open Rental Management Form
                RentalManagement_Form rentalForm = new();
                rentalForm.ShowDialog();

                // Refresh dashboard stats when returning
                LoadDashboardStatistics();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error opening Rental Management", ex.Message);
            }
        }
        private void TileCategories_Click(object sender, EventArgs e)
        {
            try
            {
                // Open Category Management Form
                CategoryManagement_Form categoryForm = new();
                categoryForm.ShowDialog();

                // Refresh dashboard stats when returning
                LoadDashboardStatistics();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error opening Category Management", ex.Message);
            }
        }
        private void TileReports_Click(object sender, EventArgs e)
        {
            try
            {
                // Open Reports Form
                Reports_Form reportsForm = new();
                reportsForm.ShowDialog();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error opening Reports", ex.Message);
            }
        }

        // Methods
        private int GetActiveRentalsCount()
        {
            // TODO: Implement actual database query
            // SELECT COUNT(*) FROM Rentals WHERE Status = 'Active'
            return 12; // Placeholder
        }
        private int GetTotalEquipmentCount()
        {
            // TODO: Implement actual database query
            // SELECT COUNT(*) FROM Equipment WHERE Status != 'Sold'
            return 85; // Placeholder
        }
        private int GetTotalCustomersCount()
        {
            // TODO: Implement actual database query
            // SELECT COUNT(*) FROM Customers WHERE IsActive = 1
            return 147; // Placeholder
        }
        private decimal GetTodayRevenue()
        {
            // TODO: Implement actual database query
            // SELECT SUM(TotalCost) FROM Rentals WHERE DATE(RentalDate) = DATE('now')
            return 425.00m; // Placeholder
        }
        private void ShowErrorMessage(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}