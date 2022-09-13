/*
Copyright © 2022 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System;
using System.IO;
using System.Xml.Serialization;

namespace TvpMain.Project
{
    /// <summary>
    /// Used for getting information out of the ProjectUserAccess.xml of the project directory. The intent is
    /// to be able to determine the role of the user based on their username, see <see cref="HostUtil"/>::isCurrentUserAdmin()
    /// </summary>
    [Serializable]
    [XmlRoot("ProjectUserAccess")]
    public class ProjectUserAccess
    {
        /// <summary>
        /// The list of users
        /// </summary>
        [XmlElement("User")]
        public User[] Users;

        /// <summary>
        /// Method to load the data from a reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns>The ProjectUserAccess loaded from the project file.</returns>
        public static ProjectUserAccess LoadFromXML(Stream reader)
        {
            var serializer = new XmlSerializer(typeof(ProjectUserAccess));

            ProjectUserAccess projectUserAccess = (ProjectUserAccess)serializer.Deserialize(reader);

            return projectUserAccess;
        }
    }

    /// <summary>
    /// The user information, the username to match and their role.
    /// </summary>
    [Serializable]
    public class User
    {
        [XmlAttribute("UserName")]
        public string UserName;
        [XmlElement("Role")]
        public string Role;

    }

}
