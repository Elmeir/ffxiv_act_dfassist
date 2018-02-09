using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Windows.Forms;

namespace DFAssist
{
    public partial class Toast : Form
    {
        private readonly string _title;
        private readonly string _message;
        private readonly ConcurrentDictionary<int, ProcessNet> _networks;

        private Timer _timer;

        public Toast(string title, string message, ConcurrentDictionary<int, ProcessNet> networks)
        {
            _title = title;
            _message = message;
            _networks = networks;
            InitializeComponent();

            ShowInTaskbar = false;
            FormBorderStyle = FormBorderStyle.None;
            TopMost = true;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Close();
        }

        protected override void OnLoad(EventArgs e)
        {
            PlaceLowerRight();
            BackColor = DefaultForeColor;
            ForeColor = DefaultBackColor;

            label1.Font = new Font("Serif", 18, FontStyle.Bold);
            label1.ForeColor = Color.White;
            label2.Font = new Font("Serif", 16);
            label2.ForeColor = Color.Gray;

            label1.Text = _title;
            label2.Text = _message;

            base.OnLoad(e);
            SystemSounds.Exclamation.Play();
            StartTimer();
        }

        private void StartTimer()
        {
            _timer = new Timer {Interval = 10000};
            _timer.Tick += TimerOnTick;
            _timer.Start();
        }

        private void TimerOnTick(object sender, EventArgs eventArgs)
        {
            Close();
        }

        private void PlaceLowerRight()
        {
            var rightmost = Screen.PrimaryScreen;
            if (!_networks.Any())
            {
                foreach (var screen in Screen.AllScreens)
                {
                    if (screen.WorkingArea.Right > rightmost.WorkingArea.Right)
                        rightmost = screen;
                }
            }
            else
            {
                var processMainWindowHandle = _networks.Values.First().Process.MainWindowHandle;
                rightmost = Screen.FromHandle(processMainWindowHandle);
            }

            Left = rightmost.WorkingArea.Right - Width;
            Top = rightmost.WorkingArea.Bottom - (Height + 50);
        }
    }
}
