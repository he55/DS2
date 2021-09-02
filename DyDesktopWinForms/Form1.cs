using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DyDesktopWinForms
{
    public partial class Form1 : Form
    {
        private VideoWindow videoWindow;
        private bool _isPlaying;
       private IntPtr workerWindowHandle ;
        private string _recentPath;
        private List<string> _recentFiles;
       private PerformanceCounter cpu;
       private MySettings settings=MySettings.Load();

        public Form1()
        {
            InitializeComponent();
            notifyIcon1.Icon = this.Icon;
            this.MaximumSize = this.MinimumSize = this.Size;
            trackBar1.Value = settings.Volume;
            toolStripMenuItem13.Checked = settings.AutoPause;
            checkBox1.Checked = settings.IsMuted;
            toolStripMenuItem3.Checked = settings.IsMuted;
           trackBar1.Enabled = !settings.IsMuted;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cpu = new PerformanceCounter("Processor", "% Processor Time", "_Total");

            workerWindowHandle =PClass1.getC();
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

            if (settings.AutoPlay)
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

                if (settings.FirstRun)
                {
                    notifyIcon1.ShowBalloonTip(1000, "","程序正在后台运行", ToolTipIcon.None);
                    settings.FirstRun = false;
                }
            }
            else
            {
                MySettings.Save();
                restoreDesktop();
            }
        }

        private void CreateVideoWindow()
        {
            if (videoWindow == null)
            {
                videoWindow = new VideoWindow();
                videoWindow.IsMuted =settings.IsMuted;
                videoWindow.Volume =settings.Volume / 10.0;
                videoWindow.FullScreen();
                videoWindow.Show();

               PClass1.SetParent(videoWindow.Handle, workerWindowHandle);

                button4.Enabled = true;
                button5.Enabled = true;
                checkBox1.Enabled = true;

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
            timer1.Enabled = false;
            saveRecent(path);

            CreateVideoWindow();

            videoWindow.Source = new Uri(path, UriKind.Absolute);
            videoWindow.Play();

            _isPlaying = true;
            button4.Text = "暂停";
            toolStripMenuItem2.Text = "暂停";
            timer1.Enabled = settings.AutoPause;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Video Files (*.mp4;*.mov)|*.mp4;*.mov";
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

            if (sender != null)
            {
                timer1.Enabled = settings.AutoPause && _isPlaying;
            }
        }

        private void checkBox1_Click(object sender, EventArgs e)
        {
            settings.IsMuted = checkBox1.Checked;
            toolStripMenuItem3.Checked =settings.IsMuted;
            trackBar1.Enabled = !settings.IsMuted;
            videoWindow.IsMuted = settings.IsMuted;
        }

        private void checkBox2_Click(object sender, EventArgs e)
        {
            settings.AutoPlay = checkBox2.Checked;
            toolStripMenuItem6.Checked =settings.AutoPlay;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            settings.Volume = trackBar1.Value;
            videoWindow.Volume = settings.Volume / 10.0;
        }
     
        private void restoreDesktop()
        {
            PClass1.getD();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
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
            notifyIcon1_MouseDoubleClick(null, null);
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            button4_Click(sender, null);
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
                startupKey.SetValue("DyDesktopWinForms", Application.ExecutablePath+" -c");
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
           settings.AutoPause = !toolStripMenuItem13.Checked;
            toolStripMenuItem13.Checked = settings.AutoPause;
            timer1.Enabled = settings.AutoPause;

            if (!_isPlaying)
            {
                button4_Click(null, null);
            }
            else
            {
                timer1.Enabled = true;
            }
        }

        int cplayCount;
        int cpauseCount;
        int playCount;
        int pauseCount;
        private void timer1_Tick(object sender, EventArgs e)
        {
            int ttt = PClass1.getB();
            if (ttt==0)
            {
                cplayCount = 0;
                cpauseCount = 0;
                playCount = 0;
                pauseCount = 0;
                if (_isPlaying)
                {
                    button4_Click(null, null);
                }
                return;
            }

            float val = cpu.NextValue();
            bool cpStatus =val>15.0;
            if (cpStatus)
            {
                cplayCount = 0;
                ++cpauseCount;
            }
            else
            {
                cpauseCount = 0;
                ++cplayCount;
            }

            if (cpauseCount > 4)
            {
                cplayCount = 0;
                cpauseCount = 0;
                playCount = 0;
                pauseCount = 0;
                if (_isPlaying)
                {
                    button4_Click(null, null);
                }
                return;
            }

            ulong ti = PClass1.getA();
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

            if (pauseCount > 4)
            {
                cplayCount = 0;
                cpauseCount = 0;
                playCount = 0;
                pauseCount = 0;
                if (_isPlaying)
                {
                    button4_Click(null, null);
                }
                return;
            }

            if (cplayCount > 4&& playCount > 4)
            {
                cplayCount = 0;
                cpauseCount = 0;
                playCount = 0;
                pauseCount = 0;
                if (!_isPlaying)
                {
                    button4_Click(null, null);
                }
            }
        }

      
    }
}
