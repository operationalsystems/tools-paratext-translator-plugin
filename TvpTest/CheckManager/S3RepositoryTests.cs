using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TvpMain.Check;
using TvpMain.CheckManager;

namespace TvpTest
{
    public class TestS3Repository : S3Repository
    {
        private TestS3Service service = new TestS3Service();

        public override S3Service GetService()
        {
            return service;
        }

        public void DeleteFile(string file)
        {
            service.DeleteFile(file);
        }
    }

    [TestCategory("IgnoreOnBuild")]
    [TestClass()]
    public class S3RepositoryTests
    {
        readonly TestS3Repository s3Repository = new TestS3Repository();
        const string filename1 = "samplecheck-1.0.0.xml";
        const string filename2 = "sampleasynccheck-1.0.0.xml";

        [TestMethod()]
        public void AddCheckAndFixItem()
        {
            CheckAndFixItem check = new CheckAndFixItem
            {
                Name = "A Sample Check",
                Version = "1.0.1",
                Description = "A check pushed by the AddCheckAndFixItem test"
            };
            s3Repository.AddCheckAndFixItem(filename1, check);

            List<CheckAndFixItem> checkAndFixItems = s3Repository.GetCheckAndFixItems();

            Assert.IsTrue(checkAndFixItems.Contains(check));
            Assert.IsTrue(checkAndFixItems[0].Version == "1.0.1");
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
            s3Repository.AddCheckAndFixItemAsync(filename2, check).Wait();

            List<CheckAndFixItem> checkAndFixItems = s3Repository.GetCheckAndFixItems();

            Assert.IsTrue(checkAndFixItems.Contains(check));
        }

        [TestCleanup()]
        public void TestCleanup()
        {
            s3Repository.DeleteFile(filename1);
            s3Repository.DeleteFile(filename2);
        }
    }
}