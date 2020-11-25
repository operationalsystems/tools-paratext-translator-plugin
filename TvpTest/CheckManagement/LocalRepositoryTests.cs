using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TvpMain.Check;
using TvpMain.CheckManagement;

namespace TvpTest
{
    [TestClass()]
    public class LocalRepositoryTests
    {
        const string filename = "test.xml";
        private readonly string folderPath = Path.Combine(Path.GetTempPath(), "checks");

        private LocalRepository LocalRepository { get; set; }

        [TestInitialize()]
        public void TestSetup()
        {
            LocalRepository = new LocalRepository(folderPath);
        }

        /// <summary>
        /// This test verifies that the <c>LocalRepository</c> class can both add and remove checks from a local repository.
        /// </summary>
        [TestMethod()]
        public void It_can_add_and_remove_checks()
        {
            CheckAndFixItem checkAndFixItem = new CheckAndFixItem
            {
                Version = "1.2.3.4"
            };

            //Throws an exception if Name is null.
            Assert.ThrowsException<ArgumentNullException>(() => LocalRepository.AddCheckAndFixItem(null, checkAndFixItem));

            CheckAndFixItem checkAndFixItem2 = new CheckAndFixItem
            {
                Name = "Test Check",
                Version = "1.2.3.4",
                Description = "A test check",
                CheckRegex = "*.",
                CheckScript = "return null;"
            };

            LocalRepository.AddCheckAndFixItem(filename, checkAndFixItem2);

            List<CheckAndFixItem> checkAndFixItems = LocalRepository.GetCheckAndFixItems();
            Assert.IsTrue(checkAndFixItems.Count == 1);
            Assert.IsTrue(checkAndFixItems[0].Version == "1.2.3.4");
            LocalRepository.RemoveCheckAndFixItem(filename);
            Assert.IsTrue(LocalRepository.GetCheckAndFixItems().Count == 0);
        }

        [TestCleanup()]
        public void TestCleanup()
        {
            Directory.Delete(folderPath);
        }
    }
}
