using System.Text;
using System.Text.RegularExpressions;

namespace CSB310_AlternativeLanguageProject
{
    /// <summary>
    /// Class for ingesting, cleaning and storing cell phone data from csv file.
    /// </summary>
    public class Cell : IComparable
    {

        // I am keeping these as fields. I could use "automatic properties" but can't pass them as references easily (see the builder method)
        private int db_index = 0;
        Object[] attributesArray;

        private static string[] columnHeaders = 
        {"OEM",
        "Model",
        "Launch Announcment Data",
        "Launch Date",
        "Body Dimensions",
        "Body Weight",
        "SIM Type",
        "Display Type",
        "Display Size",
        "Display Resolution",
        "Features",
        "Operating System"};

        public enum attributes{// using enum for accessing the source string parts. also removes magic numbers
            oem,
            model, 
            launch_announced, 
            launch_status,
            body_dimensions,
            body_weight,
            body_sim,
            display_type,
            display_size,
            display_resolution,
            features_sensors,
            os,
        }
        
        public Cell(int db_index, string[] sourceFields)
        {
            attributesArray = new Object[sourceFields.Length];
            this.db_index = db_index;

            //CleanCellDataFromSource(sourceFields);
            CleanCellDataFromSourceIntoObjectArray(sourceFields);
        }
        /// <summary>
        /// method for getting a specific attribute
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public Object GetAttribute(Cell.attributes attribute)
        {
            return attributesArray[(int)attribute];
        }
        /// <summary>
        /// method for getting the entire array of attributes
        /// </summary>
        /// <returns></returns>
        public Object[] GetAttributes()
        {
            return attributesArray;
        }
        /// <summary>
        /// method for getting the name of a column. mostly for String formatting and printing
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static String GetAttributeColumnName(Cell.attributes attribute)
        {
            return columnHeaders[(int)attribute];
        }
        /// <summary>
        /// Get the index of this cell in the database
        /// </summary>
        /// <returns></returns>
        public int GetIndex()
        {
            return db_index;
        }
        /// <summary>
        /// to string information about hte cell including the column name
        /// 
        /// </summary>
        /// <returns></returns>
        public String ToStringVerbose()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(String.Format("\nDatabase Index: {0}", db_index));
            for (int i  = 0; i < attributesArray.Length; i++)
            {
                sb.Append(String.Format("\n{0}: {1}", columnHeaders[i], attributesArray[i]));
            }

            return sb.ToString();
        }

        /// <summary>
        /// method for converting the cell data into a more easily readable string for human
        /// 
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(db_index);

            for(int i = 0; i < attributesArray.Length; i++)
            {
                sb.Append(" " + attributesArray[i]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Helper function for taking the source strings and further cleaning them for the source. 
        /// Uses a bunch of crazy regex, string replacement and casting
        /// </summary>
        /// <param name="source"></param>
        private void CleanCellDataFromSourceIntoObjectArray(string[] source)
        {
            //we will be reusing regex and match objects frequently
            Regex regex;
            Match match;

            // enum is type int by default but still considered 'distinct types'.
            // we have to cast to int in order to use them in a meaningful way. bummer
            if (source[(int)attributes.oem] != "")
            {
                attributesArray[(int)attributes.oem] = source[(int)attributes.oem];
            }

            if (source[(int)attributes.model] != "")
            {
                attributesArray[(int)attributes.model] = source[(int)attributes.model];
            }

            String launchAnnouncedSource = source[(int)attributes.launch_announced];
            regex = new Regex("\"(?:[^\\\\\"]|\\\\\\\\|\\\\\")*\"", RegexOptions.None);
            match = regex.Match(launchAnnouncedSource);
            if (match.Success)
            {
                //not 100% certain I'm using regex properly here but this is how I was able to cleanly remove the \ escape characters
                launchAnnouncedSource = match.Groups[0].Value.Replace("\"", "");
            }

            //regex = new Regex("[0-9]+", RegexOptions.None);
            regex = new Regex("\\d{4}", RegexOptions.None);
            match = regex.Match(launchAnnouncedSource);
            int launchAnnouncement;
            Int32.TryParse(match.Value, out launchAnnouncement); // stuff like this can't easily be done with properties
            attributesArray[(int)attributes.launch_announced] = launchAnnouncement;

            String launchStatusSource = source[(int)attributes.launch_status];
            if (launchStatusSource == "Discontinued" || launchStatusSource == "Cancelled")
            {
                attributesArray[(int)(attributes.launch_status)] = launchStatusSource;
            }
            else
            {
                match = regex.Match(launchStatusSource);
                attributesArray[(int)(attributes.launch_status)] = match.Value;
            }

            if (source[(int)attributes.body_dimensions] != "-")
            {
                attributesArray[(int)attributes.body_dimensions] = source[(int)attributes.body_dimensions];
            }

            String bodyWeightSource = source[(int)attributes.body_weight];
            float bodyWeight;
            float.TryParse(bodyWeightSource.Split(" ")[0], out bodyWeight);
            attributesArray[(int) attributes.body_weight] = bodyWeight;

            String bodySimSource = source[(int)attributes.body_sim];
            regex = new Regex("\"(?:[^\\\\\"]|\\\\\\\\|\\\\\")*\"", RegexOptions.None);
            match = regex.Match(bodySimSource);
            if (match.Success)
            {
                bodySimSource = match.Groups[0].Value.Replace("\"", "");
            }
            if (bodySimSource.Contains("SIM"))
            {
                attributesArray[(int)attributes.body_sim] = bodySimSource;
            }

            String displayTypeSource = source[(int)attributes.display_type];
            match = regex.Match(displayTypeSource);
            if (match.Success)
            {
                attributesArray[(int) attributes.display_type] = match.Groups[0].Value.Replace("\"", "");
            }
            else if (displayTypeSource.Equals("-"))
            {
                attributesArray[(int)attributes.display_type] = null;
            }
            else 
            {
                attributesArray[(int)attributes.display_type] = displayTypeSource;
            }


            String displaySizeSource = source[(int)attributes.display_size];
            displaySizeSource = displaySizeSource.Split(" ")[0];
            if (displaySizeSource != "")
            {
                float displaySize;
                float.TryParse(displaySizeSource, out displaySize);
                attributesArray[(int)attributes.display_size] = displaySize;
            }

            String displayResolutionSource = source[(int)attributes.display_resolution];
            match = regex.Match(displayResolutionSource);
            if (displayResolutionSource != "" || displayResolutionSource != "-")
            {
                attributesArray[(int)attributes.display_resolution] = displayResolutionSource;
            }
            if (match.Success)
            {
                attributesArray[(int)attributes.display_resolution] = match.Groups[0].Value.Replace("\"", "");
            }
            else
            {
                attributesArray[(int)attributes.display_resolution] = displayResolutionSource;
            }

            String featuresSensorsSource = source[(int)attributes.features_sensors];
            match = regex.Match(featuresSensorsSource);
            if (match.Success)
            {
                attributesArray[(int)(attributes.features_sensors)] = match.Groups[0].Value.Replace("\"", "");
            }
            else
            {
                attributesArray[(int)(attributes.features_sensors)] = featuresSensorsSource;
            }

            String osSource = source[(int)attributes.os];
            match = regex.Match(osSource);
            if (match.Success)
            {
                attributesArray[((int)attributes.os)] = match.Groups[0].Value.Replace("\"", "");
            }
            osSource = osSource.Split(",")[0];
            if (osSource != "")
            {
                attributesArray[(int)attributes.os] = osSource;
            }
        }

        /// <summary>
        /// Method for comparing two cell phones by weight
        /// </summary>
        /// <param name="otherCellWeight"> the other cell phone's weight</param>
        /// <returns> 1,-1,0 matching IComparable standard</returns>
        public int CompareTo(object? obj)
        {
            float thisCellWeight = (float)attributesArray[(int)attributes.body_weight];
            Cell otherCell = (Cell)obj;
            float otherCellWeight = (float)otherCell.GetAttribute(attributes.body_weight);
            if (thisCellWeight > otherCellWeight)
            {
                return 1;
            }
            if(thisCellWeight < otherCellWeight)
            {
                return -1;
            }
            return 0;
        }
    }
}
