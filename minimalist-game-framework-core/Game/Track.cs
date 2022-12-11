using System;
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

        public Vector2[] splines;
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
                string test = File.ReadAllText("Assets/Track" + j + ".json");
                TrackLoader loaded = JsonConvert.DeserializeObject<TrackLoader>(test);

                float sf = 10f;
                // apply sf

                for (int i = 0; i < loaded.collidable.Length; i++)
                {
                    for (int k = 0; k < loaded.collidable[i].Length; k++)
                    {
                        loaded.collidable[i][k] *= sf;
                    }
                }

                for (int i = 0; i < loaded.interactable.Length; i++)
                {
                    for (int k = 0; k < loaded.interactable[i].Length; k++)
                    {
                        loaded.interactable[i][k] *= sf;
                    }
                }

                for (int i = 0; i < loaded.visual.Length; i++)
                {
                    for (int k = 0; k < loaded.visual[i].Length; k++)
                    {
                        loaded.visual[i][k] *= sf;
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

                tracks[j] = new Track(collidable, interactable, visual, new List<Vector2>(),
                    new Tuple<Vector2, Vector2, bool>(loaded.checkpoint.Item1, loaded.checkpoint.Item2, true));

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

