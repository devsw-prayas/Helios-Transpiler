using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Shell;
using Helios_Transpiler.Models;

namespace Helios_Transpiler.Views
{
    public partial class MainWindow : Window
    {
        private readonly HdxProject? _project;

        // ── P/Invoke ──────────────────────────────────────────────────────────
        [DllImport("user32.dll")] private static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);
        [DllImport("user32.dll")] private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);
        [DllImport("user32.dll")] private static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);
        private const uint MONITOR_DEFAULTTONEAREST = 2;
        [StructLayout(LayoutKind.Sequential)] private struct RECT { public int Left, Top, Right, Bottom; }
        private struct MONITORINFO { public int cbSize; public RECT rcMonitor; public RECT rcWork; public uint dwFlags; }

        // ── Ctor ──────────────────────────────────────────────────────────────
        public MainWindow(HdxProject? project)
        {
            _project = project;
            InitializeComponent();
            DataContext = this;

            var chrome = new WindowChrome
            {
                CaptionHeight = 0,
                ResizeBorderThickness = new Thickness(6),
                GlassFrameThickness = new Thickness(0),
                UseAeroCaptionButtons = false,
                NonClientFrameEdges = NonClientFrameEdges.None
            };
            WindowChrome.SetWindowChrome(this, chrome);

            Loaded += (_, _) => StateChanged += OnStateChanged;

            if (project != null)
            {
                TitleProjectLabel.Text = project.Name;
                Title = $"{project.Name} — Helios-DLX Transpiler";
                StatusText.Text = $"project: {project.Name}  ·  {project.SourceFiles.Count} file(s)";
                IROutputBox.Text =
                    $"────────────────────────────────────────────\n" +
                    $"  Ready  ·  {project.Name}\n" +
                    $"  {project.SourceFiles.Count} source file(s)\n" +
                    $"  Press F5 to build\n" +
                    $"────────────────────────────────────────────\n";
            }
            else
            {
                TitleProjectLabel.Text = "no project";
                Title = "Helios-DLX Transpiler";
                StatusText.Text = "no project loaded  ·  use File → Open Project";
                IROutputBox.Text =
                    $"────────────────────────────────────────────\n" +
                    $"  No project loaded.\n" +
                    $"  File → Open Project  to load a .hdxproj\n" +
                    $"────────────────────────────────────────────\n";
            }
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

        // ── Title bar chrome ──────────────────────────────────────────────────
        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2) { MaximizeBtn_Click(sender, e); return; }
            DragMove();
        }

        private void MinimizeBtn_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;
        private void MaximizeBtn_Click(object sender, RoutedEventArgs e)
            => WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        private void CloseBtn_Click(object sender, RoutedEventArgs e) => Close();

        // ── Menu stubs ────────────────────────────────────────────────────────
        private void FileMenuBtn_Click(object sender, RoutedEventArgs e)
            => FileMenuPopup.IsOpen = !FileMenuPopup.IsOpen;

        private void ViewMenuBtn_Click(object sender, RoutedEventArgs e)
            => ViewMenuPopup.IsOpen = !ViewMenuPopup.IsOpen;

        private void NewFile_Click(object sender, RoutedEventArgs e) { FileMenuPopup.IsOpen = false; }
        private void SaveFile_Click(object sender, RoutedEventArgs e) { FileMenuPopup.IsOpen = false; }
        private void SaveAsFile_Click(object sender, RoutedEventArgs e) { FileMenuPopup.IsOpen = false; }
        private void CloseTab_Click(object sender, RoutedEventArgs e) { FileMenuPopup.IsOpen = false; }

        private void OpenProject_Click(object sender, RoutedEventArgs e)
        {
            FileMenuPopup.IsOpen = false;
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Open Helios Project",
                Filter = "Helios Project (*.hdxproj)|*.hdxproj"
            };
            if (dlg.ShowDialog() == true)
            {
                var svc = new Services.RecentProjectsService();
                var proj = svc.ParseProjectFile(dlg.FileName, out _);
                if (proj != null)
                {
                    TitleProjectLabel.Text = proj.Name;
                    Title = $"{proj.Name} — Helios-DLX Transpiler";
                    StatusText.Text = $"project: {proj.Name}  ·  {proj.SourceFiles.Count} file(s)";
                }
            }
        }

        private void PreferencesBtn_Click(object sender, RoutedEventArgs e) { }

        private void BuildBtn_Click(object sender, RoutedEventArgs e)
        {
            StatusText.Text = "building...";
            IROutputBox.Text =
                $"────────────────────────────────────────────\n" +
                $"  Build triggered — compiler not yet wired.\n" +
                $"  (Phase 3)\n" +
                $"────────────────────────────────────────────\n";
            StatusText.Text = "build complete  ·  0 errors  ·  0 warnings";
        }

        // ── View toggles ──────────────────────────────────────────────────────
        private void ToggleProjectPanel_Click(object sender, RoutedEventArgs e)
        {
            ViewMenuPopup.IsOpen = false;
            ProjectPanelCol.Width = ProjectPanelCol.Width.Value > 0
                ? new GridLength(0)
                : new GridLength(220);
        }

        private void ToggleIRPanel_Click(object sender, RoutedEventArgs e)
        {
            ViewMenuPopup.IsOpen = false;
            IRPanelCol.Width = IRPanelCol.Width.Value > 0
                ? new GridLength(0)
                : new GridLength(340);
        }

        private void ToggleOutputPanel_Click(object sender, RoutedEventArgs e)
        {
            ViewMenuPopup.IsOpen = false;
        }

        // ── Keyboard shortcuts ────────────────────────────────────────────────
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.F5)
            {
                BuildBtn_Click(this, new RoutedEventArgs());
                e.Handled = true;
            }
        }
    }
}