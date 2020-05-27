using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ReminderGenerator
{
    class Program
    {
        static void Main()
        {
            string command = "";
            // Main loop
            do
            {
                Console.WriteLine("Commands:\nnew - new reminder_number{int} date{int} season{int} year{int} time{int} message{string}\nhelp\nquit");
                command = Console.ReadLine();
                CommandHandler(command);
            } while (!command.ToLower().StartsWith("quit"));
        }

        static private void CommandHandler(string command)
        {
            string[] command_parts = command.Split(' ');
            switch (command_parts[0].ToLower())
            {
                case "new":
                    {
                        string ReminderMessage = "";
                        int DaysSinceStart, Time;

                        DaysSinceStart = ConvertToDays(command_parts[2], command_parts[3], command_parts[4]);
                        Time = Convert.ToInt32(command_parts[5]);
                        for (int i = 0; i < command_parts.Length - 6; i++)
                        {
                            int index = i + 6;
                            ReminderMessage += " " + command_parts[index];
                        }
                        WriteToFile(command_parts[1], ReminderMessage, DaysSinceStart, Time);
                        break;
                    }
                case "help":
                    Console.WriteLine("Create new reminders using the 'new' command.\nExit the application using 'quit' command.\nThis program generates new json files for the mod to process.\n");
                    break;
                case "quit":
                    break;
                default:
                    Console.WriteLine("Invalid commands");
                    break;
            }

        }
        
        static private void WriteToFile(string reminder_num, string ReminderMessage, int DaysSinceStart, int Time)
        {
            var options = new JsonWriterOptions { Indented = true };
            using (var stream = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(stream, options))
                {
                    writer.WriteStartObject();
                    writer.WriteString("ReminderMessage", ReminderMessage);
                    writer.WriteNumber("DaysSinceStart", DaysSinceStart);
                    writer.WriteNumber("Time", Time);
                    writer.WriteEndObject();
                }
                string json = Encoding.UTF8.GetString(stream.ToArray());
                System.IO.File.WriteAllText($"C:\\Program Files (x86)\\Steam\\steamapps\\common\\Stardew Valley\\Mods\\CustomReminders\\data\\Wolfie_183009987\\reminder{reminder_num}.json", json);
            }
        }

        static private int ConvertToDays(string date, string season, string year)
        {
            int DaysSinceStart = ((Convert.ToInt32(season) - 1) * 28) + ((Convert.ToInt32(year) - 1) * 112) + Convert.ToInt32(date) + 28;
            return DaysSinceStart;
        }
    }
}
