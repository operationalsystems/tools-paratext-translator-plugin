﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TvpMain.Project;
using TvpMain.Result;
using TvpMain.Text;

namespace TvpMain.Check
{
    public class ScriptureReferenceCheck : ITextCheck
    {
        /// <summary>
        /// Provides project setting & metadata access.
        /// </summary>
        private ProjectManager _projectManager;

        /// <summary>
        /// Basic ctor.
        /// </summary>
        /// <param name="projectManager">Project manager (required).</param>
        public ScriptureReferenceCheck(ProjectManager projectManager)
        {
            _projectManager = projectManager ?? throw new ArgumentNullException(nameof(projectManager));
        }

        /// <summary>
        /// Check implementation.
        /// </summary>
        /// <param name="partData">Verse part data, including original verse, location, etc.</param>
        /// <param name="checkResults">Result items list to populate.</param>
        public void CheckText(
            VersePart partData,
            ICollection<ResultItem> checkResults)
        {
            var matchedParts = new HashSet<VersePart>();
            foreach (var regexItem in _projectManager.TargetReferenceRegexes)
            {
                foreach (Match matchItem in regexItem.Matches(partData.PartText))
                {
                    var matchedPart = new VersePart(partData.VerseData,
                        new PartLocation(partData.PartLocation.PartStart + matchItem.Index,
                            matchItem.Length,
                            partData.PartLocation.PartContext),
                        matchItem.Value);
                    if (!matchedParts.Contains(matchedPart))
                    {
                        matchedParts.Add(matchedPart);
                        checkResults.Add
                        (new ResultItem(matchedPart,
                            $"Found reference at {matchItem.Index}.",
                            partData.PartText, matchItem.Value, "May be ok...",
                            CheckType.ScriptureReference));
                    }
                }
            }
        }
    }
}
