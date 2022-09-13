/*
Copyright © 2022 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using Paratext.Data.ProjectSettingsAccess;
using System;
using System.IO;
using TvpMain.Util;

namespace TvpMain.Util
{
    /// <summary>
    /// Helper functions related to working with Paratext projects.
    /// </summary>
    public static class ParatextProjectHelper
    {
        /// <summary>
        /// Returns whether or not a specified directory is a Paratext Project directory or not.
        /// </summary>
        /// <param name="projectDir">Directory path of potential Paratext project</param>
        /// <returns>true: Valid Paratext project; otherwise false</returns>
        public static bool IsValidParatextProjectDirectory(string projectDir)
        {
            // ensure the project directory exists
            if (!Directory.Exists(projectDir))
            {
                throw new DirectoryNotFoundException(string.Concat(projectDir, " directory not found"));
            }

            // check for the existence of a known PT project file
            return File.Exists(projectDir);
        }

        /// <summary>
        /// Opens and deserializes the project's settings.
        /// </summary>
        /// <param name="settingsPath">The full path of the Paratext project directory, containing a Settings.xml file.</param>
        /// <returns>A ProjectSettings object</returns>
        public static ProjectSettings GetProjectSettings(string projectPath)
        {
            // validate input
            _ = projectPath ?? throw new ArgumentNullException(nameof(projectPath));

            // load and return the settings.
            using var settingsFileReader = File.OpenText(Path.Combine(projectPath, MainConsts.SETTINGS_FILE_PATH));
            var projectSettings = new ProjectSettings(settingsFileReader);
            return projectSettings;
        }
    }
}
