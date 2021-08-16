﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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
        private string _recentPath;
        private List<string> _recentFiles;


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

            
            _recentFiles = new List<string>();
            //_recentPath = Path.Combine(Application.UserAppDataPath, "recent.txt");
            _recentPath = "recent.txt";
            if (File.Exists(_recentPath))
            {
                string[] paths = File.ReadAllLines(_recentPath);
                _recentFiles.AddRange(paths);
            }

            if (File.Exists(".autoPlay"))
            {
                checkBox2.Checked = true;
                toolStripMenuItem6.Checked = true;

                if (_recentFiles.Count!=0&&File.Exists(_recentFiles[0]))
                {
                    openFile(_recentFiles[0]);
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();

                if (!File.Exists(".firstRun"))
                {
                    notifyIcon1.ShowBalloonTip(1000, "", "窗口已经隐藏到托盘", ToolTipIcon.None);
                    File.WriteAllText(".firstRun", "");
                }
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

                toolStripMenuItem2.Enabled = true;
                toolStripMenuItem3.Enabled = true;
                toolStripMenuItem5.Enabled = true;
            }
        }

        private void saveRecent(string filePath)
        {
            if (_recentFiles.Count!=0&&_recentFiles[0]==filePath)
            {
                return;
            }

            if (_recentFiles.Contains(filePath))
            {
                _recentFiles.Remove(filePath);
            }
            _recentFiles.Insert(0, filePath);

            File.WriteAllLines(_recentPath, _recentFiles);
        }

        private void openFile(string path)
        {
            saveRecent(path);

            CreateVideoWindow();

            videoWindow.Source = new Uri(path, UriKind.Absolute);
            videoWindow.Play();

            _isPlaying = true;
            button4.Text = "暂停";
            toolStripMenuItem2.Text = "暂停";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "MP4 Files (*.mp4)|*.mp4";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                openFile(openFileDialog.FileName);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (_isPlaying)
            {
                _isPlaying = false;
                videoWindow.Pause();
                button4.Text = "播放";
                toolStripMenuItem2.Text = "播放";
            }
            else
            {
                _isPlaying = true;
                videoWindow.Play();
                button4.Text = "暂停";
                toolStripMenuItem2.Text = "暂停";
            }
        }

        private void checkBox1_Click(object sender, EventArgs e)
        {
            toolStripMenuItem3.Checked = checkBox1.Checked;
            trackBar1.Enabled = !checkBox1.Checked;
            videoWindow.IsMuted = checkBox1.Checked;
        }

        private void checkBox2_Click(object sender, EventArgs e)
        {
            toolStripMenuItem6.Checked = checkBox2.Checked;

            if (checkBox2.Checked)
            {
                File.WriteAllText(".autoPlay", "");
            }
            else
            {
                File.Delete(".autoPlay");
            }
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            videoWindow.Volume = trackBar1.Value / 10.0;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            videoWindow.Close();
            videoWindow = null;
            GC.Collect();

            _isPlaying = false;
            button4.Text = "播放";
            toolStripMenuItem2.Text = "播放";

            button4.Enabled = false;
            button5.Enabled = false;
            checkBox1.Enabled = false;
            trackBar1.Enabled = false;

            toolStripMenuItem2.Enabled = false;
            toolStripMenuItem3.Enabled = false;
            toolStripMenuItem5.Enabled = false;
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Show();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            button4_Click(null, null);
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            checkBox1.Checked = !checkBox1.Checked;
            checkBox1_Click(null, null);
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            button5_Click(null, null);
        }

        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            checkBox2.Checked = !checkBox2.Checked;
            checkBox2_Click(null, null);
        }
    }
}
