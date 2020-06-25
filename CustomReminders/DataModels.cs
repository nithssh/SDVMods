using StardewModdingAPI;

namespace Dem1se.CustomReminders
{
    /// <summary> Mod config.json data model </summary>
    class ModConfig
    {
        public SButton Button { get; set; } = SButton.F2;
        public bool SubtlerReminderSound { get; set; } = false;
    }

    /// <summary> Data model for reminders </summary>
    class ReminderModel
    {
        public string ReminderMessage { get; set; }
        public int DaysSinceStart { get; set; }
        public int Time { get; set; }
    }
}