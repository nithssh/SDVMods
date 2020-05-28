using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using System.IO;
using StardewValley.Objects;

namespace Dem1se.CustomReminders
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
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

        }
        
        /// <summary> Defines what happens when user press the config button </summary>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs ev)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;
            
            // reminder menu
            if (ev.Button == Config.Button)
            {
                // new reminder menu
            }
        }
        
        /// <summary> Loop that checks if </summary>
        private void ReminderNotifier(object sender, TimeChangedEventArgs ev)
        {
            // returns function if game time isn't multiple of 30 in-game minutes.
            string TimeString = Convert.ToString(ev.NewTime);
            if (!(TimeString.EndsWith("30") || TimeString.EndsWith("00"))) { return; }

            // Loops through all the reminder files and evaluates if they are current.
            #region CoreEvaluationLoop
            SDate CurrentDate = SDate.Now();
            foreach (string FilePathAbsolute in Directory.EnumerateFiles($"{Constants.ExecutionPath}\\Mods\\CustomReminders\\data\\{Constants.SaveFolderName}"))
            {
                try
                {
                    string[] FilePathAbsoulute_Parts = FilePathAbsolute.Split('\\');
                    string FilePathRelative = "";
                    int FilePathIndex = Array.IndexOf(FilePathAbsoulute_Parts, "data");
                    for (int i = FilePathIndex; i < FilePathAbsoulute_Parts.Length; i++)
                    {
                        FilePathRelative += FilePathAbsoulute_Parts[i] + "\\";
                    }
                    
                    // Remove the trailing forward slash in Relative path
                    FilePathRelative = FilePathRelative.Remove(FilePathRelative.LastIndexOf("\\"));

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
