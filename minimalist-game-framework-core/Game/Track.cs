using System;
using System.IO;
using System.Linq;
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

        public Tuple<Vector2, Vector2> checkpoint;

        public Vector2[] splines;
        public Vector2[] playerSplines;

        // Stuff on the track

        public Vector2[] boxes;
        public Vector2[] coins;
    }

    public class Track
    {
        public Vector2 startPos;
        public float startAngle;

        public List<Polygon> collidable;
        public List<PhysicsPolygon> interactable;
        public List<Polygon> visual;

        public List<Vector2> splines;
        public List<Vector2> playerSplines;
        //public List<Vector2> barrier;


        public float totalLen;
        public float[] lens;
        public float[] lensToPoint;

        public float pTotalLen;
        public float[] pLens;
        public float[] pLensToPoint;


        //the bool is true if the correct normal direction is 90 degrees clockwise of Item2-Item1
        public Tuple<Vector2, Vector2, bool> finish;

        public Vector2[] boxes;
        public Vector2[] coins;

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

                tracks[j] = new Track(collidable, interactable, visual, loaded.splines.ToList(),
                    new Tuple<Vector2, Vector2, bool>(loaded.checkpoint.Item1, loaded.checkpoint.Item2, true));

                tracks[j].lens = new float[tracks[j].splines.Count];
                tracks[j].lensToPoint = new float[tracks[j].splines.Count];

                //implement pLen, pLenToPoints, etc.
                tracks[j].playerSplines = loaded.playerSplines.ToList();

                tracks[j].pLens = new float[tracks[j].playerSplines.Count];
                tracks[j].pLensToPoint = new float[tracks[j].playerSplines.Count];

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
                    if(i == 0)
                    {
                        tracks[j].lensToPoint[i] = tracks[j].lens[i];
                    }
                    else
                    {
                        tracks[j].lensToPoint[i] = tracks[j].lens[i] + tracks[j].lensToPoint[i - 1];
                    }
                }

                deltaX = (tracks[j].splines[0].X - tracks[j].splines[tracks[j].splines.Count-1].X);
                deltaY = (tracks[j].splines[0].Y - tracks[j].splines[tracks[j].splines.Count - 1].Y);
                dist = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

                tracks[j].totalLen += dist;
                tracks[j].lens[tracks[j].splines.Count - 1] = dist;

                tracks[j].lensToPoint[tracks[j].splines.Count - 1] = tracks[j].totalLen;


                for (int i = 0; i < tracks[j].playerSplines.Count - 1; i++)
                {
                    deltaX = (tracks[j].playerSplines[i].X - tracks[j].playerSplines[i + 1].X);
                    deltaY = (tracks[j].playerSplines[i].Y - tracks[j].playerSplines[i + 1].Y);
                    dist = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

                    tracks[j].pTotalLen += dist;
                    tracks[j].pLens[i] = dist;
                    if (i == 0)
                    {
                        tracks[j].pLensToPoint[i] = tracks[j].pLens[i];
                    }
                    else
                    {
                        tracks[j].pLensToPoint[i] = tracks[j].pLens[i] + tracks[j].pLensToPoint[i - 1];
                    }
                }

                deltaX = (tracks[j].playerSplines[0].X - tracks[j].playerSplines[tracks[j].playerSplines.Count - 1].X);
                deltaY = (tracks[j].playerSplines[0].Y - tracks[j].playerSplines[tracks[j].playerSplines.Count - 1].Y);
                dist = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

                tracks[j].pTotalLen += dist;
                tracks[j].pLens[tracks[j].playerSplines.Count - 1] = dist;

                tracks[j].pLensToPoint[tracks[j].playerSplines.Count - 1] = tracks[j].pTotalLen;


                tracks[j].startPos = loaded.startPos;

                tracks[j].startAngle = loaded.startAngle;

                tracks[j].boxes = loaded.boxes;
                tracks[j].coins = loaded.coins;
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

