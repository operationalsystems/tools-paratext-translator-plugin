using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pidgin;
using TvpMain.Reference;

namespace TvpTest
{
    /// <summary>
    /// Scripture reference parser tests.
    /// </summary>
    [TestClass]
    public class ScriptureReferenceParserTests : AbstractCheckTests
    {
        [TestInitialize]
        public override void TestSetup()
        {
            base.TestSetup();
        }

        /// <summary>
        /// A starter test.
        /// </summary>
        [TestMethod]
        public void FirstTest()
        {
            var builder = new ScriptureReferenceBuilder(MockProjectManager.Object);
            var parser1 = builder.OpeningScriptureReferenceTag();
            string openingTag = parser1.ParseOrThrow(@"\xt");
            openingTag = parser1.ParseOrThrow(@"\+xt");
            openingTag = parser1.ParseOrThrow(@"\+xt foo");
            openingTag = parser1.ParseOrThrow(@"\+xt foo \xt *");

            var parser2 = builder.ClosingScriptureReferenceTag();
            string closingTag = parser2.ParseOrThrow(@"\xt*");
            closingTag = parser2.ParseOrThrow(@"\+xt*");
            closingTag = parser2.ParseOrThrow(@"\+xt* foo");

            var parser3 = builder.CreateParser();
            ScriptureReferenceWrapper result = parser3.ParseOrThrow("mat 1:23");
            result = parser3.ParseOrThrow("mat 1:1-3");
            result = parser3.ParseOrThrow("mat 1:1,3");
            result = parser3.ParseOrThrow("mat 1:2–3:4");
            result = parser3.ParseOrThrow("mat 1:2; 3:4");
            result = parser3.ParseOrThrow("Mat 1:2; Luk 3:4");
            result = parser3.ParseOrThrow(@"\xt Mat 1:2; Luk 3:4 \xt*");
            result = parser3.ParseOrThrow(@"\xt Mat 1:2; Luk 3:4");
            result = parser3.ParseOrThrow(@"Mat 1:2; Luk 3:4 \xt*");
        }
    }
}
