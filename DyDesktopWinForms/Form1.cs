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
        private bool _isPlaying;
       private IntPtr workerWindowHandle ;


        public Form1()
        {
            InitializeComponent();
            notifyIcon1.Icon = this.Icon;
            this.MaximumSize = this.MinimumSize = this.Size;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
             workerWindowHandle = DesktopWorker.GetWorkerWindowHandle();
            if (workerWindowHandle==IntPtr.Zero)
            {
                button2.Enabled = false;
                label1.Visible = true;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        private void CreateVideoWindow()
        {
            if (videoWindow == null)
            {
                videoWindow = new VideoWindow();
                videoWindow.IsMuted = checkBox1.Checked;
                videoWindow.Volume = trackBar1.Value / 10.0;
                videoWindow.FullScreen();
                videoWindow.Show();

                PInvoke.SetParent(videoWindow.Handle, workerWindowHandle);

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

                _isPlaying = true;
                button4.Text = "暂停";
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (_isPlaying)
            {
                _isPlaying = false;
                videoWindow.Pause();
                button4.Text = "播放";
            }
            else
            {
                _isPlaying = true;
                videoWindow.Play();
                button4.Text = "暂停";
            }
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

            _isPlaying = false;
            button4.Text = "播放";

            button4.Enabled = false;
            button5.Enabled = false;
            checkBox1.Enabled = false;
            trackBar1.Enabled = false;
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Show();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
