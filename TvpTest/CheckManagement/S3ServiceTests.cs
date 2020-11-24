using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using TvpMain.Check;
using TvpMain.CheckManagement;
using TVPTest;

namespace TvpTest
{
    public class TestS3Service : S3Service
    {
        // Read-only PPM repository and CLI AWS configuration parameters.
        string accessKey = TestAWSCredentials.AWS_TVP_TEST_ACCESS_KEY_ID;
        string secretKey = TestAWSCredentials.AWS_TVP_TEST_ACCESS_KEY_SECRET;
        RegionEndpoint region = RegionEndpoint.GetBySystemName(TestAWSCredentials.AWS_TVP_TEST_REGION) ?? RegionEndpoint.USEast1;
        public override string BucketName { get; set; } = TestAWSCredentials.AWS_TVP_TEST_BUCKET_NAME;

        public override AmazonS3Client S3Client {get; set;}
        public TestS3Service()
        {
            S3Client = new AmazonS3Client(accessKey, secretKey, region);
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
                CheckScript = "return null;"
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
                CheckScript = "return null;"
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