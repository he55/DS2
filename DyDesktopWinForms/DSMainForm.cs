using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        Screen _screen = Screen.PrimaryScreen;

        public DSMainForm()
        {
            InitializeComponent();
            notifyIcon1.Icon = this.Icon;
            this.MaximumSize = this.MinimumSize = this.Size;
            trackBar1.Value = _settings.Volume;
            toolStripMenuItem13.Checked = _settings.AutoPause;
            checkBox1.Checked = _settings.IsMuted;
            toolStripMenuItem3.Checked = _settings.IsMuted;
        }


        #region MyRegion

        private void CreateVideoWindow()
        {
            if (videoWindow == null)
            {
                videoWindow = new DSVideoWindow();
                videoWindow.IsMuted = _settings.IsMuted;
                videoWindow.Volume = _settings.Volume / 10.0;
                videoWindow.SetPosition(_screen.Bounds);
                videoWindow.Show();

                DSPInvoke.SetParent(videoWindow.GetHandle(), workerWindowHandle);

                button4.Enabled = true;
                button5.Enabled = true;
                checkBox1.Enabled = true;
                trackBar1.Enabled = !_settings.IsMuted;

                toolStripMenuItem2.Enabled = true;
                toolStripMenuItem3.Enabled = true;
                toolStripMenuItem5.Enabled = true;
            }
        }

        private void PlayOrPauseVideo()
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

        private void CloseVideo()
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

            DSPInvoke.reWall();
        }

        private void SaveRecent(string path)
        {
            if (_recentFiles.Count == 0 || _recentFiles[0] != path)
            {
                if (_recentFiles.Contains(path))
                {
                    _recentFiles.Remove(path);
                }
                _recentFiles.Insert(0, path);

                File.WriteAllLines(_recentPath, _recentFiles);
            }
        }

        private void OpenFile(string path)
        {
            RestoreDesktop();
            timer1.Enabled = false;
            SaveRecent(path);

            CreateVideoWindow();

            videoWindow.Source = new Uri(path, UriKind.Absolute);
            videoWindow.Play();

            _isPlaying = true;
            button4.Text = "暂停";
            toolStripMenuItem2.Text = "暂停";
            timer1.Enabled = _settings.AutoPause;
        }

        private void RestoreDesktop()
        {
            hhw = 0;
            DSPInvoke.reLastPos();
            DSPInvoke.reWall();
        }

        #endregion


        #region MyRegion

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
                    OpenFile(_recentFiles[0]);
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
                timer1.Enabled = false;
                videoWindow?.Close();
                RestoreDesktop();

                DSSettings.Save();
            }
        }

        #endregion


        #region MyRegion

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Video Files (*.mp4;*.mov)|*.mp4;*.mov";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                OpenFile(openFileDialog.FileName);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            PlayOrPauseVideo();
            timer1.Enabled = _settings.AutoPause && _isPlaying;
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
            CloseVideo();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.Activate();
        }

        #endregion


        #region MyRegion

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            notifyIcon1_MouseDoubleClick(null, null);
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            PlayOrPauseVideo();
            timer1.Enabled = _settings.AutoPause && _isPlaying;
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
            CloseVideo();
        }

        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            checkBox2.Checked = !checkBox2.Checked;
            checkBox2_Click(null, null);
        }

        private void toolStripMenuItem7_DropDownOpening(object sender, EventArgs e)
        {
            toolStripMenuItem7.DropDownItems.Clear();
            for (int i = 0; i < _recentFiles.Count; i++)
            {
                string item = _recentFiles[i];
                string v = Path.GetFileName(item);
                ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem($"{i + 1}. {v}");
                toolStripMenuItem.Tag = item;
                toolStripMenuItem.Click += ToolStripMenuItem_Click;
                toolStripMenuItem7.DropDownItems.Add(toolStripMenuItem);
            }

            toolStripMenuItem8.Enabled = _recentFiles.Count != 0;
            toolStripMenuItem7.DropDownItems.Add(toolStripMenuItem8);
        }

        private void ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFile((string)((ToolStripMenuItem)sender).Tag);
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

        private void toolStripMenuItem12_Click(object sender, EventArgs e)
        {
            toolStripMenuItem12.Checked = !toolStripMenuItem12.Checked;
            if (toolStripMenuItem12.Checked)
            {
                DSHelper.StToggleStartOnBoot();
            }
            else
            {
                DSHelper.DelToggleStartOnBoot();
            }
        }

        private void toolStripMenuItem13_Click(object sender, EventArgs e)
        {
            _settings.AutoPause = !toolStripMenuItem13.Checked;
            toolStripMenuItem13.Checked = _settings.AutoPause;
            timer1.Enabled = _settings.AutoPause;

            if (!_isPlaying)
            {
                PlayOrPauseVideo();
            }
            else
            {
                timer1.Enabled = true;
            }
        }

        private void toolStripMenuItem14_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
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

            if (allScreens.Length == 0)
            {
                toolStripMenuItem10.DropDownItems.Add(toolStripMenuItem11);
            }
        }

        private void ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem item in toolStripMenuItem10.DropDownItems)
            {
                item.Checked = false;
            }

            idx = (int)((ToolStripMenuItem)sender).Tag;
            _screen = Screen.AllScreens[idx];

            DSPInvoke.reWall();
            videoWindow?.SetPosition(_screen.Bounds);
        }

        int hhw;
        private void toolStripMenuItem16_DropDownOpening(object sender, EventArgs e)
        {
            toolStripMenuItem16.DropDownItems.Clear();

            string[] vs = Directory.GetFiles(DSHelper.met());
            foreach (string item in vs)
            {
                string v2 = Path.GetFileName(item);
                string[] vs1 = v2.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
                if (int.TryParse(vs1[0], System.Globalization.NumberStyles.HexNumber, null, out int val))
                {
                    bool v = DSPInvoke.IsWindowVisible((IntPtr)val);
                    ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(vs1[1] + (v ? "" : " (Invalidate)"));
                    toolStripMenuItem.Enabled = v;
                    toolStripMenuItem.Checked = hhw == val;
                    toolStripMenuItem.Tag = val;
                    toolStripMenuItem.Click += ToolStripMenuItem23_Click;
                    toolStripMenuItem16.DropDownItems.Add(toolStripMenuItem);
                }
            }

            if (vs.Length == 0)
            {
                toolStripMenuItem16.DropDownItems.Add(toolStripMenuItem17);
            }
        }

        private void ToolStripMenuItem23_Click(object sender, EventArgs e)
        {
            if (videoWindow != null)
            {
                CloseVideo();
            }

            int hw = (int)((ToolStripMenuItem)sender).Tag;
            if (hhw != hw)
            {
                hhw = hw;

                foreach (ToolStripMenuItem item in toolStripMenuItem16.DropDownItems)
                {
                    item.Checked = false;
                }

                DSPInvoke.reLastPos();
                IntPtr ptr = (IntPtr)hhw;
                DSPInvoke.setPos(ptr, _screen.Bounds.ToRECT());
                DSPInvoke.SetParent(ptr, workerWindowHandle);
            }
            else
            {
                RestoreDesktop();
            }
        }

        #endregion


        #region MyRegion

        int cplayCount;
        int cpauseCount;
        int playCount;
        int pauseCount;
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (DSPInvoke.getB2(_screen.WorkingArea.ToRECT()) == 0)
            {
                xplayMethod();
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
                xplayMethod();
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
                xplayMethod();
                return;
            }

            if (cplayCount > 4 && playCount > 4)
            {
                xplayMethod();
            }
        }

        private void xplayMethod()
        {
            cplayCount = 0;
            cpauseCount = 0;
            playCount = 0;
            pauseCount = 0;
            if (!_isPlaying)
            {
                PlayOrPauseVideo();
            }
        }

        #endregion

        private void toolStripMenuItem18_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            if (form2.ShowDialog() == DialogResult.OK)
            {

            }
        }
    }
}
