using System;
using System.Drawing;
using System.Windows.Forms;
using NAudio.CoreAudioApi;

namespace MicVolumeKeeper
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // ����������� ������������� ��� ���������� WinForms �� .NET Core
            ApplicationConfiguration.Initialize();

            // ��������� ���� ���������� � ���� ���������� ��������� ��� �����
            Application.Run(new TrayAppContext());
        }
    }

    // ����� ��� ���������� ������� ���������� � ����.
    // ��� ���������� �� ����������, �� ��� ������� ��������� �����.
    public class TrayAppContext : ApplicationContext
    {
        private readonly NotifyIcon _trayIcon;
        private readonly System.Windows.Forms.Timer _volumeCheckTimer;

        public TrayAppContext()
        {
            // ������� ���� ��� ������ � ����
            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("�����", null, Exit);

            // ������������� ����� ������
            _trayIcon = new NotifyIcon()
            {
                // ��������� ������ �� ���������� ��������
                Icon = new Icon(typeof(Program), "mic.ico"),
                ContextMenuStrip = contextMenu,
                Visible = true,
                Text = "�������� ��������� ��������� (100%)"
            };

            // ������������� ������� ��� ������������� ��������
            _volumeCheckTimer = new System.Windows.Forms.Timer
            {
                Interval = 1500 // ��������� ������ 1.5 �������
            };
            _volumeCheckTimer.Tick += OnTimerTick;
            _volumeCheckTimer.Start();

            // ����� ��������� ��������� ��� �������
            SetMicrophoneVolumeToMax();
        }

        private void OnTimerTick(object? sender, EventArgs e)
        {
            SetMicrophoneVolumeToMax();
        }

        private void SetMicrophoneVolumeToMax()
        {
            try
            {
                // NAudio �������� ��������� � � .NET Framework, � � .NET Core
                using var enumerator = new MMDeviceEnumerator();
                using var device = enumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Console);

                // MasterVolumeLevelScalar � ��� �������� �� 0.0f (0%) �� 1.0f (100%)
                if (device.AudioEndpointVolume.MasterVolumeLevelScalar < 1.0f)
                {
                    device.AudioEndpointVolume.MasterVolumeLevelScalar = 1.0f;
                }
            }
            catch (Exception)
            {
                // ���������� ������ (��������, ���� �������� ��������).
                // ����� �������� ����������� ��� �������������.
            }
        }

        // ����� ��� ����������� ������ �� ����������
        private void Exit(object? sender, EventArgs e)
        {
            _trayIcon.Visible = false;
            _volumeCheckTimer.Stop();
            Application.Exit();
        }
    }
}