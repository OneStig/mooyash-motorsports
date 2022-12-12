using System;
using System.Collections.Generic;

namespace Mooyash.Modules
{
    public class Track
    {
        public List<Polygon> collidable;
        public List<PhysicsPolygon> interactable;
        public List<Polygon> visual;
        public List<TextureObj> textureObjs;

        public List<Vector2> splines;

        //the bool is true if the correct normal direction is 90 degrees clockwise of Item2-Item1
        public Tuple<Vector2, Vector2, bool> finish;

        public static List<Track> tracks = new List<Track>();

        public Track(List<Polygon> collidable, List<PhysicsPolygon> interactable,
            List<Polygon> visual, List<Vector2> splines,
            Tuple<Vector2, Vector2, bool> finish, List<TextureObj> textureObjs)
        {
            this.collidable = collidable;
            this.interactable = interactable;
            this.visual = visual;
            this.splines = splines;
            this.finish = finish;
            this.textureObjs =  textureObjs;
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
            tracks.Add(
                new Track(
                new List<Polygon>() {
                    new Polygon(
                        new float[] { -5000, 5000, 5000, -5000},
                        new float[] { 5000, 5000, -5000, -5000},
                        new Color(0, 0, 0, 0)
                    ),

                    new Polygon(
                        new float[] { -2000, 2000, 2000, -2000},
                        new float[] { 2000, 2000, -2000, -2000},
                        Color.Yellow
                    )
                },

                new List<PhysicsPolygon>() {
                    new PhysicsPolygon(
                        new float[] { -5000, 5000, 5000, -5000},
                        new float[] { 5000, 5000, -5000, -5000},
                        Color.LawnGreen, 1
                    ),
                    new PhysicsPolygon(
                        new float[] { -4000, 4000, 4000, -4000},
                        new float[] { 4000, 4000, -4000, -4000},
                        Color.LightSlateGray, 0
                    )
                },

                new List<Polygon>() {
                    new Polygon(
                        new float[] { -1000, 0, 1000},
                        new float[] { -1000, 1000, -1000},
                        Color.White
                    )
                },

                new List<Vector2>() {
                    new Vector2(-4500, -4500),
                    new Vector2(-4500, 4500),
                    new Vector2(4500, 4500),
                    new Vector2(4500, -4500)
                },
                new Tuple<Vector2, Vector2, bool>(new Vector2(4000, 0), new Vector2(2000, 0), true),
                new List<TextureObj>()
                {
                    new TextureObj(Engine.LoadResizableTexture("R.jpg",0,0,0,0), new Vector2(3000,0), new Vector2(1000,1000))
                }
            ));;
        }
    }

    public class PhysicsPolygon : Polygon
    {
        public int id; //-1 = empty space, 0 = track, 1 = grass, 2 = dirt

        public PhysicsPolygon(float[] xVals, float[] yVals, Color color, int id) : base(xVals, yVals, color)
        {
            this.id = id;
        }
    }

    public class TextureObj
    {
        public ResizableTexture texture;
        public Vector2 pos;
        public Vector2 size;
        //for collisions
        public float radius;

        public TextureObj(ResizableTexture texture, Vector2 pos, Vector2 size)
        {
            this.texture = texture;
            this.pos = pos;
            this.size = size;
        }
    }
}

