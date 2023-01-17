using System;
using System.IO;
using Newtonsoft.Json;
using Mooyash.Services;
using Mooyash.Modules;
using System.Collections.Generic;
using System.Reflection;
using System.Net.Sockets;

namespace Mooyash.Modules
{
    public class Leaderboard
    {
        //map, cc
        public List<float>[,] scores;

        public Leaderboard()
        {
            scores = new List<float>[3,2];
            for (int i = 0; i < scores.GetLength(0); i++)
            {
                for(int j = 0; j < scores.GetLength(1); j++)
                {
                    scores[i,j] = new List<float>();
                }
            }
        }

        public void addScore(float score, int map, int cc)
        {
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
            File.WriteAllText(Path.Combine("Assets","scores.json"), JsonConvert.SerializeObject(new Leaderboard(), Formatting.Indented));
        }

        public static void saveScore(float score, int map, int cc)
        {
            Leaderboard l = readScores();
            l.addScore(score, map, cc);
            writeScores(l);
        }

        public static List<float> getScores(int map, int cc)
        {
            //System.Diagnostics.Debug.WriteLine(String.Join(",", readScores().scores[map, cc]));
            return readScores().scores[map, cc];
        }

        private static void writeScores(Leaderboard l)
        {
            if (Engine.MacOS)
            {
                File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "scores.json"), JsonConvert.SerializeObject(l, Formatting.Indented));
            }
            else
            {
                File.WriteAllText(Path.Combine("Assets", "scores.json"), JsonConvert.SerializeObject(l, Formatting.Indented));
            }
        }

        private static Leaderboard readScores()
        {
            if (Engine.MacOS)
            {
                return JsonConvert.DeserializeObject<Leaderboard>(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "scores.json")));
            }
            else
            {
                return JsonConvert.DeserializeObject<Leaderboard>(File.ReadAllText(Path.Combine("Assets", "scores.json")));
            }
        }
    }
}

