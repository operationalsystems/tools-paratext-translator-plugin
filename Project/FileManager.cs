using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private readonly string _activeProjectName;

        /// <summary>
        /// Project directory.
        /// </summary>
        private readonly DirectoryInfo _projectDir;

        /// <summary>
        /// Basic ctor.
        /// </summary>
        /// <param name="host">Paratext host interface (required).</param>
        /// <param name="activeProjectName">Active project name (required).</param>
        public FileManager(IHost host, string activeProjectName)
        {
            _host = host ?? throw new ArgumentNullException(nameof(host));
            _activeProjectName = activeProjectName
                                 ?? throw new ArgumentNullException(nameof(activeProjectName));

            _projectDir = Directory.GetParent(_host.GetFigurePath(_activeProjectName, false));
        }

        /// <summary>
        /// Gets book names file, if present.
        /// </summary>
        /// <param name="outputStream">Read-only file stream if file found, null otherwise.</param>
        /// <returns>True if file found, false otherwise.</returns>
        public bool TryGetBookNamesFile(out FileStream outputStream)
        {
            var fileInfo = new FileInfo(Path.Combine(_projectDir.FullName, "BookNames.xml"));
            outputStream = fileInfo.Exists ? fileInfo.OpenRead() : null;

            return outputStream != null;
        }
    }
}
