using System;
using System.Collections.Generic;
using System.Text;

namespace TvpMain.Check
{
    /// <summary>
    /// Exception class for <code>CheckAndFixItem</code> related issues.
    /// </summary>
    class CheckAndFixException : Exception
    {
        /// <summary>
        /// Constructor for providing CheckAndFixItem that caused the exception.
        /// </summary>
        /// <param name="check"></param>
        public CheckAndFixException(string message, CheckAndFixItem causeCheck) : base(message)
        {
            CauseCheck = causeCheck ?? throw new ArgumentNullException(nameof(causeCheck));
        }

        /// <summary>
        /// Check that caused the exception.
        /// </summary>
        public CheckAndFixItem CauseCheck { get; }

        /// <summary>
        /// Override Message to mention Check that caused the exception.
        /// </summary>
        /// <returns>Exception message.</returns>
        public new string Message
        {
            get
            {
                if (CauseCheck is null)
                {
                    return "(Unknown check) " + base.Message;
                }

                var infoList = new List<string>()
            {
                "Id: " + CauseCheck.Id,
                "Name: " + CauseCheck.Name,
                "Version: " + CauseCheck.Id
            };

                return string.Format("Check ( %s ), Message: %s", string.Join(", ", infoList), base.Message);
            }
        }
    }
}
