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
            {"itemBox", "collide", "hit", "lapFinish", "useItem", "coin", "empty"};

        private static SoundInstance music;

        public static void testSounds(int n)
        {
            for (int i = 0; i < n; i++)
            {
                sounds.Add("test" + i, Engine.LoadSound(Path.Combine("Sounds/", "test" + i + ".mp3")));
                System.Diagnostics.Debug.WriteLine(i);
            }
        }

        public static void loadSounds()
        {
            foreach(string s in mp3Names)
            {
                sounds.Add(s, Engine.LoadSound(Path.Combine("Sounds/", s + ".mp3")));
                System.Diagnostics.Debug.WriteLine(s);
            }
            foreach(string s in wavNames)
            {
                sounds.Add(s, Engine.LoadSound(Path.Combine("Sounds/", s + ".wav")));
                System.Diagnostics.Debug.WriteLine(s);
            }
        }
        
        public static void playMenuMusic()
        {
            if (music != null)
            {
                Engine.StopSound(music);
            }
            music = Engine.PlaySound(sounds["menuMusic"], repeat:true);
        }

        public static void playGameMusic()
        {
            if(music != null)
            {
                Engine.StopSound(music);
            }
            music = Engine.PlaySound(sounds["gameMusic"], repeat:true);
        }
    }
}

