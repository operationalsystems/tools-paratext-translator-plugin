using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using TvpMain.Check;
using TvpMain.CheckManager;

namespace TvpTest
{
    [TestClass()]
    public class CheckManagerTests
    {
        Mock<CheckManager> checkManager = new Mock<CheckManager>();

        [TestMethod()]
        public void GetOutdatedCheckAndFixItems_Returns_Dictionary_with_Matching_Checks()
        {
            // Create two basic checks--one of which is an update of the other.
            string testName = "check";
            CheckAndFixItem installedItem = new CheckAndFixItem
            {
                Name = testName,
                Version = "1.0.0.0"
            };
            CheckAndFixItem availableItem = new CheckAndFixItem
            {
                Name = testName,
                Version = "1.0.0.1"
            };

            // Return the basic checks and the logic to compare them.
            checkManager.Setup(cm => cm.GetInstalledCheckAndFixItems()).Returns(() => new List<CheckAndFixItem>
            {
                installedItem
            });
            checkManager.Setup(cm => cm.GetAvailableCheckAndFixItems()).Returns(() => new List<CheckAndFixItem>
            {
               availableItem
            });

            // Verify that the check and its match are returned.
            checkManager.Setup(cm => cm.IsNewVersion(It.IsAny<CheckAndFixItem>(), It.IsAny<CheckAndFixItem>())).Returns(true);
            checkManager.Setup(cm => cm.GetOutdatedCheckAndFixItems()).CallBase();
            var outdatedItems = checkManager.Object.GetOutdatedCheckAndFixItems();

            Assert.IsTrue(outdatedItems.ContainsKey(installedItem));
            Assert.IsTrue(outdatedItems[installedItem].Equals(availableItem));
        }

        [TestMethod()]
        public void GetOutdatedCheckAndFixItems_Returns_Dictionary_Excluding_NonMatching_Checks()
        {
            // Create one basic check which will not have a match.
            string checkName1 = "check1";
            CheckAndFixItem installedCheckA = new CheckAndFixItem
            {
                Name = checkName1,
                Version = "1.0.0.0"
            };
            // Create two basic checks--one of which is an update of the other.
            string checkName2 = "check2";
            CheckAndFixItem installedCheckB = new CheckAndFixItem
            {
                Name = checkName2,
                Version = "1.0.0.0"
            };
            CheckAndFixItem availableCheckB = new CheckAndFixItem
            {
                Name = checkName2,
                Version = "1.0.0.1"
            };

            // Return the basic checks and the logic to compare them.
            checkManager.Setup(cm => cm.GetInstalledCheckAndFixItems()).Returns(() => new List<CheckAndFixItem>
            {
                installedCheckA,
                installedCheckB
            });
            checkManager.Setup(cm => cm.GetAvailableCheckAndFixItems()).Returns(() => new List<CheckAndFixItem>
            {
               availableCheckB
            });

            // Verify that the check with no match is not included in the dictionary.
            checkManager.Setup(cm => cm.IsNewVersion(installedCheckA, availableCheckB)).Returns(false);
            checkManager.Setup(cm => cm.IsNewVersion(installedCheckB, availableCheckB)).Returns(true);
            checkManager.Setup(cm => cm.GetOutdatedCheckAndFixItems()).CallBase();
            var outdatedItems = checkManager.Object.GetOutdatedCheckAndFixItems();

            Assert.IsFalse(outdatedItems.ContainsKey(installedCheckA));
            Assert.IsTrue(outdatedItems.ContainsKey(installedCheckB));
            Assert.IsTrue(outdatedItems[installedCheckB].Equals(availableCheckB));
        }

        [TestMethod()]
        public void GetOutdatedCheckAndFixItems_Handles_Duplicates()
        {
            // Create three basic checks--one of which is the latest update of another
            string testName = "check";
            CheckAndFixItem installedCheckA = new CheckAndFixItem
            {
                Name = testName,
                Version = "1.0.0.0"
            };
            CheckAndFixItem availableCheckA_Old = new CheckAndFixItem
            {
                Name = testName,
                Version = "1.0.0.1"
            };
            CheckAndFixItem availableCheckA_New = new CheckAndFixItem
            {
                Name = testName,
                Version = "1.0.0.2"
            };

            // Return the basic checks and the logic to compare them.
            checkManager.Setup(cm => cm.GetInstalledCheckAndFixItems()).Returns(() => new List<CheckAndFixItem>
            {
                installedCheckA
            });
            checkManager.Setup(cm => cm.GetAvailableCheckAndFixItems()).Returns(() => new List<CheckAndFixItem>
            {
               availableCheckA_Old,
               availableCheckA_New
            });

            // Verify that only the latest version is considered.
            checkManager.Setup(cm => cm.IsNewVersion(It.IsAny<CheckAndFixItem>(), It.IsAny<CheckAndFixItem>())).Returns(true);
            checkManager.Setup(cm => cm.GetOutdatedCheckAndFixItems()).CallBase();
            var outdatedItems = checkManager.Object.GetOutdatedCheckAndFixItems();

            Assert.IsTrue(outdatedItems.ContainsKey(installedCheckA));
            Assert.IsTrue(outdatedItems[installedCheckA].Equals(availableCheckA_New));
        }

        [TestMethod()]
        public void IsNewVersionTest()
        {
            checkManager.Setup(cm => cm.IsNewVersion(It.IsAny<CheckAndFixItem>(), It.IsAny<CheckAndFixItem>())).CallBase();

            // Set up our base check that others will be compared to.
            CheckAndFixItem check = new CheckAndFixItem
            {
                Name = "test",
                Version = "1.0.0.1"
            };

            // Set up the checks to compare against the base check.
            CheckAndFixItem sameVersion = new CheckAndFixItem
            {
                Name = check.Name,
                Version = check.Version
            };
            CheckAndFixItem differentCheck = new CheckAndFixItem
            {
                Name = "text",
                Version = "1.0.0.1"
            };
            CheckAndFixItem oldVersion = new CheckAndFixItem
            {
                Name = check.Name,
                Version = "1.0.0.0"
            };
            CheckAndFixItem newVersion = new CheckAndFixItem
            {
                Name = check.Name,
                Version = "1.0.0.2"
            };

            // Test that only the new version of the same check will work.
            Assert.IsFalse(checkManager.Object.IsNewVersion(check, sameVersion));
            Assert.IsFalse(checkManager.Object.IsNewVersion(check, differentCheck));
            Assert.IsFalse(checkManager.Object.IsNewVersion(check, oldVersion));
            Assert.IsTrue(checkManager.Object.IsNewVersion(check, newVersion));
        }
    }
}