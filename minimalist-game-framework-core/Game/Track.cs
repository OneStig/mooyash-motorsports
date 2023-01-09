﻿using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Mooyash.Modules
{
    public class TrackLoader
    {
        public Vector2 startPos;
        public float startAngle;

        public Vector2[][] collidable;
        public Vector2[][] interactable;
        public Vector2[][] visual;

        public Color[] collidableColor;
        public Color[] interactableColor;
        public Color[] visualColor;

        public List<Vector2> splines;
        public Tuple<Vector2, Vector2> checkpoint;
    }

    public class Track
    {
        public Vector2 startPos;
        public float startAngle;

        public List<Polygon> collidable;
        public List<PhysicsPolygon> interactable;
        public List<Polygon> visual;

        public List<Vector2> splines;

        public float totalLen;
        public float[] lens;

        //the bool is true if the correct normal direction is 90 degrees clockwise of Item2-Item1
        public Tuple<Vector2, Vector2, bool> finish;

        public static Track[] tracks;

        public Track(List<Polygon> collidable, List<PhysicsPolygon> interactable,
            List<Polygon> visual, List<Vector2> splines,
            Tuple<Vector2, Vector2, bool> finish)
        {
            this.collidable = collidable;
            this.interactable = interactable;
            this.visual = visual;
            this.splines = splines;
            this.finish = finish;
        }

        public bool isConvex()
        {
            foreach (Polygon p in collidable)
            {
                if (!p.isConvex()) { return false; }
            }
            foreach (Polygon p in interactable) 
            {
                if (!p.isConvex()) { return false; }
            }
            foreach (Polygon p in visual)
            {
                if (!p.isConvex()) { return false; }
            }
            return true;
        }

        public static void LoadTracks()
        {
            tracks = new Track[1];

            for (int j = 0; j < tracks.Length; j++)
            {
                string RawTrack;

                if (Engine.MacOS)
                {
                    RawTrack = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Track" + j + ".json"));
                }
                else
                {
                    RawTrack = File.ReadAllText(Path.Combine("Assets", "Track" + j + ".json"));
                }
                
                TrackLoader loaded = JsonConvert.DeserializeObject<TrackLoader>(RawTrack);

                float scaleFactor = 10f;
                // apply sf

                for (int i = 0; i < loaded.collidable.Length; i++)
                {
                    for (int k = 0; k < loaded.collidable[i].Length; k++)
                    {
                        loaded.collidable[i][k] *= scaleFactor;
                    }
                }

                for (int i = 0; i < loaded.interactable.Length; i++)
                {
                    for (int k = 0; k < loaded.interactable[i].Length; k++)
                    {
                        loaded.interactable[i][k] *= scaleFactor;
                    }
                }

                for (int i = 0; i < loaded.visual.Length; i++)
                {
                    for (int k = 0; k < loaded.visual[i].Length; k++)
                    {
                        loaded.visual[i][k] *= scaleFactor;
                    }
                }


                List<Polygon> collidable = new List<Polygon>();

                for (int i = 0; i < loaded.collidable.Length; i++)
                {
                    collidable.Add(new Polygon(loaded.collidable[i], loaded.collidableColor[i]));
                }

                List<PhysicsPolygon> interactable = new List<PhysicsPolygon>();

                for (int i = 0; i < loaded.interactable.Length; i++)
                {
                    int type = 0;

                    if (loaded.interactableColor[i].R == 124)
                    {
                        type = 1;
                    }
                    else if (loaded.interactableColor[i].R == 244)
                    {
                        type = 2;
                    }

                    interactable.Add(new PhysicsPolygon(loaded.interactable[i], loaded.interactableColor[i], type));
                }

                List<Polygon> visual = new List<Polygon>();

                for (int i = 0; i < loaded.visual.Length; i++)
                {
                    visual.Add(new Polygon(loaded.visual[i], loaded.visualColor[i]));
                }

                tracks[j] = new Track(collidable, interactable, visual,
                new List<Vector2>{
                loaded.startPos,
                new Vector2(2270, 6530),
                new Vector2(2640, 7030),
                new Vector2(5260, 6700),
                new Vector2(7260, 7100),
                new Vector2(8600, 7000),
                new Vector2(9020, 6650),
                new Vector2(9000, 6120),
                new Vector2(7410, 4760),
                new Vector2(6730, 5040),
                new Vector2(6030, 5050),
                new Vector2(5510, 4600),
                new Vector2(5720, 3960),
                new Vector2(6200, 3350),
                new Vector2(6000, 2590),
                new Vector2(4870, 2340),
                new Vector2(2860, 2220),
                new Vector2(2260, 2760),
                },
                    new Tuple<Vector2, Vector2, bool>(loaded.checkpoint.Item1, loaded.checkpoint.Item2, true));

                tracks[j].lens = new float[tracks[j].splines.Count];

                float deltaX;
                float deltaY;
                float dist;

                for(int i = 0; i < tracks[j].splines.Count-1; i++)
                {
                    deltaX = (tracks[j].splines[i].X - tracks[j].splines[i+1].X);
                    deltaY = (tracks[j].splines[i].Y - tracks[j].splines[i+1].Y);
                    dist = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

                    tracks[j].totalLen += dist;
                    tracks[j].lens[i] = dist;
                }

                deltaX = (tracks[j].splines[0].X - tracks[j].splines[tracks[j].splines.Count-1].X);
                deltaY = (tracks[j].splines[0].Y - tracks[j].splines[tracks[j].splines.Count - 1].Y);
                dist = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
                tracks[j].totalLen += dist;
                tracks[j].lens[tracks[j].splines.Count - 1] = dist;

                tracks[j].startPos = loaded.startPos;

                tracks[j].startAngle = loaded.startAngle;


            }
        }
    }

    public class PhysicsPolygon : Polygon
    {
        public int id; //-1 = empty space, 0 = track, 1 = grass, 2 = dirt

        public PhysicsPolygon(float[] xVals, float[] yVals, Color color, int id) : base(xVals, yVals, color)
        {
            this.id = id;
        }

        public PhysicsPolygon(Vector2[] points, Color color, int id) : base(points, color)
        {
            this.id = id;
        }
    }
}

