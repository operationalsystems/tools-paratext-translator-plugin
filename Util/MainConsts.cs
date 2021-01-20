/*
 * A Class to handle the Global Constants.
 */

namespace TvpMain.Util
{
    /// <summary>
    /// Utility constants, magic numbers, etc.
    /// </summary>
    public static class MainConsts
    {
        /// <summary>
        /// Default separator for a sequence of book names.
        /// </summary>
        public const string DEFAULT_REFERENCE_BOOK_SEQUENCE_SEPARATOR = ";";

        /// <summary>
        /// Default separator for a sequence of chapter numbers.
        /// </summary>
        public const string DEFAULT_REFERENCE_CHAPTER_SEQUENCE_SEPARATOR = ";";

        /// <summary>
        /// Default separator for a range of chapter numbers.
        /// </summary>
        public const string DEFAULT_REFERENCE_BOOK_OR_CHAPTER_RANGE_SEPARATOR = "–";

        /// <summary>
        /// Default separator for chapters and verses.
        /// </summary>
        public const string DEFAULT_REFERENCE_CHAPTER_AND_VERSE_SEPARATOR = ":";

        /// <summary>
        /// Default separator for verse sequences.
        /// </summary>
        public const string DEFAULT_REFERENCE_VERSE_SEQUENCE_SEPARATOR = ",";

        /// <summary>
        /// Default separator for verse ranges.
        /// </summary>
        public const string DEFAULT_REFERENCE_VERSE_RANGE_SEPARATOR = "-";

        /// <summary>
        /// Default prefix or suffix for references.
        /// </summary>
        public const string DEFAULT_REFERENCE_PREFIX_OR_SUFFIX = "";

        /// <summary>
        /// Default final punctuation for references.
        /// </summary>
        public const string DEFAULT_REFERENCE_FINAL_PUNCTUATION = "";

        /// <summary>
        /// The settings file for each project's default checks
        /// </summary>
        public const string CHECK_SETTINGS_DATA_ID = "CheckSettings.xml";

        /// <summary>
        /// The settings file for each project's denied results
        /// </summary>
        public const string DENIED_RESULTS_DATA_ID = "DeniedResults.xml";

        /// <summary>
        /// Update rate for progress form and similar loops.
        /// </summary>
        public const int CHECK_EVENTS_UPDATE_RATE_IN_FPS = 10;

        /// <summary>
        /// Update delay for progress form and similar loops, based on FPS.
        /// </summary>
        public const int CHECK_EVENTS_DELAY_IN_MSEC =
            (int)(1000f / (float)CHECK_EVENTS_UPDATE_RATE_IN_FPS);

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
        public const int BookRefMultiplier = 1000000;

        /// <summary>
        /// Multiplier for chapter numbers in BCV-style references.
        /// </summary>
        public const int ChapRefMultiplier = 1000;

        /// <summary>
        /// Range ref parts (i.e., chapters, verses).
        /// </summary>
        public const int RefPartRange = 1000;

        /// <summary>
        /// A value used to create hashes.
        /// </summary>
        public const int HASH_PRIME = 397;

        /// <summary>
        /// The copyright for this plugin.
        /// </summary>
        public const string COPYRIGHT = "© 2020 Biblica, Inc.";

        /// <summary>
        /// The folder name where checks should be installed. Assumes that the plugin shortname is "TVP".
        /// </summary>
        public const string INSTALLED_CHECK_FOLDER_NAME = @"plugins\TVP\installed-checks";

        /// <summary>
        /// The folder name where checks should be installed. Assumes that the plugin shortname is "TVP".
        /// </summary>
        public const string LOCAL_CHECK_FOLDER_NAME = @"plugins\TVP\local-checks";

        /// <summary>
        /// The file extension used by checks.
        /// </summary>
        public const string CHECK_FILE_EXTENSION = "xml";

        /// <summary>
        /// The max size for displaying book names
        /// </summary>
        public const int MAX_BOOK_NAME_DISPLAY_LENGTH = 20;
    }
}
