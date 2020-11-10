using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TvpMain.Check;
using TvpMain.CheckManager;

namespace TvpTest
{
    public class TestFolderRepository : FolderRepository
    {
        public const string folderName = "checks";
        public override string FolderPath => Path.Combine(Directory.GetCurrentDirectory(), folderName);
    }

    [TestClass()]
    public class FolderRepositoryTests
    {
        const string filename = "test.xml";

        private readonly TestFolderRepository testFolderRepository = new TestFolderRepository();

        [TestMethod()]
        public void AddCheckAndFixItemsTest()
        {
            CheckAndFixItem checkAndFixItem = new CheckAndFixItem
            {
                Version = "1.2.3.5"
            };

            //Throws an exception if Name is null.
            Assert.ThrowsException<ArgumentNullException>(() => testFolderRepository.AddCheckAndFixItem(null, checkAndFixItem));

            CheckAndFixItem checkAndFixItem2 = new CheckAndFixItem
            {
                Name = "Test Check",
                Version = "1.2.3.5",
                Description = "A test check",
                CheckRegex = "*.",
                FixRegex = "*.",
                CheckScript = "return null;",
                FixScript = ""
            };

            testFolderRepository.AddCheckAndFixItem(filename, checkAndFixItem2);

            List<CheckAndFixItem> checkAndFixItems = testFolderRepository.GetCheckAndFixItems();
            Assert.IsTrue(checkAndFixItems.Count == 1);
            Assert.IsTrue(checkAndFixItems[0].Version == "1.2.3.5");
        }

        [TestCleanup()]
        public void TestCleanup()
        {
            File.Delete(Path.Combine(testFolderRepository.FolderPath, filename));
            Directory.Delete(testFolderRepository.FolderPath);
        }
    }
}
