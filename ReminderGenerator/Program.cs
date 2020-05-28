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
        private static string Path;
        
        static void Main()
        {
            StreamReader sr = new StreamReader(@"C:\Users\Wolfie\Source\Repos\CustomReminders\ReminderGenerator\ModPath.txt");
            Path = sr.ReadLine();
            sr.Close();

            string command = "";
            // Main loop
            do
            {
                Console.WriteLine("Commands:\nnew - new date{int} season{int} year{int} time{int} message{string}\nhelp\nquit");
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

                        DaysSinceStart = ConvertToDays(command_parts[1], command_parts[2], command_parts[3]);
                        Time = Convert.ToInt32(command_parts[4]);
                        for (int i = 0; i < command_parts.Length - 5; i++)
                        {
                            int index = i + 5;
                            ReminderMessage += " " + command_parts[index];
                        }
                        WriteToFile(ReminderMessage, DaysSinceStart, Time);
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
        
        static private void WriteToFile(string ReminderMessage, int DaysSinceStart, int Time)
        {
            var options = new JsonWriterOptions { Indented = true };
            int ReminderCount = 0;
            foreach (string Path in Directory.EnumerateFiles(Path))
            { 
                ReminderCount++; 
            }
            using (var stream = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(stream, options))
                {
                    writer.WriteStartObject();
                    writer.WriteString("ReminderMessage", ReminderMessage.Trim());
                    writer.WriteNumber("DaysSinceStart", DaysSinceStart);
                    writer.WriteNumber("Time", Time);
                    writer.WriteEndObject();
                }
                string json = Encoding.UTF8.GetString(stream.ToArray());
                System.IO.File.WriteAllText($"{Path}\\reminder{++ReminderCount}.json", json);
            }
        }

        static private int ConvertToDays(string date, string season, string year)
        {
            int DaysSinceStart = ((Convert.ToInt32(season) - 1) * 28) + ((Convert.ToInt32(year) - 1) * 112) + Convert.ToInt32(date) + 28;
            return DaysSinceStart;
        }
    }
}
