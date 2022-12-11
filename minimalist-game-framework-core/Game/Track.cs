using System;
using System.Collections.Generic;

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

        public List<Tuple<Vector2, Vector2>> checkpoints;


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

        public PhysicsPolygon(Vector2[] vals, Color c, int id) : base(vals, c)
        {
            this.id = id;
        }
    }
}

