using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using TvpMain.Check;
using TvpMain.CheckManager;

namespace TvpTest
{
    public class TestS3Service : S3Service
    {
        // Read-only PPM repository and CLI AWS configuration parameters.
        const string accessKey = "ACCESS_KEY_PLACEHOLDER";
        const string secretKey = "KEY_SECRET_PLACEHOLDER";
        const string bucketName = "biblica-tvp-check-repo";
        private readonly AmazonS3Client s3Client = new AmazonS3Client(accessKey, secretKey, RegionEndpoint.USEast1);

        public override AmazonS3Client GetS3Client()
        {
            return s3Client;
        }

        public void DeleteFile(string file)
        {
            DeleteObjectRequest deleteObjectRequest = new DeleteObjectRequest
            {
                BucketName = bucketName,
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
                Version = "1.2.3.4"
            };
            Service.PutFileStream(filename, checkAndFix.WriteToXmlStream());
        }

        [TestMethod()]
        public void PutFileStream()
        {
            CheckAndFixItem checkAndFix = new CheckAndFixItem
            {
                Name = "test",
                Version = "1.2.3.5"
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
            Assert.IsTrue(file.Length > 0);
        }

        [TestCleanup()]
        public void TestCleanup()
        {
            Service.DeleteFile(filename);
        }
    }
}