using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Discord_RP
{
    class Program
    {
        //static void UpdateActivity(Discord.Discord discord, string serverName, string clientName)

        static string GetCurrentServer()
        {
            File.Copy("C:/Users/natha/AppData/Roaming/.minecraft/logs/latest.log", "minecraft_latest.log");

            string serverName = "";
            using (StreamReader r = new StreamReader("minecraft_latest.log"))
            {
                string f = r.ReadToEnd();
                int i = f.LastIndexOf("[Client thread/INFO]: Connecting to ") + 36;
                if (i >= 0)
                {
                    while (f[i] != ',')
                    {
                        serverName += f[i];
                        i++;
                    }
                }
            }

            File.Delete("minecraft_latest.log");

            return serverName;
        }

        static string GetClientName()
        {
            string clientName = "";
            using (StreamReader r = new StreamReader("C:/Users/natha/AppData/Roaming/.minecraft/launcher_profiles.json"))
            {
                string f = r.ReadToEnd();
                int i = f.IndexOf("\"displayName\" : \"") + 17;
                if (i >= 0)
                {
                    while (f[i] != '"')
                    {
                        clientName += f[i];
                        i++;
                    }
                }
            }
            return clientName;
        }

        static void Main(string[] args)
        {
            dynamic config;
            using (StreamReader r = new StreamReader("../../config/client.json"))
            {
                string json_string_literal = r.ReadToEnd();
                config = JsonConvert.DeserializeObject(json_string_literal);
            }

            string serverName = GetCurrentServer();
            string clientName = GetClientName();

            Console.WriteLine("Retrieved user -- Name: {0}, Current Server: {1}", clientName, serverName);

            var discord = new Discord.Discord(Int64.Parse(config.CLIENT_ID.ToString()), (UInt64)Discord.CreateFlags.Default);
            discord.SetLogHook(Discord.LogLevel.Debug, (level, message) =>
            {
                Console.WriteLine("Log[{0}] {1}", level, message);
            });

            var activityManager = discord.GetActivityManager();
            var activity = new Discord.Activity
            {
                Details = "Playing GameName",
                State = $"On {serverName}",
                Assets =
                {
                    LargeImage = "logo",
                    LargeText = "Playing Minecraft ~ GameName",
                    SmallImage = "avatar",
                    SmallText = clientName
                }
            };

            Console.WriteLine(activity.Assets.SmallImage);

            Console.WriteLine(activity.ToString());

            activityManager.UpdateActivity(activity, (res) =>
            {
                if (res == Discord.Result.Ok)
                    Console.WriteLine("***UPDATED ACTIVITY***");
                else
                    Console.WriteLine("Could not update activity");
            });

            Console.WriteLine("after");

            //UpdateActivity(discord, serverName, clientName);

            while (Console.ReadKey().Key != ConsoleKey.Enter)
            {
                discord.RunCallbacks();
                System.Threading.Thread.Sleep(1000 / 60);
            }
        }
    }
}
