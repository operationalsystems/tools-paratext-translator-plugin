using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using TvpMain.Check;
using TvpMain.CheckManager;
using TVPTest;

namespace TvpTest
{
    public class TestS3Service : S3Service
    {
        // Read-only PPM repository and CLI AWS configuration parameters.
        const string accessKey = TestAWSCredentials.AWS_TVP_TEST_ACCESS_KEY_ID;
        const string secretKey = TestAWSCredentials.AWS_TVP_TEST_ACCESS_KEY_SECRET;
        private string BucketName = TestAWSCredentials.AWS_TVP_TEST_BUCKET_NAME;

        public override string GetBucketName()
        {
            return BucketName;
        }

        private readonly AmazonS3Client s3Client = new AmazonS3Client(accessKey, secretKey, RegionEndpoint.USEast1);

        public override AmazonS3Client GetS3Client()
        {
            return s3Client;
        }

        public void DeleteFile(string file)
        {
            DeleteObjectRequest deleteObjectRequest = new DeleteObjectRequest
            {
                BucketName = GetBucketName(),
                Key = file
            };
            s3Client.DeleteObject(deleteObjectRequest);
        }
    }
    [TestCategory("IgnoreOnBuild")]
    [TestClass()]
    public class S3ServiceTests
    {
        const string filename = "test.xml";

        TestS3Service Service { get; set; } = new TestS3Service();

        [TestInitialize()]
        public void TestSetup()
        {
            CheckAndFixItem checkAndFix = new CheckAndFixItem
            {
                Name = "test",
                Version = "1.2.3.4",
                CheckRegex = "*.",
                FixRegex = "*.",
                CheckScript = "return null;",
                FixScript = "return null;"
            };
            Service.PutFileStream(filename, checkAndFix.WriteToXmlStream());
        }

        [TestMethod()]
        public void PutFileStream()
        {
            CheckAndFixItem checkAndFix = new CheckAndFixItem
            {
                Name = "test",
                Version = "1.2.3.5",
                CheckRegex = "*.",
                FixRegex = "*.",
                CheckScript = "return null;",
                FixScript = "return null;"
            };
            Service.PutFileStream(filename, checkAndFix.WriteToXmlStream());

            var files = Service.ListAllFiles();
            Assert.IsTrue(files.Contains(filename));
        }

        [TestMethod()]
        public void ListFilesTest()
        {
            var files = Service.ListAllFiles();
            Assert.IsTrue(files.Contains(filename));
        }

        [TestMethod()]
        public void GetFileTest()
        {
            using Stream file = Service.GetFileStream(filename);
            Assert.IsTrue(CheckAndFixItem.LoadFromXmlContent(file) is CheckAndFixItem);
        }

        [TestCleanup()]
        public void TestCleanup()
        {
            Service.DeleteFile(filename);
        }
    }
}