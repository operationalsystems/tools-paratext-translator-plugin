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
            ScriptureReferenceWrapper result = null;

            builder.TryParseScriptureReference("mat 1:23", out result);
            builder.TryParseScriptureReference("mat 1:1-3", out result);
            builder.TryParseScriptureReference("mat 1:1,3", out result);
            builder.TryParseScriptureReference("mat 1:2–3:4", out result);
            builder.TryParseScriptureReference("mat 1:2; 3:4", out result);
            builder.TryParseScriptureReference("Mat 1:2; Luk 3:4", out result);
            builder.TryParseScriptureReference(@"\xt Mat 1:2; Luk 3:4 \xt*", out result);
            builder.TryParseScriptureReference(@"\xt Mat 1:2; Luk 3:4", out result);
            builder.TryParseScriptureReference(@"Mat 1:2; Luk 3:4 \xt*", out result);
        }
    }
}
