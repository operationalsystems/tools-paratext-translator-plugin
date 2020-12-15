using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TvpMain.Check.CheckAndFixItem;

namespace TvpMain.Check
{
    /// <summary>
    /// This class is used to track and validate the context of what is being checked against.
    /// </summary>
    public class CheckRunContext
    {
        /// <summary>
        /// The Paratext project that we will check against. EG: "usNIV11".
        /// </summary>
        public string Project { get; set; }

        /// <summary>
        /// The scope at which we are checking.
        /// </summary>
        public CheckScope CheckScope { get; set; }

        /// <summary>
        /// The list of books to check.
        /// </summary>
        public List<int> Books { get; set; } = null;

        /// <summary>
        /// The list of chapters to check.
        /// </summary>
        public List<int> Chapters { get; set; } = null;

        /// <summary>
        /// This function will validate if the set options are valid for use. If invalid, an exception is thrown.
        /// </summary>
        public void Validate()
        {
            // ensure that we have a Project to check
            _ = this.Project ?? throw new ArgumentNullException(nameof(this.Project));

            // validate based on the scope
            switch (this.CheckScope)
            {
                case CheckScope.BOOK:
                    if (Books == null)
                    {
                        throw new ArgumentNullException($"{this.CheckScope} requires that {nameof(Books)} is non-null.");
                    }
                    break;
                case CheckScope.CHAPTER:
                    if (Books == null)
                    {
                        throw new ArgumentNullException($"{this.CheckScope} requires that {nameof(Books)} is non-null.");
                    }
                    if (Chapters == null)
                    {
                        throw new ArgumentNullException($"{this.CheckScope} requires that {nameof(Chapters)} is non-null.");
                    }
                    break;
                default:
                    throw new ArgumentException($"{this.CheckScope} isn't supported at this time.");
            }
        }
    }
}
