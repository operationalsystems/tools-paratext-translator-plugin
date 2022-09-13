﻿/*
Copyright © 2022 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using TvpMain.Check;

namespace TvpTest
{
    [TestClass]

    public class LocalCheckAndFixManagerTests
    {
        /// <summary>
        /// Test path to create files for testing purposes.
        /// </summary>
        static readonly DirectoryInfo testPath = new DirectoryInfo(Path.Combine(Path.GetTempPath(), nameof(LocalCheckAndFixManagerTests)));

        static readonly Random rand = new Random();

        [TestInitialize]
        public void Setup()
        {
            if (testPath.Exists)
            {
                testPath.Delete(true);
            }

            testPath.Create();
        }

        [TestMethod]
        public void TestGetFiles()
        {
            var localManager = new LocalCheckAndFixManager(testPath);

            // add first item
            var testCafItem1 = CreateTestItem();

            localManager.Create(testCafItem1);

            // add second item
            var testCafItem2 = CreateTestItem();

            localManager.Create(testCafItem2);

            var items = localManager.GetAll();

            Assert.AreEqual(2, items.Count);
            Assert.IsTrue(items.Contains(testCafItem1));
            Assert.IsTrue(items.Contains(testCafItem2));
        }

        [TestMethod]
        public void TestDelete()
        {
            var localManager = new LocalCheckAndFixManager(testPath);

            // create an item
            var testCafItem = CreateTestItem();
            localManager.Create(testCafItem);

            // assess the item is as expected
            var returnedCafItem = localManager.Get(testCafItem.Name, testCafItem.Version);
            Assert.AreEqual(testCafItem, returnedCafItem);

            // delete the item
            localManager.Delete(testCafItem.Name, testCafItem.Version);

            // test that we can't retrieve the deleted item
            var failedAsExpected = false;
            try
            {
                localManager.Get(testCafItem.Name, testCafItem.Version);
            }
            catch
            {
                failedAsExpected = true;
            }

            Assert.IsTrue(failedAsExpected, "Item wasn't deleted as expected.");
        }

        [TestMethod]
        public void TestUpdate()
        {
            var localManager = new LocalCheckAndFixManager(testPath);

            // create an item
            var testCafItem = CreateTestItem();
            var originalName = testCafItem.Name;
            var originalVersion = testCafItem.Version;
            localManager.Create(testCafItem);

            // assess the item is as expected
            var returnedCafItem = localManager.Get(originalName, originalVersion);
            Assert.AreEqual(testCafItem, returnedCafItem);

            // update the item (hold onto the version as it should be revved if the version hasn't changed)
            var previousVersion = returnedCafItem.Version;
            localManager.Update(returnedCafItem);

            // test that the returned updated item is equal (with the version change)
            var returnedUpdatedCaF = localManager.Get(originalName, returnedCafItem.Version);
            Assert.AreEqual(returnedCafItem, returnedUpdatedCaF);
            var prevVer = new Version(previousVersion);
            var updVer = new Version(returnedUpdatedCaF.Version);
            Assert.AreEqual(prevVer.Major, updVer.Major);
            Assert.AreEqual(prevVer.Minor, updVer.Minor);
            Assert.AreEqual(prevVer.Build + 1, updVer.Build);
        }

        private static CheckAndFixItem CreateTestItem()
        {
            var rngNumber = rand.Next();

            return new CheckAndFixItem()
            {
                Name = $"testName{rngNumber}",
                Description = $"testDescription{rngNumber}",
                Version = $"{rngNumber}.0.0",
                CheckRegex = @$"\b\w*z+\w*\b",
                CheckScript = @"function(input){
                    return input;
                }",
            };
        }
    }
}
