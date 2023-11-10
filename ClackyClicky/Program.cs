using IWshRuntimeLibrary;
using Microsoft.Win32;
using System.ComponentModel;
using System.Diagnostics;
using System.Security.Principal;
using File = System.IO.File;

namespace ClackyClicky
{
    internal static class Program
    {
        public static Process CurrentProcess = Process.GetCurrentProcess();

        public static string ProgramPath = Application.ExecutablePath;
        public static string ProgramDirectory = new FileInfo(ProgramPath).Directory.FullName;
        public static bool ConsoleAttached = RunningWithConsole();

        public static Icon ProgramIco = Icon.ExtractAssociatedIcon(ProgramPath);

        public static bool UACEnabled = (int)Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\System").GetValue("EnableLUA") == 1;
        public static bool Admin = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);

        [STAThread]
        private static void Main(string[] args)
        {
            CurrentProcess.PriorityBoostEnabled = true;
            CurrentProcess.PriorityClass = ProcessPriorityClass.RealTime;

            if (args.Length > 0)
            {
                if (args[0] == "/SetUAC")
                {
                    try
                    {
                        AlternUAC(int.Parse(args[1]) == 1);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);

                        Environment.Exit(1);
                    }

                    Environment.Exit(0);
                }
                else if (args[0] == "/Elevated")
                {
                    try
                    {
                        Process OriginalProcess = Process.GetProcessById(int.Parse(args[1]));

                        if (!OriginalProcess.HasExited)
                        {
                            OriginalProcess.Kill();
                        }
                    }
                    catch { }
                }
                else if (args[0] == "/FixDesigner")
                {
                    string FormDesignerPath = "ClackyClicky.Designer.cs";

                    if (System.IO.File.Exists(FormDesignerPath))
                    {
                        File.WriteAllText(FormDesignerPath + ".tmp", File.ReadAllText(FormDesignerPath).Replace("this.VolumeTrackBar = new ClackyClicky.ClackyClicky.TrackBarMenuItem();", "this.VolumeTrackBar = new ClackyClicky.TrackBarMenuItem();"));

                        File.Copy(FormDesignerPath + ".tmp", FormDesignerPath, true);

                        File.Delete(FormDesignerPath + ".tmp");
                    }

                    Environment.Exit(0);
                }
            }



            if (!UACEnabled && !Admin)
            {
                RunElevated(ProgramPath, "/Elevated " + CurrentProcess.Id);

                Environment.Exit(0);
            }

            if (Process.GetProcessesByName(Application.ProductName).Where(p => p.MainModule.FileName.ToLower().Trim() == ProgramPath.ToLower().Trim() && p.Id != CurrentProcess.Id).Count() > 0)
            {
                MessageBox.Show("El programa ya se está ejecutando, asegúrate que no tengas duplicado el proceso en segundo plano antes de ejecutarme de nuevo", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }

            CreateShortcut();

            ApplicationConfiguration.Initialize();
            Application.Run(new ClackyClicky());
        }

        public static bool RunElevated(string fileName, string arguments = default)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo();

            processInfo.UseShellExecute = true;
            processInfo.Verb = "runas";

            processInfo.FileName = fileName;
            processInfo.Arguments = arguments;

            try
            {
                Process.Start(processInfo);

                return true;
            }
            catch (Win32Exception) { }// User canceled

            return false;
        }

        private static void AlternUAC(bool enableLua)
        {
            Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", true).SetValue("EnableLUA", enableLua ? 1 : 0, RegistryValueKind.DWord);
        }

        private static bool RunningWithConsole()
        {
            try
            {
                return (Console.CursorLeft >= int.MinValue);
            }
            catch { }

            return false;
        }

        private static void CreateShortcut()
        {
            if (!Admin)
            {
                return;
            }

            string ShortcutFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonPrograms), Application.ProductName);

            if (!Directory.Exists(ShortcutFolder))
            {
                Directory.CreateDirectory(ShortcutFolder);
            }

            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(Path.Combine(ShortcutFolder, Application.ProductName + ".lnk"));

            shortcut.Description = Application.ProductName;
            shortcut.IconLocation = ProgramPath;
            shortcut.TargetPath = ProgramPath;
            shortcut.Save();

            if (!File.Exists(Path.Combine(ShortcutFolder, "ClickyClacky.lnk")))
            {
                File.Copy(Path.Combine(ShortcutFolder, Application.ProductName + ".lnk"), Path.Combine(ShortcutFolder, "ClickyClacky.lnk"));
            }
        }
    }
}