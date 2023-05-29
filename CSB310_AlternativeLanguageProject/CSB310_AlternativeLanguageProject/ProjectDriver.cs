namespace CSB310_AlternativeLanguageProject
{
    public class ProjectDriver
    {

        static string[] pixelFold = {"Google", 
            "Pixel Fold" ,
            "2023, May 10", 
            "Coming soon. Exp. release 2023, June",
            "139.7 x 79.5 x 12.1 mm",
            "283 g (9.98 oz)",
            "Nano-SIM and eSIM",
            "OLED capacitive touchscreen, 16M colors",
            "5.8 inches",
            "1840 x 2208 pixels, 17.4:9 ratio, 408 ppi",
            "Fingerprint (side-mounted), folding screen, accelerometer, gyro, proximity, compass, barometer, Ultra Wideband (UWB) support",
            "Android 13"}; 

        public static void Main(string[] args)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "cells.csv");
            PhoneDatabase phoneDB = new PhoneDatabase();
            phoneDB.BuildPhoneDBFromTextFieldParser(filePath);
            //phoneDB.BuildPhoneDBFromRegex(path);
            
            //phoneDB.PrintAllPhones();
            
            phoneDB.PrintPhonesByManufacturerVerbose("Lava");
            phoneDB.PrintWeightStats();
            phoneDB.PrintUniqueManufacturers();
            phoneDB.PrintModelsWithFeature("Barometer");

            phoneDB.AddPhoneToDB(pixelFold);
            phoneDB.PrintModelsWithFeature("folding screen");
            phoneDB.PrintPhonesByManufacturerVerbose("Google");
            int foldIndex = 1000;
            phoneDB.DeletePhoneFromDB(foldIndex);

            phoneDB.PrintYearlyReleases();
            phoneDB.PrintYearlyReleases(2004);
            phoneDB.PrintYearlyReleasesRange(2010, 2020);

            //report required queries
            //highest weight by oem
            phoneDB.PrintDelayedRelease();
            //phones with only 1 feature
            //year with the most phone launches
            
        }
    }
}
