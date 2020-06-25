using Newtonsoft.Json;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Dem1se.CustomReminders.Utilities
{
    /// <summary>
    /// Contains some useful functions that other classes use.
    /// </summary>
    static class Utilities
    {
        /// <summary>
        /// <para>
        /// Contains the save folder name for mulitplayer support.
        /// Host generates own value, peers recieve value from host.
        /// </para>
        /// <para>
        /// This is a critical field, and will cause multiple exceptions across namespaces if null. 
        /// </para>
        /// </summary>
        public static string SaveFolderName;

        public static SButton MenuButton = GetMenuButton();

        /// <summary>
        /// This function will write the reminder to the json file reliably.
        /// </summary>
        /// <param name="ReminderMessage">The message that will pop up in reminder</param>
        /// <param name="DaysSinceStart">The date converted to DaysSinceStart</param>
        /// <param name="Time">The time of the reminder in 24hrs format</param>
        public static void WriteToFile(string ReminderMessage, int DaysSinceStart, int Time, IModHelper Helper)
        {
            ReminderModel ReminderData = new ReminderModel
            {
                DaysSinceStart = DaysSinceStart,
                ReminderMessage = ReminderMessage,
                Time = Time
            };

            string PathToWrite = Path.Combine(Helper.DirectoryPath, "data", Utilities.SaveFolderName);
            string SerializedReminderData = JsonConvert.SerializeObject(ReminderData, Formatting.Indented);
            int ReminderCount = 0;
            bool bWritten = false;
            while (!bWritten)
            {
                if (!File.Exists(Path.Combine(PathToWrite, $"reminder_{DaysSinceStart}_{Time}_{ReminderCount}.json")))
                {
                    File.WriteAllText(Path.Combine(PathToWrite, $"reminder_{DaysSinceStart}_{Time}_{ReminderCount}.json"), SerializedReminderData);
                    bWritten = true;
                }
                else
                    ReminderCount++;
            } 
        }

        /// <summary>
        /// Returns the SDate.DaysSinceStart() int equivalent given the date season and year
        /// </summary>
        /// <param name="date"></param>
        /// <param name="season"></param>
        /// <param name="year"></param>
        /// <returns>Returns int of DaysSinceStart</returns>
        public static int ConvertToDays(int date, int season, int year)
        {
            int DaysSinceStart = (season * 28) + ((year - 1) * 112) + date;
            return DaysSinceStart;
        }

        /// <summary>
        /// Returns the SDate.DaysSinceStart() int equivalent given the date season and year
        /// </summary>
        /// <param name="date">The date in int, 1 to 28</param>
        /// <param name="season">The season in int, where 1 is summer, ... , winter is 4</param>
        /// <returns>Returns int of DaysSinceStart</returns>
        public static int ConvertToDays(int date, int season)
        {
            int year = SDate.Now().Year;
            int DaysSinceStart = (season * 28) + ((year - 1) * 112) + date;
            return DaysSinceStart;
        }

        /// <summary>
        /// Returns the button that is set to open the menu in current save.
        /// </summary>
        /// <returns>the button that opens menu as SButton</returns>
        public static SButton GetMenuButton()
        {
            if (Context.IsMainPlayer)
            {
                var SaveFile = XDocument.Load(Path.Combine(Constants.CurrentSavePath, Constants.SaveFolderName));
                var query = from xml in SaveFile.Descendants("menuButton")
                            select xml.Element("InputButton").Element("key").Value;
                string MenuString = "";
                foreach (string Key in query)
                {
                    MenuString = Key;
                }
                SButton MenuButton = (SButton)Enum.Parse(typeof(SButton), MenuString);
                return MenuButton;
            }
            else
            {
                return SButton.E;
            }
        }

        /// <summary>
        /// Makes platform sensitive relative paths from absoulute paths.
        /// </summary>
        /// <param name="FilePathAbsolute">The file path from the directory enumerator</param>
        /// <param name="monitor">The SMAPI Monitor for logging purposed within the function</param>
        /// <returns>Relative path that starts from mod folder instead of full fs path.</returns>
        public static string MakeRelativePath(string FilePathAbsolute)
        {
            // Make relative path from absolute path
            string FilePathRelative = "";
            string[] FilePathAbsoulute_Parts;
            int FilePathIndex;

            // windows style
            if (Constants.TargetPlatform.ToString() == "Windows")
            {
                FilePathAbsoulute_Parts = FilePathAbsolute.Split('\\');
                FilePathIndex = Array.IndexOf(FilePathAbsoulute_Parts, "data");
                for (int i = FilePathIndex; i < FilePathAbsoulute_Parts.Length; i++)
                {
                    FilePathRelative += FilePathAbsoulute_Parts[i] + "\\";
                }
                // Remove the trailing forward slash in Relative path
                FilePathRelative = FilePathRelative.Remove(FilePathRelative.LastIndexOf("\\"));
            }
            //unix style
            else if (Constants.TargetPlatform.ToString() == "Mac" || Constants.TargetPlatform.ToString() == "Linux")
            {
                FilePathAbsoulute_Parts = FilePathAbsolute.Split('/');
                FilePathIndex = Array.IndexOf(FilePathAbsoulute_Parts, "data");
                for (int i = FilePathIndex; i < FilePathAbsoulute_Parts.Length; i++)
                {
                    FilePathRelative += FilePathAbsoulute_Parts[i] + "/";
                }
                // Remove the trailing slash in Relative path
                FilePathRelative = FilePathRelative.Remove(FilePathRelative.LastIndexOf("/"));
            }
            return FilePathRelative;
        }

        /// <summary>
        /// Converts DaysSinceStart to pretty date format (Season Day)
        /// </summary>
        /// <param name="DaysSinceStart">The DaysSinceStart of the date to convert</param>
        /// <returns></returns>
        public static string ConvertToPrettyDate(int DaysSinceStart)
        {
            int RemainderAfterYears = DaysSinceStart % 112;
            int Years = (DaysSinceStart - RemainderAfterYears) / 112;
            int Day = RemainderAfterYears % 28;
            int Months;
            if (Day == 0)
                Day = 28;
            Months = (RemainderAfterYears - Day) / 28;

            string Month;
            switch (Months)
            {
                case 0:
                    Month = "Spring";
                    break;
                case 1:
                    Month = "Summer";
                    break;
                case 2:
                    Month = "Fall";
                    break;
                case 3:
                    Month = "Winter";
                    break;
                default:
                    Month = "Season";
                    break;
            }

            // this is a special case (winter 28)
            if (Months == -1)
            {
                Month = "Winter";
                Years--;
            }

            return $"{Month} {Day}, Year {Years + 1}";
        }

        /// <summary>
        /// Converts the 24hrs time int to 12hrs string
        /// </summary>
        /// <param name="TimeIn24"></param>
        /// <returns></returns>
        public static string ConvertToPrettyTime(int TimeIn24)
        {
            string PrettyTime;
            if (TimeIn24 <= 1230) // Pre noon
            {
                PrettyTime = Convert.ToString(TimeIn24);
                if (PrettyTime.EndsWith("00")) // ends with 00
                {
                    PrettyTime = PrettyTime.Remove(2);
                    if (PrettyTime.StartsWith("0"))
                        PrettyTime = PrettyTime.Replace("0", " ");
                    PrettyTime = PrettyTime.Trim();
                    PrettyTime += " AM";
                }
                else // ends with 30
                {
                    PrettyTime = PrettyTime.Replace("30", ":30");
                    if (PrettyTime.StartsWith("0"))
                        PrettyTime = PrettyTime.Replace("0", " ");
                    PrettyTime = PrettyTime.Trim();
                    PrettyTime += " AM";
                    if (TimeIn24 == 1230)
                        PrettyTime = PrettyTime.Replace("AM", "PM");
                }
            }
            else // after noon
            {
                PrettyTime = Convert.ToString(TimeIn24 - 1200);
                if (PrettyTime.EndsWith("00")) // ends with 00
                {
                    PrettyTime = PrettyTime.Replace("00", " ");
                    if (PrettyTime.StartsWith("0"))
                        PrettyTime = PrettyTime.Replace("0", " ");
                    PrettyTime = PrettyTime.Trim();
                    PrettyTime += " PM";
                }
                else // ends with 30
                {
                    PrettyTime = PrettyTime.Replace("30", ":30");
                    if (PrettyTime.StartsWith("0"))
                        PrettyTime = PrettyTime.Replace("0", " ");
                    PrettyTime = PrettyTime.Trim();
                    PrettyTime += " PM";
                }
            }
            return PrettyTime;
        }
        
        /// <summary>
        /// Estimates the amount of pixels a string will be wide.
        /// </summary>
        /// <param name="reminderMessage">The string to estimate for</param>
        /// <returns>{int} The pixel count of estimated witdth the string would take</returns>
        public static int EstimateStringDimension(string reminderMessage)
        {
            int Width = 0;
            char[] Characters = reminderMessage.ToCharArray();
            
            // add time number
            if (Characters[1] == ' ')
                // 1 digit time
                Width += 20;
            else if (Characters[1] == ':')
                // 3 digit time
                Width += 80; // 20 extra for colon
            else
            {
                if (Characters[2] == ':')
                    // 4 digit time
                    Width += 100; // 20 extra for colon
                else
                    // 2 digit time
                    Width += 40;
            }

            // add space
            Width += 24;

            // add AM/PM
            Width += 68;

            // add season
            if (reminderMessage.Contains("Spring")) { Width += 189; }
            else if (reminderMessage.Contains("Summer")) { Width += 196; }
            else if (reminderMessage.Contains("Fall")) { Width += 135; }
            else { Width += 186; }

            // add space
            Width += 24;

            // add two digits
            Width += 40;

            // add year
            Width += 156;

            // add two spaces
            Width += 48;

            return Width;
        }

        /// <summary>
        /// Deletes the reminder of the specified index. Index being the serial position of reminder file in directory.
        /// </summary>
        /// <param name="ReminderIndex">Zero-indexed serial position of reminder file in directory</param>
        /// <param name="Helper">IModHelper instance for its fields</param>
        public static void DeleteReminder(int ReminderIndex, IModHelper Helper)
        {
            int IterationIndex = 1;
            foreach (string path in Directory.EnumerateFiles(Path.Combine(Helper.DirectoryPath, "data", Utilities.SaveFolderName)))
            {
                if (ReminderIndex == IterationIndex)
                {
                    File.Delete(path);
                    IterationIndex++;
                }
                else
                {
                    IterationIndex++;
                }
            }
            
        }
    }
}
