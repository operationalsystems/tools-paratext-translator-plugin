using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using TvpMain.Project;

namespace TvpMain.Reference
{
    /// <summary>
    /// Manages a full or normalized (de-conflicted) separator set.
    ///
    /// At least some projects have higher-level separators that are the same as
    /// lower-level ones (e.g., lists of books and lists of verses, both of which
    /// include ";" for "spaNVI15").
    ///
    /// Keeping these in place makes for un-navigable grammar, since most of the
    /// reference fields are indistinguishable integers without dubious classification
    /// opportunities (e.g., book number ranges), so the this class may optionally
    /// de-conflict them before parsing to limit higher-level separators (e.g., lists
    /// of books) being accepted as lower-level ones (e.g., lists of verses).
    ///
    /// This is ok as we ultimately want a standard, parseable reference format and
    /// any reference actually exhibiting these inversions will at least be caught as
    /// invalid by the checker and corrected or ignored.
    ///
    /// The next evolution of this may be enhanced to infer separators based on what's
    /// encountered earlier in the expression.
    /// </summary>
    public class ScriptureReferenceSeparators
    {
        public IList<string> BookSequenceSeparators { get; }

        public IList<string> ChapterSequenceSeparators { get; }

        public IList<string> BookOrChapterRangeSeparators { get; }

        public IList<string> ChapterAndVerseSeparators { get; }

        public IList<string> VerseSequenceSeparators { get; }

        public IList<string> VerseRangeSeparators { get; }

        public bool IsAnyDuplicates { get; }

        public bool IsUsable =>
            BookSequenceSeparators.Count > 0
            && ChapterSequenceSeparators.Count > 0
            && BookOrChapterRangeSeparators.Count > 0
            && ChapterAndVerseSeparators.Count > 0
            && VerseSequenceSeparators.Count > 0
            && VerseRangeSeparators.Count > 0;

        /// <summary>
        /// Standard constructor
        /// </summary>
        /// <param name="projectManager"></param>
        /// <param name="isNormalized"></param>
        public ScriptureReferenceSeparators(
            ProjectManager projectManager,
            bool isNormalized)
        : this(projectManager.BookSequenceSeparators,
            projectManager.ChapterSequenceSeparators,
            projectManager.BookOrChapterRangeSeparators,
            projectManager.ChapterAndVerseSeparators,
            projectManager.VerseSequenceSeparators,
            projectManager.VerseRangeSeparators,
            isNormalized)
        { }

        /// <summary>
        /// Extended constructor
        /// </summary>
        /// <param name="bookSequenceSeparators"></param>
        /// <param name="chapterSequenceSeparators"></param>
        /// <param name="bookOrChapterRangeSeparators"></param>
        /// <param name="chapterAndVerseSeparators"></param>
        /// <param name="verseSequenceSeparators"></param>
        /// <param name="verseRangeSeparators"></param>
        /// <param name="isNormalized"></param>
        public ScriptureReferenceSeparators(
            IEnumerable<string> bookSequenceSeparators,
            IEnumerable<string> chapterSequenceSeparators,
            IEnumerable<string> bookOrChapterRangeSeparators,
            IEnumerable<string> chapterAndVerseSeparators,
            IEnumerable<string> verseSequenceSeparators,
            IEnumerable<string> verseRangeSeparators,
            bool isNormalized)
        {
            // Book sequence separators aren't literally used to separate 
            // collections by this parser as they're at the top level, so
            // they need not be used to mask other separators.
            BookSequenceSeparators = bookSequenceSeparators.ToImmutableList();

            var prevSeparators = new HashSet<string>();
            var isAnyDuplicates = false;

            ChapterSequenceSeparators = FilterSeparators(
                chapterSequenceSeparators,
                prevSeparators, isNormalized, ref isAnyDuplicates);
            BookOrChapterRangeSeparators = FilterSeparators(
                bookOrChapterRangeSeparators,
                prevSeparators, isNormalized, ref isAnyDuplicates);
            ChapterAndVerseSeparators = FilterSeparators(
                chapterAndVerseSeparators,
                prevSeparators, isNormalized, ref isAnyDuplicates);
            VerseSequenceSeparators = FilterSeparators(
                verseSequenceSeparators,
                prevSeparators, isNormalized, ref isAnyDuplicates);
            VerseRangeSeparators = FilterSeparators(
                verseRangeSeparators,
                prevSeparators, isNormalized, ref isAnyDuplicates);

            IsAnyDuplicates = isAnyDuplicates;
        }

        /// <summary>
        /// Filters based on sepearators and returns a list of strings
        /// </summary>
        /// <param name="inputSeparators"></param>
        /// <param name="prevSeparators"></param>
        /// <param name="isNormalized"></param>
        /// <param name="isAnyDuplicate"></param>
        private static IList<string> FilterSeparators(
            IEnumerable<string> inputSeparators, ISet<string> prevSeparators,
            bool isNormalized, ref bool isAnyDuplicate)
        {
            var isListDuplicates = false;
            var result = inputSeparators
                    .Select(value => value.Trim().ToLower())
                    .Where(value =>
                    {
                        var isDuplicate = !prevSeparators.Add(value);
                        isListDuplicates = isListDuplicates || isDuplicate;

                        if (isNormalized)
                        {
                            return !isDuplicate;
                        }
                        else
                        {
                            return true;
                        }
                    })
                    .ToImmutableList();
            isAnyDuplicate = isAnyDuplicate || isListDuplicates;

            return result;
        }
    }
}
