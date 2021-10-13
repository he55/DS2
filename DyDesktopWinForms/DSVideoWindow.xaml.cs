using System;
using System.Windows;
using System.Windows.Interop;

namespace DyDesktopWinForms
{
    /// <summary>
    /// Interaction logic for DSVideoWindow.xaml
    /// </summary>
    public partial class DSVideoWindow : Window
    {
        public DSVideoWindow()
        {
            InitializeComponent();
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

        public void Play() => mediaElement.Play();

        public void Pause() => mediaElement.Pause();

        private void mediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            mediaElement.Position = TimeSpan.Zero;
            mediaElement.Play();
        }
    }
}
