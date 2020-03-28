using System;
using System.IO;
using AddInSideViews;
using System.Windows.Forms;

namespace TvpMain.Project
{
    /// <summary>
    /// Provides access to the PT project files directory. Project files are where PT is set up to store translation projects. 
    /// In each project there is a pluginsData directory and then plugin specific storage. Generally called something like 'My Paratext 9 Projects'
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
        public DirectoryInfo ProjectDir { get; }

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

            var figurePath = _host.GetFigurePath(_projectName, false)
                                    ?? _host.GetFigurePath(_projectName, true);
            if (figurePath == null)
            {
                MessageBox.Show($"The plugin could not find the project path for this project. Please create the project path or try another project",
                "Warning...", MessageBoxButtons.OK, MessageBoxIcon.Information);
                throw new Exception("Need to close plugin, project path not found.");
            }
            ProjectDir = Directory.GetParent(figurePath);
        }

        /// <summary>
        /// Gets book names file, if present.
        /// </summary>
        /// <param name="readOnlyStream">Read-only file stream if file found, null otherwise.</param>
        /// <returns>True if file found, false otherwise.</returns>
        public bool TryGetBookNamesFile(out FileStream readOnlyStream)
        {
            if (ProjectDir == null)
            {
                Util.HostUtil.Instance.LogLine("Project directory unavailable, responding with empty book name file.", true);
                readOnlyStream = null;
                return false;
            }

            var fileInfo = new FileInfo(Path.Combine(ProjectDir.FullName, "BookNames.xml"));
            readOnlyStream = fileInfo.Exists ? fileInfo.OpenRead() : null;

            return readOnlyStream != null;
        }
    }
}
