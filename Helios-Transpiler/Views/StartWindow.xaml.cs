using System.Windows;
using System.Windows.Input;
using Helios_Transpiler.Models;
using Helios_Transpiler.ViewModels;

namespace Helios_Transpiler.Views
{
    public partial class StartWindow : Window
    {
        private readonly StartViewModel _vm;

        public StartWindow()
        {
            InitializeComponent();

            _vm = new StartViewModel();
            _vm.ProjectOpenRequested += OnProjectOpenRequested;
            _vm.NewProjectRequested  += OnNewProjectRequested;

            DataContext = _vm;
        }

        // ── Custom chrome ────────────────────────────────────────────────

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                WindowState = WindowState == WindowState.Maximized
                    ? WindowState.Normal
                    : WindowState.Maximized;
            }
            else
            {
                DragMove();
            }
        }

        private void MinimizeBtn_Click(object sender, RoutedEventArgs e)
            => WindowState = WindowState.Minimized;

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
            => Close();

        // ── Alt+S focuses search ─────────────────────────────────────────

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

        // ── Double-click row opens project ───────────────────────────────

        private void List_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_vm.SelectedRecent is RecentProject rp)
                _vm.OpenRecentCommand.Execute(rp);
        }

        // ── Window transitions ───────────────────────────────────────────

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

        private void MaximizeBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;
        }
    }
}
