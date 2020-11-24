using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.IO;
using TvpMain.Check;
using TvpMain.CheckManagement;
using TvpMain.Util;

namespace TvpTest
{
    [TestClass()]
    public class CheckManagerTests
    {
        Mock<CheckManager> checkManager = new Mock<CheckManager>();

        [TestMethod()]
        public void GetOutdatedCheckAndFixItems_returns_a_dictionary_with_any_checks_that_match()
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
        public void GetOutdatedCheckAndFixItems_returns_a_dictionary_excluding_checks_that_do_not_have_a_match()
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
        public void GetOutdatedCheckAndFixItems_handles_duplicates()
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

        [TestMethod()]
        public void It_can_manage_both_installed_and_locally_developed_checks()
        {
            CheckAndFixItem remoteCheck = new CheckAndFixItem
            {
                Name = "Remote Check",
                Version = "1.0.0.0",
                Description = "A check that has been installed from a remote source."
            };
            CheckAndFixItem locallyDevelopedCheck = new CheckAndFixItem
            {
                Name = "Local Check",
                Version = "1.0.0.0",
                Description = "A check that was developed locally."
            };

            // Expose the methods under test.
            checkManager.Setup(cm => cm.InstallCheckAndFixItem(It.IsAny<CheckAndFixItem>())).CallBase();
            checkManager.Setup(cm => cm.UninstallCheckAndFixItem(It.IsAny<CheckAndFixItem>())).CallBase();
            checkManager.Setup(cm => cm.SaveCheckAndFixItem(It.IsAny<CheckAndFixItem>())).CallBase();
            checkManager.Setup(cm => cm.DeleteCheckAndFixItem(It.IsAny<CheckAndFixItem>())).CallBase();
            checkManager.Setup(cm => cm.GetInstalledCheckAndFixItems()).CallBase();
            checkManager.Setup(cm => cm.GetSavedCheckAndFixItems()).CallBase();

            // Add the checks to the repo using their respective methods.
            checkManager.Object.InstallCheckAndFixItem(remoteCheck);
            checkManager.Object.SaveCheckAndFixItem(locallyDevelopedCheck);

            // Fetch the checks using their respective methods.
            List<CheckAndFixItem> installedChecks = checkManager.Object.GetInstalledCheckAndFixItems();
            List<CheckAndFixItem> locallyDevelopedChecks = checkManager.Object.GetSavedCheckAndFixItems();

            // Ensure that the repos are separate and that there is no cross-pollution.
            Assert.IsTrue(installedChecks.Contains(remoteCheck));
            Assert.IsFalse(installedChecks.Contains(locallyDevelopedCheck));
            Assert.IsTrue(locallyDevelopedChecks.Contains(locallyDevelopedCheck));
            Assert.IsFalse(locallyDevelopedChecks.Contains(remoteCheck));

            // Remove the checks from their respective repos.
            checkManager.Object.UninstallCheckAndFixItem(remoteCheck);
            checkManager.Object.DeleteCheckAndFixItem(locallyDevelopedCheck);
            
            // Fetch the checks using their respective methods.
            Assert.IsTrue(checkManager.Object.GetInstalledCheckAndFixItems().Count == 0);
            Assert.IsTrue(checkManager.Object.GetSavedCheckAndFixItems().Count == 0);
        }

        [TestMethod()]
        public void GetNewAndUpdatedCheckAndFixItems()
        {
            CheckAndFixItem remoteCheckAlpha = new CheckAndFixItem
            {
                Name = "Alpha",
                Version = "1.0.0.0",
                Description = "Remote Alpha"
            };
            CheckAndFixItem remoteCheckBeta = new CheckAndFixItem
            {
                Name = "Beta",
                Version = "1.0.0.0",
                Description = "Remote Beta"
            };
            CheckAndFixItem remoteCheckGamma = new CheckAndFixItem
            {
                Name = "Gamma",
                Version = "1.0.0.1",
                Description = "Remote Gamma"
            };
            CheckAndFixItem localCheckAlpha = new CheckAndFixItem
            {
                Name = "Alpha",
                Version = "1.0.0.0",
                Description = "Local Alpha"
            };
            CheckAndFixItem localCheckGamma = new CheckAndFixItem
            {
                Name = "Gamma",
                Version = "1.0.0.0",
                Description = "Local Gamma"
            };
            CheckAndFixItem localCheckDelta = new CheckAndFixItem
            {
                Name = "Delta",
                Version = "1.0.0.0",
                Description = "Local Delta"
            };

            checkManager.Setup(cm => cm.GetAvailableCheckAndFixItems()).Returns(new List<CheckAndFixItem> { remoteCheckAlpha, remoteCheckBeta, remoteCheckGamma });
            checkManager.Setup(cm => cm.GetInstalledCheckAndFixItems()).Returns(new List<CheckAndFixItem> { localCheckAlpha, localCheckGamma, localCheckDelta });
            checkManager.Setup(cm => cm.GetNewAndUpdatedCheckAndFixItems()).CallBase();

            List<CheckAndFixItem> newAndUpdated = checkManager.Object.GetNewAndUpdatedCheckAndFixItems();

            // Ensure that the list contains only the remote items which are unique or updates, and none of the local checks.
            Assert.IsFalse(newAndUpdated.Contains(remoteCheckAlpha));
            Assert.IsTrue(newAndUpdated.Contains(remoteCheckBeta));
            Assert.IsTrue(newAndUpdated.Contains(remoteCheckGamma));
            Assert.IsFalse(newAndUpdated.Contains(localCheckAlpha));
            Assert.IsFalse(newAndUpdated.Contains(localCheckGamma));
            Assert.IsFalse(newAndUpdated.Contains(localCheckDelta));
        }

        [TestMethod()]
        public void GetDeprecatedCheckAndFixItems()
        {
            CheckAndFixItem remoteCheckAlpha = new CheckAndFixItem
            {
                Name = "Alpha",
                Version = "1.0.0.0",
                Description = "Remote Alpha"
            };
            CheckAndFixItem remoteCheckBeta = new CheckAndFixItem
            {
                Name = "Beta",
                Version = "1.0.0.0",
                Description = "Remote Beta"
            };
            CheckAndFixItem remoteCheckGamma = new CheckAndFixItem
            {
                Name = "Gamma",
                Version = "1.0.0.1",
                Description = "Remote Gamma"
            };
            CheckAndFixItem localCheckAlpha = new CheckAndFixItem
            {
                Name = "Alpha",
                Version = "1.0.0.0",
                Description = "Local Alpha"
            };
            CheckAndFixItem localCheckGamma = new CheckAndFixItem
            {
                Name = "Gamma",
                Version = "1.0.0.0",
                Description = "Local Gamma"
            };
            CheckAndFixItem localCheckDelta = new CheckAndFixItem
            {
                Name = "Delta",
                Version = "1.0.0.0",
                Description = "Local Delta"
            };

            checkManager.Setup(cm => cm.GetAvailableCheckAndFixItems()).Returns(new List<CheckAndFixItem> { remoteCheckAlpha, remoteCheckBeta, remoteCheckGamma });
            checkManager.Setup(cm => cm.GetInstalledCheckAndFixItems()).Returns(new List<CheckAndFixItem> { localCheckAlpha, localCheckGamma, localCheckDelta });
            checkManager.Setup(cm => cm.GetDeprecatedCheckAndFixItems()).CallBase();

            List<CheckAndFixItem> deprecated = checkManager.Object.GetDeprecatedCheckAndFixItems();

            // Ensure that the list contains only installed items which do not exist remotely.
            Assert.IsFalse(deprecated.Contains(remoteCheckAlpha));
            Assert.IsFalse(deprecated.Contains(remoteCheckBeta));
            Assert.IsFalse(deprecated.Contains(remoteCheckGamma));
            Assert.IsFalse(deprecated.Contains(localCheckAlpha));
            Assert.IsFalse(deprecated.Contains(localCheckGamma));
            Assert.IsTrue(deprecated.Contains(localCheckDelta));
        }

        [TestCleanup()]
        public void TestCleanup()
        {
            string installedCheckRoot = Path.Combine(Directory.GetCurrentDirectory(), MainConsts.INSTALLED_CHECK_FOLDER_NAME.Split('\\')[0]);
            if (Directory.Exists(installedCheckRoot)) Directory.Delete(installedCheckRoot, true);

            string localCheckRoot = Path.Combine(Directory.GetCurrentDirectory(), MainConsts.INSTALLED_CHECK_FOLDER_NAME.Split('\\')[0]);
            if (Directory.Exists(localCheckRoot)) Directory.Delete(localCheckRoot, true);
        }
    }
}