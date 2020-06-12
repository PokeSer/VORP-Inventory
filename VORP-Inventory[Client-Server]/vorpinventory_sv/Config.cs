using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace vorpinventory_sv
{
    public class Config:BaseScript
    {
        public static JObject config = new JObject();
        public static string ConfigString;
        public static Dictionary<string,string> lang = new Dictionary<string, string>();
        public static string resourcePath = $"{API.GetResourcePath(API.GetCurrentResourceName())}";

        public Config()
        {
            if (File.Exists($"{resourcePath}/Config.json"))
            {
                ConfigString = File.ReadAllText($"{resourcePath}/Config.json", Encoding.UTF8);
                config = JObject.Parse(ConfigString);
                if (File.Exists($"{resourcePath}/languages/{config["defaultlang"]}.json"))
                {
                    string langstring = File.ReadAllText($"{resourcePath}/languages/{config["defaultlang"]}.json",
                        Encoding.UTF8);
                    lang = JsonConvert.DeserializeObject<Dictionary<string, string>>(langstring);
                    //Debug.WriteLine($"{API.GetCurrentResourceName()}: Language {config["defaultlang"]}.json loaded!");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"{API.GetCurrentResourceName()}: Language {config["defaultlang"]}.json loaded!");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Debug.WriteLine($"{API.GetCurrentResourceName()}: {config["defaultlang"]}.json Not Found");
                }
            }
        }
    }
}