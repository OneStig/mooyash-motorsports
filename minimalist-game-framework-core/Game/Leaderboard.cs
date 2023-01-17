using System;
using System.IO;
using Newtonsoft.Json;
using Mooyash.Services;
using Mooyash.Modules;
using System.Collections.Generic;

namespace Mooyash.Modules
{
    public class Leaderboard
    {
        //map, cc, order of scores
        public float[,,] scores;

        public Leaderboard()
        {
            scores = new float[3,2,LeaderboardLoader.numScores];
        }

        public void addScore(float score, int map, int cc)
        {
            scores[map, cc, 0] = score;
        }
    }

    public class LeaderboardLoader
    {
        public static int numScores = 5;

        public static void setScores()
        {
            File.WriteAllText("scores.json", JsonConvert.SerializeObject(new Leaderboard(), Formatting.Indented));
        }

        public static void saveScore(float score, int map, int cc)
        {
            Leaderboard l = JsonConvert.DeserializeObject<Leaderboard>(File.ReadAllText("scores.json"));
            l.addScore(score, map, cc);
            File.WriteAllText("scores.json", JsonConvert.SerializeObject(l, Formatting.Indented));
            /*
            Kart k;

            string json = JsonConvert.SerializeObject(k, Formatting.Indented);
            File.WriteAllText("test.json", json);

            string readJson = File.ReadAllText("test.json");

            k = JsonConvert.DeserializeObject<Kart>(readJson);
            */
        }

        public static float[] readScores(int map, int cc)
        {
            float[,,] scores = JsonConvert.DeserializeObject<Leaderboard>(File.ReadAllText("scores.json")).scores;
            float[] output = new float[numScores];
            for(int i = 0; i < output.Length; i++)
            {
                output[i] = scores[map, cc, i];
            }
            System.Diagnostics.Debug.WriteLine(output.ToString());
            return output;
        }
    }
}

