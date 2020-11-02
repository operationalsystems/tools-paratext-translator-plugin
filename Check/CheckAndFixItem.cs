using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace TvpMain.Check
{
    /// <summary>
    /// This is a model class that represents a Translation Check and Fix.
    /// </summary>
    public class CheckAndFixItem : ICloneable
    {
        /// <summary>
        /// The item's default constructor. It will ensure that a unique ID is generated.
        /// </summary>
        public CheckAndFixItem()
        {
            _id = Guid.NewGuid();
        }

        /// <summary>
        /// The internal GUID object for the ID.
        /// </summary>
        private Guid _id;

        /// <summary>
        /// The internal version object.
        /// </summary>
        private Version _version;

        /// <summary>
        /// The GUID for this Check and Fix.
        /// </summary>
        public String Id
        {
            get { return _id.ToString(); }
            set
            {
                _id = Guid.Parse(value);
            }
        }

        /// <summary>
        /// The name of this Check and Fix item.
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// The description of this Check and Fix item.
        /// </summary>
        public String Description { get; set; }
        /// <summary>
        /// The version of this Check and Fix item.
        /// </summary>
        public String Version
        {
            get { return _version.ToString(); }
            set
            {
                _version = new Version(value);
            }
        }
        /// <summary>
        /// The Check's regular expression. The check regex will be evaluated before the check script.
        /// </summary>
        public String CheckRegex { get; set; }
        /// <summary>
        /// The Check's javascript script content.
        /// </summary>
        public String CheckScript { get; set; }
        /// <summary>
        /// The Fix's regular expression. The fix regex will be evaluated before the fix script.
        /// </summary>
        public String FixRegex { get; set; }
        /// <summary>
        /// The Fix's javascript script content.
        /// </summary>
        public String FixScript { get; set; }

        //////////////// Serialization and Deserialization functions ///////////////////////

        /// <summary>
        /// Deserialize a <c>CheckAndFixItem</c> XML file into a corresonding object.
        /// </summary>
        /// <param name="xmlFilePath">The absolute path of the <c>CheckAndFixItem</c> XML file. (required)</param>
        /// <returns>Corresponding <c>CheckAndFixItem</c> object.</returns>
        public static CheckAndFixItem LoadFromXmlFile(string xmlFilePath)
        {
            // validate input
            _ = xmlFilePath ?? throw new ArgumentNullException(nameof(xmlFilePath));

            // deserialize the file into an object
            var serializer = new XmlSerializer(typeof(CheckAndFixItem));
            using var xmlReader = new XmlTextReader(xmlFilePath);
            var obj = (CheckAndFixItem)serializer.Deserialize(xmlReader);

            return obj;
        }

        /// <summary>
        /// Deserialize <c>CheckAndFixItem</c> XML content into a corresonding object.
        /// </summary>
        /// <param name="xmlContent">The absolute path of the <c>CheckAndFixItem</c> XML file. (required)</param>
        /// <returns>Corresponding <c>CheckAndFixItem</c> object.</returns>
        public static CheckAndFixItem LoadFromXmlContent(string xmlContent)
        {
            // validate input
            _ = xmlContent ?? throw new ArgumentNullException(nameof(xmlContent));

            // deserialize the file into an object
            var serializer = new XmlSerializer(typeof(CheckAndFixItem));

            using TextReader reader = new StringReader(xmlContent);
            var testString = reader.ReadToEnd();
            CheckAndFixItem result = (CheckAndFixItem)serializer.Deserialize(reader);
            return result;
        }

        /// <summary>
        /// Serialize the current <c>CheckAndFixItem</c> object into an XML file.
        /// </summary>
        /// <param name="xmlFilePath">The absolute path of the <c>CheckAndFixItem</c> XML file. (required)</param>
        public void SaveToXmlFile(string xmlFilePath)
        {
            // validate input
            _ = xmlFilePath ?? throw new ArgumentNullException(nameof(xmlFilePath));

            XmlSerializer writer = new XmlSerializer(this.GetType());

            FileStream file = File.Create(xmlFilePath);

            writer.Serialize(file, this);
            file.Close();
        }

        /// <summary>
        /// Serialize the current <c>CheckAndFixItem</c> object into an XML string.
        /// </summary>
        /// <returns>Corresponding <c>CheckAndFixItem</c> object as an XML string.</returns>
        public string WriteToXmlString()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(this.GetType());

            using StringWriter textWriter = new StringWriter();
            xmlSerializer.Serialize(textWriter, this);
            return textWriter.ToString();
        }

        /// <summary>
        /// An override of the equals capability to validate the content of two <c>CheckAndFixItem</c>s are the same.
        /// </summary>
        /// <param name="obj">The <c>CheckAndFixItem</c> to compare against. (required)</param>
        /// <returns>True: if both objects are equal, False: otherwise.</returns>
        public override bool Equals(object obj)
        {
            return obj is CheckAndFixItem item &&
                   Id == item.Id &&
                   Name == item.Name &&
                   Description == item.Description &&
                   Version == item.Version &&
                   CheckRegex == item.CheckRegex &&
                   CheckScript == item.CheckScript &&
                   FixRegex == item.FixRegex &&
                   FixScript == item.FixScript;
        }

        /// <summary>
        /// This function will make a deep copy of our <c>CheckAndFixItem</c>.
        /// </summary>
        /// <returns>A copy of the current <c>CheckAndFixItem</c> artifact.</returns>
        public object Clone()
        {
            // deep clone the object by utilizing the serializing and deserializing functions.
            return CheckAndFixItem.LoadFromXmlContent(this.WriteToXmlString());
        }
    }
}
