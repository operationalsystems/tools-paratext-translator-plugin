﻿/*
Copyright © 2022 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using AddInSideViews;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TvpMain.Project;
using TvpMain.Util;

namespace TvpTest
{
    [TestClass]
    public class ProjectManagerTests
    {

        /// <summary>
        /// Mock Host class
        /// </summary>
        private Mock<IHost> _mockHost;

        /// <summary>
        /// Mock File Manager
        /// </summary>
        private Mock<FileManager> _mockFileManager;

        /// <summary>
        /// Mock Project Manager
        /// </summary>
        private Mock<ProjectManager> _mockProjectManager;

        private const string TEST_PROJECT_NAME = "testProjectManagerProject";
        private const string TEST_PROJECT_PATH = @"Resources\testProjects\" + TEST_PROJECT_NAME;

        /// <summary>
        /// Test setup.
        /// </summary>
        [TestInitialize]
        public void TestSetup()
        {
            // create mocks

            // mock: host util
            _mockHost = new Mock<IHost>();
            _mockHost
                .Setup(host => host.GetFigurePath(TEST_PROJECT_NAME, It.IsAny<bool>()))
                .Returns(@$"{TEST_PROJECT_PATH}\figures");

            // mock: file manager
            _mockFileManager = new Mock<FileManager>(
                _mockHost.Object,
                TEST_PROJECT_NAME
                );

            // mock: project manager
            _mockProjectManager = new Mock<ProjectManager>(
                _mockHost.Object,
                TEST_PROJECT_NAME,
                _mockFileManager.Object
                )
            {
                // call base functions unless overridden
                CallBase = true
            };
        }

        /// <summary>
        /// Test that Project Manager correctly maps books to ID.
        /// </summary>
        [TestMethod]
        public void TestProjectManagerBookIdMapping()
        {
            var bookNamesByNum = _mockProjectManager.Object.BookNamesByNum;
            
            // check generic assumptions
            Assert.IsNotNull(bookNamesByNum);
            Assert.IsTrue(bookNamesByNum.Count > 0, "bookNamesByNum unexpectedly returned empty");

            // establish what expected output book names are for input book codes for test project
            var expectedInputsVsOuputs = new Dictionary<string, int>()
            {
                {"GEN", 1}, {"JOB", 18}, {"MAT", 40},
                {"COL", 51}, {"2JN", 63}, {"REV", 66},
                {"XXA", 93}, {"FRT", 100}, {"GLO", 109}
            };

            foreach (var entry in expectedInputsVsOuputs)
            {
                Assert.AreEqual(entry.Key, bookNamesByNum[entry.Value].BookCode);
            }

            // make sure that books without attributes are not included in the books.xml
            var expectedMissingBooks = new Dictionary<string,int>()
            {
                {"NDX", 111}, {"TOB", 67}, {"WIS", 70}, 
                {"LJE", 73}, {"SUS", 75}, {"3MA", 79}, 
                {"PS2", 84}, {"JDB", 88}, {"BLT", 92}
            };

            // catches if an expected missing book is found in the books.xml
            foreach (var entry in expectedMissingBooks)
            {
                Assert.IsFalse(bookNamesByNum.ContainsKey(entry.Value), $"{entry.Key} unexpectedly appears in the book name list.");
            }
        }
    }
}
