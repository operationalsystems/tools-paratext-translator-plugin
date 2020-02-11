using System;
using System.IO;
using AddInSideViews;

namespace TvpMain.Project
{
    /// <summary>
    /// Provides access to project files.
    /// </summary>
    public class FileManager
    {
        /// <summary>
        /// Paratext host interface.
        /// </summary>
        private readonly IHost _host;

        /// <summary>
        /// Active project name.
        /// </summary>
        private readonly string _projectName;

        /// <summary>
        /// Project directory.
        /// </summary>
        private readonly DirectoryInfo _projectDir;

        /// <summary>
        /// Basic ctor.
        /// </summary>
        /// <param name="host">Paratext host interface (required).</param>
        /// <param name="projectName">Active project name (required).</param>
        public FileManager(IHost host, string projectName)
        {
            _host = host ?? throw new ArgumentNullException(nameof(host));
            _projectName = projectName
                                 ?? throw new ArgumentNullException(nameof(projectName));

            _projectDir = Directory.GetParent(_host.GetFigurePath(_projectName, false));
        }

        /// <summary>
        /// Gets book names file, if present.
        /// </summary>
        /// <param name="readOnlyStream">Read-only file stream if file found, null otherwise.</param>
        /// <returns>True if file found, false otherwise.</returns>
        public bool TryGetBookNamesFile(out FileStream readOnlyStream)
        {
            var fileInfo = new FileInfo(Path.Combine(_projectDir.FullName, "BookNames.xml"));
            readOnlyStream = fileInfo.Exists ? fileInfo.OpenRead() : null;

            return readOnlyStream != null;
        }

        /// <summary>
        /// Opens a file within the project directory for writing.
        /// </summary>
        /// <param name="outputName">Destination file name element (required).</param>
        /// <param name="isMakeDirs">True to make missing directories, false to throw an exception.</param>
        /// <param name="isOverwrite">True to overwrite existing files, false to throw an exception.</param>
        /// <param name="writableStream">Writable file stream if created, null otherwise.</param>
        /// <returns>True if stream created, false otherwise.</returns>
        public bool TryGetOutputFile(
            string outputName,
            bool isMakeDirs, bool isOverwrite,
            out FileStream writableStream)
        {
            return TryGetOutputFile(
                _projectDir, outputName,
                isMakeDirs, isOverwrite,
                out writableStream);
        }

        /// <summary>
        /// Opens a file within a target directory for writing.
        /// </summary>
        /// <param name="outputDir">Destination directory (required).</param>
        /// <param name="outputName">Destination file name element (required).</param>
        /// <param name="isMakeDirs">True to make missing directories, false to throw an exception.</param>
        /// <param name="isOverwrite">True to overwrite existing files, false to throw an exception.</param>
        /// <param name="writableStream">Writable file stream if created, null otherwise.</param>
        /// <returns>True if stream created, false otherwise.</returns>
        public bool TryGetOutputFile(
            DirectoryInfo outputDir, string outputName,
            bool isMakeDirs, bool isOverwrite,
            out FileStream writableStream)
        {
            var fileInfo = new FileInfo(Path.Combine(outputDir.FullName, outputName));

            // delete existing file, as needed
            if (fileInfo.Exists)
            {
                if (!isOverwrite)
                {
                    throw new ArgumentException($"file exists: {fileInfo.FullName} (isOverwrite=false)");
                }
                fileInfo.Delete();
                fileInfo.Refresh();
            }

            // make output directory, as needed
            if (fileInfo.Directory != null
                && !fileInfo.Directory.Exists)
            {
                if (!isMakeDirs)
                {
                    throw new ArgumentException($"missing directory: {fileInfo.Directory.FullName} (isMakeDirs=false)");
                }
                Directory.CreateDirectory(fileInfo.Directory.FullName);
                fileInfo.Refresh();
            }

            writableStream = fileInfo.OpenWrite();
            return writableStream != null;
        }
    }
}
