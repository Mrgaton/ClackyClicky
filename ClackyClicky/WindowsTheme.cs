using Microsoft.Win32;

namespace ClackyClicky
{
    internal class WindowsTheme
    {
        public static WindowsThemeMode CurrentWindowsTheme => GetWindowsTheme();

        private static WindowsThemeMode GetWindowsTheme()
        {
            try
            {
                return (int)Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", false).GetValue("SystemUsesLightTheme") == 1 ? WindowsThemeMode.LightTheme : WindowsThemeMode.DarkTheme;
            }
            catch { }

            return WindowsThemeMode.LightTheme;
        }

        public enum WindowsThemeMode
        {
            DarkTheme,
            LightTheme
        }
    }
}