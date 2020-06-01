using Newtonsoft.Json;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using System.IO;

namespace Dem1se.CustomReminders.Utilities
{
    static class Utilities
    {
        /// <summary>
        /// This function will write the reminder to the json file reliably.
        /// </summary>
        /// <param name="ReminderMessage">The message that will pop up in reminder</param>
        /// <param name="DaysSinceStart">The date converted to DaysSinceStart</param>
        /// <param name="Time">The time of the reminder in 24hrs format</param>
        public static void WriteToFile(string ReminderMessage, int DaysSinceStart, int Time)
        {
            ReminderModel ReminderData = new ReminderModel()
            {
                DaysSinceStart = DaysSinceStart,
                ReminderMessage = ReminderMessage,
                Time = Time
            };
            string SerializedReminderData = JsonConvert.SerializeObject(ReminderData, Formatting.Indented);
            int ReminderCount = 0;
            foreach (string Path in Directory.EnumerateFiles($"{Constants.ExecutionPath}\\mods\\CustomReminders\\data\\{Constants.SaveFolderName}"))
            {
                ReminderCount++;
            }
            File.WriteAllText($"{Constants.ExecutionPath}\\mods\\CustomReminders\\data\\{Constants.SaveFolderName}\\reminder{++ReminderCount}.json", SerializedReminderData);
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
    }
}
