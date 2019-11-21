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
        /// Max number of books in standard versification (1 = Gensis, 66 = Revelations).
        /// </summary>
        public const int MAX_BOOK_NUM = 66; // 66 = all books

        /// <summary>
        /// Abbreviations for bible book names.
        /// </summary>
        public static readonly string[] SHORT_BOOK_NAMES = {
            "GEN","EXO","LEV","NUM","DEU","JOS","JDG","RUT",
            "1SA","2SA","1KI","2KI","1CH","2CH","EZR","NEH",
            "EST","JOB","PSA","PRO","ECC","SNG","ISA","JER",
            "LAM","EZE","DAN","HOS","JOL","AMO","OBA","JON",
            "MIC","NAM","HAB","ZEP","HAG","ZEC","MAL","MAT",
            "MRK","LUK","JHN","ACT","ROM","1CO","2CO","GAL",
            "EPH","PHP","COL","1TH","2TH","1TI","2TI","TIT",
            "PHM","HEB","JAS","1PE","2PE","1JN","2JN","3JN",
            "JUD","REV"
        };
    }
}
