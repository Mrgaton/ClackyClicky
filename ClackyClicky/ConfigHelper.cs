using System.Runtime.InteropServices;
using System.Text;

namespace ClackyClicky
{
    internal class ConfigHelper
    {
        public static readonly string configFile = Path.Combine(Program.ProgramDirectory, "Config.ini");

        private static class ReadWriteINIfile
        {
            [DllImport("kernel32")]
            public static extern long WritePrivateProfileString(string name, string key, string val, string filePath);

            [DllImport("kernel32")]
            public static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        }

        public static string ReadConfig(string key, int maxLengh = 255) => ReadConfig(Application.ProductName, key, maxLengh);

        public static string ReadConfig(string name, string key, int maxLengh = 255)
        {
            StringBuilder sb = new StringBuilder(maxLengh);

            ReadWriteINIfile.GetPrivateProfileString(name, key, "", sb, maxLengh, configFile);

            return sb.ToString();
        }

        public static void WriteConfig(string key, string Value) => WriteConfig(Application.ProductName, key, Value);

        public static void WriteConfig(string name, string key, string Value)
        {
            ReadWriteINIfile.WritePrivateProfileString(name, key, Value, configFile);
        }
    }
}