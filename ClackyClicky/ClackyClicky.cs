using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ClackyClicky
{
    public partial class ClackyClicky : Form
    {
        public static readonly string defaultSoundPackName = "cream";

        /*public static MixingSampleProvider mixer = null;
        public static void PlayAudio(byte[] audioData) => mixer.AddMixerInput(GetSampleProvider(mixer, audioData));
        public static ISampleProvider GetSampleProvider(MixingSampleProvider mixer, byte[] audioData) => FixInptut(mixer, new WdlResamplingSampleProvider(new Mp3FileReader(new MemoryStream(audioData)).ToSampleProvider(), mixer.WaveFormat.SampleRate));
        private static ISampleProvider FixInptut(MixingSampleProvider mixer, ISampleProvider resampler)
        {
            ISampleProvider inputWithCorrectChannelCount = resampler;

            int channelCount = mixer.WaveFormat.Channels;

            if (channelCount == 1 && resampler.WaveFormat.Channels == 2) inputWithCorrectChannelCount = new StereoToMonoSampleProvider(resampler);
            else if (channelCount == 2 && resampler.WaveFormat.Channels == 1) inputWithCorrectChannelCount = new MonoToStereoSampleProvider(resampler);

            return inputWithCorrectChannelCount;
        }*/

        public ClackyClicky()
        {
            InitializeComponent();

            this.WindowState = FormWindowState.Minimized;
            this.Visible = false;
            this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
            this.ShowInTaskbar = false;
            this.Opacity = 0;

            this.TopMost = true;

            if (!File.Exists(ConfigHelper.ConfigFile))
            {
                NotifyIcon.BalloonTipText = "Bienvenido a " + Application.ProductName + " la app se ejecutara en segundo plano, puedes configurar el programa desde el menu de iconos ocultos";
                NotifyIcon.Visible = true;
                NotifyIcon.BalloonTipTitle = Application.ProductName;
                NotifyIcon.Icon = Program.ProgramIco;
                NotifyIcon.BalloonTipIcon = ToolTipIcon.Info;

                NotifyIcon.ShowBalloonTip(10000);
            }

            NotifyIcon.Text = Application.ProductName;

            this.Icon = this.NotifyIcon.Icon = Program.ProgramIco;

            this.NotifyIcon.ContextMenuStrip = this.TrayMenuStrip;

            ExitMenuItem.Image = Properties.Resources.CloseImage;

            SwitchSoundMenuItem.Image = Properties.Resources.SoundImage;

            this.VolumenMenuItem.Image = Properties.Resources.VolumeImage;
            this.VolumeTrackBar.TrackBar.Maximum = 100;
            this.VolumeTrackBar.TrackBar.TickFrequency = 5;
            this.VolumeTrackBar.TrackBar.TickStyle = TickStyle.Both;
            this.VolumeTrackBar.TrackBar.Size = new Size(VolumeTrackBar.Size.Width + 15, VolumeTrackBar.Size.Height);
            this.VolumeTrackBar.TrackBar.ValueChanged += VolumeTrackBar_Update;
            this.TopMost = true;

            Task.Factory.StartNew(() =>
            {
                bool createKey = false;

                if (!RegistryHelper.StrartupProgramExist(Application.ProductName))
                {
                    createKey = true;
                }
                else if (!RegistryHelper.ReadAutoStartProgram(Application.ProductName).ToLower().Trim().Contains(Program.ProgramPath.ToLower().Trim()))
                {
                    createKey = true;
                }

                if (createKey) RegistryHelper.CreateAutoStartProgram(Application.ProductName, Program.ProgramPath);
            });

            TrayMenuStrip.Items.Insert(0, new ToolStripLabel(Application.ProductName) { Image = Program.ProgramIco.ToBitmap() });

            bool autoRunEnabled = false;

            if (RegistryHelper.StrartupProgramExist(Application.ProductName)) autoRunEnabled = !RegistryHelper.StrartupProgramDisabled(Application.ProductName);

            RunOnStartUpMenuItem.Checked = autoRunEnabled;

            DisableUACMenuItem.Checked = !Program.UACEnabled;

            GlobalKeyPressed += KeyboardHook_GlobalKeyPressed;
            GlobalKeyReleased += KeyboardHook_GlobalKeyReleased;

            if (WindowsTheme.CurrentWindowsTheme == WindowsTheme.WindowsThemeMode.DarkTheme) LoadBlackTheme();

            string pauseOnGame = ConfigHelper.ReadConfig(Application.ProductName, "pauseOnGame");

            if (!string.IsNullOrWhiteSpace(pauseOnGame)) PauseOnGameMenuItem.Checked = bool.TryParse(pauseOnGame, out bool Pause) ? Pause : false;

            string volumeSavedString = ConfigHelper.ReadConfig(Application.ProductName, "KeysVolume");

            int savedVolume = 50;

            if (!string.IsNullOrWhiteSpace(volumeSavedString)) int.TryParse(volumeSavedString, out savedVolume);

            VolumeHelper.SetProgramVolume(savedVolume);

            this.VolumeTrackBar.TrackBar.Value = savedVolume;

            LoadKeysAudio();

            if (keysSoundPacks.Count() <= 0)
            {
                MessageBox.Show("Error no se encontraron ningun pack de teclas :C", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }

            /*var prop = (new MMDeviceEnumerator()).GetDefaultAudioEndpoint(DataFlow.Render, Role.Console).AudioClient.MixFormat;

            WaveOutEvent outputDevice = new WaveOutEvent();

            mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(prop.SampleRate, prop.Channels))
            {
                ReadFully = true,
            };

            outputDevice.Init(mixer);
            outputDevice.Play();*/

            keysSoundPacks.ForEach(sndPack => SoundPackComboBox.Items.Add("Teclas " + sndPack.KeysName + ((sndPack.EnterPressAudio.Count() > 0) ? " ⭐" : null)));

            string SoundPackSavedString = ConfigHelper.ReadConfig(Application.ProductName, "SelectedSoundPack");

            if (string.IsNullOrWhiteSpace(SoundPackSavedString)) SoundPackSavedString = defaultSoundPackName;

            for (int i = 0; i < 2; i++)
            {
                if (currentSoundPack != null) break;

                try
                {
                    currentSoundPack = keysSoundPacks.Where(p => p.KeysName.ToLower().Contains(SoundPackSavedString)).ToList()[0];
                }
                catch (Exception ex)
                {
                    SoundPackSavedString = defaultSoundPackName;

                    if (Program.ConsoleAttached) Console.WriteLine(ex.ToString());
                }
            }

            if (currentSoundPack == null)
            {
                MessageBox.Show("Error no se pudo encontrar el pack de sonido guardado ni el predeterminado por el programa revisa que la carpeta de sonidos este en el mismo directorio que el programa", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);

                Environment.Exit(0);
            }

            //currentSoundPack.LoadSoundsAsync();

            Initialize();

            int itemIndex = 0;

            foreach (var comboBoxItem in SoundPackComboBox.Items)
            {
                if (comboBoxItem.ToString().Contains(currentSoundPack.KeysName)) SoundPackComboBox.SelectedIndex = itemIndex;

                itemIndex++;
            }

            GC.Collect(GC.MaxGeneration);
        }

        private void LoadBlackTheme()
        {
            Color BackColor = Color.FromArgb(32, 32, 32);
            Color DiffBackColor = Color.FromArgb(42, 42, 42);

            this.BackColor = this.ForeColor = TrayMenuStrip.BackColor = VolumenMenuItem.BackColor = SoundPackComboBox.BackColor = BackColor;
            SoundPackComboBox.ComboBox.BackColor = VolumeTrackBar.TrackBar.BackColor = DiffBackColor;

            TrayMenuStrip.ForeColor = Color.White;
            TrayMenuStrip.Renderer = new ToolStripProfessionalRenderer(new MenuColorTable());
            SoundPackComboBox.ForeColor = Color.White;
            SwitchSoundMenuItem.Image = InvertImage(SwitchSoundMenuItem.Image);
            VolumenMenuItem.Image = InvertImage(VolumenMenuItem.Image);
        }

        public class MenuColorTable : ProfessionalColorTable
        {
            public MenuColorTable()
            { UseSystemColors = false; }

            public override Color ImageMarginGradientEnd
            { get { return Color.FromArgb(27, 27, 27); } }

            public override Color ImageMarginGradientBegin
            { get { return Color.FromArgb(22, 22, 22); } }

            public override Color ImageMarginGradientMiddle
            { get { return Color.Black; } }

            public override Color MenuBorder
            { get { return Color.Gray; } }

            public override Color MenuItemBorder
            { get { return Color.Gray; } }

            public override Color MenuItemSelected
            { get { return Color.Cornsilk; } }
        }

        private Image InvertImage(Image image)
        {
            Bitmap pic = (Bitmap)image;

            for (int y = 0; (y <= (pic.Height - 1)); y++)
            {
                for (int x = 0; (x <= (pic.Width - 1)); x++)
                {
                    Color inv = pic.GetPixel(x, y);

                    pic.SetPixel(x, y, Color.FromArgb(inv.A, (255 - inv.R), (255 - inv.G), (255 - inv.B)));
                }
            }

            return pic;
        }

        private static string keysAudioDirectory = Path.Combine(Program.ProgramDirectory, "KeySounds");

        private static List<SoundPack> keysSoundPacks = new List<SoundPack>();

        private static SoundPack currentSoundPack;

        private enum PressType
        {
            Press,
            Release
        }

        private static void LoadKeysAudio()
        {
            if (!Directory.Exists(keysAudioDirectory))
            {
                MessageBox.Show("Error no se pudo encontrar la carpeta con los datos de las teclas", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);

                Environment.Exit(1);
            }

            List<string> ValidSoundsDirs = new List<string>();

            foreach (string KeysDir in Directory.GetDirectories(keysAudioDirectory))
            {
                if (Directory.Exists(Path.Combine(KeysDir, "press")) && Directory.Exists(Path.Combine(KeysDir, "release")))
                {
                    ValidSoundsDirs.Add(KeysDir);
                }
            }

            Parallel.For(0, ValidSoundsDirs.Count, item =>
            {
                try
                {
                    string KeysDir = ValidSoundsDirs[item];

                    if (Program.ConsoleAttached) Console.WriteLine("Loading soundpack:" + KeysDir.Split('\\').Last());

                    SoundPack SndPack = new SoundPack();

                    SndPack.KeysName = KeysDir.Split('\\').Last();

                    //Hacer esto con parallels para que se inicie el programa mas rapido

                    string[] soundDirs = Directory.GetDirectories(KeysDir);

                    foreach (string SoundDir in soundDirs)
                    {
                        PressType soundDirType = SoundDir.ToLower().EndsWith("press") ? PressType.Press : PressType.Release;

                        foreach (string audio in Directory.EnumerateFiles(SoundDir))
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                MP3Player player = new MP3Player(audio);

                                string audioName = Path.GetFileName(audio).ToUpper();

                                Console.WriteLine(audioName);

                                if (audioName.StartsWith("GENERIC"))
                                {
                                    (soundDirType == PressType.Press ? SndPack.GenericPressAudio : SndPack.GenericReleaseAudio).Add(player);
                                }
                                else if (audioName.StartsWith("BACKSPACE"))
                                {
                                    (soundDirType == PressType.Press ? SndPack.BackspacePressAudio : SndPack.BackspaceReleaseAudio).Add(player);
                                }
                                else if (audioName.StartsWith("ENTER"))
                                {
                                    (soundDirType == PressType.Press ? SndPack.EnterPressAudio : SndPack.EnterReleaseAudio).Add(player);
                                }
                                else if (audioName.StartsWith("SPACE"))
                                {
                                    (soundDirType == PressType.Press ? SndPack.SpacePressAudio : SndPack.SpaceReleaseAudio).Add(player);
                                }
                            }
                        }
                    }

                    keysSoundPacks.Add(SndPack);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            });
        }

        private class SoundPack
        {
            private bool AlredyLoaded = false;
            public string KeysName { get; set; } = null;

            public List<MP3Player> GenericPressAudio = new List<MP3Player>();

            public MP3Player RandomGenericPressAudio() => RandomElement(GenericPressAudio);

            public List<MP3Player> GenericReleaseAudio = new List<MP3Player>();

            public MP3Player RandomGenericReleaseAudio() => RandomElement(GenericReleaseAudio);

            public List<MP3Player> BackspacePressAudio = new List<MP3Player>();

            public MP3Player RandomBackspacePressAudio() => RandomElement(BackspacePressAudio);

            public List<MP3Player> BackspaceReleaseAudio = new List<MP3Player>();

            public MP3Player RandomBackspaceReleaseAudio() => RandomElement(BackspaceReleaseAudio);

            public List<MP3Player> EnterPressAudio = new List<MP3Player>();

            public MP3Player RandomEnterPressAudio() => RandomElement(EnterPressAudio);

            public List<MP3Player> EnterReleaseAudio = new List<MP3Player>();

            public MP3Player RandomEnterReleaseAudio() => RandomElement(EnterReleaseAudio);

            public List<MP3Player> SpacePressAudio = new List<MP3Player>();

            public MP3Player RandomSpacePressAudio() => RandomElement(SpacePressAudio);

            public List<MP3Player> SpaceReleaseAudio = new List<MP3Player>();

            public MP3Player RandomSpaceReleaseAudio() => RandomElement(SpaceReleaseAudio);

            private Dictionary<int, int> lastPlayed = new Dictionary<int, int>();

            private dynamic RandomElement(dynamic element)
            {
                int elementCounts = element.Count;

                if (elementCounts == 0) return null;

                int hashCode = element.GetHashCode();

                if (!lastPlayed.ContainsKey(hashCode)) lastPlayed.Add(hashCode, 0);

                lastPlayed[hashCode]++;

                if (lastPlayed[hashCode] >= elementCounts) lastPlayed[hashCode] = 0;

                Console.WriteLine("Returning element " + lastPlayed[hashCode]);

                return element[lastPlayed[hashCode]];
            }

            public List<MP3Player> GetSoundsList()
            {
                List<MP3Player> sounds = new List<MP3Player>();

                GenericPressAudio.ForEach(sounds.Add);
                GenericReleaseAudio.ForEach(sounds.Add);
                BackspacePressAudio.ForEach(sounds.Add);
                BackspaceReleaseAudio.ForEach(sounds.Add);
                EnterPressAudio.ForEach(sounds.Add);
                EnterReleaseAudio.ForEach(sounds.Add);
                SpacePressAudio.ForEach(sounds.Add);
                SpaceReleaseAudio.ForEach(sounds.Add);

                return sounds;
            }

            public async Task LoadSoundsAsync() => await Task.Run(LoadSounds);

            public void LoadSounds()
            {
                if (AlredyLoaded) return;

                AlredyLoaded = true;

                List<MP3Player> sounds = GetSoundsList();

                foreach (MP3Player Player in sounds)
                {
                    if (Program.ConsoleAttached) Console.WriteLine($"[{DateTime.Now}]:Loading " + new FileInfo(Player.AudioFilePath).FullName.Replace(new FileInfo(Program.ProgramPath).Directory.FullName, "").Trim('\\'));
                    Player.Load();
                }
            }

            public async Task UnLoadSoundsAsync() => await Task.Run(() => UnLoadSounds());

            public void UnLoadSounds()
            {
                if (!AlredyLoaded) return;

                AlredyLoaded = false;

                List<MP3Player> sounds = GetSoundsList();

                foreach (MP3Player Player in sounds)
                {
                    if (Program.ConsoleAttached) Console.WriteLine($"[{DateTime.Now}]:Unloading " + new FileInfo(Player.AudioFilePath).FullName.Replace(new FileInfo(Program.ProgramPath).Directory.FullName, "").Trim('\\'));

                    Player.Dispose();
                }
            }
        }

        private static List<Keys> pressedKeys = new List<Keys>();
        private static bool AltTabPressed = false;

        private void KeyboardHook_GlobalKeyPressed(Keys Key)
        {
            if (PauseOnGameMenuItem.Checked)
            {
                if (Key == Keys.Tab && pressedKeys.Contains(Keys.LMenu))
                {
                    AltTabPressed = true;
                }
            }

            if (VolumeHelper.CurrentVolume > 0)
            {
                try
                {
                    if (pressedKeys.Contains(Key))
                    {
                        return;
                    }
                    else
                    {
                        pressedKeys.Add(Key);
                    }

                    if (Program.ConsoleAttached) Console.WriteLine($"[{DateTime.Now}]: Press {Key}" + Environment.NewLine);

                    var audioToPlay = GetPlayerPerKey(Key, currentSoundPack, PressType.Press);

                    if (audioToPlay == null) audioToPlay = currentSoundPack.RandomGenericPressAudio();

                    audioToPlay.Play();
                }
                catch (Exception ex)
                {
                    if (Program.ConsoleAttached) Console.WriteLine(ex.ToString());
                }
            }
        }

        private void KeyboardHook_GlobalKeyReleased(Keys Key)
        {
            if (PauseOnGameMenuItem.Checked)
            {
                if (Key == Keys.LMenu && AltTabPressed) Task.Factory.StartNew(() => this.BeginInvoke((MethodInvoker)delegate { Thread.Sleep(200); FocusChanged?.Invoke(GetForegroundWindow()); }));
            }

            if (VolumeHelper.CurrentVolume > 0)
            {
                try
                {
                    if (Program.ConsoleAttached) Console.WriteLine($"[{DateTime.Now}]: Released {Key}" + Environment.NewLine);

                    var audioToPlay = GetPlayerPerKey(Key, currentSoundPack, PressType.Release);

                    if (audioToPlay == null) audioToPlay = currentSoundPack.RandomGenericReleaseAudio();

                    audioToPlay.Play();

                    if (pressedKeys.Contains(Key)) pressedKeys.Remove(Key);
                }
                catch (Exception ex)
                {
                    if (Program.ConsoleAttached) Console.WriteLine(ex.ToString());
                }
            };
        }

        private static MP3Player GetPlayerPerKey(Keys key, SoundPack sndPack, PressType pType)
        {
            switch (key)
            {
                case Keys.Back:
                    return (pType == PressType.Press ? sndPack.RandomBackspacePressAudio() : sndPack.RandomBackspaceReleaseAudio());

                case Keys.Enter:
                    return (pType == PressType.Press ? sndPack.RandomEnterPressAudio() : sndPack.RandomEnterReleaseAudio());

                case Keys.Space:
                    return (pType == PressType.Press ? sndPack.RandomSpacePressAudio() : sndPack.RandomSpaceReleaseAudio());
            }

            return (pType == PressType.Press ? sndPack.RandomGenericPressAudio() : sndPack.RandomGenericReleaseAudio());
        }

        public static bool LockAllKeys { get; set; }

        public static event GlobalKeyPressedEventHandler GlobalKeyPressed;

        public static event GlobalKeyPressedEventHandler GlobalKeyReleased;

        private LowLevelKeyboardProc callback;
        private IntPtr _hookID = IntPtr.Zero;

        private void Initialize()
        {
            this.callback = (int nCode, IntPtr wParam, IntPtr lParam) =>
            {
                nint nextHook = CallNextHookEx(_hookID, nCode, wParam, lParam); ;

                HookStruct info = (HookStruct)Marshal.PtrToStructure(lParam, typeof(HookStruct));  //if (Program.ConsoleAttached) Console.WriteLine(wParam + "   " + (Keys)Marshal.ReadInt32(lParam));

                if (nCode >= 0)
                {
                    Task.Factory.StartNew(() =>
                    {
                        if (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_COMMAND)
                        {
                            GlobalKeyPressed?.Invoke((Keys)Marshal.ReadInt32(lParam));
                        }
                        else if (wParam == (IntPtr)WM_KEYUP || wParam == (IntPtr)WM_SYSCOMMAND)
                        {
                            GlobalKeyReleased?.Invoke((Keys)Marshal.ReadInt32(lParam));
                        }
                    });
                }

                return nextHook;
            };

            _hookID = SetHook(this.callback);
        }

        public new void Dispose()
        {
            UnhookWindowsHookEx(_hookID);
            GC.KeepAlive(this.callback);
        }

        private IntPtr SetHook(LowLevelKeyboardProc proc) => SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle("user32"), 0);

        private const int WH_KEYBOARD_LL = 13;

        private const int WM_KEYDOWN = 256;
        private const int WM_KEYUP = 257;
        private const int WM_COMMAND = 260;
        private const int WM_SYSCOMMAND = 261;

        private const int LLKHF_INJECTED = 16;

        private struct HookStruct
        {
            public int vkCode;
            public int scanCode;

            public int flags;
            public int time;

            public int dwExtraInfo;
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        public delegate void GlobalKeyPressedEventHandler(Keys key);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private void ExitMenuItem_Click(object sender, EventArgs e) => ClackyClicky_FormClosing(sender, new FormClosingEventArgs(CloseReason.UserClosing, false));

        private void ClackyClicky_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.NotifyIcon.Visible = false;

            if (Program.ConsoleAttached) Console.WriteLine("Unhooking keyboard");

            Dispose();

            if (Program.ConsoleAttached) Console.WriteLine("Exiting...");

            Environment.Exit(0);
        }

        private void TrashTruckTimer_Tick(object sender, EventArgs e)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private void RunOnStartUpMenuItem_Click(object sender, EventArgs e)
        {
            RunOnStartUpMenuItem.Checked = !RunOnStartUpMenuItem.Checked;

            AlternAutoRun(RunOnStartUpMenuItem.Checked);
        }

        private void PauseOnGameMenuItem_Click(object sender, EventArgs e)
        {
            PauseOnGameMenuItem.Checked = !PauseOnGameMenuItem.Checked;

            if (PauseOnGameMenuItem.Checked)
            {
                Task.Factory.StartNew(() => FocusChangeWatcher());
            }
            else
            {
                VolumeHelper.SetProgramVolume(VolumeTrackBar.TrackBar.Value);
            }

            ConfigHelper.WriteConfig(Application.ProductName, "pauseOnGame", PauseOnGameMenuItem.Checked.ToString());
        }

        private static void AlternAutoRun(bool AutoRun)
        {
            if (AutoRun)
            {
                RegistryHelper.CreateAutoStartProgram(Application.ProductName, Program.ProgramPath);

                if (RegistryHelper.StrartupProgramDisabled(Application.ProductName))
                {
                    RegistryHelper.AcceptAutoRunRegedit(Application.ProductName);
                }
            }
            else
            {
                RegistryHelper.DisableAutoRunRegedit(Application.ProductName);
            }
        }

        private void DisableUACMenuItem_Click(object sender, EventArgs e)
        {
            if (DisableUACMenuItem.Checked)
            {
                DialogResult QuiestionResult = MessageBox.Show("Aviso vas a activar el control de cuentas de usuario del equipo, estás seguro que quieres hacerlo?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (QuiestionResult == DialogResult.Yes)
                {
                    Program.RunElevated(Program.ProgramPath, "/SetUAC 1");
                }
            }
            else
            {
                DialogResult QuiestionResult = MessageBox.Show("Aviso vas a desactivar el control de cuentas de usuario del equipo esto hará que las aplicaciones que se ejecuten como administrador no pidan permiso para ser ejecutadas, estás seguro que quieres hacerlo?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (QuiestionResult == DialogResult.Yes)
                {
                    Program.RunElevated(Program.ProgramPath, "/SetUAC 0");
                }
            }
        }

        private void VolumeTrackBar_Update(object? sender, EventArgs e)
        {
            VolumeChangeDelayTimer.Enabled = false;
            VolumeChangeDelayTimer.Enabled = true;

            SaveDelayTimer.Enabled = false;
            SaveDelayTimer.Enabled = true;
        }

        private void VolumeChangeDelayTimer_Tick(object sender, EventArgs e)
        {
            VolumeChangeDelayTimer.Enabled = false;

            if (Program.ConsoleAttached) Console.WriteLine("Volumen cambiado");
            if (Program.ConsoleAttached) Console.WriteLine();

            if (FullScreen && PauseOnGameMenuItem.Checked)
            {
                return;
            }

            VolumeHelper.SetProgramVolume(VolumeTrackBar.TrackBar.Value);
        }

        private void SaveDelayTimer_Tick(object sender, EventArgs e)
        {
            SaveDelayTimer.Enabled = false;

            if (currentSoundPack == null)
            {
                return;
            }

            ConfigHelper.WriteConfig(Application.ProductName, "KeysVolume", VolumeTrackBar.TrackBar.Value.ToString());
            ConfigHelper.WriteConfig(Application.ProductName, "SelectedSoundPack", currentSoundPack.KeysName);

            if (Program.ConsoleAttached) Console.WriteLine("Configuracion guardada");
            if (Program.ConsoleAttached) Console.WriteLine();
        }

        private async void SoundPackComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SoundPackComboBox.SelectedIndex < 0)
            {
                return;
            }

            SoundPack NewSelectedPack = keysSoundPacks.Where(p => p.KeysName.ToLower().Contains(SoundPackComboBox.SelectedItem.ToString().ToLower().Split(' ')[1])).ToList()[0];

            if (NewSelectedPack == currentSoundPack)
            {
                return;
            }

            SoundPackComboBox.Enabled = false;
            await currentSoundPack.UnLoadSoundsAsync();

            currentSoundPack = NewSelectedPack;

            //await currentSoundPack.LoadSoundsAsync();
            SoundPackComboBox.Enabled = true;

            if (Program.ConsoleAttached) Console.WriteLine("Cambiando el pack de sonido a " + currentSoundPack.KeysName);
            if (Program.ConsoleAttached) Console.WriteLine();

            SaveDelayTimer.Enabled = false;
            SaveDelayTimer.Enabled = true;
        }

        private static bool FullScreen = false;

        private Process[] explorerProcesses = Process.GetProcessesByName("explorer").Where(Proc => Proc.MainWindowHandle != IntPtr.Zero).ToArray();

        private void ClackyClicky_Load(object sender, EventArgs e)
        {
            if (PauseOnGameMenuItem.Checked)
            {
                Task.Factory.StartNew(() => FocusChangeWatcher());
            }

            FocusChanged += (IntPtr WindowHandle) =>
            {
                if (WindowHandle == IntPtr.Zero) return;

                GetWindowThreadProcessId(WindowHandle, out uint ProcessId);

                if (explorerProcesses.Any(proc => proc.Id == ProcessId)) return; //We dont want to check if explorer its on full screen xd

                Screen processScreen = Screen.FromHandle(WindowHandle);

                GetWindowRect(WindowHandle, out Rect ProcessRectangle);

                FullScreen = (ProcessRectangle.Bottom == processScreen.Bounds.Bottom && ProcessRectangle.Right == processScreen.Bounds.Right && ProcessRectangle.Left == processScreen.Bounds.Left && ProcessRectangle.Top == processScreen.Bounds.Top);

                VolumeHelper.SetProgramVolume((FullScreen && PauseOnGameMenuItem.Checked ? 0 : VolumeTrackBar.TrackBar.Value));
            };
        }

        [DllImport("user32.dll")] private static extern Int32 GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        private static IntPtr LastActiveWindowsID = IntPtr.Zero;

        public static event UpdateFocusedApplication FocusChanged;

        public delegate void UpdateFocusedApplication(IntPtr WindowHandle);

        private void FocusChangeWatcher()
        {
            bool Running = true;

            while (PauseOnGameMenuItem.Checked && Running)
            {
                try
                {
                    IntPtr CurrentActiveWindow = GetForegroundWindow();

                    if (CurrentActiveWindow != IntPtr.Zero)
                    {
                        this.BeginInvoke((MethodInvoker)delegate
                        {
                            FocusChanged?.Invoke(CurrentActiveWindow);
                        });

                        if (CurrentActiveWindow != LastActiveWindowsID) LastActiveWindowsID = CurrentActiveWindow;

                        Thread.Sleep(200);
                    }
                }
                catch (InvalidOperationException)
                {
                    Running = false;
                }
                catch (Exception ex) { if (Program.ConsoleAttached) Console.WriteLine(ex.ToString()); }

                Thread.Sleep(800);
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)] private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")] private static extern bool GetWindowRect(IntPtr hWnd, out Rect rect);

        [StructLayout(LayoutKind.Sequential)]
        private struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
    }
}