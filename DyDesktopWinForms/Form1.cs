using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PInvoke.User32;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

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

            CheckStartOnBoot();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();

                if (!File.Exists(".firstRun"))
                {
                    notifyIcon1.ShowBalloonTip(1000, "","程序正在后台运行", ToolTipIcon.None);
                    File.WriteAllText(".firstRun", "");
                }
            }
            else
            {
                restoreDesktop();
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

        private void restoreDesktop()
        {
            IDesktopWallpaper desktopWallpaper = (IDesktopWallpaper)new DesktopWallpaper();
            desktopWallpaper.GetWallpaper(null, out StringBuilder wallpaper);

            if (wallpaper.Length!=0)
            {
                desktopWallpaper.SetWallpaper(null, wallpaper.ToString());
            }
            else
            {
                desktopWallpaper.GetBackgroundColor(out uint c);
                desktopWallpaper.SetBackgroundColor(c);
            }
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

            restoreDesktop();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.Activate();
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

        private void toolStripMenuItem7_DropDownOpening(object sender, EventArgs e)
        {
            toolStripMenuItem7.DropDownItems.Clear();
            foreach (string item in _recentFiles)
            {
                ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(item);
                toolStripMenuItem.Click += ToolStripMenuItem_Click;
                toolStripMenuItem7.DropDownItems.Add(toolStripMenuItem);
            }

            toolStripMenuItem8.Enabled = _recentFiles.Count != 0;
            toolStripMenuItem7.DropDownItems.Add(toolStripMenuItem8);
        }

        private void ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFile(((ToolStripMenuItem)sender).Text);
        }

        private void toolStripMenuItem8_Click(object sender, EventArgs e)
        {
            _recentFiles.Clear();
            File.WriteAllText(_recentPath, "");
        }

        private void toolStripMenuItem9_Click(object sender, EventArgs e)
        {
            button2_Click(null, null);
        }

        int idx;
        private void toolStripMenuItem10_DropDownOpening(object sender, EventArgs e)
        {
            toolStripMenuItem10.DropDownItems.Clear();

            Screen[] allScreens = Screen.AllScreens;
            for (int i = 0; i < allScreens.Length; i++)
            {
                ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(allScreens[i].Primary?allScreens[i].DeviceName+ " - Primary" : allScreens[i].DeviceName);
                toolStripMenuItem.Checked = idx == i;
                toolStripMenuItem.Tag = i;
                toolStripMenuItem.Click += ToolStripMenuItem2_Click;
                toolStripMenuItem10.DropDownItems.Add(toolStripMenuItem);
            }
        }

        private void ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem item in toolStripMenuItem10.DropDownItems)
            {
                item.Checked = false;
            }

            idx = (int)((ToolStripMenuItem)sender).Tag;

            Screen[] allScreens = Screen.AllScreens;
            if (allScreens.Length>idx)
            {
                restoreDesktop();

                Rectangle bounds = allScreens[idx].Bounds;
                videoWindow.SetScreen(bounds.X, bounds.Y, bounds.Width, bounds.Height);
            }
        }

         const string registryStartupLocation = @"Software\Microsoft\Windows\CurrentVersion\Run";

         bool startOnBoot;

        public  void CheckStartOnBoot()
        {
            Microsoft.Win32.RegistryKey startupKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(registryStartupLocation);
            startOnBoot = startupKey.GetValue("DyDesktopWinForms") != null;
            startupKey.Close();

           toolStripMenuItem12.Checked = startOnBoot;
        }

        public  void ToggleStartOnBoot()
        {
            Microsoft.Win32.RegistryKey startupKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(registryStartupLocation, true);

            if (!startOnBoot)
            {
                startupKey.SetValue("DyDesktopWinForms", Application.ExecutablePath);
                startOnBoot = true;
            }
            else
            {
                startupKey.DeleteValue("DyDesktopWinForms");
                startOnBoot = false;
            }

           toolStripMenuItem12.Checked = startOnBoot;
        }

        private void toolStripMenuItem12_Click(object sender, EventArgs e)
        {
            ToggleStartOnBoot();
        }

        private void toolStripMenuItem13_Click(object sender, EventArgs e)
        {
            toolStripMenuItem13.Checked = !toolStripMenuItem13.Checked;
            timer1.Enabled = toolStripMenuItem13.Checked;

            if (!_isPlaying)
            {
                _isPlaying = true;
                videoWindow.Play();
            }
        }

        int playCount;
        int pauseCount;
        private void timer1_Tick(object sender, EventArgs e)
        {
            LASTINPUTINFO lii = LASTINPUTINFO.Create();
            GetLastInputInfo(out lii);
            long ti = Environment.TickCount - lii.dwTime;

           bool pStatus = ti > 500;
            if (pStatus)
            {
                pauseCount = 0;
                ++playCount;
            }
            else
            {
                playCount = 0;
                ++pauseCount;
            }

            if (playCount>9||pauseCount>1)
            {
                playCount = 0;
                pauseCount = 0;
                if (pStatus != _isPlaying)
                {
                    button4_Click(null, null);
                }
            }
        }
    }
}
