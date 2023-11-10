using Microsoft.Win32;

namespace ClackyClicky
{
    internal class RegistryHelper
    {
        public static void CreateAutoStartProgram(string appName, string AppPath) => CreateAutoStartProgram(Registry.CurrentUser, appName, AppPath);

        public static void CreateAutoStartProgram(RegistryKey hive, string appName, string AppPath) => hive.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true).SetValue(appName, AppPath, RegistryValueKind.String);

        public static string ReadAutoStartProgram(string appName) => ReadAutoStartProgram(Registry.CurrentUser, appName);

        public static string ReadAutoStartProgram(RegistryKey hive, string appName) => hive.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", false).GetValue(appName).ToString();

        public static void RemoveAutoStartProgram(string appName) => RemoveAutoStartProgram(Registry.CurrentUser, appName);

        public static void RemoveAutoStartProgram(RegistryKey hive, string appName)
        {
            if (StrartupProgramExist(appName))
            {
                hive.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true).DeleteValue(appName);
            }
        }

        public static void AcceptAutoRunRegedit(string aplicationRegeditName) => AcceptAutoRunRegedit(Registry.CurrentUser, aplicationRegeditName);

        public static void AcceptAutoRunRegedit(RegistryKey hive, string aplicationRegeditName) => hive.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run", true).SetValue(aplicationRegeditName, new byte[] { 0002, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00 }, RegistryValueKind.Binary);

        public static void DisableAutoRunRegedit(string aplicationRegeditName) => DisableAutoRunRegedit(Registry.CurrentUser, aplicationRegeditName);

        public static void DisableAutoRunRegedit(RegistryKey hive, string aplicationRegeditName) => hive.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run", true).SetValue(aplicationRegeditName, new byte[] { 0099, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 }, RegistryValueKind.Binary);

        public static bool StrartupProgramDisabled(string appName) => StrartupProgramDisabled(Registry.CurrentUser, appName);

        public static bool StrartupProgramDisabled(RegistryKey hive, string appName)
        {
            try
            {
                return !BitConverter.ToString((byte[])ReadRegristyKey(hive, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run", appName)).Trim().StartsWith("02");
            }
            catch
            { }

            return false;
        }

        public static bool StrartupProgramExist(string appName) => StrartupProgramExist(Registry.CurrentUser, appName);

        public static bool StrartupProgramExist(RegistryKey hive, string appName) => RegistryValueExists(hive, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", appName);

        public static bool RegistryValueExists(RegistryKey hive, string registryRoot, string valueName) => hive.OpenSubKey(registryRoot, false).GetValue(valueName) != null;

        private static object? ReadRegristyKey(RegistryKey hive, string Key, string Value)
        {
            try
            {
                using (RegistryKey key = hive?.OpenSubKey(Key))
                {
                    if (key != null)
                    {
                        return key.GetValue(Value);
                    }
                }
            }
            catch (Exception ex) { if (Program.ConsoleAttached) Console.WriteLine(ex.ToString()); }

            return null;
        }
    }
}