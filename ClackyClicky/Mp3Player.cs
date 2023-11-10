﻿using NAudio.Wave;

namespace ClackyClicky
{
    public class MP3Player
    {
        public string AudioFilePath { get; set; }
        public MP3Player(string audioFilePath) => AudioFilePath = audioFilePath;

        private Mp3FileReader? reader;
        private WaveOutEvent? waveOut;

        public void Open(string FileName)
        {
            if (reader != null) reader.Dispose();
            if (waveOut != null) waveOut.Dispose();

            AudioFilePath = FileName;

            reader = null;
            waveOut = null;
        }

        public void Load()
        {
            if (AudioFilePath == null) throw new Exception("Open audio file frist");
            if (reader == null) reader = new Mp3FileReader(new MemoryStream(File.ReadAllBytes(AudioFilePath), false));
        }
        public void Play()
        {
            if (reader == null) Load();

            if (waveOut == null)
            {
                waveOut = new WaveOutEvent();
                waveOut.Init(reader);
            }
            else
            {
                reader?.Seek(0, SeekOrigin.Begin);
            }

            waveOut.Play();
        }

        public void Reset()
        {
            if (reader == null || waveOut == null) return;

            waveOut?.Stop();
            reader?.Seek(0, SeekOrigin.Begin);
            waveOut?.Play();
        }

        public void Stop()
        {
            if (reader == null || waveOut == null) return;

            waveOut?.Stop();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (reader != null)
                {
                    reader.Dispose();
                    reader = null;
                }

                if (waveOut != null)
                {
                    waveOut.Dispose();
                    waveOut = null;
                }
            }
        }
    }
}