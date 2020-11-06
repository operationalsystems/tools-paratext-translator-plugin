using Microsoft.VisualStudio.TestTools.UnitTesting;
using TvpMain.CheckManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TvpMain.Check;

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

            Assert.IsTrue(checkAndFixItems.Count == 1);
        }
    }
}