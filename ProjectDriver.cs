using System.Text.RegularExpressions;
using System.Text;

namespace CSB310_AlternativeLanguageProject
{
    /// <summary>
    /// Class for demonstrating the cellphone database features
    /// </summary>
    public class ProjectDriver
    {
        static string fileName = "cells.csv";
        /// <summary>
        /// demonstrations of my work and test functions for the extra features and required testing.
        /// I think all of the functions in this driver class are O(n) because we don't do anything special when sweeping through the phone DB
        /// any extra looping or logic comes after gathering what we want from that sweep. 
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            
            PhoneDatabase phoneDB = new PhoneDatabase();
            int linesRead = phoneDB.BuildPhoneDBFromTextFieldParser(fileName);
            //phoneDB.BuildPhoneDBFromRegex(path);

            //phoneDB.PrintAllPhones(); this makes too much data for a demo

            //basic query
            phoneDB.PrintPhonesByManufacturerVerbose("Lava");

            //stats generation for phone weights
            phoneDB.PrintWeightStats();

            //more advanced searches
            phoneDB.PrintUniqueColumnValues(Cell.attributes.oem);
            phoneDB.PrintUniqueColumnValues(Cell.attributes.display_type);
            phoneDB.PrintModelsWithFeature("Barometer");
            phoneDB.PrintYearlyReleases(2004);
            phoneDB.PrintYearlyReleasesRange(2010, 2020);

            //add and remove feature w/ verification

            //demo data
            string[] pixelFold = {"Google",
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

            phoneDB.AddPhoneToDB(pixelFold);
            phoneDB.PrintModelsWithFeature("folding screen");
            phoneDB.PrintPhonesByManufacturerVerbose("Google");
            int foldIndex = 1000;
            phoneDB.DeletePhoneFromDB(foldIndex);
            phoneDB.PrintPhonesByManufacturerVerbose("Google");

            //required unit testing
            RunRequiredTests();

            //report required queries
            Console.WriteLine("\n====Report Required Tests====");
            PrintHighestAverageWeight(phoneDB);
            PrintDelayedRelease(phoneDB);
            PrintPhonesWithOneFeature(phoneDB);
            phoneDB.PrintYearlyReleases();

        }
        /// <summary>
        /// Group of general tests used to mimic unit testing.
        /// I attempted to get a unit test project running but I could not fix all the .Net framework version errors 
        /// </summary>
        public static void RunRequiredTests()
        {

            Console.WriteLine("\n\n====Required Tests====");
            
            PhoneDatabase phoneDB = new PhoneDatabase();
            int linesRead = phoneDB.BuildPhoneDBFromTextFieldParser(fileName);

            //check for empty file
            Console.WriteLine(String.Format("Lines read from file:{0}", linesRead));
            //check for empty database
            Console.WriteLine(String.Format("Database has more than 0 entries:{0}", phoneDB.GetNumberOfEntries() > 0));

            //test the column types to see if data was stored properly
            Cell testPhone = phoneDB.GetPhoneAtIndex(5);
            Object[] attributes = testPhone.GetAttributes();
            Console.WriteLine(String.Format("OEM type: {0}: Matches requirement: {1}", attributes[0].GetType(), attributes[0].GetType() == typeof(string)));
            Console.WriteLine(String.Format("Model type: {0}: Matches requirement:{1}", attributes[1].GetType(), attributes[1].GetType() == typeof(string)));
            Console.WriteLine(String.Format("Launch Announce type: {0}: Matches requirement:{1}", attributes[2].GetType(), attributes[2].GetType() == typeof(int)));
            Console.WriteLine(String.Format("Launch status type: {0}: Matches requirement:{1}", attributes[3].GetType(), attributes[3].GetType() == typeof(string)));
            Console.WriteLine(String.Format("Body Dimensions : {0}: Matches requirement:{1}", attributes[4].GetType(), attributes[4].GetType() == typeof(string)));
            Console.WriteLine(String.Format("Body Weight : {0}: Matches requirement:{1}", attributes[5].GetType(), attributes[5].GetType() == typeof(float)));
            Console.WriteLine(String.Format("Body SIM type: {0}: Matches requirement:{1}", attributes[6].GetType(), attributes[6].GetType() == typeof(string)));
            Console.WriteLine(String.Format("Display type: {0}: Matches requirement:{1}", attributes[7].GetType(), attributes[7].GetType() == typeof(string)));
            Console.WriteLine(String.Format("Display size type: {0}: Matches requirement:{1}", attributes[8].GetType(), attributes[8].GetType() == typeof(float)));
            Console.WriteLine(String.Format("Display resolution type: {0}: Matches requirement:{1}", attributes[9].GetType(), attributes[9].GetType() == typeof(string)));
            Console.WriteLine(String.Format("Feature sensors type: {0}: Matches requirement:{1}", attributes[10].GetType(), attributes[10].GetType() == typeof(string)));
            Console.WriteLine(String.Format("Operating System type: {0}: Matches requirement:{1}", attributes[11].GetType(), attributes[11].GetType() == typeof(string)));


            //check to see if an invalid string "-" was not properly cleaned
            string forbiddenCharacter = "-";
            bool foundTheForbiddenCharacter = false;
            int entries = phoneDB.GetNumberOfEntries();
            for (int i = 0; i < entries; i++)
            {
                Cell cell = phoneDB.GetPhoneAtIndex(i);
                Object[] cellAttributes = cell.GetAttributes();

                for (int j = 0; i < cellAttributes.Length; i++)
                {
                    Object attribute = cellAttributes[i];
                    if (attribute == typeof(string))
                    {
                        foundTheForbiddenCharacter = attribute.Equals(forbiddenCharacter);
                    }
                }
            }
            Console.WriteLine(String.Format("\nThe forbidden character was found: {0}", foundTheForbiddenCharacter));


            //basic unit test style tests
            //test cell data is transformed properly
            string[] applePhone = {"Apple",
            "iPhone 40" ,
            "2040, May 10",
            "Coming soon. Exp. release 2041, June",
            "-",
            "283 g (9.98 oz)",
            "-",
            "-",
            "4 inches",
            "1840 x 2208 pixels, 17.4:9 ratio, 1000 ppi",
            "Fingerprint (side-mounted), bug zapper, stereo headphone jack, credit card swiper",
            "Android 30"};

            //expected output
            Object[] expectedAttributes = { "Apple", "iPhone 40", 2040, "2041", null, 283f, null, null, 4f, "1840 x 2208 pixels, 17.4:9 ratio, 1000 ppi", "Fingerprint (side-mounted), bug zapper, stereo headphone jack, credit card swiper", "Android 30" };

            int applephoneindex = phoneDB.AddPhoneToDB(applePhone);
            Cell appleentry = phoneDB.GetPhoneAtIndex(applephoneindex);
            Object[] appleAttributes = appleentry.GetAttributes();

            bool allAttributesMatch = true;
            for (int i = 0; i < appleAttributes.Length; i++)
            {
                if (appleAttributes[i] == null)
                {
                    allAttributesMatch = expectedAttributes[i] == null;
                }

                else if (!appleAttributes[i].Equals(expectedAttributes[i]))
                {
                    allAttributesMatch = false;
                }
            }
            Console.WriteLine(String.Format("\nAll outputs match expected: {0}", allAttributesMatch));

            //test removing some entries from the DB
            int[] indicestoRemove = { 100, 200, 300, 400, 500 };
            int expectedDBSize = 996;

            for (int i = 0; i < indicestoRemove.Length; i++)
            {
                phoneDB.DeletePhoneFromDB(indicestoRemove[i]);
            }

            bool dbSizeMatches = phoneDB.GetNumberOfEntries() == expectedDBSize;
            Console.WriteLine(String.Format("\nSuccessfully removed all indices: {0}", dbSizeMatches));

            //test if the overidden comparison works
            Cell google4 = phoneDB.GetPhoneAtIndex(5);
            Cell google4XL = phoneDB.GetPhoneAtIndex(4);
            int expectedComparison = 1;

            int actualComparison = google4XL.CompareTo(google4);
            Console.WriteLine(String.Format("\nGoogle pixel4XL is heavier than pixel 4: {0}", expectedComparison == actualComparison));
        }
        public static void PrintHighestAverageWeight(PhoneDatabase phoneDB)
        {
            Dictionary<string, List<Cell>> cellsByOEM = phoneDB.GetPhonesByManufacturer();

            float heaviestAverage = 0;
            string heaviestAverageOEM = "";

            foreach (KeyValuePair<string, List<Cell>> pair in cellsByOEM)
            {
                string oem = pair.Key;
                float averageWeight = 0;

                List<Cell> cells = pair.Value;

                for (int i = 0; i < cells.Count; i++)
                {
                    averageWeight += (float)cells[i].GetAttribute(Cell.attributes.body_weight);
                }

                averageWeight /= cells.Count;
                if (averageWeight > heaviestAverage)
                {
                    heaviestAverage = averageWeight;
                    heaviestAverageOEM = oem;
                }
            }
            Console.WriteLine("\n====Heaviest OEM test====");
            Console.WriteLine(String.Format("OEM with heaviest average phone: {0} at {1}(g)", heaviestAverageOEM, heaviestAverage));

        }
        /// <summary>
        /// function for printing the models that did not release in the year they were announced.
        /// This does not include discontinued or canceled results
        /// </summary>
        public static void PrintDelayedRelease(PhoneDatabase phoneDB)
        {
            List<Cell> delayedReleases = new List<Cell>();

            int entries = phoneDB.GetNumberOfEntries();

            for (int i = 0; i < entries; i++)
            {
                Cell cell = phoneDB.GetPhoneAtIndex(i);
                int announceYear = (int)cell.GetAttribute(Cell.attributes.launch_announced);
                string launchStatus = (string)cell.GetAttribute(Cell.attributes.launch_status);
                int launchYear;
                Regex regex = new Regex("\\d{4}", RegexOptions.None);
                Match match = regex.Match(launchStatus);
                Int32.TryParse(match.Value, out launchYear);

                if (launchYear > 0 && announceYear != launchYear)
                {
                    delayedReleases.Add(cell);
                }
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("\n====Delayed Releases====");
            sb.Append(String.Format("\nFound {0} delayed releases in the database", delayedReleases.Count));
            foreach (Cell cell in delayedReleases)
            {
                sb.Append(String.Format("\n{0} {1}", cell.GetAttribute(Cell.attributes.oem), cell.GetAttribute(Cell.attributes.model)));
            }

            Console.WriteLine(sb.ToString());
        }
        public static void PrintPhonesWithOneFeature(PhoneDatabase phoneDB)
        {
            List<Cell> oneFeatureCells = new List<Cell>();

            int entries = phoneDB.GetNumberOfEntries();

            for (int i = 0; i < entries; i++)
            {
                Cell cell = phoneDB.GetPhoneAtIndex(i);

                string featuresAttribute = (string)cell.GetAttribute(Cell.attributes.features_sensors);
                string[] featuresArray = featuresAttribute.Split(',');

                if (featuresArray.Length == 1)
                {
                    oneFeatureCells.Add(cell);
                }
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("\n====Phones with One feature====");
            /*
            foreach (Cell cell in oneFeatureCells)
            {
                sb.Append(String.Format("\n{0} {1}", cell.GetAttribute(Cell.attributes.oem), cell.GetAttribute(Cell.attributes.model)));
            }
            */
            sb.Append(String.Format("\nTotal number of cells with 1 feature: {0}", oneFeatureCells.Count));
            Console.WriteLine(sb.ToString());
        }
    }
}
