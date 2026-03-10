using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using Helios_Transpiler.Models;

namespace Helios_Transpiler.Services
{
    /// <summary>
    /// Persists the recent-projects list to %AppData%\HeliosDLX\recent.json
    /// and parses .hdxproj XML files into HdxProject instances.
    /// </summary>
    public class RecentProjectsService
    {
        private const int MaxRecent = 10;

        private static readonly string StorageDir =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                         "HeliosDLX");

        private static readonly string StoragePath =
            Path.Combine(StorageDir, "recent.json");

        // ── Load ────────────────────────────────────────────────────────────

        public List<RecentProject> Load()
        {
            try
            {
                if (!File.Exists(StoragePath))
                    return [];

                var json = File.ReadAllText(StoragePath);
                var list = JsonSerializer.Deserialize<List<RecentProject>>(json);
                return list ?? [];
            }
            catch
            {
                return [];
            }
        }

        // ── Save ────────────────────────────────────────────────────────────

        public void Save(IEnumerable<RecentProject> projects)
        {
            try
            {
                Directory.CreateDirectory(StorageDir);
                var json = JsonSerializer.Serialize(
                    projects.Take(MaxRecent).ToList(),
                    new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(StoragePath, json);
            }
            catch
            {
                // best-effort — never crash the app over settings
            }
        }

        // ── Add or update an entry ──────────────────────────────────────────

        /// <summary>
        /// Inserts or bumps a project to the top of the recent list.
        /// Preserves pinned state. Trims to MaxRecent.
        /// </summary>
        public List<RecentProject> RecordOpened(List<RecentProject> existing, string filePath, string name)
        {
            var list = existing.ToList();

            var existing_ = list.FirstOrDefault(
                r => string.Equals(r.FilePath, filePath, StringComparison.OrdinalIgnoreCase));

            if (existing_ != null)
            {
                existing_.LastOpened = DateTime.Now;
                existing_.Name       = name;
            }
            else
            {
                list.Insert(0, new RecentProject
                {
                    Name        = name,
                    FilePath    = filePath,
                    LastOpened  = DateTime.Now,
                    IsPinned    = false
                });
            }

            // Pinned first, then by last opened descending
            list = list
                .OrderByDescending(r => r.IsPinned)
                .ThenByDescending(r => r.LastOpened)
                .Take(MaxRecent)
                .ToList();

            Save(list);
            return list;
        }

        // ── Toggle pin ─────────────────────────────────────────────────────

        public List<RecentProject> TogglePin(List<RecentProject> existing, string filePath)
        {
            var list = existing.ToList();
            var entry = list.FirstOrDefault(
                r => string.Equals(r.FilePath, filePath, StringComparison.OrdinalIgnoreCase));

            if (entry != null)
                entry.IsPinned = !entry.IsPinned;

            list = list
                .OrderByDescending(r => r.IsPinned)
                .ThenByDescending(r => r.LastOpened)
                .ToList();

            Save(list);
            return list;
        }

        // ── Remove ─────────────────────────────────────────────────────────

        public List<RecentProject> Remove(List<RecentProject> existing, string filePath)
        {
            var list = existing
                .Where(r => !string.Equals(r.FilePath, filePath, StringComparison.OrdinalIgnoreCase))
                .ToList();
            Save(list);
            return list;
        }

        // ── Parse .hdxproj ─────────────────────────────────────────────────

        /// <summary>
        /// Parses a .hdxproj XML file into an HdxProject.
        /// Returns null and sets errorMessage if parsing fails.
        /// </summary>
        public HdxProject? ParseProjectFile(string filePath, out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                var doc  = XDocument.Load(filePath);
                var root = doc.Root;

                if (root == null || root.Name.LocalName != "HeliosProject")
                {
                    errorMessage = "Not a valid Helios project file (missing <HeliosProject> root).";
                    return null;
                }

                var meta     = root.Element("Metadata");
                var compiler = root.Element("Compiler");
                var sources  = root.Element("Sources");

                var project = new HdxProject
                {
                    ProjectFilePath  = filePath,
                    Name             = meta?.Element("Name")?.Value             ?? Path.GetFileNameWithoutExtension(filePath),
                    Author           = meta?.Element("Author")?.Value           ?? string.Empty,
                    Created          = meta?.Element("Created")?.Value          ?? string.Empty,
                    Modified         = meta?.Element("Modified")?.Value         ?? string.Empty,
                    EntryPoint       = compiler?.Element("EntryPoint")?.Value   ?? string.Empty,
                    OutputPath       = compiler?.Element("OutputPath")?.Value   ?? string.Empty,
                    OptimizationLevel = int.TryParse(compiler?.Element("OptimizationLevel")?.Value, out var ol) ? ol : 2,
                    EmitDebug        = bool.TryParse(compiler?.Element("EmitDebug")?.Value, out var dbg) ? dbg : true,
                    SourceFiles      = sources?
                                        .Elements("File")
                                        .Select(e => e.Attribute("path")?.Value ?? string.Empty)
                                        .Where(p => !string.IsNullOrEmpty(p))
                                        .ToList()
                                       ?? new List<string>()
                };

                return project;
            }
            catch (Exception ex)
            {
                errorMessage = $"Failed to parse project file: {ex.Message}";
                return null;
            }
        }
    }
}
