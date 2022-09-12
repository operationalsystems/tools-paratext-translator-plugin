/*
Copyright © 2022 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using AddInSideViews;
using System;
using System.IO;
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
        /// Filename of the Paratext projet book names file.
        /// </summary>
        public const string BookNamesFilename = "BookNames.xml";

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

            var fileInfo = new FileInfo(Path.Combine(ProjectDir.FullName, BookNamesFilename));
            readOnlyStream = fileInfo.Exists ? fileInfo.OpenRead() : null;

            return readOnlyStream != null;
        }
    }
}
