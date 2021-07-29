﻿/*
 * A Class to handle the Global Constants.
 */

using TvpMain.Check;

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
        public const string COPYRIGHT = "© 2021 Biblica, Inc.";

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

        /// <summary>
        /// Number of seconds to wait after changes to save result items.
        /// </summary>
        public const int RESULT_ITEM_SAVE_DELAY_IN_SEC = 5;

        /// <summary>
        /// Plugin data identifier format for result items.
        /// </summary>
        public const string RESULT_ITEMS_DATA_ID_FORMAT = "ResultItems-{0}.json";

        /// <summary>
        /// Number of seconds to wait before loading result items.
        /// </summary>
        public const int RESULT_ITEM_LOAD_DELAY_IN_SEC = 5;

        /// <summary>
        /// Paratext project settings filename.
        /// </summary>
        public const string SETTINGS_FILE_PATH = "Settings.xml";

        /// <summary>
        /// This is the hardcoded id for the TVP V1 scripture reference checks. 
        /// This value should not be changed unless the check is intended to do something completely different from initially implemented.
        /// </summary>
        public const string V1_SCRIPTURE_REFERENCE_CHECK_GUID = "7335bbdc-3a75-41dd-b21d-03c817907309";

        /// <summary>
        /// This is the hardcoded id for the TVP V1 missing punctuation checks. 
        /// This value should not be changed unless the check is intended to do something completely different from initially implemented.
        /// </summary>
        public const string V1_PUNCTUATION_CHECK_GUID = "239ca2c4-98ee-11eb-a8b3-0242ac130003";

        public const string PERMISSIONS_FILE_NAME = "permission_list.csv";
    }
}
