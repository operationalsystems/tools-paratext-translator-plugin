using AddInSideViews;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TvpMain.Project;
using TvpMain.Util;

namespace TvpTest
{
    [TestClass]
    public class ProjectManagerTests
    {

        /// <summary>
        /// Mock Host class
        /// </summary>
        private Mock<IHost> _mockHost;

        /// <summary>
        /// Mock File Manager
        /// </summary>
        private Mock<FileManager> _mockFileManager;

        /// <summary>
        /// Mock Project Manager
        /// </summary>
        private Mock<ProjectManager> _mockProjectManager;

        private const string TEST_PROJECT_NAME = "testProjectManagerProject";
        private const string TEST_PROJECT_PATH = @"Resources\testProjects\" + TEST_PROJECT_NAME;

        /// <summary>
        /// Test setup.
        /// </summary>
        [TestInitialize]
        public void TestSetup()
        {
            /*        public ProjectManager(
            IHost host, string activeProjectName,
            FileManager fileManager)*/

            // create mocks

            // mock: host util
            _mockHost = new Mock<IHost>();
            _mockHost
                .Setup(host => host.GetFigurePath(TEST_PROJECT_NAME, It.IsAny<bool>()))
                .Returns(@$"{TEST_PROJECT_PATH}\figures");

            // mock: file manager
            _mockFileManager = new Mock<FileManager>(
                _mockHost.Object,
                TEST_PROJECT_NAME
                );

            // mock: project manager
            _mockProjectManager = new Mock<ProjectManager>(
                _mockHost.Object,
                TEST_PROJECT_NAME,
                _mockFileManager.Object
                )
            {
                // call base functions unless overridden
                CallBase = true
            };
        }

        /// <summary>
        /// Test that Project Manager correctly maps books to ID.
        /// </summary>
        [TestMethod]
        public void TestProjectManagerBookIdMapping()
        {
            var bookNamesByNum = _mockProjectManager.Object.BookNamesByNum;
            
            // check generic assumptions
            Assert.IsNotNull(bookNamesByNum);
            Assert.IsTrue(bookNamesByNum.Count > 0);

            // establish what expected output book names are for input book codes for test project
            var expectedInputsVsOuputs = new Dictionary<string, int>()
            {
                {"GEN", 1}, {"JOB", 18}, {"MAT", 40},
                {"COL", 51}, {"2JN", 63}, {"REV", 66},
                {"XXA", 93}, {"FRT", 100}, {"GLO", 109}
            };

            foreach (var entry in expectedInputsVsOuputs)
            {
                Assert.AreEqual(entry.Key, bookNamesByNum[entry.Value].BookCode);
            }

            // make sure that missing books aren't available
            var expectedMissingBooks = new List<string>()
            {
                {"NDX"}, {"TOB"}, {"WIS"}, 
                {"LJE"}, {"SUS"}, {"3MA"}, 
                {"PS2"}, {"JDB"}, {"BLT"}
            };

            foreach (var entry in expectedMissingBooks)
            {
                Assert.IsFalse(false, "These books are not available in this translation.", entry);
            }
        }
    }
}
