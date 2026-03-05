using NUnit.Framework;
using Edia.Eye;

namespace Edia.Eye.Tests
{
    [TestFixture]
    public class EyeDataPackageTests
    {
        [Test]
        public void EyeDataPackage_Initialization_HasDefaultValues()
        {
            var package = new EyeDataPackage();
            Assert.IsNotNull(package);
            Assert.AreEqual("", package.eye);
            Assert.IsFalse(package.isValid);
            Assert.AreEqual(0f, package.direction_x_local);
            Assert.AreEqual("NA", package.target_id);
        }

        [Test]
        public void EyeDataPackage_SettingValues_RetainsValues()
        {
            var package = new EyeDataPackage();
            package.eye = "left";
            package.isValid = true;
            package.direction_x_local = 1.0f;
            package.target_id = "target_01";

            Assert.AreEqual("left", package.eye);
            Assert.IsTrue(package.isValid);
            Assert.AreEqual(1.0f, package.direction_x_local);
            Assert.AreEqual("target_01", package.target_id);
        }
    }
}
