/*
 * A Class to handle the Global Constants.
 */
namespace TvpMain.Util
{
    /// <summary>
    /// Utility constants, magic numbers, etc.
    /// </summary>
    public class MainConsts
    {
        /// <summary>
        /// Plugin data identifier for ignore list items.
        /// </summary>
        public const string IGNORE_LIST_ITEMS_ID = "IgnoreListItems";

        /// <summary>
        /// Update rate for progress form and similar loops.
        /// </summary>
        public const int CHECK_EVENTS_UPDATE_RATE_IN_FPS = 10;

        /// <summary>
        /// Max threads to use for validation checks.
        /// </summary>
        public const int MAX_CHECK_THREADS = 4;

        /// <summary>
        /// Max consecutive empty verses before chapter is considered empty.
        /// </summary>
        public const int MAX_CONSECUTIVE_EMPTY_VERSES = 10;

        /// <summary>
        /// Multiplier for book numbers in BCV-style references.
        /// </summary>
        public static readonly int BookRefMultiplier = 1000000;

        /// <summary>
        /// Multiplier for chapter numbers in BCV-style references.
        /// </summary>
        public static readonly int ChapRefMultiplier = 1000;

        /// <summary>
        /// Range ref parts (i.e., chapters, verses).
        /// </summary>
        public static readonly int RefPartRange = 1000;
    }
}
