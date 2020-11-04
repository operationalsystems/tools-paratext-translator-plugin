using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using TvpMain.CheckManager;

namespace TvpTest
{
    [TestClass()]
    public class FolderRepositoryTests
    {
        private Mock<FolderRepository> folderRepository;

        [TestInitialize]
        [DeploymentItem(@"Resources\checks\test-checkandfixitem-1.xml", "Resources")]
        public void TestSetup()
        {
            folderRepository = new Mock<FolderRepository>
            {
                CallBase = true
            };
            folderRepository.Setup(folderRepository => folderRepository.FolderPath).Returns("Resources\\checks");
        }

        [TestMethod()]
        public void GetCheckAndFixItemsTest()
        {
            List<TvpMain.Check.CheckAndFixItem> checkAndFixItems = folderRepository.Object.GetCheckAndFixItems();
            Assert.IsTrue(checkAndFixItems.Count == 1);
        }

        [TestMethod()]
        public void GetCheckAndFixItemsTestAsync()
        {
            Task<List<TvpMain.Check.CheckAndFixItem>> checkAndFixItems = folderRepository.Object.GetCheckAndFixItemsAsync();
            checkAndFixItems.Wait();
            Assert.IsTrue(checkAndFixItems.Result.Count == 1);
        }
    }
}
