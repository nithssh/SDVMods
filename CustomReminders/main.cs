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
        private ModConfig Config;
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            
            // Binds the event with method.
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.GameLoop.TimeChanged += ReminderNotifier;
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs ev)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;
            
            // reminder menu
            if (ev.Button == this.Helper.Data.ReadJsonFile<ModConfig>("config.json").Button)
            {
                // Create new reminder  (testing)
                //this.Helper.Data.WriteJsonFile($"data/{Constants.SaveFolderName}/reminder1.json", reminder);
            }
        }
        
        //TODO: make it run every 30 secs and not evey 10 secs
        private void ReminderNotifier(object sender, TimeChangedEventArgs ev)
        {
            string TimeString = Convert.ToString(ev.NewTime);
            if (!(TimeString.EndsWith("30") || TimeString.EndsWith("00"))) { return; }
            SDate CurrentDate = SDate.Now();
            for (int i = 0; i < 20; i++)
            {
                try
                {
                    this.Monitor.VerboseLog($"Processed {ev.NewTime}");
                    var Reminder = this.Helper.Data.ReadJsonFile<ReminderModel>($"data/{Constants.SaveFolderName}/reminder{i}.json");
                    if (Reminder.DaysSinceStart == CurrentDate.DaysSinceStart)
                    {
                        if (Reminder.Time == ev.NewTime)
                        {
                            this.Monitor.VerboseLog($"Reminder set for {Reminder.DaysSinceStart} on {CurrentDate.DaysSinceStart}");
                            Game1.addHUDMessage(new HUDMessage(Reminder.ReminderMessage, 2));
                        }
                    }
                }
                catch (FileNotFoundException)
                {
                    break;
                }
                catch (NullReferenceException)
                {
                    break;
                }
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
}
