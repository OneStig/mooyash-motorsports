using System;
using System.Collections.Generic;
using Mooyash.Modules;

namespace Mooyash.Services
{
    public static class PhysicsEngine
    {
        public static Dictionary<string, GameObject> gameObjects;
        public static Kart player;
        public static int lapCount;
        public static int lapDisplay;

        public static void init()
        {
            player = new Kart();
            gameObjects = new Dictionary<string, GameObject>();
            gameObjects.Add("player", player);
            lapCount = 0;
            lapDisplay = 1; // e.g. Lap 1/3
        }

        public static void update(float dt)
        {
            Vector2 pastPos = new Vector2(player.position.X, player.position.Y);

            player.updateInput(dt);
            player.update(dt);
            
            // This checks for crossing on every frame, probably needs to be optimized later
            if (TestLineLine(pastPos, player.position, Track.defaultTrack.checkpoints[0].Item1, Track.defaultTrack.checkpoints[0].Item2))
            {
                if (pastPos.Y < player.position.Y) {
                    lapCount++;
                }
                else
                {
                    lapCount = Math.Max(lapCount - 1, lapDisplay - 1);
                }

                lapDisplay = Math.Max(lapDisplay, lapCount);
            }

            RenderEngine.camera.followKart(player);
        }

        //OPTIMIZATION: Should be faster to directly calculate instead of using Vector2 methods
        //tests if line segments p and q intersect
        public static bool TestLineLine(Vector2 p1, Vector2 p2, Vector2 q1, Vector2 q2)
        {
            int o = Math.Sign(Vector2.Cross(p2 - p1, q1 - p1));

            if (o == Math.Sign(Vector2.Cross(p2 - p1, q2 - p1)) ||
                Math.Sign(Vector2.Cross(q2 - q1, p1 - q1)) == Math.Sign(Vector2.Cross(q2 - q1, p2 - q1)))
            {
                //if all points are colinear
                if (o == 0 && Math.Max(p1.X, p2.X) > Math.Min(q1.X, q2.X) && Math.Max(q1.X, q2.X) > Math.Min(p1.X, p2.X))
                {
                    return true;
                }
                return false;
            }
            return true;
        }

        //tests if a point is within a physics polygon
        public static bool TestPointPoly(Vector2 point, PhysicsPolygon poly)
        {
            return poly.findArea(point) == poly.area;
        }

        //tests if the path of a circle (with radius r from c1 to c2) will intersect segment p
        public static bool TestCircleLine(CirclePath c, Vector2 p1, Vector2 p2)
        {
            Vector2 radius = (c.c2 - c.c1).Normalized();
            return TestLineLine(c.c1 - c.r * radius, c.c2 + c.r * radius, p1, p2) ||
                TestPointCircle(c, p1) || TestPointCircle(c, p2);
        }

        //tests if a point p lies in the path of a circle c
        public static bool TestPointCircle(CirclePath c, Vector2 p)
        {
            float dot = Vector2.Dot(c.c2 - c.c1, p - c.c1);
            if(dot < 0)
            {
                return Vector2.Dot(p-c.c1, p-c.c1) < c.r * c.r;
            }
            else if (dot > Vector2.Dot(c.c2-c.c1, c.c2-c.c1))
            {
                return Vector2.Dot(p - c.c2, p - c.c2) < c.r * c.r;
            }
            else
            {
                return Vector2.Dot(p - c.c1, p - c.c1) - dot * dot < c.r * c.r;
            }
        }

        //tests if the path of a circle a intersects the path of a circle b
        public static bool TestCircleCircle(CirclePath a, CirclePath b)
        {
            Vector2 radiusA = (b.c1 - b.c2).Normalized().Rotated(90);
            Vector2 radiusB = (a.c1 - a.c2).Normalized().Rotated(90);
            return TestCircleLine(a, b.c1 + b.r * radiusB, b.c2 + b.r * radiusB) || TestCircleLine(a, b.c1 - b.r * radiusB, b.c2 - b.r * radiusB);
        }

        //tests if a static circle c intersects a line segment p
        public static bool TestStaticCircleLine(Vector2 c, float r, Vector2 p1, Vector2 p2)
        {
            return true;
        }
    }

    public class CirclePath
    {
        public Vector2 c1;
        public Vector2 c2;
        public float r;

        public CirclePath(Vector2 c1, Vector2 c2, float r)
        {
            this.c1 = c1;
            this.c2 = c2;
            this.r = r;
        }
    }
}

