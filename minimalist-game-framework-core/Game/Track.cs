using System;
using System.Collections.Generic;

namespace Mooyash.Modules
{
    public class Track
    {
        public List<Polygon> collidable;
        public List<PhysicsPolygon> interactable;
        public List<Polygon> visual;

        public List<Vector2> splines;

        public List<Tuple<Vector2, Vector2>> checkpoints;

        public static Track defaultTrack = new Track(
                new List<Polygon>() {
                    new Polygon(
                        new float[] { -500, 500, 500, -500},
                        new float[] { 500, 500, -500, -500},
                        new Color(0, 0, 0, 0)
                    ),

                    new Polygon(
                        new float[] { -200, 200, 200, -200},
                        new float[] { 200, 200, -200, -200},
                        Color.Yellow
                    )
                },

                new List<PhysicsPolygon>() {
                    new PhysicsPolygon(
                        new float[] { -500, 500, 500, -500},
                        new float[] { 500, 500, -500, -500},
                        Color.LawnGreen, 1
                    ),
                    new PhysicsPolygon(
                        new float[] { -400, 400, 400, -400},
                        new float[] { 400, 400, -400, -400},
                        Color.LightSlateGray, 1
                    )
                },

                new List<Polygon>() {
                    new Polygon(
                        new float[] { -100, 0, 100},
                        new float[] { -100, 100, -100},
                        Color.White
                    )
                },

                new List<Vector2>() {
                    new Vector2(-450, -450),
                    new Vector2(-450, 450),
                    new Vector2(450, 450),
                    new Vector2(450, -450)
                },
                new List<Tuple<Vector2, Vector2>>() {
                    new Tuple<Vector2, Vector2> (new Vector2(400, 0), new Vector2(500, 0))
                }
            );
        public Track(List<Polygon> collidable, List<PhysicsPolygon> interactable,
            List<Polygon> visual, List<Vector2> splines,
            List<Tuple<Vector2, Vector2>> checkpoints)
        {
            this.collidable = collidable;
            this.interactable = interactable;
            this.visual = visual;
            this.splines = splines;
            this.checkpoints = checkpoints;
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
    }

    public class PhysicsPolygon : Polygon
    {
        public int id; // 0 = track, 1 = grass, 2 = dirt

        public PhysicsPolygon(float[] xVals, float[] yVals, Color color, int id) : base(xVals, yVals, color)
        {
            this.id = id;
        }
    }
}

