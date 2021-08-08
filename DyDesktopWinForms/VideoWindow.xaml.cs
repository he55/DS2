using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
        }

        public IntPtr Handle
        {
            get
            {
                WindowInteropHelper windowInteropHelper = new WindowInteropHelper(this);
                return windowInteropHelper.Handle;
            }
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

        public void FullScreen()
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
