using Microsoft.VisualStudio.TestTools.UnitTesting;
using TvpMain.CheckManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TvpTest
{
    [TestClass()]
    public class S3ServiceTests
    {
        S3Service S3Service { get; set; } = new S3Service();

        [TestMethod()]
        public void ListFilesTest()
        {
            var files = S3Service.ListAllFiles();
            Assert.IsTrue(files.Contains("test.txt"));
        }

        [TestMethod()]
        public void GetFileTest()
        {
            var file = S3Service.GetFileStreamAsync("test.txt");

            Assert.IsTrue(file.Result.Length > 0);
        }
    }
}