using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Helios_Transpiler.Infrastructure;
using Helios_Transpiler.Models;
using Helios_Transpiler.Services;

namespace Helios_Transpiler.ViewModels
{
    public class StartViewModel : ViewModelBase
    {
        private readonly RecentProjectsService _service = new();

        // ── Observable collections ──────────────────────────────────────────

        public ObservableCollection<RecentProject> PinnedProjects  { get; } = new();
        public ObservableCollection<RecentProject> RecentProjects  { get; } = new();

        private RecentProject? _selectedRecent;
        public RecentProject? SelectedRecent
        {
            get => _selectedRecent;
            set => SetField(ref _selectedRecent, value);
        }

        // ── Search ─────────────────────────────────────────────────────────

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetField(ref _searchText, value))
                    ApplyFilter();
            }
        }

        // ── Event raised when a project should be opened ───────────────────

        public event Action<HdxProject>? ProjectOpenRequested;
        public event Action?             NewProjectRequested;

        // ── Commands ───────────────────────────────────────────────────────

        public ICommand OpenProjectCommand    { get; }
        public ICommand NewProjectCommand     { get; }
        public ICommand OpenRecentCommand     { get; }
        public ICommand TogglePinCommand      { get; }
        public ICommand RemoveRecentCommand   { get; }
        public ICommand OpenFolderCommand     { get; }

        // ── Raw list (pre-filter) ───────────────────────────────────────────

        private System.Collections.Generic.List<RecentProject> _allRecent = new();

        // ── Constructor ────────────────────────────────────────────────────

        public StartViewModel()
        {
            OpenProjectCommand  = new RelayCommand(ExecuteOpenProject);
            NewProjectCommand   = new RelayCommand(ExecuteNewProject);
            OpenRecentCommand   = new RelayCommand(ExecuteOpenRecent);
            TogglePinCommand    = new RelayCommand(ExecuteTogglePin);
            RemoveRecentCommand = new RelayCommand(ExecuteRemoveRecent);
            OpenFolderCommand   = new RelayCommand(ExecuteOpenFolder);

            LoadRecent();
        }

        // ── Load & filter ──────────────────────────────────────────────────

        private void LoadRecent()
        {
            _allRecent = _service.Load();
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            var filtered = string.IsNullOrWhiteSpace(_searchText)
                ? _allRecent
                : _allRecent.Where(r =>
                    r.Name.Contains(_searchText, StringComparison.OrdinalIgnoreCase) ||
                    r.FilePath.Contains(_searchText, StringComparison.OrdinalIgnoreCase))
                  .ToList();

            PinnedProjects.Clear();
            RecentProjects.Clear();

            foreach (var r in filtered)
            {
                if (r.IsPinned) PinnedProjects.Add(r);
                else            RecentProjects.Add(r);
            }
        }

        // ── Command handlers ───────────────────────────────────────────────

        private void ExecuteOpenProject(object? _)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                Title  = "Open Helios Project",
                Filter = "Helios Project Files (*.hdxproj)|*.hdxproj|All Files (*.*)|*.*"
            };

            if (dlg.ShowDialog() != true) return;

            OpenProjectFile(dlg.FileName);
        }

        private void ExecuteNewProject(object? _)
        {
            NewProjectRequested?.Invoke();
        }

        private void ExecuteOpenRecent(object? param)
        {
            if (param is RecentProject rp)
                OpenProjectFile(rp.FilePath);
        }

        private void ExecuteTogglePin(object? param)
        {
            if (param is not RecentProject rp) return;
            _allRecent = _service.TogglePin(_allRecent, rp.FilePath);
            ApplyFilter();
        }

        private void ExecuteRemoveRecent(object? param)
        {
            if (param is not RecentProject rp) return;
            _allRecent = _service.Remove(_allRecent, rp.FilePath);
            ApplyFilter();
        }

        private void ExecuteOpenFolder(object? _)
        {
            // Opens a folder (future: open a .hds file directly without a project)
            var dlg = new Microsoft.Win32.OpenFolderDialog
            {
                Title = "Select a folder containing .hds files"
            };
            if (dlg.ShowDialog() == true)
            {
                // Future: create an ad-hoc project from folder
            }
        }

        // ── Open helper ────────────────────────────────────────────────────

        private void OpenProjectFile(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                System.Windows.MessageBox.Show(
                    $"Project file not found:\n{filePath}",
                    "Helios-DLX",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Warning);
                return;
            }

            var project = _service.ParseProjectFile(filePath, out var error);
            if (project == null)
            {
                System.Windows.MessageBox.Show(
                    error,
                    "Helios-DLX — Failed to open project",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
                return;
            }

            _allRecent = _service.RecordOpened(_allRecent, filePath, project.Name);
            ApplyFilter();

            ProjectOpenRequested?.Invoke(project);
        }
    }
}
