using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TvpMain.CheckManager;

namespace TvpTest
{
    [TestClass]
    public class CheckManagerTests
    {
        private Mock<CheckManager> mockCheckManager;

        [TestInitialize]
        public void TestSetup()
        {
            mockCheckManager = new Mock<CheckManager>();
        }

        [TestMethod]
        public void TestMethod1()
        {

        }
    }
}
