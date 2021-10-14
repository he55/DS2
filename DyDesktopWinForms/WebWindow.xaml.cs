using System;
using System.Windows;

namespace DyDesktopWinForms
{
    /// <summary>
    /// Interaction logic for WebWindow.xaml
    /// </summary>
    public partial class WebWindow : Window
    {
        public WebWindow()
        {
            InitializeComponent();
        }

        public Uri Source
        {
            get => webView2.Source;
            set => webView2.Source = value;
        }
    }
}
