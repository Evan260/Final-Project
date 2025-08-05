using Guna.UI2.WinForms;

namespace Final_Project.Classes
{
    internal class DataGridViewManager
    {
        // Properties
        private static readonly byte _rowHeight = 35, _columnHeaderHeight = 60;

        public static void Initialize(Guna2DataGridView dataGridView)
        {
            dataGridView.ReadOnly = true;
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToResizeRows = false;
            dataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridView.ColumnHeadersHeight = _columnHeaderHeight;
            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridView.RowTemplate.Height = _rowHeight;
            dataGridView.RowTemplate.DefaultCellStyle.Padding = new Padding(10, 0, 0, 0);
            dataGridView.ColumnHeadersDefaultCellStyle.Padding = new Padding(10, 0, 0, 0);
            dataGridView.DefaultCellStyle.Font = new Font("Segoe UI", 12);
            dataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = CustomColors.Text;
            dataGridView.Theme = CustomColors.DataGridViewTheme;
            dataGridView.BackgroundColor = CustomColors.ControlBack;
            dataGridView.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridView.CellBorderStyle = DataGridViewCellBorderStyle.None;
            dataGridView.ScrollBars = ScrollBars.Vertical;

            ThemeManager.UpdateDataGridViewHeaderTheme(dataGridView);
        }
    }
}