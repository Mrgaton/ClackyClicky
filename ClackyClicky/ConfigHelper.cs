using System.Runtime.InteropServices;
using System.Text;

namespace ClackyClicky
{
    internal class ConfigHelper
    {
        public static readonly string ConfigFile = Path.Combine(Program.ProgramDirectory, "Config.ini");

        private static class ReadWriteINIfile
        {
            [DllImport("kernel32")]
            public static extern long WritePrivateProfileString(string name, string key, string val, string filePath);

            [DllImport("kernel32")]
            public static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        }

        public static string ReadConfig(string Name, string Key, int MaxLengh = 255)
        {
            StringBuilder sb = new StringBuilder(MaxLengh);

            ReadWriteINIfile.GetPrivateProfileString(Name, Key, "", sb, MaxLengh, ConfigFile);

            return sb.ToString();
        }

        public static void WriteConfig(string Name, string Key, string Value)
        {
            ReadWriteINIfile.WritePrivateProfileString(Name, Key, Value, ConfigFile);
        }
    }
}