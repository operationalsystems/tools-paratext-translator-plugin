using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TvpMain.Check;
using TvpMain.CheckManagement;

namespace TvpTest
{
    public class TestS3Repository : S3Repository
    {
        public override IRemoteService Service { get; set; } = new TestS3Service();
    }

    [TestCategory("IgnoreOnBuild")]
    [TestClass()]
    public class S3RepositoryTests
    {
        readonly TestS3Repository s3Repository = new TestS3Repository();
        const string filename1 = "samplecheck-1.0.0.xml";
        const string filename2 = "sampleasynccheck-1.0.0.xml";

        /// <summary>
        /// This test verifies that the <c>S3Repository</c> class can add <c>CheckAndFixItem</c>s to an S3 bucket.
        /// </summary>
        [TestMethod()]
        public void AddCheckAndFixItem()
        {
            CheckAndFixItem check = new CheckAndFixItem
            {
                Name = "A Sample Check",
                Version = "1.0.1",
                Description = "A check pushed by the AddCheckAndFixItem test",
                CheckRegex = "*.",
                CheckScript = "return null;"
            };
            s3Repository.AddCheckAndFixItem(filename1, check);

            List<CheckAndFixItem> checkAndFixItems = s3Repository.GetCheckAndFixItems();

            Assert.IsTrue(checkAndFixItems.Contains(check));
        }

        /// <summary>
        /// This test verifies that the <c>S3Repository</c> class can asynchronously add <c>CheckAndFixItem</c>s to an S3-based repository.
        /// </summary>
        [TestMethod()]
        public void AddCheckAndFixItemAsync()
        {
            CheckAndFixItem check = new CheckAndFixItem
            {
                Name = "A Sample Async Check",
                Version = "1.0.0",
                Description = "A check pushed by the AddCheckAndFixItemAsync test",
                CheckRegex = "*.",
                CheckScript = "return null;"
            };
            s3Repository.AddCheckAndFixItemAsync(filename2, check).Wait();

            List<CheckAndFixItem> checkAndFixItems = s3Repository.GetCheckAndFixItems();

            Assert.IsTrue(checkAndFixItems.Contains(check));
        }

        [TestCleanup()]
        public void TestCleanup()
        {
            s3Repository.RemoveCheckAndFixItem(filename1);
            s3Repository.RemoveCheckAndFixItem(filename2);
        }
    }
}
