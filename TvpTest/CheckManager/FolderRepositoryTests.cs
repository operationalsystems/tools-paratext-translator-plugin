using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PtxUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TvpMain.Check;
using TvpMain.CheckManager;

namespace TvpTest
{
    [TestClass()]
    public class FolderRepositoryTests
    {
        private Mock<FolderRepository> mockFolderRepository;

        [TestInitialize]
        [DeploymentItem(@"Resources\checks\test-checkandfixitem-1.xml", "Resources")]
        public void TestSetup()
        {
            // Set up a fully-functional mock of the repository class so that we can easily override members and methods.
            mockFolderRepository = new Mock<FolderRepository>
            {
                CallBase = true
            };
            mockFolderRepository.Setup(folderRepository => folderRepository.FolderPath).Returns("Resources\\checks");
        }

        [TestMethod()]
        public void GetCheckAndFixItemsTest()
        {
            List<TvpMain.Check.CheckAndFixItem> checkAndFixItems = mockFolderRepository.Object.GetCheckAndFixItems();
            Assert.IsTrue(checkAndFixItems.Count == 1);
        }

        [TestMethod()]
        public void GetCheckAndFixItemsTestAsync()
        {
            Task<List<TvpMain.Check.CheckAndFixItem>> checkAndFixItems = mockFolderRepository.Object.GetCheckAndFixItemsAsync();
            checkAndFixItems.Wait();
            Assert.IsTrue(checkAndFixItems.Result.Count == 1);
        }

        [TestMethod()]
        public void AddCheckAndFixItemsTest()
        {
            CheckAndFixItem checkAndFixItem = new CheckAndFixItem
            {
                Version = "1.2.3.4"
            };

            //Throws an exception if Name is null.
            Assert.ThrowsException<ArgumentNullException>(() => mockFolderRepository.Object.AddCheckAndFixItem(checkAndFixItem));
        }

        [TestMethod()]
        public void AddCheckAndFixItemsTestAsync()
        {
            CheckAndFixItem checkAndFixItem = new CheckAndFixItem
            {
                Version = "1.2.3.4"
            };

            //Throws an exception if Name is null. Tasks throw AggregateExceptions, so we need to check the first error within.
            try
            {
                mockFolderRepository.Object.AddCheckAndFixItemAsync(checkAndFixItem).Wait();
            } catch (AggregateException ae)
            {
                var e = ae.Flatten().InnerExceptions[0];
                // Check the type of the first exception.
                if (e.GetType() == typeof(ArgumentNullException))
                {
                    Assert.IsTrue(true);
                } else
                {
                    Assert.Fail($"Expected ArgumentNullException, got {e.GetType()}.");
                }
            }
        }
    }
}
