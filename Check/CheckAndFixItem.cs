using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TvpMain.Check
{
    /// <summary>
    /// This is a model class that represents a Translation Check and Fix.
    /// </summary>
    class CheckAndFixItem
    {
        /// <summary>
        /// The internal GUID object for the ID.
        /// </summary>
        private Guid _id;

        /// <summary>
        /// The GUID for this Check and Fix.
        /// </summary>
        public String Id {
            get
            {
                // create a new GUID if one hasn't been set yet.
                if (_id == null)
                {
                    _id = Guid.NewGuid();
                }
                return _id.ToString();
            }
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
        public Version Version { get; set; }
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
    }
}
