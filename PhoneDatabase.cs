using Microsoft.VisualBasic.FileIO;
using System.Text;
using System.Text.RegularExpressions;

namespace CSB310_AlternativeLanguageProject
{
    /// <summary>
    /// Basic database wrapper class for holding cell phone objects
    /// I think if I had more time for this project I would put in more complex logic for add/delete since we 
    /// don't reuse indices for entries that were deleted.
    /// 
    /// </summary>
    public class PhoneDatabase  
    {
        private Dictionary<int, Cell> phoneDB;// I think most of the functions in this class are O(n) since we always go through this dictionary to gather data off the Cell objects
        private int entryCounter = 0;
        public PhoneDatabase() {

            phoneDB = new Dictionary<int, Cell>();        
        }

        /// <summary>
        /// method for using regex to partially clean and seperate data before giving it to the cell class constructor
        /// </summary>
        /// <param name="filePath"></param>
        public void BuildPhoneDBFromRegex(string filePath)

        {
            string[] lines = File.ReadAllLines(filePath);
            for (int i = 1; i < lines.Length; i++) // start after teh header line
            {
                Regex innerQuotations = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))", RegexOptions.IgnoreCase);
                string[] cellDataFields = innerQuotations.Split(lines[i]);
                AddPhoneToDB(cellDataFields);
            }
        }// O(n) because we have to go through the entire file's contents

        /// <summary>
        /// method for using the csv parser class to partially clean the data before giving it to the cell class constructor
        /// </summary>
        /// <param name="filePath"></param>
        public int BuildPhoneDBFromTextFieldParser(string fileName)
        {
            int linesRead = 0;
            //this relative file path stuff was a NIGHTMARE to figure out
            string workingDirectory = Environment.CurrentDirectory;
            string filePath = Directory.GetParent(workingDirectory).Parent.Parent.FullName + @"\\input\\" + fileName; // <- what even is this!?
            TextFieldParser parser = new TextFieldParser(filePath);
            parser.HasFieldsEnclosedInQuotes = true;
            parser.SetDelimiters(",");

            parser.ReadFields(); //discard the header line
            linesRead++;
            while (!parser.EndOfData)
            {
                string[] cellDataFields = parser.ReadFields();
                AddPhoneToDB(cellDataFields);
                linesRead++;
            }
            return linesRead;
        }
        /// <summary>
        /// method for getting a specific index out of the hashmap.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Cell GetPhoneAtIndex(int index)
        {
            return phoneDB[index];
        }


        /// <summary>
        /// simple function for adding a phone to the DB. returns the index of the added phone
        /// </summary>
        /// <param name="phoneDetails"></param>
        public int AddPhoneToDB(string[] phoneDetails)
        {
            Cell newCell = new Cell(entryCounter, phoneDetails);
            phoneDB.Add(entryCounter, newCell);

            return entryCounter++;
        }

        /// <summary>
        /// simple function for deleting a phone from the DB
        /// </summary>
        /// <param name="index"></param>
        public bool DeletePhoneFromDB(int index)
        {
            if(phoneDB.ContainsKey(index))
            {
                phoneDB.Remove(index);
                entryCounter--;
                return true;
            }
            return false;
            
        }
        public int GetNumberOfEntries()
        {
            return entryCounter;
        }

        /// <summary>
        /// Spam console with all the phone and their info to console
        /// 
        /// </summary>
        public void PrintAllPhones()
        {

            foreach (KeyValuePair<int, Cell> pair in phoneDB)
            {
                Console.WriteLine(pair.Value.ToString());
            }
        }
        public void PrintAllPhonesVerbose()
        {
            foreach (KeyValuePair<int, Cell> pair in phoneDB)
            {
                Console.WriteLine(pair.Value.ToStringVerbose());
            }
        }

        public void PrintPhonesByManufacturerVerbose(string manufacturer)
        {

            List<Cell> foundCells = new List<Cell>();

            Console.WriteLine(String.Format("Searching for phones by manufacturer: \"{0}\"...", manufacturer));
            foreach (KeyValuePair<int, Cell> pair in phoneDB)
            {
                if (pair.Value.GetAttribute(Cell.attributes.oem).Equals(manufacturer))
                {
                    foundCells.Add(pair.Value);
                }
            }

            Console.WriteLine("Total phones found: " +  foundCells.Count);
            foreach(Cell cell in foundCells)
            {
                Console.WriteLine(cell.ToStringVerbose());
            }
        }

        /// <summary>
        /// function for printing weight statistics about the phones in the database
        /// 
        /// 
        /// </summary>
        public void PrintWeightStats()
        {
            //local vars
            float lightest = int.MaxValue;
            int lightestIndex = 0;
            float heaviest = 0;
            int heaviestIndex = 0;
            float mean = 0;
            float median = 0;
            int medianIndex = 0;
            float mode = 0;
            float range = 0;

            List<Cell> validPhones = new List<Cell>();
            Dictionary<float, int> weightDistribution = new Dictionary<float, int>();

            //get the lightest and heaviest while adding entries to a list
            foreach (KeyValuePair<int, Cell> pair in phoneDB)
            {
                Cell currentCell = pair.Value;
                //skip this entry if the weight is invalid
                float currentCellWeight = (float)currentCell.GetAttribute(Cell.attributes.body_weight);
                if (currentCellWeight != 0)
                {
                    if (currentCellWeight > heaviest) { heaviest = currentCellWeight; heaviestIndex = currentCell.GetIndex(); }
                    if (currentCellWeight < lightest) { lightest = currentCellWeight; lightestIndex = currentCell.GetIndex(); }
                    mean += currentCellWeight;

                    if (weightDistribution.ContainsKey(currentCellWeight))
                    {
                        weightDistribution[currentCellWeight] += 1;
                    }
                    else
                    {
                        weightDistribution[currentCellWeight] = 1;
                    }

                    validPhones.Add(currentCell);
                }
            }

            //sort the list and get the mean and median
            validPhones.Sort((a,b) => a.CompareTo(b));
            
            mean = mean / validPhones.Count;
            medianIndex = validPhones.Count / 2;
            median = (float)validPhones.ElementAt(medianIndex).GetAttribute(Cell.attributes.body_weight);

            //go through the weight distribution dictionary and find the most common weight
            foreach(KeyValuePair<float, int> pair in weightDistribution)
            {
                if( pair.Value > mode)
                {
                    mode = pair.Key;
                }
            }

            //calculate the range
            range = heaviest - lightest;

            //print all the results
            StringBuilder sb = new StringBuilder();
            sb.Append("\n====Database entry weight stats==== \n");
            sb.Append(String.Format("Lighest: {0} {1}: {2} (g)", phoneDB[lightestIndex].GetAttribute(Cell.attributes.oem), phoneDB[lightestIndex].GetAttribute(Cell.attributes.model), phoneDB[lightestIndex].GetAttribute(Cell.attributes.body_weight)));
            sb.Append(String.Format("\nHeaviest: {0} {1}: {2} (g)", phoneDB[heaviestIndex].GetAttribute(Cell.attributes.oem), phoneDB[heaviestIndex].GetAttribute(Cell.attributes.model), phoneDB[heaviestIndex].GetAttribute(Cell.attributes.body_weight)));
            sb.Append(String.Format("\nAverage weight: {0} (g)", mean));
            sb.Append(String.Format("\nMedian: {0} {1}: {2}(g)", phoneDB[medianIndex].GetAttribute(Cell.attributes.oem), phoneDB[medianIndex].GetAttribute(Cell.attributes.model), median));
            sb.Append(String.Format("\nMost common weight: {0}(g)", mode));
            sb.Append(String.Format("\nWeight range: {0}(g)", range));

            Console.WriteLine(sb.ToString());
        } // O(n) since we go through the entire database.

        //This was a test function before I made it more generic for columns rather than just OEM (function below this one)
        public void PrintUniqueManufacturers()
        {
            Dictionary<string, int> manufacturers = new Dictionary<string, int>();
            string mostCommonManufacturer = "";
            int mostCommonFrequency = 0;
            foreach (KeyValuePair<int, Cell> pair in phoneDB)
            {
                if (!manufacturers.ContainsKey((string)pair.Value.GetAttribute(Cell.attributes.oem)))
                {
                    manufacturers.Add((string)pair.Value.GetAttribute(Cell.attributes.oem), 1);
                }
                else
                {
                    manufacturers[(string)pair.Value.GetAttribute(Cell.attributes.oem)]++;
                }
            }

            //print all the results
            StringBuilder sb = new StringBuilder();
            sb.Append("\n====Unique Manufacturers====");
            foreach(KeyValuePair<string, int> pair in manufacturers)
            {
                sb.Append(String.Format("\nManufacturer: {0} Models found: {1}", pair.Key, pair.Value));
                if(pair.Value > mostCommonFrequency)
                {
                    mostCommonManufacturer = pair.Key;
                    mostCommonFrequency = pair.Value;
                }
            }
            sb.Append(String.Format("\nMost common manufacturer: {0} with {1} models found.", mostCommonManufacturer, mostCommonFrequency));
            Console.WriteLine(sb.ToString());
        }

        public Dictionary<string, List<Cell>> GetPhonesByManufacturer()
        {
            Dictionary<string, List<Cell>> output = new Dictionary<string, List<Cell>>();

            foreach(KeyValuePair<int, Cell> pair in phoneDB)
            {
                if(!output.ContainsKey((string)pair.Value.GetAttribute(Cell.attributes.oem)))
                {
                    List<Cell> newList = new List<Cell>();
                    newList.Add(pair.Value);
                    output.Add((string)pair.Value.GetAttribute(Cell.attributes.oem), newList);
                }
                else
                {
                    List<Cell> cellsList = output[(string)pair.Value.GetAttribute(Cell.attributes.oem)];
                    cellsList.Add(pair.Value);
                }

            }

            return output;
        }

        /// <summary>
        /// Function for printing all the unique values found in any column
        /// 
        /// </summary>
        /// <param name="column"></param>
        public void PrintUniqueColumnValues(Cell.attributes column)
        {
            Dictionary<string, int> results = new Dictionary<string, int>();
            string columnHeader = Cell.GetAttributeColumnName(column);
            string mostCommonResult = "";
            int mostCommonFrequency = 0;
            foreach (KeyValuePair<int, Cell> pair in phoneDB)
            {
                if (!results.ContainsKey((string)pair.Value.GetAttribute(column)))
                {
                    results.Add((string)pair.Value.GetAttribute(column), 1);
                }
                else
                {
                    results[(string)pair.Value.GetAttribute(column)]++;
                }
            }

            //print all the results
            StringBuilder sb = new StringBuilder();
            sb.Append(String.Format("\n====Unique {0}====", columnHeader));
            foreach (KeyValuePair<string, int> pair in results)
            {
                sb.Append(String.Format("\n{0}: {1}  results found: {2}",columnHeader, pair.Key, pair.Value));
                if (pair.Value > mostCommonFrequency)
                {
                    mostCommonResult = pair.Key;
                    mostCommonFrequency = pair.Value;
                }
            }
            sb.Append(String.Format("\nMost common {0}: {1} with {2} results found.", columnHeader, mostCommonResult, mostCommonFrequency));
            Console.WriteLine(sb.ToString());
        }//O(n)

        /// <summary>
        /// function for finding cell entries with a specific feature
        /// 
        /// </summary>
        /// <param name="feature"></param>
        public void PrintModelsWithFeature(string feature)
        {
            //flatten the incomming text
            string lowerCase = feature.ToLower();

            List<Cell> phonesFound = new List<Cell>();
            
            foreach (KeyValuePair<int, Cell> pair in phoneDB)
            {
                Cell cell = pair.Value;
                string features = (string)cell.GetAttribute(Cell.attributes.features_sensors);
                features = features.ToLower(); // flatten the current cell's feature list as well so we can search more effectively

                if (features != null && features.Contains(lowerCase)) {
                
                    phonesFound.Add(cell);
                }
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("\n====Feature Search====");
            sb.Append(String.Format("\nSearched for feature: \"{0}\"", feature));
            sb.Append(String.Format("\nFound: {0} models...", phonesFound.Count));
            
            foreach(Cell cell in phonesFound)
            {
                sb.Append(String.Format("\n{0} {1}", cell.GetAttribute(Cell.attributes.oem), cell.GetAttribute(Cell.attributes.model)));
            }

            Console.WriteLine(sb.ToString());
        }// O(n)

        /// <summary>
        /// method for printing the number of releases for each year.
        /// relies on announcement year. for almost all cases the announce year matches release year.
        /// use PrintDelayedRelease() to find models that released in a different year than they were announced
        /// </summary>
        /// <param name="year"></param>
        public void PrintYearlyReleases(int year = 0)
        {
            SortedDictionary<int, int> yearlyReleases = new SortedDictionary<int, int>();
            int mostActiveYear = 0;
            int releases = 0;
            foreach (KeyValuePair<int, Cell> pair in phoneDB)
            {
                Cell cellPhone = pair.Value;
                int announceDate = (int)cellPhone.GetAttribute(Cell.attributes.launch_announced);
                string launchStatus = (string)cellPhone.GetAttribute(Cell.attributes.launch_status);
                if (announceDate > 0 && !launchStatus.Contains("Cancelled"))
                {
                    if (!yearlyReleases.ContainsKey(announceDate))
                    {
                        yearlyReleases.Add(announceDate, 1);
                    }
                    else
                    {
                        yearlyReleases[announceDate]++;
                    }
                }
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("\n====Yearly Release Stats====");
            if(year != 0)
            {
                sb.Append(String.Format("\nReleases in the year {0}: {1}", year, yearlyReleases[year]));
            }
            else
            {
                foreach (KeyValuePair <int, int> yearlyRelease in yearlyReleases) {

                    sb.Append(String.Format("\nYear: {0} Releases: {1}", yearlyRelease.Key, yearlyRelease.Value));
                    if(yearlyRelease.Value > releases) { 
                        mostActiveYear = yearlyRelease.Key;
                        releases = yearlyRelease.Value;
                    }
                }
                sb.Append(String.Format("\nMost active year: {0}", mostActiveYear));
            }
            
            Console.WriteLine(sb.ToString());
        }// O(n)

        /// <summary>
        /// function for printing yearly releases between two years. uses default values if none are provided
        /// </summary>
        /// <param name="lowerRange"></param>
        /// <param name="upperRange"></param>
        public void PrintYearlyReleasesRange(int lowerRange = 1000, int upperRange = 9999)
        {
            SortedDictionary<int, int> yearlyReleases = new SortedDictionary<int, int>();
            int mostActiveYear = 0;
            int mostReleases = 0;
            int totalReleases = 0;
            foreach (KeyValuePair<int, Cell> pair in phoneDB)
            {
                Cell cellPhone = pair.Value;
                int announceDate = (int)cellPhone.GetAttribute(Cell.attributes.launch_announced);
                string launchStatus = (string)cellPhone.GetAttribute(Cell.attributes.launch_status);
                if (announceDate > 0 &&
                    !launchStatus.Contains("Cancelled") &&
                    announceDate > lowerRange && announceDate < upperRange)
                {
                    if (!yearlyReleases.ContainsKey(announceDate))
                    {
                        yearlyReleases.Add(announceDate, 1);
                    }
                    else
                    {
                        yearlyReleases[announceDate]++;
                    }

                    totalReleases++;
                }
                
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("\n====Yearly Release Stats====");
            sb.Append(String.Format("\nReleases between the years {0} and {1}: {2}", lowerRange, upperRange, totalReleases));
            foreach (KeyValuePair<int, int> yearlyRelease in yearlyReleases)
            {

                sb.Append(String.Format("\nYear: {0} Releases: {1}", yearlyRelease.Key, yearlyRelease.Value));
                if (yearlyRelease.Value > mostReleases)
                {
                    mostActiveYear = yearlyRelease.Key;
                    mostReleases = yearlyRelease.Value;
                }
            }
            sb.Append(String.Format("\nMost active year: {0}", mostActiveYear));
            Console.WriteLine(sb.ToString());
        }//O(n)
    }
}
