using Microsoft.VisualStudio.TestTools.UnitTesting;
using CSB310_AlternativeLanguageProject;

namespace AlternativeLanguageProjectTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestCellObjectConstructedProperly()
        {
            string[] pixelFold = {"Apple",
            "Iphone 50" ,
            "2050, May 10",
            "Coming soon. Exp. release 2051, June",
            "139.7 x 79.5 x 12.1 mm",
            "283 g (9.98 oz)",
            "Nano-SIM and eSIM",
            "OLED capacitive touchscreen, 16M colors",
            "5.8 inches",
            "1840 x 2208 pixels, 17.4:9 ratio, 408 ppi",
            "Fingerprint (side-mounted), folding screen, accelerometer, gyro, proximity, compass, barometer, Ultra Wideband (UWB) support",
            "Android 13"};

            string expectedOEM = "Apple";
            string expectedModel = "Iphone 50";
            int expectedLaunchAnnounced = 2050;
            string expectedLaunchDate = "2051";
            string expectedDimensions = "139.7 x 79.5 x 12.1 mm";
            float expectedWeight = 283f;
            string expectedSIM = "Nano-SIM and eSIM";
            string expectedScreenType = "OLED capacitive touchscreen, 16M colors";
            float expectedDisplaySize = 5.8f;
            string expectedResolution = "1840 x 2208 pixels, 17.4:9 ratio, 408 ppi";
            string expectedFeatures = "Fingerprint (side-mounted), folding screen, accelerometer, gyro, proximity, compass, barometer, Ultra Wideband (UWB) support";
            string expectedOS = "Android 13";

            T
        }
    }
}
