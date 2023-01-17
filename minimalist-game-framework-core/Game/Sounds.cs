using System;
using System.IO;
using System.Collections.Generic;
using Mooyash.Services;

namespace Mooyash.Modules
{
    public class Sounds
    {
        public static Dictionary<string, Sound> sounds = new Dictionary<string, Sound>();
        private static readonly string[] mp3Names = new string[] { 
            "terrain0", "terrain1", "terrain2", "zeroRev", "lowRev", "highRev",
        "menuMusic", "gameMusic"};
        //empty is for testing
        private static readonly string[] wavNames = new string[]
            {"itemBox", "collide", "hit", "lapFinish", "useItem", "coin", "empty", "drift"};

        private static SoundInstance music;

        public static void loadSounds()
        {
            foreach(string s in mp3Names)
            {
                sounds.Add(s, Engine.LoadSound(Path.Combine("Sounds/", s + ".mp3")));
                //System.Diagnostics.Debug.WriteLine(s);
            }
            foreach(string s in wavNames)
            {
                sounds.Add(s, Engine.LoadSound(Path.Combine("Sounds/", s + ".wav")));
                //System.Diagnostics.Debug.WriteLine(s);
            }
        }
        
        public static void playMenuMusic()
        {
            if (music != null)
            {
                Engine.StopSound(music, fadeTime:1f);
            }
            music = Engine.PlaySound(sounds["menuMusic"], repeat:true);
        }

        public static void playGameMusic()
        {
            if(music != null)
            {
                Engine.StopSound(music, fadeTime:0.5f);
            }
            music = Engine.PlaySound(sounds["gameMusic"], repeat:true);
        }
    }
}

