using Dem1se.CustomReminders.UI;
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
        /// <summary> Object with all the properties of the config.</summary>
        private ModConfig Config;

        private string ReminderMessage;
        private int ReminderDate;
        private int ReminderTime;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // Load the Config
            this.Config = this.Helper.ReadConfig<ModConfig>();

            // Binds the event with method.
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.GameLoop.TimeChanged += ReminderNotifier;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        }

        ///<summary> Define the behaviour after the reminder menu OkButton is pressed.</summary>
        public void Page1OnChangedBehaviour(string message, string season, int day)
        {
            int Season = 0;
            int Year;
            switch (season)
            {
                case "spring":
                    Season = 0;
                    break;
                case "summer":
                    Season = 1;
                    break;
                case "fall":
                    Season = 2;
                    break;
                case "winter":
                    Season = 3;
                    break;
            }
            // covert the date to dayssincestart initially
            
            Game1.exitActiveMenu();
            if (SDate.Now().SeasonIndex == Season) // same seasons
            {
                if (SDate.Now().Day > day) // same season , past date
                {
                    Year = SDate.Now().Year + 1;
                    this.ReminderDate = Utilities.Utilities.ConvertToDays(day, Season, Year);
                }
                else if (SDate.Now().Day == day) // same season, same date
                {
                    Year = SDate.Now().Year;
                    this.ReminderDate = Utilities.Utilities.ConvertToDays(day, Season, Year);
                }
                else // same season, Future Date
                {
                    Year = SDate.Now().Year;
                    this.ReminderDate = Utilities.Utilities.ConvertToDays(day, Season, Year);
                }
            }
            else if (SDate.Now().SeasonIndex > Season) // past season
            {
                Year = SDate.Now().Year + 1;
                this.ReminderDate = Utilities.Utilities.ConvertToDays(day, Season, Year);
            }
            else // future season
            {
                Year = SDate.Now().Year;
                this.ReminderDate = Utilities.Utilities.ConvertToDays(day, Season, Year);
            }
            this.ReminderMessage = message;
            
            // open the second page
            Game1.activeClickableMenu = (IClickableMenu)new ReminderMenuPage2(Page2OnChangedBehaviour);

        }

        /// <summary>Define the behaviour after the Page2 OkButton menu is pressed</summary>
        public void Page2OnChangedBehaviour(int time)
        {
            this.ReminderTime = time;
            // write the data to file
            Utilities.Utilities.WriteToFile(this.ReminderMessage, this.ReminderDate, this.ReminderTime);
            Monitor.Log("Saved the reminder: " + this.ReminderMessage + " for " + this.ReminderDate + " at" + this.ReminderTime);
        }
        
        ///<summary> Defines what happens when a save is loaded</summary>
        public void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            // Create the data subfolder for the save for first time users. ( Avoid DirectoryNotFound Exception in OnChangedBehaviour() )
            if (!(Directory.Exists($"{Constants.ExecutionPath}\\mods\\CustomReminders\\data\\{Constants.SaveFolderName}")))
            {
                Directory.CreateDirectory($"{Constants.ExecutionPath}\\mods\\CustomReminders\\data\\{Constants.SaveFolderName}");
            }
        }

        /// <summary> Defines what happens when user press the config button </summary>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs ev)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            if (Game1.activeClickableMenu != null || (!Context.IsPlayerFree) || ev.Button != Config.Button) { return; }

            Game1.activeClickableMenu = (IClickableMenu)new ReminderMenuPage1(Page1OnChangedBehaviour);
        }

        /// <summary> Loop that checks if any reminders are mature.</summary>
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
                            this.Monitor.Log($"Reminder set for {Reminder.DaysSinceStart} on {CurrentDate.DaysSinceStart}: {Reminder.ReminderMessage}", LogLevel.Trace);
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
}

