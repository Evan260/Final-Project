using Guna.UI2.WinForms.Enums;

namespace Final_Project.Classes
{
    /// <summary>
    /// Manages the custom colors, providing different color settings for dark and light themes.
    /// </summary>
    public static class CustomColors
    {
        public static Color ControlBack { get; private set; }
        public static Color HeaderBackground { get; private set; }
        public static Color Text { get; private set; }
        public static DataGridViewPresetThemes DataGridViewTheme { get; private set; }

        /// <summary>
        /// Sets all color values based on the current theme. These colors are used for UI controls throughout the application.
        /// </summary>
        public static void SetColors()
        {
            if (ThemeManager.IsDarkTheme())
            {
                SetDarkThemeColors();
            }
            else
            {
                SetLightThemeColors();
            }
        }
        private static void SetDarkThemeColors()
        {
            // Control colors
            ControlBack = Color.FromArgb(62, 62, 66);
            HeaderBackground = Color.FromArgb(30, 30, 30);

            // Text colors
            Text = Color.White;

            // DataGridView colors
            DataGridViewTheme = DataGridViewPresetThemes.Dark;
        }
        private static void SetLightThemeColors()
        {
            // Control colors
            ControlBack = Color.FromArgb(220, 220, 220);
            HeaderBackground = Color.FromArgb(250, 250, 250);

            // Text colors
            Text = Color.Black;

            // DataGridView colors
            DataGridViewTheme = DataGridViewPresetThemes.White;
        }
    }
}