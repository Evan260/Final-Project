using System.Runtime.InteropServices;

namespace Final_Project
{
    internal static partial class ThemeManager
    {
        // Set the form header theme
        // https://stackoverflow.com/questions/57124243/winforms-dark-title-bar-on-windows-10

        [LibraryImport("dwmapi.dll", EntryPoint = "DwmSetWindowAttribute")]
        private static partial int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);
        public static bool UseImmersiveDarkMode(IntPtr handle, bool useDarkMode)
        {
            if (Environment.OSVersion.Version.Major >= 10)
            {
                int attribute = 19;
                if (Environment.OSVersion.Version.Build >= 18985)
                {
                    attribute = 20;
                }

                int useImmersiveDarkMode = useDarkMode ? 1 : 0;
                return DwmSetWindowAttribute(handle, attribute, ref useImmersiveDarkMode, sizeof(int)) == 0;
            }
            return false;
        }
    }
}