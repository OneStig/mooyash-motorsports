using System;
using System.IO;
using Newtonsoft.Json;
using Mooyash.Services;
using Mooyash.Modules;
using System.Collections.Generic;
using System.Reflection;

namespace Mooyash.Modules
{
    public class Leaderboard
    {
        //map, cc
        public List<float>[,] scores;

        public Leaderboard()
        {
            scores = new List<float>[3,2];
        }

        public void addScore(float score, int map, int cc)
        {
            if (scores[map, cc] == null)
            {
                scores[map, cc] = new List<float>();
            }
            int index = scores[map,cc].BinarySearch(score);
            if (index < 0) { index = ~index; }
            scores[map, cc].Insert(index, score);
            if (scores[map, cc].Count > LeaderboardLoader.numScores)
            {
                scores[map, cc].RemoveAt(LeaderboardLoader.numScores);
            }
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
        }

        public static List<float> readScores(int map, int cc)
        {
            List<float> output = JsonConvert.DeserializeObject<Leaderboard>(File.ReadAllText("scores.json")).scores[map, cc];
            System.Diagnostics.Debug.WriteLine(String.Join(",", output));
            return output;
        }
    }
}

