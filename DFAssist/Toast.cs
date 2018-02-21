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
        private readonly FormAnimator _animator;
        private readonly ConcurrentDictionary<int, ProcessNet> _networks;

        private bool _allowFocus;
        private Timer _timer;
        private IntPtr _currentForegroundWindow;

        public Toast(string title, string message, ConcurrentDictionary<int, ProcessNet> networks)
        {
            InitializeComponent();

            _title = title;
            _message = message;
            _networks = networks;
            _animator = new FormAnimator(this, FormAnimator.AnimationMethod.Slide, FormAnimator.AnimationDirection.Left, 500);

            ShowInTaskbar = false;
            FormBorderStyle = FormBorderStyle.None;
            TopMost = true;
        }

        public new void Show()
        {
            // Determine the current foreground window so it can be reactivated each time this form tries to get the focus
            _currentForegroundWindow = NativeMethods.GetForegroundWindow();

            base.Show();
        }

        private void StartTimer()
        {
            _timer = new Timer {Interval = 10000};
            _timer.Tick += TimerOnTick;
            _timer.Start();
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

        #region Events
        private void TimerOnTick(object sender, EventArgs eventArgs)
        {
            Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Toast_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Toast_Load(object sender, EventArgs e)
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

            SystemSounds.Exclamation.Play();
            StartTimer();
        }

        private void Toast_Activated(object sender, EventArgs e)
        {
            if (!_allowFocus)
            {
                // Activate the window that previously had focus
                NativeMethods.SetForegroundWindow(_currentForegroundWindow);
            }
        }

        private void Toast_Shown(object sender, EventArgs e)
        {
            // Once the animation has completed the form can receive focus
            _allowFocus = true;

            // Close the form by sliding down.
            _animator.Duration = 0;
            _animator.Direction = FormAnimator.AnimationDirection.Down;
        }
        #endregion
    }
}
