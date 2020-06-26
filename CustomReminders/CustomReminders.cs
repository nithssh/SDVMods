﻿using Dem1se.CustomReminders.UI;
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
        /// <summary> Object containing the read data from config file.</summary>
        private ModConfig Config;

        private string ReminderMessage;
        private int ReminderDate;
        private int ReminderTime;
        private string NotificationSound;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // Load the Config
            this.Config = this.Helper.ReadConfig<ModConfig>();
            Monitor.Log("Config loaded and read.", LogLevel.Trace);

            Utilities.Data.Helper = Helper;
            Utilities.Data.Monitor = Monitor;

            // Set the notification sound
            if (Config.SubtlerReminderSound)
            {
                this.NotificationSound = "crit";
            }
            else
            {
                this.NotificationSound = "questcomplete";
            }

            // Binds the event with method.
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.GameLoop.TimeChanged += ReminderNotifier;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.Multiplayer.ModMessageReceived += Multiplayer.Multiplayer.OnModMessageReceived;
            helper.Events.Multiplayer.PeerContextReceived += Multiplayer.Multiplayer.OnPeerConnected;
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
                default:
                    // just here for code quality
                    break;
            }
            Game1.exitActiveMenu();
            // Convert to DaysSinceStart - calculate year fix.
            if (SDate.Now().SeasonIndex == Season) // same seasons
            {
                if (SDate.Now().Day > day) // same season , past date
                {
                    Year = SDate.Now().Year + 1;
                    this.ReminderDate = Utilities.Converts.ConvertToDays(day, Season, Year);
                }
                else if (SDate.Now().Day == day) // same season, same date
                {
                    Year = SDate.Now().Year;
                    this.ReminderDate = Utilities.Converts.ConvertToDays(day, Season, Year);
                }
                else // same season, Future Date
                {
                    Year = SDate.Now().Year;
                    this.ReminderDate = Utilities.Converts.ConvertToDays(day, Season, Year);
                }
            }
            else if (SDate.Now().SeasonIndex > Season) // past season
            {
                Year = SDate.Now().Year + 1;
                this.ReminderDate = Utilities.Converts.ConvertToDays(day, Season, Year);
            }
            else // future season
            {
                Year = SDate.Now().Year;
                this.ReminderDate = Utilities.Converts.ConvertToDays(day, Season, Year);
            }
            this.ReminderMessage = message;
            Monitor.Log("First page completed. Date: " + season + " " + this.ReminderDate + " Time: " + this.ReminderTime + "Message: " + this.ReminderMessage, LogLevel.Trace);
            // open the second page
            Game1.activeClickableMenu = (IClickableMenu)new ReminderMenuPage2(Page2OnChangedBehaviour);

        }

        /// <summary>Define the behaviour after the Page2 OkButton menu is pressed</summary>
        private void Page2OnChangedBehaviour(int time)
        {
            this.ReminderTime = time;
            // write the data to file
            Utilities.Files.WriteToFile(this.ReminderMessage, this.ReminderDate, this.ReminderTime);
            Monitor.Log("Saved the reminder: '" + this.ReminderMessage + "' for " + this.ReminderDate + " at " + this.ReminderTime, LogLevel.Info);
        }

        ///<summary> Defines what happens when a save is loaded</summary>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            // set the SaveFolderName field if multiplayer host or singleplayer
            if (Context.IsMainPlayer)
            {
                Utilities.Data.SaveFolderName = Constants.SaveFolderName;
                Utilities.Data.MenuButton = Utilities.Data.GetMenuButton();     
            }
            else
            {
                // SaveFolderName will be assigned on peerContextRecieved
                Utilities.Data.MenuButton = Config.FarmhandInventoryButton;
            }

            // Create the data subfolder for the save for first time users. ( Avoid DirectoryNotFound Exception in OnChangedBehaviour() )
            if (!Directory.Exists(Path.Combine(Helper.DirectoryPath, "data", Utilities.Data.SaveFolderName)))
            {
                Monitor.Log("Reminders directory not found. Creating directory.", LogLevel.Info);
                Directory.CreateDirectory(Path.Combine(Helper.DirectoryPath, "data", Utilities.Data.SaveFolderName));
                Monitor.Log("Reminders directory created successfully.", LogLevel.Info);
            }
        }

        /// <summary> Defines what happens when user press the config button </summary>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs ev)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;
            if (Game1.activeClickableMenu != null || (!Context.IsPlayerFree) || ev.Button != Config.CustomRemindersButton) { return; }
            Monitor.Log("Opened ReminderMenu page 1", LogLevel.Trace);
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
            foreach (string FilePathAbsolute in Directory.EnumerateFiles(Path.Combine(this.Helper.DirectoryPath, "data", Utilities.Data.SaveFolderName)))
            {
                try
                {
                    // make relative path from absolute path
                    string FilePathRelative = Utilities.Extras.MakeRelativePath(FilePathAbsolute);

                    // Read the reminder and notify if mature
                    this.Monitor.Log($"Processing {ev.NewTime}", LogLevel.Trace);
                    ReminderModel Reminder = Helper.Data.ReadJsonFile<ReminderModel>(FilePathRelative);
                    if (Reminder.DaysSinceStart == CurrentDate.DaysSinceStart)
                    {
                        if (Reminder.Time == ev.NewTime)
                        {
                            Game1.addHUDMessage(new HUDMessage(Reminder.ReminderMessage, 2));
                            Game1.playSound(this.NotificationSound);
                            this.Monitor.Log($"Reminder notified for {Reminder.DaysSinceStart}: {Reminder.ReminderMessage}", LogLevel.Info);
                            File.Delete(FilePathAbsolute);
                        }
                        else if (Reminder.DaysSinceStart < SDate.Now().DaysSinceStart)
                        {
                            File.Delete(FilePathAbsolute);
                            Monitor.Log("Deleted old, useless reminder", LogLevel.Info);
                        }
                    }
                }
                catch (Exception e)
                {
                    Monitor.Log(e.Message, LogLevel.Error);
                }
            }
            #endregion
        }
    }
}