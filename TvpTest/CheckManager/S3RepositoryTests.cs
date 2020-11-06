using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TvpMain.Check;
using TvpMain.CheckManager;

namespace TvpTest
{
    [TestClass()]
    public class S3RepositoryTests
    {
        S3Repository s3Repository = new S3Repository();

        [TestMethod()]
        public void GetCheckAndFixItemsAsyncTest()
        {
            List<CheckAndFixItem> checkAndFixItems = s3Repository.GetCheckAndFixItems();

            Assert.IsTrue(checkAndFixItems.Count > 0);
        }

        [TestMethod()]
        public void AddCheckAndFixItem()
        {
            CheckAndFixItem check = new CheckAndFixItem
            {
                Name = "A Sample Check",
                Version = "1.0.0",
                Description = "A check pushed by the AddCheckAndFixItem test"
            };
            s3Repository.AddCheckAndFixItem("samplecheck-1.0.0.xml", check);

            List<CheckAndFixItem> checkAndFixItems = s3Repository.GetCheckAndFixItems();

            Assert.IsTrue(checkAndFixItems.Contains(check));
        }

        [TestMethod()]
        public void AddCheckAndFixItemAsync()
        {
            CheckAndFixItem check = new CheckAndFixItem
            {
                Name = "A Sample Async Check",
                Version = "1.0.0",
                Description = "A check pushed by the AddCheckAndFixItemAsync test"
            };
            s3Repository.AddCheckAndFixItemAsync("sampleasynccheck-1.0.0.xml", check).Wait();

            List<CheckAndFixItem> checkAndFixItems = s3Repository.GetCheckAndFixItems();

            Assert.IsTrue(checkAndFixItems.Contains(check));
        }
    }
}