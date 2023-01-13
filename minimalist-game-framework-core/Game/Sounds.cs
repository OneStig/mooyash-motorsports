﻿using System;
using System.IO;
using System.Collections.Generic;
using Mooyash.Services;

namespace Mooyash.Modules
{
    public class Sounds
    {
        public static Dictionary<string, Sound> sounds = new Dictionary<string, Sound>();
        private static readonly string[] soundNames = new string[] { 
            "collide", "hit", "coin", "itemBox", "lapFinish", "terrain0", "terrain1", "terrain2", "idle", "lowRev", "highRev",
        "menuMusic", "gameMusic"};

        private static SoundInstance music;

        public static void loadSounds()
        {
            foreach(string s in soundNames)
            {
                sounds.Add(s, Engine.LoadSound(Path.Combine("Sounds", s + ".mp3")));
                System.Diagnostics.Debug.WriteLine(s);
            }
        }
        
        public static void playMenuMusic()
        {
            music = Engine.PlaySound(sounds["menuMusic"], repeat:true);
        }

        public static void playGameMusic()
        {
            Engine.StopSound(music);
            music = Engine.PlaySound(sounds["gameMusic"], repeat:true);
        }
    }
}

