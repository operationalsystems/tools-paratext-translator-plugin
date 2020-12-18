using Paratext.Data.ProjectSettingsAccess;
using System;
using System.IO;
using TvpMain.Util;

namespace TvpMain.ParatextProjects
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
