using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DyDesktopWinForms
{
    public partial class Form1 : Form
    {
        private VideoWindow videoWindow;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            videoWindow = new VideoWindow();
            videoWindow.FullScreen();
            videoWindow.Show();

            IntPtr workerWindowHandle = DesktopWorker.GetWorkerWindowHandle();
            PInvoke.SetParent(videoWindow.Handle, workerWindowHandle);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "MP4 Files (*.mp4)|*.mp4";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                videoWindow.Source = new Uri(openFileDialog.FileName, UriKind.Absolute);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            videoWindow.Play();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            videoWindow.Pause();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            trackBar1.Enabled = !checkBox1.Checked;
            videoWindow.IsMuted = checkBox1.Checked;
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            videoWindow.Volume = trackBar1.Value / 10.0;
        }
    }
}
