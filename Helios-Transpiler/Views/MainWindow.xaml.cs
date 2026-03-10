using System.Windows;
using System.Windows.Input;
using Helios_Transpiler.Models;

namespace Helios_Transpiler.Views
{
    public partial class MainWindow : Window
    {
        private readonly HdxProject? _project;

        public MainWindow(HdxProject? project)
        {
            _project = project;
            InitializeComponent();
            DataContext = this;

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

        // ── Title bar chrome ──────────────────────────────────────────────

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
                WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            else
                DragMove();
        }

        private void MinimizeBtn_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;
        private void MaximizeBtn_Click(object sender, RoutedEventArgs e)
            => WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        private void CloseBtn_Click(object sender, RoutedEventArgs e) => Close();

        // ── Menu stubs ────────────────────────────────────────────────────

        private void FileMenuBtn_Click(object sender, RoutedEventArgs e)
            => FileMenuPopup.IsOpen = !FileMenuPopup.IsOpen;

        private void ViewMenuBtn_Click(object sender, RoutedEventArgs e)
            => ViewMenuPopup.IsOpen = !ViewMenuPopup.IsOpen;

        private void NewFile_Click(object sender, RoutedEventArgs e)    { FileMenuPopup.IsOpen = false; }
        private void SaveFile_Click(object sender, RoutedEventArgs e)   { FileMenuPopup.IsOpen = false; }
        private void SaveAsFile_Click(object sender, RoutedEventArgs e) { FileMenuPopup.IsOpen = false; }
        private void CloseTab_Click(object sender, RoutedEventArgs e)   { FileMenuPopup.IsOpen = false; }

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
                string s = "";
                var sve = new Services.RecentProjectsService();
                var proj = sve.ParseProjectFile(dlg.FileName, out s);
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

        private void IRModeBtn_Click(object sender, RoutedEventArgs e) { }

        // ── View toggles ──────────────────────────────────────────────────

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
                : new GridLength(320);
        }

        private void ToggleOutputPanel_Click(object sender, RoutedEventArgs e)
        {
            ViewMenuPopup.IsOpen = false;
        }

        // ── Keyboard shortcuts ────────────────────────────────────────────

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
