using System;
using System.IO;
using System.Collections.Generic;
using Mooyash.Services;

namespace Mooyash.Modules
{
    public class Sounds
    {
        public static Dictionary<string, Sound> sounds = new Dictionary<string, Sound>();
        private static string[] soundNames = new string[] { "collide", "hit", "coin", "itemBox", "lapFinish", "throttle", "dirt", "road"};
        public static void loadSounds()
        {
            foreach(string s in soundNames)
            {
                sounds.Add(s, Engine.LoadSound(Path.Combine("Sounds", s + ".mp3")));
            }
        }
          
    }
}

