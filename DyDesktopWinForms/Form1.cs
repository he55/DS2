﻿using System;
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
            this.MaximumSize = this.MinimumSize = this.Size;
        }

        private void CreateVideoWindow()
        {
            if (videoWindow == null)
            {
                videoWindow = new VideoWindow();
                videoWindow.FullScreen();
                videoWindow.Show();

                IntPtr workerWindowHandle = DesktopWorker.GetWorkerWindowHandle();
                PInvoke.SetParent(videoWindow.Handle, workerWindowHandle);

                button3.Enabled = true;
                button4.Enabled = true;
                button5.Enabled = true;
                checkBox1.Enabled = true;
                trackBar1.Enabled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "MP4 Files (*.mp4)|*.mp4";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                CreateVideoWindow();
                videoWindow.Source = new Uri(openFileDialog.FileName, UriKind.Absolute);
                videoWindow.Play();
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

        private void button5_Click(object sender, EventArgs e)
        {
            videoWindow.Close();
            videoWindow = null;

            button3.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;
            checkBox1.Enabled = false;
            trackBar1.Enabled = false;
        }
    }
}
