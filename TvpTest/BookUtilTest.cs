using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using TvpMain.Text;

namespace TvpTest
{
    [TestClass]
    public class BookUtilTest
    {
        /// <summary>
        /// Test to ensure that the CsvReader works regardless of the user's CurrentCulture
        /// </summary>
        [TestMethod]
        public void TestCsvReaderIsLocaleProof()
        {
            // Hold onto actual culture, to re-set, so that we prevent breaking the culture for other tests
            var originalCulture = Thread.CurrentThread.CurrentCulture;

            const int expectedBookCount = 124;
            // expected values for 1st letter of John. middle of the list
            const int book1jnId = 62;
            const string expected1jnCode = "1JN";
            const string expected1jnUsfmNum = "63";
            
            try
            {
                // US local culture first
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                var bookListUS = CreateAndCallCsvReader();
                Assert.AreEqual(expectedBookCount, bookListUS.Count);
                Assert.AreEqual(book1jnId, bookListUS[book1jnId].BookNum);
                Assert.AreEqual(expected1jnCode, bookListUS[book1jnId].BookCode);
                Assert.AreEqual(expected1jnUsfmNum, bookListUS[book1jnId].UsfmBookNum);

                // Russian culture.
                // "ru-RU" returns SpecificCulture, "ru" return NeutralCulture. We need specific in this case
                Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU");
                var bookListRU = CreateAndCallCsvReader();
                Assert.AreEqual(expectedBookCount, bookListRU.Count);
                Assert.AreEqual(book1jnId, bookListRU[book1jnId].BookNum);
                Assert.AreEqual(expected1jnCode, bookListRU[book1jnId].BookCode);
                Assert.AreEqual(expected1jnUsfmNum, bookListRU[book1jnId].UsfmBookNum);
            }
            finally
            {
                // return to original culture
                Thread.CurrentThread.CurrentCulture = originalCulture;
            }
        }

        /// <summary>
        /// helper function to capture the instantiation and creation of the CSV reader via BookUtil
        /// </summary>
        /// <returns></returns>
        private ImmutableList<BookIdItem> CreateAndCallCsvReader()
        {
            var executingAssembly = typeof(BookUtil).GetTypeInfo().Assembly;
            using var inputStream = executingAssembly.GetManifestResourceStream("TvpMain.Resources.book-ids-1.csv");
            using var streamReader = new StreamReader(inputStream);
            using var csvReader = BookUtil.GetCsvReader(streamReader);
            return csvReader.GetRecords<BookIdItem>().ToImmutableList();
        }
    }
}
