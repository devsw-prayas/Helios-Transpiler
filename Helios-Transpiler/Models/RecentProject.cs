using System;

namespace Helios_Transpiler.Models
{
    public class RecentProject
    {
        public string Name        { get; set; } = string.Empty;
        public string FilePath    { get; set; } = string.Empty;
        public DateTime LastOpened { get; set; } = DateTime.Now;
        public bool   IsPinned    { get; set; } = false;

        // Display-friendly last opened string (e.g. "3/8/2026 7:43 PM")
        public string LastOpenedDisplay =>
            LastOpened.ToString("M/d/yyyy h:mm tt");

        // Shortened path for display, keeping last 3 segments
        public string ShortPath
        {
            get
            {
                if (string.IsNullOrEmpty(FilePath)) return string.Empty;
                var parts = FilePath.Replace('/', '\\').Split('\\');
                if (parts.Length <= 4) return FilePath;
                return $"C\\...\\{string.Join("\\", parts[^3..])}";
            }
        }
    }
}
