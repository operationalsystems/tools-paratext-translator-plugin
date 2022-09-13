/*
Copyright © 2022 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TvpMain.Project;
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
        public BookNameItem[] Books { get; set; } = null;

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
