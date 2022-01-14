/*
Copyright © 2021 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using TvpMain.Reference;
using TvpMain.Text;

namespace TvpTest
{
    /// <summary>
    /// Scripture reference parser tests.
    /// </summary>
    [TestClass]
    public class ScriptureReferenceParserTests : AbstractCheckTests
    {
        /// <summary>
        /// Per-test context, provided by MsTest framework.
        /// </summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// Test parser output lines
        /// </summary>
        protected IList<string> ParserResultLines;

        /// <summary>
        /// Test formatter output lines
        /// </summary>
        protected IList<string> FormatterResultLines;

        [TestInitialize]
        [DeploymentItem(@"Resources\parser-results-1.txt", "Resources")]
        [DeploymentItem(@"Resources\formatter-results-1.txt", "Resources")]
        public void TestSetup()
        {
            base.AbstractTestSetup(TestContext);
        }

        /// <summary>
        /// A starter test for the parser.
        /// 
        /// Parses from the test references, then serializes as JSON
        /// and compares with expected results in resource content file.
        /// </summary>
        [TestMethod]
        public void PositiveParserTest()
        {
            // setup: read expected results from resource
            ParserResultLines = File.ReadAllLines(@"Resources\parser-results-1.txt").ToImmutableList();
            Assert.AreEqual(31, ParserResultLines.Count,
                "Unexpected parser result count in test input.");

            var builder = new ScriptureReferenceBuilder(MockProjectManager.Object);
            var ctr = 0;

            foreach (var testText in CreateTestReferenceText())
            {
                Assert.IsTrue(builder.TryParseScriptureReference(testText, out var result),
                    $"Can't parse entry #{ctr + 1}, text: {testText}");
                Assert.AreEqual(ParserResultLines[ctr].Trim(),
                    result.ToString().Trim(),
                    $"Can't verify entry #{ctr + 1}, text: {testText}");

                // Console.Out.WriteLine(testText);
                // Console.Out.WriteLine(result);
                // Console.Out.WriteLine(builder.FormatStandardReference(PartContext.MainText, result));

                ctr++;
            }
        }

        /// <summary>
        /// A starter test for the formatter.
        ///
        /// Parses from the test references, re-renders as standard references,
        /// then compares with expected results in resource content file.
        /// </summary>
        [TestMethod]
        public void PositiveFormatterTest()
        {
            // setup: read expected results from resource
            FormatterResultLines = File.ReadAllLines(@"Resources\formatter-results-1.txt").ToImmutableList();
            Assert.AreEqual(31, FormatterResultLines.Count,
                "Unexpected formatter result count in test input.");

            var builder = new ScriptureReferenceBuilder(MockProjectManager.Object);
            var ctr = 0;

            foreach (var testText in CreateTestReferenceText())
            {
                Assert.IsTrue(builder.TryParseScriptureReference(testText, out var result),
                    $"Can't parse entry #{ctr + 1}, text: {testText}");
                Assert.AreEqual(FormatterResultLines[ctr].Trim(),
                    builder.FormatStandardReference(PartContext.MainText, result).Trim(),
                    $"Can't verify entry #{ctr + 1}, text: {testText}");

                // Console.Out.WriteLine(testText);
                // Console.Out.WriteLine(result);
                // Console.Out.WriteLine(builder.FormatStandardReference(PartContext.MainText, result));

                ctr++;
            }
        }

        /// <summary>
        /// Provides some typical test reference text.
        /// </summary>
        /// <returns>List of test references.</returns>
        private IEnumerable<string> CreateTestReferenceText()
        {
            return new List<string>()
            {
                @"19–20;21–23;25",
                @"1-20",
                @"1-20,21,22",
                @"1,20,21,22",
                @"20,21,22",
                @"20;21;22",
                @"mat 1:23",
                @"mat 1:1-3",
                @"mat 1:1,3",
                @"Mat 1:2–3:4",
                @"Mat 1:2–3:4; 5:6–7:8",
                @"Mat 1:2–3:4; 5:6–7:8; Luk 10:20–30:40; 50:60–70:80",
                @"Mat 1:2–3:4; 5:6–7:8; mrk 10:20–30:40; 50:60–70:80,83,85,87",
                @"Mat 1:2–3:4; 5:6–7:8; luk 10:20–30:40; 50:60–70:80; Mrk 11:21–31:41; 51:61–71:81-83,85,87",
                @"Mat 1:2; 3:4",
                @"Mat 1:2; luk 3:4",
                @"\xt Mat 1:2; Luk 3:4 \xt*",
                @"\xt Mat 1:2; Luk 3:4",
                @"mat 1:2; Luk 3:4 \xt*",
                @"\xt 1:2; 3:4 \xt*",
                @"\xt 1:2; 3:4",
                @"1:2; 3:4 \xt*",
                @"23",
                @"1-3",
                @"1,3",
                @"\xt 23,1-3,1,3 \xt*",
                @"\xt 23,1-3,1,3",
                @"23,1-3,1,3 \xt*",
                @"\xt foo 1:23,1-3,1,3 \xt*",
                @"\xt foO 1:23,1-3,1,3",
                @"Foo 1:23,1-3,1,3 \xt*"
            }.ToImmutableList();
        }
    }
}
