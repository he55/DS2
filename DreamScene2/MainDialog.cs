﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DreamScene2
{
    public partial class MainDialog : Form
    {
        VideoWindow _videoWindow;
        WebWindow _webWindow;
        IntPtr _desktopWindowHandle;
        string _recentPath;
        List<string> _recentFiles;
        bool _isPlaying;
        PerformanceCounter _performanceCounter;
        Settings _settings = Settings.Load();
        Screen _screen = Screen.PrimaryScreen;
        int _screenIndex;
        IntPtr _windowHandle;

        public MainDialog()
        {
            InitializeComponent();
            this.Icon = DreamScene2.Properties.Resources.ico3;
            notifyIcon1.Icon = this.Icon;
            trackBar1.Value = _settings.Volume;
            toolStripMenuItem13.Checked = _settings.AutoPause;
            checkBox1.Checked = _settings.IsMuted;
            toolStripMenuItem3.Checked = _settings.IsMuted;
        }


        #region MyRegion

        private void CreateVideoWindow()
        {
            if (_videoWindow == null)
            {
                _videoWindow = new VideoWindow();
                _videoWindow.IsMuted = _settings.IsMuted;
                _videoWindow.Volume = _settings.Volume / 10.0;
                _videoWindow.SetPosition(_screen.Bounds);
                _videoWindow.Show();

                PInvoke.SetParent(_videoWindow.GetHandle(), _desktopWindowHandle);

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
                _videoWindow.Pause();
                button4.Text = "播放";
                toolStripMenuItem2.Text = "播放";
            }
            else
            {
                _isPlaying = true;
                _videoWindow.Play();
                button4.Text = "暂停";
                toolStripMenuItem2.Text = "暂停";
            }
        }

        private void CloseVideo()
        {
            timer1.Enabled = false;
            _videoWindow.Close();
            _videoWindow = null;
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

            PInvoke.reWall();
        }

        private void closeweb()
        {
            _webWindow.Close();
            _webWindow = null;
            GC.Collect();

            PInvoke.reWall();
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

        private void OpenFile(string filepath)
        {
            Uri uri = new Uri(filepath);
            if (uri.Scheme == "file" && File.Exists(filepath))
            {
                if (Path.GetExtension(filepath) == ".html")
                {
                    openweb(filepath);
                }
                else
                {
                    openvideo(filepath);
                }
            }
            else
            {
                openweb(filepath);
            }
        }

        private void openvideo(string path)
        {
            if (_webWindow != null)
            {
                closeweb();
            }

            RestoreDesktop();
            timer1.Enabled = false;
            SaveRecent(path);

            CreateVideoWindow();

            _videoWindow.Source = new Uri(path, UriKind.Absolute);
            _videoWindow.Play();

            _isPlaying = true;
            button4.Text = "暂停";
            toolStripMenuItem2.Text = "暂停";
            timer1.Enabled = _settings.AutoPause;
        }

        private void RestoreDesktop()
        {
            _windowHandle = IntPtr.Zero;
            PInvoke.reLastPos();
            PInvoke.reWall();
        }

        #endregion


        #region MyRegion

        private void Form1_Load(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                _performanceCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            });

            _desktopWindowHandle = PInvoke.getC();
            if (_desktopWindowHandle == IntPtr.Zero)
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

                if (_recentFiles.Count != 0)
                {
                    OpenFile(_recentFiles[0]);
                }
            }

            toolStripMenuItem12.Checked = Helper.CheckStartOnBoot();
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
                _videoWindow?.Close();
                _webWindow?.Close();
                RestoreDesktop();

                Settings.Save();
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
                openvideo(openFileDialog.FileName);
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
            _videoWindow.IsMuted = _settings.IsMuted;
        }

        private void checkBox2_Click(object sender, EventArgs e)
        {
            _settings.AutoPlay = checkBox2.Checked;
            toolStripMenuItem6.Checked = _settings.AutoPlay;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            _settings.Volume = trackBar1.Value;
            _videoWindow.Volume = _settings.Volume / 10.0;
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
                string filePath = _recentFiles[i];
                //string v = filePath.Truncate(50);
                ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem($"{i + 1}. {filePath}");
                toolStripMenuItem.Tag = filePath;
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
                Helper.SetStartOnBoot();
            }
            else
            {
                Helper.RemoveStartOnBoot();
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
            AboutDialog aboutDialog = new AboutDialog();
            aboutDialog.Show();
        }

        private void toolStripMenuItem10_DropDownOpening(object sender, EventArgs e)
        {
            toolStripMenuItem10.DropDownItems.Clear();

            Screen[] allScreens = Screen.AllScreens;
            for (int i = 0; i < allScreens.Length; i++)
            {
                ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(allScreens[i].Primary ? allScreens[i].DeviceName + " - Primary" : allScreens[i].DeviceName);
                toolStripMenuItem.Checked = _screenIndex == i;
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
            _screenIndex = (int)((ToolStripMenuItem)sender).Tag;
            _screen = Screen.AllScreens[_screenIndex];

            PInvoke.reWall();
            _videoWindow?.SetPosition(_screen.Bounds);
        }

        private void toolStripMenuItem16_DropDownOpening(object sender, EventArgs e)
        {
            toolStripMenuItem16.DropDownItems.Clear();

            string[] files = Directory.GetFiles(Helper.ExtPath());
            foreach (string filePath in files)
            {
                string fileName = Path.GetFileName(filePath);
                string[] arr = fileName.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
                if (int.TryParse(arr[0], System.Globalization.NumberStyles.HexNumber, null, out int val))
                {
                    IntPtr ptr = (IntPtr)val;
                    bool b = PInvoke.IsWindowVisible(ptr);
                    ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(arr[1] + (b ? "" : " (Invalidate)"));
                    toolStripMenuItem.Enabled = b;
                    toolStripMenuItem.Checked = _windowHandle == ptr;
                    toolStripMenuItem.Tag = val;
                    toolStripMenuItem.Click += ToolStripMenuItem23_Click;
                    toolStripMenuItem16.DropDownItems.Add(toolStripMenuItem);
                }
            }

            if (files.Length == 0)
            {
                toolStripMenuItem16.DropDownItems.Add(toolStripMenuItem17);
            }
        }

        private void ToolStripMenuItem23_Click(object sender, EventArgs e)
        {
            int hWnd = (int)((ToolStripMenuItem)sender).Tag;
            setwindow((IntPtr)hWnd);
        }

        private void setwindow(IntPtr hWnd)
        {
            if (_windowHandle != hWnd)
            {
                _windowHandle = hWnd;

                if (_videoWindow != null)
                {
                    CloseVideo();
                }

                PInvoke.reLastPos();
                PInvoke.setPos(hWnd, _screen.Bounds.ToRECT());
                PInvoke.SetParent(hWnd, _desktopWindowHandle);
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
            if (PInvoke.getB2(_screen.WorkingArea.ToRECT()) == 0)
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

            if (PInvoke.getA() > 500)
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
            InputDialog inputDialog = new InputDialog();
            if (inputDialog.ShowDialog() == DialogResult.OK)
            {
                openweb(inputDialog.URL);
            }
        }

        private void openweb(string url)
        {
            SaveRecent(url);

            if (_videoWindow != null)
            {
                CloseVideo();
            }

            if (_windowHandle != IntPtr.Zero)
            {
                RestoreDesktop();
            }

            if (_webWindow == null)
            {
                _webWindow = new WebWindow();
                _webWindow.SetPosition(_screen.Bounds);
                _webWindow.Show();

                PInvoke.SetParent(_webWindow.GetHandle(), _desktopWindowHandle);
            }
            _webWindow.Source = new Uri(url);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == (0x0400 + 1001))
            {
                setwindow(m.WParam);
                return;
            }
            base.WndProc(ref m);
        }
    }
}
