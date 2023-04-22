using Microsoft.Win32;

namespace ClackyClicky
{
    internal class RegistryHelper
    {
        public static void CreateAutoStartProgram(string AppName, string AppPath)
        {
            CreateAutoStartProgram(Registry.CurrentUser, AppName, AppPath);
        }
        public static void CreateAutoStartProgram(RegistryKey Hive, string AppName, string AppPath)
        {
            Hive.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true).SetValue(AppName, AppPath, RegistryValueKind.String);
        }


        public static string ReadAutoStartProgram(string AppName)
        {
            return ReadAutoStartProgram(Registry.CurrentUser, AppName);
        }
        public static string ReadAutoStartProgram(RegistryKey Hive, string AppName)
        {
            return Hive.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", false).GetValue(AppName).ToString();
        }


        public static void RemoveAutoStartProgram(string AppName)
        {
            RemoveAutoStartProgram(Registry.CurrentUser, AppName);
        }
        public static void RemoveAutoStartProgram(RegistryKey Hive, string AppName)
        {
            if (StrartupProgramExist(AppName))
            {
                Hive.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true).DeleteValue(AppName);
            }
        }


        public static void AcceptAutoRunRegedit(string AplicationRegeditName)
        {
            AcceptAutoRunRegedit(Registry.CurrentUser, AplicationRegeditName);
        }
        public static void AcceptAutoRunRegedit(RegistryKey Hive, string AplicationRegeditName)
        {
            Hive.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run", true).SetValue(AplicationRegeditName, new byte[] { 0002, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00 }, RegistryValueKind.Binary);
        }


        public static void DisableAutoRunRegedit(string AplicationRegeditName)
        {
            DisableAutoRunRegedit(Registry.CurrentUser, AplicationRegeditName);
        }
        public static void DisableAutoRunRegedit(RegistryKey Hive, string AplicationRegeditName)
        {
            Hive.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run", true).SetValue(AplicationRegeditName, new byte[] { 0099, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 }, RegistryValueKind.Binary);
        }


        public static bool StrartupProgramDisabled(string AppName)
        {
            return StrartupProgramDisabled(Registry.CurrentUser, AppName);
        }
        public static bool StrartupProgramDisabled(RegistryKey Hive, string AppName)
        {
            try
            {
                return !BitConverter.ToString((byte[])ReadRegristyKey(Hive, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run", AppName)).Trim().StartsWith("02");
            }
            catch
            { }

            return false;
        }
        public static bool StrartupProgramExist(string AppName)
        {
            return StrartupProgramExist(Registry.CurrentUser, AppName);
        }
        public static bool StrartupProgramExist(RegistryKey Hive, string AppName)
        {
            return RegistryValueExists(Hive, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", AppName);
        }
        public static bool RegistryValueExists(RegistryKey Hive, string registryRoot, string valueName)
        {
            return Hive.OpenSubKey(registryRoot, false).GetValue(valueName) != null;
        }

        private static object ReadRegristyKey(RegistryKey Hive, string Key, string Value)
        {
            try
            {
                using (RegistryKey key = Hive.OpenSubKey(Key))
                {
                    if (key != null)
                    {
                        return key.GetValue(Value);
                    }
                }
            }
            catch (Exception ex) { }

            return null;
        }
    }
}
