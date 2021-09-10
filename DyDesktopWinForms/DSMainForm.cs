using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DyDesktopWinForms
{
    public partial class DSMainForm : Form
    {
        DSVideoWindow videoWindow;
        bool _isPlaying;
        IntPtr workerWindowHandle;
        string _recentPath;
        List<string> _recentFiles;
        PerformanceCounter _performanceCounter;
        DSSettings _settings = DSSettings.Load();

        public DSMainForm()
        {
            InitializeComponent();
            notifyIcon1.Icon = this.Icon;
            this.MaximumSize = this.MinimumSize = this.Size;
            trackBar1.Value = _settings.Volume;
            toolStripMenuItem13.Checked = _settings.AutoPause;
            checkBox1.Checked = _settings.IsMuted;
            toolStripMenuItem3.Checked = _settings.IsMuted;
            trackBar1.Enabled = !_settings.IsMuted;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                _performanceCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            });

            workerWindowHandle = DSPInvoke.getC();
            if (workerWindowHandle == IntPtr.Zero)
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

            if (_settings.AutoPlay)
            {
                checkBox2.Checked = true;
                toolStripMenuItem6.Checked = true;

                if (_recentFiles.Count != 0 && File.Exists(_recentFiles[0]))
                {
                    openFile(_recentFiles[0]);
                }
            }

            toolStripMenuItem12.Checked = DSHelper.CheckStartOnBoot();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();

                if (_settings.FirstRun)
                {
                    notifyIcon1.ShowBalloonTip(1000, "", "程序正在后台运行", ToolTipIcon.None);
                    _settings.FirstRun = false;
                }
            }
            else
            {
                DSSettings.Save();
                DSPInvoke.getD();
            }
        }

        private void CreateVideoWindow()
        {
            if (videoWindow == null)
            {
                videoWindow = new DSVideoWindow();
                videoWindow.IsMuted = _settings.IsMuted;
                videoWindow.Volume = _settings.Volume / 10.0;
                videoWindow.FullScreen();
                videoWindow.Show();

                DSPInvoke.SetParent(videoWindow.Handle, workerWindowHandle);

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
            if (_recentFiles.Count != 0 && _recentFiles[0] == filePath)
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
            timer1.Enabled = _settings.AutoPause;
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
                timer1.Enabled = _settings.AutoPause && _isPlaying;
            }
        }

        private void checkBox1_Click(object sender, EventArgs e)
        {
            _settings.IsMuted = checkBox1.Checked;
            toolStripMenuItem3.Checked = _settings.IsMuted;
            trackBar1.Enabled = !_settings.IsMuted;
            videoWindow.IsMuted = _settings.IsMuted;
        }

        private void checkBox2_Click(object sender, EventArgs e)
        {
            _settings.AutoPlay = checkBox2.Checked;
            toolStripMenuItem6.Checked = _settings.AutoPlay;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            _settings.Volume = trackBar1.Value;
            videoWindow.Volume = _settings.Volume / 10.0;
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

            DSPInvoke.getD();
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
                ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(allScreens[i].Primary ? allScreens[i].DeviceName + " - Primary" : allScreens[i].DeviceName);
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
            if (allScreens.Length > idx)
            {
                DSPInvoke.getD();

                videoWindow.SetPosition(allScreens[idx].Bounds);
            }
        }

        private void toolStripMenuItem12_Click(object sender, EventArgs e)
        {
            toolStripMenuItem12.Checked = DSHelper.ToggleStartOnBoot();
        }

        private void toolStripMenuItem13_Click(object sender, EventArgs e)
        {
            _settings.AutoPause = !toolStripMenuItem13.Checked;
            toolStripMenuItem13.Checked = _settings.AutoPause;
            timer1.Enabled = _settings.AutoPause;

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
            if (DSPInvoke.getB() == 0)
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

            float val = _performanceCounter?.NextValue() ?? 0;
            if (val > 15.0)
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

            if (DSPInvoke.getA() > 500)
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

            if (cplayCount > 4 && playCount > 4)
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

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            toolStripMenuItem103_DropDownOpening(null, null);
        }

        int idx0;
        private void toolStripMenuItem103_DropDownOpening(object sender, EventArgs e)
        {
            toolStripMenuItem16.DropDownItems.Clear();

            string v = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string v1 = Path.Combine(v, ".DyDesktopWinForms");
            string[] vs = Directory.GetFiles(v1);
            foreach (string item in vs)
            {
                string[] vs1 = item.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
                if(int.TryParse(vs1[0],out int res))
                {
                    ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(vs1[1]);
                    toolStripMenuItem.Checked = idx0 == res;
                    toolStripMenuItem.Tag = res;
                    toolStripMenuItem.Click += ToolStripMenuItem23_Click;
                    toolStripMenuItem16.DropDownItems.Add(toolStripMenuItem);
                }
            }
        }

        private void ToolStripMenuItem23_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem item in toolStripMenuItem16.DropDownItems)
            {
                item.Checked = false;
            }

            idx0 = (int)((ToolStripMenuItem)sender).Tag;

            Screen[] allScreens = Screen.AllScreens;
            if (allScreens.Length > idx)
            {
                DSPInvoke.reLastPos();
                DSPInvoke.setPos((IntPtr)idx0, allScreens[idx].Bounds);
            }
        }
    }
}
