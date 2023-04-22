using System.Runtime.InteropServices;

namespace ClackyClicky
{
    internal class VolumeHelper
    {
        [DllImport("winmm.dll")] public static extern int waveOutGetVolume(IntPtr hwo, out uint dwVolume);

        [DllImport("winmm.dll")] public static extern int waveOutSetVolume(IntPtr hwo, uint dwVolume);

        public static int CurrentVolume = GetProgramVolume();
        public static int GetProgramVolume()
        {
            waveOutGetVolume(IntPtr.Zero, out uint CurrVol);

            return CurrentVolume = (ushort)(CurrVol & 0x0000ffff) / (ushort.MaxValue / 100);
        }
        public static void SetProgramVolume(int NewVolume)
        {
            if (NewVolume == CurrentVolume)
            {
                return;
            }

            uint NewVolumeInternal = (uint)((ushort.MaxValue / (double)100) * NewVolume);

            CurrentVolume = NewVolume;

            waveOutSetVolume(IntPtr.Zero, Convert.ToUInt32((NewVolumeInternal & 0xFFFF) | (NewVolumeInternal << 16)));
        }
    }
}
