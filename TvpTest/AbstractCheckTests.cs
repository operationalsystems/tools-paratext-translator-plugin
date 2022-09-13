﻿/*
Copyright © 2022 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using AddInSideViews;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Paratext.Data;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using TvpMain.Import;
using TvpMain.Project;
using TvpMain.Result;
using TvpMain.Text;
using TvpMain.Util;

namespace TvpTest
{
    public abstract class AbstractCheckTests
    {
        /// <summary>
        /// Test book number, for book-scale tests.
        /// </summary>
        protected const int TEST_BOOK_NUM = 10;

        /// <summary>
        /// Test chapter number, for chapter-scale tests.
        /// </summary>
        protected const int TEST_CHAPTER_NUM = 10;

        /// <summary>
        /// Multiplier for book numbers in BCV-style references.
        /// </summary>
        protected static readonly int TestBookRefMultiplier = 1000000;

        /// <summary>
        /// Multiplier for chapter numbers in BCV-style references.
        /// </summary>
        protected static readonly int TestChapRefMultiplier = 1000;

        /// <summary>
        /// Range ref parts (i.e., chapters, verses).
        /// </summary>
        protected static readonly int TestRefPartRange = 1000;

        /// <summary>
        /// Minimum number of books in test datasets.
        /// </summary>
        protected static readonly int MinTestBooks = 11;

        /// <summary>
        /// Minimum number of verses in test datasets.
        /// </summary>
        protected static readonly int MinTestVerses = 111;

        /// <summary>
        /// Test project name.
        /// </summary>
        protected const string TEST_PROJECT_NAME = "testProjectName";

        /// <summary>
        /// Test language name.
        /// </summary>
        protected const string TEST_LANGUAGE = "English";

        /// <summary>
        /// Test language ISO code.
        /// </summary>
        protected const string TEST_LANGUAGE_ISO_CODE = "en::US:";

        /// <summary>
        /// Test versification name.
        /// </summary>
        protected const string TEST_VERSIFICATION_NAME = "testVersificationName";

        /// <summary>
        /// Books present setting for test project.
        /// </summary>
        protected const string TEST_BOOKS_PRESENT_SETTING =
            "111111111111111111111111111111111111111111111111111111111111111111000000000000000000000000000000000000000000000000000000000";

        /// <summary>
        /// Chapter verse separator setting.
        /// </summary>
        protected const string CHAPTER_VERSE_SEPARATOR_SETTING = ":";

        /// <summary>
        /// Chapter verse separator setting.
        /// </summary>
        protected const string RANGE_INDICATOR_SETTING = "-";

        /// <summary>
        /// Sequence indicator setting.
        /// </summary>
        protected const string SEQUENCE_INDICATOR_SETTING = ",|;| («terrenal»),";

        /// <summary>
        /// Chapter range separator setting.
        /// </summary>
        protected const string CHAPTER_RANGE_SEPARATOR_SETTING = "–| al |—|b—";

        /// <summary>
        /// Book sequence separator setting.
        /// </summary>
        protected const string BOOK_SEQUENCE_SEPARATOR_SETTING = "; ";

        /// <summary>
        /// Chapter number separator setting.
        /// </summary>
        protected const string CHAPTER_NUMBER_SEPARATOR_SETTING = "; | y | ( ";

        /// <summary>
        /// Reference extra material setting.
        /// </summary>
        protected const string REFERENCE_EXTRA_MATERIAL_SETTING = "a|Salmos |capítulos |capítulo |cap. |Cap. | –| Tít.-50 –|cf.|) ";

        /// <summary>
        /// Reference final punctuation setting.
        /// </summary>
        protected const string REFERENCE_FINAL_PUNCTUATION_SETTING = "";

        /// <summary>
        /// Target reference name type setting.
        /// </summary>
        protected const string TARGET_NAME_TYPE_SETTING = "Abbreviation";

        /// <summary>
        /// Parallel passage name type setting.
        /// </summary>
        protected const string PASSAGE_NAME_TYPE_SETTING = "ShortName";


        /// <summary>
        /// Map of book name items by book number (1-based), used by mock project manager.
        /// </summary>
        private IDictionary<int, BookNameItem> _testBookNamesByNum;

        /// <summary>
        /// Map of book name items by all names, used by mock project manager.
        /// </summary>
        private IDictionary<string, BookNameItem> _testBookNamesByAllNames;

        /// <summary>
        /// Mock Paratext scripture host.
        /// </summary>
        protected Mock<IHost> MockHost { get; private set; }

        /// <summary>
        /// Mock ParatextData project proxy.
        /// </summary>
        protected Mock<ScrText> MockScrText { get; private set; }

        /// <summary>
        /// Mock ParatextUser for use when creating Scr Text mock
        /// </summary>
        protected Mock<Paratext.Data.Users.ParatextUser> MockParatextUser { get; private set; }

        /// <summary>
        /// Mock file manager.
        /// </summary>
        protected Mock<FileManager> MockFileManager { get; private set; }

        /// <summary>
        /// Mock project manager.
        /// </summary>
        protected Mock<ProjectManager> MockProjectManager { get; private set; }

        /// <summary>
        /// Mock result manager.
        /// </summary>
        protected Mock<ResultManager> MockResultManager { get; private set; }

        /// <summary>
        /// Mock content import manager.
        /// </summary>
        protected Mock<ImportManager> MockImportManager { get; private set; }

        /// <summary>
        /// Test setup for verse lines and main mocks.
        /// </summary>
        /// <param name="testContext">Per-test test context (required).</param>
        public virtual void AbstractTestSetup(TestContext testContext)
        {
            MockHost = new Mock<IHost>(MockBehavior.Strict);

            // host setup
            MockHost.Setup(hostItem => hostItem.GetProjectVersificationName(TEST_PROJECT_NAME))
                .Returns(TEST_VERSIFICATION_NAME);
            MockHost.Setup(hostItem => hostItem.GetCurrentRef(TEST_VERSIFICATION_NAME))
                .Returns<string>((versificationName) =>
                GetVerseRef(TEST_BOOK_NUM, TEST_CHAPTER_NUM, 1));
            MockHost.Setup(hostItem => hostItem.GetLastChapter(It.IsAny<int>(), TEST_VERSIFICATION_NAME))
                .Returns<int, string>((bookNum, versificationName) => bookNum + MinTestBooks);
            MockHost.Setup(hostItem => hostItem.GetLastVerse(It.IsAny<int>(), It.IsAny<int>(), TEST_VERSIFICATION_NAME))
                .Returns<int, int, string>((bookNum, chapterNum, versificationName) => chapterNum + MinTestVerses);
            MockHost.Setup(hostItem => hostItem.GetProjectSetting(TEST_PROJECT_NAME, "BooksPresent"))
                .Returns<string, string>((projectName, settingsKey) => TEST_BOOKS_PRESENT_SETTING);
            MockHost.Setup(hostItem => hostItem.GetProjectSetting(TEST_PROJECT_NAME, "ChapterVerseSeparator"))
                .Returns<string, string>((projectName, settingsKey) => CHAPTER_VERSE_SEPARATOR_SETTING);
            MockHost.Setup(hostItem => hostItem.GetProjectSetting(TEST_PROJECT_NAME, "RangeIndicator"))
                .Returns<string, string>((projectName, settingsKey) => RANGE_INDICATOR_SETTING);
            MockHost.Setup(hostItem => hostItem.GetProjectSetting(TEST_PROJECT_NAME, "SequenceIndicator"))
                .Returns<string, string>((projectName, settingsKey) => SEQUENCE_INDICATOR_SETTING);
            MockHost.Setup(hostItem => hostItem.GetProjectSetting(TEST_PROJECT_NAME, "ChapterRangeSeparator"))
                .Returns<string, string>((projectName, settingsKey) => CHAPTER_RANGE_SEPARATOR_SETTING);
            MockHost.Setup(hostItem => hostItem.GetProjectSetting(TEST_PROJECT_NAME, "BookSequenceSeparator"))
                .Returns<string, string>((projectName, settingsKey) => BOOK_SEQUENCE_SEPARATOR_SETTING);
            MockHost.Setup(hostItem => hostItem.GetProjectSetting(TEST_PROJECT_NAME, "ChapterNumberSeparator"))
                .Returns<string, string>((projectName, settingsKey) => CHAPTER_NUMBER_SEPARATOR_SETTING);
            MockHost.Setup(hostItem => hostItem.GetProjectSetting(TEST_PROJECT_NAME, "ReferenceExtraMaterial"))
                .Returns<string, string>((projectName, settingsKey) => REFERENCE_EXTRA_MATERIAL_SETTING);
            MockHost.Setup(hostItem => hostItem.GetProjectSetting(TEST_PROJECT_NAME, "ReferenceFinalPunctuation"))
                .Returns<string, string>((projectName, settingsKey) => REFERENCE_FINAL_PUNCTUATION_SETTING);
            MockHost.Setup(hostItem => hostItem.GetFigurePath(TEST_PROJECT_NAME, false))
                .Returns<string, bool>((projectName, localFlag) => Path.Combine(Directory.GetCurrentDirectory(), "test"));
            MockHost.Setup(hostItem => hostItem.GetProjectSetting(TEST_PROJECT_NAME, "BookSourceForMarkerXt"))
                .Returns<string, string>((projectName, settingsKey) => TARGET_NAME_TYPE_SETTING);
            MockHost.Setup(hostItem => hostItem.GetProjectSetting(TEST_PROJECT_NAME, "BookSourceForMarkerR"))
                .Returns<string, string>((projectName, settingsKey) => PASSAGE_NAME_TYPE_SETTING);
            MockHost.Setup(hostItem => hostItem.GetProjectSetting(TEST_PROJECT_NAME, "Language"))
                .Returns<string, string>((projectName, settingsKey) => TEST_LANGUAGE);
            MockHost.Setup(hostItem => hostItem.GetProjectSetting(TEST_PROJECT_NAME, "LanguageIsoCode"))
                .Returns<string, string>((projectName, settingsKey) => TEST_LANGUAGE_ISO_CODE);
            MockHost.Setup(hostItem => hostItem.GetPlugInData(It.IsAny<IParatextAddIn>(), TEST_PROJECT_NAME, It.IsAny<string>()))
                .Returns<IParatextAddIn, string, string>((addIn, projectName, dataId) => null);
            MockHost.Setup(hostItem => hostItem.PutPlugInData(It.IsAny<IParatextAddIn>(), TEST_PROJECT_NAME, It.IsAny<string>(), It.IsAny<string>()))
                .Returns<IParatextAddIn, string, string, string>((addIn, projectName, dataId, dataText) => true);
            MockHost.Setup(hostItem => hostItem.GetFigurePath(TEST_PROJECT_NAME, false))
                .Returns<string, bool>((projectName, isLocal) => Path.Combine(testContext.DeploymentDirectory, "figure"));

            // host util setup
            HostUtil.Instance.Host = MockHost.Object;

            MockParatextUser = new Mock<Paratext.Data.Users.ParatextUser>("Admin", true);

            // create mock paratext project proxy & related
            MockScrText = new Mock<ScrText>(MockParatextUser.Object);
            MockScrText.Setup(textItem => textItem.Parser)
                .Returns((ScrParser)null);
            // Note: ScrParser _may not_ be mocked due to internal ctor w/out modifying ParatextData:
            // https://github.com/Moq/moq4/wiki/Quickstart#advanced-features

            // create mock project managers & related
            MockFileManager = new Mock<FileManager>(
                    MockHost.Object,
                    TEST_PROJECT_NAME)
            { CallBase = true };
            MockProjectManager = new Mock<ProjectManager>(
                MockHost.Object,
                TEST_PROJECT_NAME,
                MockFileManager.Object)
            { CallBase = true };
            MockResultManager = new Mock<ResultManager>(
                MockHost.Object,
                TEST_PROJECT_NAME)
            { CallBase = true };
            MockImportManager = new Mock<ImportManager>(
                    MockHost.Object,
                    TEST_PROJECT_NAME,
                    MockScrText.Object)
            { CallBase = true };

            // project manager setup
            _testBookNamesByNum = BookUtil.BookIdList
                .Select(value =>
                    new BookNameItem(value.BookCode, value.BookNum, value.BookCode, value.BookTitle, value.BookTitle))
                .ToImmutableDictionary(value => value.BookNum);

            var tempTestBookNamesByAllNames = new Dictionary<string, BookNameItem>();
            foreach (var nameItem in _testBookNamesByNum.Values)
            {
                tempTestBookNamesByAllNames[nameItem.BookCode.ToLower()] = nameItem;
                tempTestBookNamesByAllNames[nameItem.Abbreviation.ToLower()] = nameItem;
                tempTestBookNamesByAllNames[nameItem.ShortName.ToLower()] = nameItem;
                tempTestBookNamesByAllNames[nameItem.LongName.ToLower()] = nameItem;
            }
            _testBookNamesByAllNames = tempTestBookNamesByAllNames.ToImmutableDictionary();

            MockProjectManager.Setup(managerItem => managerItem.BookNamesByNum)
                .Returns(_testBookNamesByNum);
            MockProjectManager.Setup(managerItem => managerItem.BookNamesByAllNames)
                .Returns(_testBookNamesByAllNames);
        }

        /// <summary>
        /// Turn independent BCV values into a coordinate.
        /// </summary>
        /// <param name="bookNum">Paratext book number (1-66).</param>
        /// <param name="chapterNum">Paratext chapter number (book- and versification-specific).</param>
        /// <param name="verseNum">Paratext verse number (chapter- and versification-specific).</param>
        protected static int GetVerseRef(int bookNum, int chapterNum, int verseNum)
        {
            var verseRef = bookNum * TestBookRefMultiplier;
            verseRef += chapterNum * TestChapRefMultiplier;
            verseRef += verseNum;
            return verseRef;
        }
    }
}
