using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Shell;
using Helios_Transpiler.Models;
using Helios_Transpiler.ViewModels;

namespace Helios_Transpiler.Views
{
    public partial class StartWindow : Window
    {
        private readonly StartViewModel _vm;

        // ── P/Invoke ──────────────────────────────────────────────────────────
        [DllImport("user32.dll")] private static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);
        [DllImport("user32.dll")] private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);
        [DllImport("user32.dll")] private static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);
        private const uint MONITOR_DEFAULTTONEAREST = 2;
        [StructLayout(LayoutKind.Sequential)] private struct RECT { public int Left, Top, Right, Bottom; }
        private struct MONITORINFO { public int cbSize; public RECT rcMonitor; public RECT rcWork; public uint dwFlags; }

        // ── Ctor ──────────────────────────────────────────────────────────────
        public StartWindow()
        {
            InitializeComponent();

            var chrome = new WindowChrome
            {
                CaptionHeight = 0,
                ResizeBorderThickness = new Thickness(6),
                GlassFrameThickness = new Thickness(0),
                UseAeroCaptionButtons = false,
                NonClientFrameEdges = NonClientFrameEdges.None
            };
            WindowChrome.SetWindowChrome(this, chrome);

            _vm = new StartViewModel();
            _vm.ProjectOpenRequested += OnProjectOpenRequested;
            _vm.NewProjectRequested += OnNewProjectRequested;
            DataContext = _vm;

            Loaded += (_, _) => StateChanged += OnStateChanged;
        }

        // ── Maximised margin fix ──────────────────────────────────────────────
        private void OnStateChanged(object? sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                Dispatcher.BeginInvoke(
                    () => RootBorder.Margin = GetMaximizedOverhang());
            else
                RootBorder.Margin = new Thickness(0);
        }

        private Thickness GetMaximizedOverhang()
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            GetWindowRect(hwnd, out RECT winRect);
            var hMon = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);
            var mi = new MONITORINFO { cbSize = Marshal.SizeOf<MONITORINFO>() };
            GetMonitorInfo(hMon, ref mi);
            var work = mi.rcWork;
            int left = work.Left - winRect.Left;
            int top = work.Top - winRect.Top;
            int right = winRect.Right - work.Right;
            int bottom = winRect.Bottom - work.Bottom;
            var source = PresentationSource.FromVisual(this);
            if (source?.CompositionTarget == null) return new Thickness(left, top, right, bottom);
            var m = source.CompositionTarget.TransformFromDevice;
            return new Thickness(left * m.M11, top * m.M22, right * m.M11, bottom * m.M22);
        }

        // ── Custom chrome ────────────────────────────────────────────────────
        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2) { MaximizeBtn_Click(sender, e); return; }
            DragMove();
        }

        private void MinimizeBtn_Click(object sender, RoutedEventArgs e)
            => WindowState = WindowState.Minimized;

        private void MaximizeBtn_Click(object sender, RoutedEventArgs e)
            => WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
            => Close();

        // ── Alt+S focuses search ─────────────────────────────────────────────
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.S && e.KeyboardDevice.Modifiers == ModifierKeys.Alt)
            {
                SearchBox.Focus();
                SearchBox.SelectAll();
                e.Handled = true;
            }
        }

        // ── Double-click row ─────────────────────────────────────────────────
        private void List_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_vm.SelectedRecent is RecentProject rp)
                _vm.OpenRecentCommand.Execute(rp);
        }

        // ── Window transitions ───────────────────────────────────────────────
        private void OnProjectOpenRequested(HdxProject project)
        {
            var main = new MainWindow(project);
            main.Show();
            Close();
        }

        private void ContinueWithoutCode_Click(object sender, RoutedEventArgs e)
        {
            var main = new MainWindow(null);
            main.Show();
            Close();
        }

        private void OnNewProjectRequested()
        {
            MessageBox.Show(
                "New project creation will be available in a future update.",
                "Helios-DLX",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}