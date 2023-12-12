using Microsoft.Win32;

namespace ClackyClicky
{
    internal class RegistryHelper
    {
        public static void CreateAutoStartProgram(string appname, string AppPath) => CreateAutoStartProgram(Registry.CurrentUser, appname, AppPath);

        public static void CreateAutoStartProgram(RegistryKey hive, string appname, string AppPath) => hive.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true).SetValue(appname, AppPath, RegistryValueKind.String);

        public static string ReadAutoStartProgram(string appname) => ReadAutoStartProgram(Registry.CurrentUser, appname);

        public static string ReadAutoStartProgram(RegistryKey hive, string appname) => hive.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", false).GetValue(appname).ToString();

        public static void RemoveAutoStartProgram(string appname) => RemoveAutoStartProgram(Registry.CurrentUser, appname);

        public static void RemoveAutoStartProgram(RegistryKey hive, string appname)
        {
            if (StrartupProgramExist(appname)) hive.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true).DeleteValue(appname);
        }

        public static void AcceptAutoRunRegedit(string aplicationRegeditname) => AcceptAutoRunRegedit(Registry.CurrentUser, aplicationRegeditname);

        public static void AcceptAutoRunRegedit(RegistryKey hive, string aplicationRegeditname) => hive.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run", true).SetValue(aplicationRegeditname, new byte[] { 0002, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00 }, RegistryValueKind.Binary);

        public static void DisableAutoRunRegedit(string aplicationRegeditname) => DisableAutoRunRegedit(Registry.CurrentUser, aplicationRegeditname);

        public static void DisableAutoRunRegedit(RegistryKey hive, string aplicationRegeditname) => hive.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run", true).SetValue(aplicationRegeditname, new byte[] { 0099, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 }, RegistryValueKind.Binary);

        public static bool StrartupProgramDisabled(string appname) => StrartupProgramDisabled(Registry.CurrentUser, appname);

        public static bool StrartupProgramDisabled(RegistryKey hive, string appname)
        {
            try
            {
                return !BitConverter.ToString((byte[])ReadRegristykey(hive, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run", appname)).Trim().StartsWith("02");
            }
            catch { }

            return false;
        }

        public static bool StrartupProgramExist(string appname) => StrartupProgramExist(Registry.CurrentUser, appname);

        public static bool StrartupProgramExist(RegistryKey hive, string appname) => RegistryValueExists(hive, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", appname);

        public static bool RegistryValueExists(RegistryKey hive, string registryRoot, string valuename) => hive.OpenSubKey(registryRoot, false).GetValue(valuename) != null;

        private static object? ReadRegristykey(RegistryKey hive, string key, string Value)
        {
            try
            {
                using (RegistryKey regKey = hive?.OpenSubKey(key))
                {
                    if (regKey != null) return regKey.GetValue(Value);
                }
            }
            catch (Exception ex) { if (Program.ConsoleAttached) Console.WriteLine(ex.ToString()); }

            return null;
        }
    }
}