using System;
using System.Windows;

namespace DyDesktopWinForms
{
    /// <summary>
    /// Interaction logic for VideoWindow.xaml
    /// </summary>
    public partial class VideoWindow : Window
    {
        public VideoWindow()
        {
            InitializeComponent();
            FullScreen();
        }

        public Uri Source
        {
            get => mediaElement.Source;
            set => mediaElement.Source = value;
        }

        public double Volume
        {
            get => mediaElement.Volume;
            set => mediaElement.Volume = value;
        }

        public bool IsMuted
        {
            get => mediaElement.IsMuted;
            set => mediaElement.IsMuted = value;
        }

        public void Play()
        {
            mediaElement.Play();
        }

        public void Pause()
        {
            mediaElement.Pause();
        }

        private void FullScreen()
        {
            this.WindowStyle = WindowStyle.None;
            this.ResizeMode = ResizeMode.NoResize;
            this.Left = 0;
            this.Top = 0;
            this.Width = SystemParameters.PrimaryScreenWidth;
            this.Height = SystemParameters.PrimaryScreenHeight;
        }

        private void mediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            mediaElement.Position = TimeSpan.Zero;
            mediaElement.Play();
        }
    }
}
