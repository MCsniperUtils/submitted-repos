using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MCNameSniper
{
    class Program
    {
        public class OldMojangUUIDClass
        {
            public string Name { get; set; }
            public string Id { get; set; }
        }
        public class MojangNameHistory
        {
            public string Name { get; set; }
            public long ChangedToAt { get; set; }
        }
        public static int GetTimestamp()
        {
            TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            int secondsSinceEpoch = (int)t.TotalSeconds;
            return secondsSinceEpoch;
        }
        public static string Input(string text)
        {
            lock (Console.Out)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("[");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("input");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("] ");
                Console.Write(text + ": ");
                Console.ForegroundColor = ConsoleColor.Cyan;
                return Console.ReadLine();
            }
        }
        public static void Print(string text)
        {
            lock(Console.Out)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("[");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("info");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("] ");
                Console.Write(text);
                Console.WriteLine();
            }
        }
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }
        static void Main(string[] args)
        {
            int fourtyDaysAgo = (GetTimestamp() - 3456000);
            string username = Input("Username to snipe");
            try
            {
                HttpClient client = new HttpClient();
                OldMojangUUIDClass omuc = JsonConvert.DeserializeObject<OldMojangUUIDClass>(client.GetAsync($"https://api.mojang.com/users/profiles/minecraft/{username}?at={fourtyDaysAgo}").Result.Content.ReadAsStringAsync().Result);
                MojangNameHistory[] mnh = JsonConvert.DeserializeObject<MojangNameHistory[]>(client.GetAsync($"https://api.mojang.com/user/profiles/{omuc.Id}/names").Result.Content.ReadAsStringAsync().Result);
                long prevNamesIndex = mnh.Length;

                long dropTime = (mnh[prevNamesIndex - 1].ChangedToAt / 1000) + 3196800;

                if (dropTime > GetTimestamp() + 172800)
                {
                    dropTime = (mnh[prevNamesIndex - 2].ChangedToAt / 1000) + 3196800;
                }
                if (dropTime < GetTimestamp())
                {
                    Console.WriteLine("Name already dropped");
                }
                Print($"Sniping {username} @ {String.Format("{0:G}", UnixTimeStampToDateTime(dropTime))}");
                bool found = false;
                while (!found)
                {
                    if (GetTimestamp() > dropTime)
                    {
                        Print("Name just dropped, attempting to claim username...");
                        found = true;
                    }
                }
            }
            catch
            {
                Print("That name has never been claimed before, please choose a different name.");
            }
            Console.ReadLine();
        }
    }
}
