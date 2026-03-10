using System.Collections.Generic;

namespace Helios_Transpiler.Models
{
    public class HdxProject
    {
        // Metadata
        public string Name        { get; set; } = string.Empty;
        public string Author      { get; set; } = string.Empty;
        public string Created     { get; set; } = string.Empty;
        public string Modified    { get; set; } = string.Empty;

        // Compiler settings
        public string EntryPoint       { get; set; } = string.Empty;
        public string OutputPath       { get; set; } = string.Empty;
        public int    OptimizationLevel { get; set; } = 2;
        public bool   EmitDebug        { get; set; } = true;

        // Source files (relative paths as stored in .hdxproj)
        public List<string> SourceFiles { get; set; } = [];

        // Full path to the .hdxproj file on disk
        public string ProjectFilePath { get; set; } = string.Empty;

        // Directory that contains the .hdxproj file
        public string ProjectDirectory =>
            string.IsNullOrEmpty(ProjectFilePath)
                ? string.Empty
                : System.IO.Path.GetDirectoryName(ProjectFilePath) ?? string.Empty;
    }
}
