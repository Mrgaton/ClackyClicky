using System.Runtime.CompilerServices;
using System.Windows.Forms.Design;

namespace ClackyClicky
{
    partial class ClackyClicky
    {
        [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.MenuStrip | ToolStripItemDesignerAvailability.ContextMenuStrip)]
        public class TrackBarMenuItem : ToolStripControlHost
        {
            public TrackBar TrackBar;

            public TrackBarMenuItem() : base(new TrackBar())
            {
                this.TrackBar = this.Control as TrackBar;
            }

            // Add properties, events etc. you want to expose...
        }


        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        /// 
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            TrayMenuStrip = new ContextMenuStrip(components);
            StripSeparator_4 = new ToolStripSeparator();
            RunOnStartUpMenuItem = new ToolStripMenuItem();
            DisableUACMenuItem = new ToolStripMenuItem();
            PauseOnGameMenuItem = new ToolStripMenuItem();
            DisablePressSoundMenuItem = new ToolStripMenuItem();
            DisableReleaseSoundMenuItem = new ToolStripMenuItem();
            StripSeparator_3 = new ToolStripSeparator();
            VolumenMenuItem = new ToolStripMenuItem();
            VolumeTrackBar = new TrackBarMenuItem();
            StripSeparator_2 = new ToolStripSeparator();
            SwitchSoundMenuItem = new ToolStripMenuItem();
            SoundPackComboBox = new ToolStripComboBox();
            StripSeparator_1 = new ToolStripSeparator();
            ExitMenuItem = new ToolStripMenuItem();
            NotifyIcon = new NotifyIcon(components);
            TrashTruckTimer = new System.Windows.Forms.Timer(components);
            SaveDelayTimer = new System.Windows.Forms.Timer(components);
            VolumeChangeDelayTimer = new System.Windows.Forms.Timer(components);
            TrayMenuStrip.SuspendLayout();
            SuspendLayout();
            // 
            // TrayMenuStrip
            // 
            TrayMenuStrip.AllowMerge = false;
            TrayMenuStrip.BackColor = Color.Transparent;
            TrayMenuStrip.BackgroundImageLayout = ImageLayout.None;
            TrayMenuStrip.DropShadowEnabled = false;
            TrayMenuStrip.ImageScalingSize = new Size(20, 20);
            TrayMenuStrip.ImeMode = ImeMode.On;
            TrayMenuStrip.Items.AddRange(new ToolStripItem[] { StripSeparator_4, RunOnStartUpMenuItem, DisableUACMenuItem, PauseOnGameMenuItem,DisablePressSoundMenuItem,DisableReleaseSoundMenuItem, StripSeparator_3, VolumenMenuItem, VolumeTrackBar, StripSeparator_2, SwitchSoundMenuItem, SoundPackComboBox, StripSeparator_1, ExitMenuItem });
            TrayMenuStrip.Name = "TrayMenuStrip";
            TrayMenuStrip.RenderMode = ToolStripRenderMode.Professional;
            TrayMenuStrip.Size = new Size(355, 263);
            // 
            // StripSeparator_4
            // 
            StripSeparator_4.BackColor = Color.GhostWhite;
            StripSeparator_4.Name = "StripSeparator_4";
            StripSeparator_4.Size = new Size(351, 6);
            // 
            // RunOnStartUpMenuItem
            // 
            RunOnStartUpMenuItem.BackColor = Color.Transparent;
            RunOnStartUpMenuItem.Name = "RunOnStartUpMenuItem";
            RunOnStartUpMenuItem.Size = new Size(354, 24);
            RunOnStartUpMenuItem.Text = "Iniciar programa al encender el equipo";
            RunOnStartUpMenuItem.Click += RunOnStartUpMenuItem_Click;
            // 
            // DisableUACMenuItem
            // 
            DisableUACMenuItem.BackColor = Color.Transparent;
            DisableUACMenuItem.Name = "DisableUACMenuItem";
            DisableUACMenuItem.Size = new Size(354, 24);
            DisableUACMenuItem.Text = "Añadir soporte a apps con administrador";
            DisableUACMenuItem.Click += DisableUACMenuItem_Click;

            // 
            // DisablePressSoundMenuItem
            // 
            DisablePressSoundMenuItem.BackColor = Color.Transparent;
            DisablePressSoundMenuItem.Name = "DisablePressSoundMenuItem";
            DisablePressSoundMenuItem.Size = new Size(354, 24);
            DisablePressSoundMenuItem.Text = "Desactivar sonido al presionar las teclas";
            DisablePressSoundMenuItem.Click += DisablePressSoundMenuItem_Click;

            // 
            // DisableReleaseSoundMenuItem
            // 
            DisableReleaseSoundMenuItem.BackColor = Color.Transparent;
            DisableReleaseSoundMenuItem.Name = "DisableReleaseSoundMenuItem";
            DisableReleaseSoundMenuItem.Size = new Size(354, 24);
            DisableReleaseSoundMenuItem.Text = "Desactivar sonido al soltar las teclas";
            DisableReleaseSoundMenuItem.Click += DisableReleaseSoundMenuItem_Click;

            // 
            // PauseOnGameMenuItem
            // 
            PauseOnGameMenuItem.BackColor = Color.Transparent;
            PauseOnGameMenuItem.Name = "PauseOnGameMenuItem";
            PauseOnGameMenuItem.Size = new Size(354, 24);
            PauseOnGameMenuItem.Text = "Pausar programa mientras se juega (beta)";
            PauseOnGameMenuItem.Click += PauseOnGameMenuItem_Click;
            // 
            // StripSeparator_3
            // 
            StripSeparator_3.BackColor = Color.GhostWhite;
            StripSeparator_3.ForeColor = SystemColors.ControlLight;
            StripSeparator_3.Name = "StripSeparator_3";
            StripSeparator_3.Size = new Size(351, 6);
            // 
            // VolumenMenuItem
            // 
            VolumenMenuItem.BackColor = Color.GhostWhite;
            VolumenMenuItem.Name = "VolumenMenuItem";
            VolumenMenuItem.Size = new Size(354, 24);
            VolumenMenuItem.Text = "Volumen de las teclas";
            // 
            // VolumeTrackBar
            // 
            VolumeTrackBar.BackColor = Color.GhostWhite;
            VolumeTrackBar.Name = "VolumeTrackBar";
            VolumeTrackBar.RightToLeft = RightToLeft.No;
            VolumeTrackBar.Size = new Size(290, 56);
            // 
            // StripSeparator_2
            // 
            StripSeparator_2.BackColor = Color.GhostWhite;
            StripSeparator_2.Name = "StripSeparator_2";
            StripSeparator_2.Size = new Size(351, 6);
            // 
            // SwitchSoundMenuItem
            // 
            SwitchSoundMenuItem.BackColor = Color.Transparent;
            SwitchSoundMenuItem.Name = "SwitchSoundMenuItem";
            SwitchSoundMenuItem.Size = new Size(354, 24);
            SwitchSoundMenuItem.Text = "Sonido de las teclas";
            // 
            // SoundPackComboBox
            // 
            SoundPackComboBox.BackColor = Color.GhostWhite;
            SoundPackComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            SoundPackComboBox.FlatStyle = FlatStyle.Standard;
            SoundPackComboBox.Name = "SoundPackComboBox";
            SoundPackComboBox.Size = new Size(290, 28);
            SoundPackComboBox.Sorted = true;
            SoundPackComboBox.ToolTipText = "Cambiar el sonido de las teclas";
            SoundPackComboBox.SelectedIndexChanged += SoundPackComboBox_SelectedIndexChanged;
            // 
            // StripSeparator_1
            // 
            StripSeparator_1.BackColor = Color.GhostWhite;
            StripSeparator_1.Name = "StripSeparator_1";
            StripSeparator_1.Size = new Size(351, 6);
            // 
            // ExitMenuItem
            // 
            ExitMenuItem.BackColor = Color.Transparent;
            ExitMenuItem.Name = "ExitMenuItem";
            ExitMenuItem.Size = new Size(354, 24);
            ExitMenuItem.Text = "Cerrar del programa";
            ExitMenuItem.Click += ExitMenuItem_Click;
            // 
            // NotifyIcon
            // 
            NotifyIcon.Visible = true;
            // 
            // TrashTruckTimer
            // 
            TrashTruckTimer.Enabled = true;
            TrashTruckTimer.Interval = 60000;
            TrashTruckTimer.Tick += TrashTruckTimer_Tick;
            // 
            // SaveDelayTimer
            // 
            SaveDelayTimer.Interval = 2000;
            SaveDelayTimer.Tick += SaveDelayTimer_Tick;
            // 
            // VolumeChangeDelayTimer
            // 
            VolumeChangeDelayTimer.Interval = 300;
            VolumeChangeDelayTimer.Tick += VolumeChangeDelayTimer_Tick;
            // 
            // ClackyClicky
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            ClientSize = new Size(218, 141);
            Name = "ClackyClicky";
            WindowState = FormWindowState.Minimized;
            FormClosing += ClackyClicky_FormClosing;
            Load += ClackyClicky_Load;
            TrayMenuStrip.ResumeLayout(false);
            TrayMenuStrip.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private ContextMenuStrip TrayMenuStrip;
        private NotifyIcon NotifyIcon;

        private ToolStripMenuItem ExitMenuItem;
        private System.Windows.Forms.Timer TrashTruckTimer;
        private ToolStripSeparator StripSeparator_1;
        private ToolStripMenuItem RunOnStartUpMenuItem;
        private ToolStripMenuItem DisablePressSoundMenuItem;
        private ToolStripMenuItem DisableReleaseSoundMenuItem;
        private ToolStripMenuItem DisableUACMenuItem;
        private ClackyClicky.TrackBarMenuItem VolumeTrackBar;
        private ToolStripSeparator StripSeparator_2;
        private ToolStripMenuItem VolumenMenuItem;
        private System.Windows.Forms.Timer SaveDelayTimer;
        private ToolStripSeparator StripSeparator_3;
        private ToolStripComboBox SoundPackComboBox;
        private ToolStripMenuItem SwitchSoundMenuItem;
        private ToolStripSeparator StripSeparator_4;
        private System.Windows.Forms.Timer VolumeChangeDelayTimer;
        private ToolStripMenuItem PauseOnGameMenuItem;
    }
}