using Newtonsoft.Json;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using System;
using System.IO;

namespace Dem1se.CustomReminders
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /// <summary>
        /// This is the delegate that defines what happens after the naming is complete.
        /// </summary>
        public ReminderMenu.doneNamingBehavior DoneNaming;
        /// <summary> Object with all the properties of the config.</summary>
        private ModConfig Config;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // Load the Config
            this.Config = this.Helper.ReadConfig<ModConfig>();

            // Binds the event with method.
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.GameLoop.TimeChanged += ReminderNotifier;

            this.DoneNaming = new ReminderMenu.doneNamingBehavior(AfterNaming);
        }

        /// <summary>
        /// Parse and Store the reminder data from the reminder menu
        /// </summary>
        /// <param name="ReminderMessage">This is the input from the textbox.</param>
        public void AfterNaming(string RemMsg, string DateTime)
        {
            Monitor.Log(RemMsg + DateTime, LogLevel.Trace);
            Game1.exitActiveMenu();

            // separate the date and time
            string[] DateAndTime = DateTime.Split(' ');
            string Date = DateAndTime[0];
            int Season = 0;
            switch (DateAndTime[1].ToLower())
            {
                case "summer":
                    Season = 1;
                    break;
                case "spring":
                    Season = 2;
                    break;
                case "fall":
                    Season = 3;
                    break;
                case "winter":
                    Season = 4;
                    break;
            }
            int Time = Convert.ToInt32(DateAndTime[2]);

            // covert the date to dayssincestart
            int DaysSince = ConvertToDays(Date, Season);
            // write the data to file

            WriteToFile(RemMsg, DaysSince, Time);
        }


        /// <summary> Defines what happens when user press the config button </summary>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs ev)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            if (Game1.activeClickableMenu != null || (!Context.IsPlayerFree) || ev.Button != Config.Button) { return; }
            Game1.activeClickableMenu = (IClickableMenu)new ReminderMenu(DoneNaming, "New Reminder");

        }

        /// <summary> Loop that checks if </summary>
        private void ReminderNotifier(object sender, TimeChangedEventArgs ev)
        {
            // returns function if game time isn't multiple of 30 in-game minutes.
            string TimeString = Convert.ToString(ev.NewTime);
            if (!(TimeString.EndsWith("30") || TimeString.EndsWith("00"))) { return; }

            // Loops through all the reminder files and evaluates if they are current.
            #region CoreReminderLoop
            SDate CurrentDate = SDate.Now();
            foreach (string FilePathAbsolute in Directory.EnumerateFiles($"{Constants.ExecutionPath}\\Mods\\CustomReminders\\data\\{Constants.SaveFolderName}"))
            {
                try
                {
                    // Make relative path from absolute path
                    string[] FilePathAbsoulute_Parts = FilePathAbsolute.Split('\\');
                    string FilePathRelative = "";
                    int FilePathIndex = Array.IndexOf(FilePathAbsoulute_Parts, "data");
                    for (int i = FilePathIndex; i < FilePathAbsoulute_Parts.Length; i++)
                    {
                        FilePathRelative += FilePathAbsoulute_Parts[i] + "\\";
                    }

                    // Remove the trailing forward slash in Relative path
                    FilePathRelative = FilePathRelative.Remove(FilePathRelative.LastIndexOf("\\"));

                    // Read the reminder and notify if mature
                    this.Monitor.Log($"Processed {ev.NewTime}", LogLevel.Trace);
                    var Reminder = this.Helper.Data.ReadJsonFile<ReminderModel>(FilePathRelative);
                    if (Reminder.DaysSinceStart == CurrentDate.DaysSinceStart)
                    {
                        if (Reminder.Time == ev.NewTime)
                        {
                            this.Monitor.Log($"Reminder set for {Reminder.DaysSinceStart} on {CurrentDate.DaysSinceStart}", LogLevel.Trace);
                            Game1.addHUDMessage(new HUDMessage(Reminder.ReminderMessage, 2));
                            File.Delete(FilePathAbsolute);
                        }
                    }
                }
                catch (Exception e)
                {
                    Monitor.Log(e.Message, LogLevel.Debug);
                }
            }
            #endregion
        }

        /// <summary>
        /// This function will write the reminder to the json file reliably.
        /// </summary>
        /// <param name="ReminderMessage">The message that will pop up in reminder</param>
        /// <param name="DaysSinceStart">The date converted to DaysSinceStart</param>
        /// <param name="Time">The time of the reminder in 24hrs format</param>
        private static void WriteToFile(string ReminderMessage, int DaysSinceStart, int Time)
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
            System.IO.File.WriteAllText($"{Constants.ExecutionPath}\\mods\\CustomReminders\\data\\{Constants.SaveFolderName}\\reminder{++ReminderCount}.json", SerializedReminderData);
        }

        private static int ConvertToDays(string date, int season, string year)
        {
            int DaysSinceStart = ((Convert.ToInt32(season) - 1) * 28) + ((Convert.ToInt32(year) - 1) * 112) + Convert.ToInt32(date) + 28;
            return DaysSinceStart;
        }

        private static int ConvertToDays(string date, int season)
        {
            int year = SDate.Now().Year;
            int DaysSinceStart = ((season - 1) * 28) + ((year - 1) * 112) + Convert.ToInt32(date) + 28;
            return DaysSinceStart;
        }
    }
}

/// <summary> Data model for reminders </summary>
class ReminderModel
{
    public string ReminderMessage { get; set; }
    public int DaysSinceStart { get; set; }
    public int Time { get; set; }
}

/// <summary> Mod config.json data model </summary>
class ModConfig
{
    public SButton Button { get; set; } = SButton.F2;
}

