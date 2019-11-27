using System;
using System.Collections.Generic;
using AddInSideViews;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TvpMain.Data;
using TvpMain.Filter;

namespace TvpTest
{
    /// <summary>
    /// Basic tests for ignore list and biblical terms filters.
    /// </summary>
    [TestClass]
    public class FilterTests
    {
        [TestMethod]
        public void TestIgnoreListFilter()
        {
            // empty checks
            IgnoreListTextFilter ignoreFilter = new IgnoreListTextFilter();
            Assert.IsTrue(ignoreFilter.IsEmpty);

            IList<IgnoreListItem> ignoreList = new List<IgnoreListItem>();

            ignoreList.Add(new IgnoreListItem("CaseSensitiveWord1", false));
            ignoreList.Add(new IgnoreListItem("CaseSensitiveWord2", false));
            ignoreList.Add(new IgnoreListItem("CaseInSensitiveWord", true));
            ignoreList.Add(new IgnoreListItem("Case Sensitive Phrase", false));
            ignoreList.Add(new IgnoreListItem("Case InSensitive Phrase", true));

            ignoreFilter.SetIgnoreList(ignoreList);
            Assert.IsFalse(ignoreFilter.IsEmpty);

            // collection composition
            Assert.AreEqual(2, ignoreFilter.CaseSensitiveWords.Count);
            Assert.IsTrue(ignoreFilter.CaseSensitiveWords.Contains("CaseSensitiveWord1"));
            Assert.IsTrue(ignoreFilter.CaseSensitiveWords.Contains("CaseSensitiveWord2"));

            Assert.AreEqual(1, ignoreFilter.CaseInsensitiveWords.Count);
            Assert.IsTrue(ignoreFilter.CaseInsensitiveWords.Contains("CaseInSensitiveWord".ToLower()));

            Assert.AreEqual(1, ignoreFilter.CaseSensitivePhrases.Count);
            Assert.IsTrue(ignoreFilter.CaseSensitivePhrases.Contains("Case Sensitive Phrase"));

            Assert.AreEqual(1, ignoreFilter.CaseInsensitivePhrases.Count);
            Assert.IsTrue(ignoreFilter.CaseInsensitivePhrases.Contains("Case InSensitive Phrase".ToLower()));

            // filtering
            Assert.IsTrue(ignoreFilter.FilterText("CaseSensitiveWord1"));
            Assert.IsFalse(ignoreFilter.FilterText("caseSensitiveWord1"));
            Assert.IsTrue(ignoreFilter.FilterText("CaseSensitiveWord2"));
            Assert.IsFalse(ignoreFilter.FilterText("caseSensitiveWord2"));
            Assert.IsFalse(ignoreFilter.FilterText("CaseSensitiveWord"));

            Assert.IsTrue(ignoreFilter.FilterText("CaseInSensitiveWord"));
            Assert.IsTrue(ignoreFilter.FilterText("caseInSensitiveWord"));

            Assert.IsTrue(ignoreFilter.FilterText("Case Sensitive Phrase"));
            Assert.IsFalse(ignoreFilter.FilterText("case Sensitive Phrase"));

            Assert.IsTrue(ignoreFilter.FilterText("Case InSensitive Phrase"));
            Assert.IsTrue(ignoreFilter.FilterText("case InSensitive Phrase"));

            Assert.IsFalse(ignoreFilter.FilterText("ASDFqwer1234"));
        }

        [TestMethod]
        public void TestBiblicalTermsFilter()
        {
            // empty checks
            BiblicalTermsTextFilter termFilter = new BiblicalTermsTextFilter();
            Assert.IsTrue(termFilter.IsEmpty);

            IList<IKeyTerm> termList = new List<IKeyTerm>();

            termList.Add(new TestKeyTerm("CaseSensitiveWord1"));
            termList.Add(new TestKeyTerm("CaseSensitiveWord2"));
            termList.Add(new TestKeyTerm("Case Sensitive Phrase"));

            termFilter.SetKeyTerms(termList);
            Assert.IsFalse(termFilter.IsEmpty);

            // collection composition
            Assert.AreEqual(2, termFilter.CaseSensitiveWords.Count);
            Assert.IsTrue(termFilter.CaseSensitiveWords.Contains("CaseSensitiveWord1"));
            Assert.IsTrue(termFilter.CaseSensitiveWords.Contains("CaseSensitiveWord2"));
            Assert.AreEqual(0, termFilter.CaseInsensitiveWords.Count);

            Assert.AreEqual(1, termFilter.CaseSensitivePhrases.Count);
            Assert.IsTrue(termFilter.CaseSensitivePhrases.Contains("Case Sensitive Phrase"));
            Assert.AreEqual(0, termFilter.CaseInsensitivePhrases.Count);

            // filtering
            Assert.IsTrue(termFilter.FilterText("CaseSensitiveWord1"));
            Assert.IsFalse(termFilter.FilterText("caseSensitiveWord1"));
            Assert.IsTrue(termFilter.FilterText("CaseSensitiveWord2"));
            Assert.IsFalse(termFilter.FilterText("caseSensitiveWord2"));
            Assert.IsFalse(termFilter.FilterText("CaseSensitiveWord"));

            Assert.IsTrue(termFilter.FilterText("Case Sensitive Phrase"));
            Assert.IsFalse(termFilter.FilterText("case Sensitive Phrase"));

            Assert.IsFalse(termFilter.FilterText("ASDFqwer1234"));
        }

        /// <summary>
        /// Test key term class.
        /// </summary>
        private class TestKeyTerm : IKeyTerm
        {
            public string Term { get; }

            public string Id => throw new NotImplementedException();

            public IList<int> BcvOccurences => throw new NotImplementedException();

            public int Category => throw new NotImplementedException();

            /// <summary>
            /// Basic ctor.
            /// </summary>
            /// <param name="inputTerm">Input term (required).</param>
            public TestKeyTerm(string inputTerm)
            {
                Term = inputTerm;
            }
        }
    }
}
