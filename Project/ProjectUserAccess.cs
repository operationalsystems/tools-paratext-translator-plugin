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
