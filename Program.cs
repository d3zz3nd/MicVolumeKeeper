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
            // Стандартная инициализация для приложений WinForms на .NET Core
            ApplicationConfiguration.Initialize();

            // Запускаем наше приложение в виде кастомного контекста без формы
            Application.Run(new TrayAppContext());
        }
    }

    // Класс для управления логикой приложения в трее.
    // Его содержимое не изменилось, но для полноты приведено здесь.
    public class TrayAppContext : ApplicationContext
    {
        private readonly NotifyIcon _trayIcon;
        private readonly System.Windows.Forms.Timer _volumeCheckTimer;

        public TrayAppContext()
        {
            // Создаем меню для иконки в трее
            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Выход", null, Exit);

            // Инициализация самой иконки
            _trayIcon = new NotifyIcon()
            {
                // Загружаем иконку из внедренных ресурсов
                Icon = new Icon(typeof(Program), "mic.ico"),
                ContextMenuStrip = contextMenu,
                Visible = true,
                Text = "Фиксатор громкости микрофона (100%)"
            };

            // Инициализация таймера для периодической проверки
            _volumeCheckTimer = new System.Windows.Forms.Timer
            {
                Interval = 1500 // Проверяем каждые 1.5 секунды
            };
            _volumeCheckTimer.Tick += OnTimerTick;
            _volumeCheckTimer.Start();

            // Сразу проверяем громкость при запуске
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
                // NAudio работает одинаково и в .NET Framework, и в .NET Core
                using var enumerator = new MMDeviceEnumerator();
                using var device = enumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Console);

                // MasterVolumeLevelScalar — это значение от 0.0f (0%) до 1.0f (100%)
                if (device.AudioEndpointVolume.MasterVolumeLevelScalar < 1.0f)
                {
                    device.AudioEndpointVolume.MasterVolumeLevelScalar = 1.0f;
                }
            }
            catch (Exception)
            {
                // Игнорируем ошибки (например, если микрофон отключен).
                // Можно добавить логирование при необходимости.
            }
        }

        // Метод для корректного выхода из приложения
        private void Exit(object? sender, EventArgs e)
        {
            _trayIcon.Visible = false;
            _volumeCheckTimer.Stop();
            Application.Exit();
        }
    }
}