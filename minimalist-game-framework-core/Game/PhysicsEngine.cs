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
        public static Track track;

        public static float time;
        public static float finalTime;

        //Item 1 is for quadratic drag, Item2 is for linear drag, Item3 is for naturalDecel
        public static Tuple<float,float,float>[] terrainConsts = new Tuple<float,float,float>[] {
            new Tuple<float, float, float>(1,1,1), new Tuple<float, float, float>(2,2,2), new Tuple<float, float, float>(3,3,3)};

        public static void init()
        {
            //GameSettings[2]: 0 = 50cc, 1 = 100cc
            player = new Kart("mario", 2400 * (Game.GameSettings[2]+1), false);

            gameObjects = new Dictionary<string, GameObject>();
            gameObjects.Add("player", player);
            player.position = track.startPos;
            player.angle = track.startAngle;

            lapCount = 0;
            lapDisplay = 1; // e.g. Lap 1/3
            time = 0;
        }

        public static void update(float dt)
        {

            time += dt;

            if (lapDisplay > 3)
            {
                lapDisplay = 3;
                Game.playing = false;
                finalTime = time;
                MenuSystem.SetFinalTime(finalTime);
            }

            Vector2 pastPos = new Vector2(player.position.X, player.position.Y);

            player.updateInput(dt);
            
            int id = GetPhysicsID(player.position);

            //this shouldn't happen, maybe we should do something else?
            if(id == -1)
            {
                player = new Kart("mario", 1200 + Game.GameSettings[2]*600, false);
                player.position = track.startPos;
                player.angle = track.startAngle;
                id = GetPhysicsID(player.position);
            }

            player.update(dt, terrainConsts[id]);

            float minCollision = 1;
            Vector2 finalPos = new Vector2();
            Vector2 cur;
            Vector2 next;
            CirclePath c = new CirclePath(pastPos, player.position, player.radius);

            //Handles collisions between player and walls of polygon
            //Should implement bounding box idea
            foreach(Polygon p in track.collidable)
            {
                for(int i = 0; i < p.vertices; i++)
                {
                    //if p has the same point twice in a row, this fails
                    cur = p.points[i];
                    next = p.points[(i+1) % p.vertices]; 
                    if(cur.Equals(next))
                    {
                        throw new Exception("Polygon cannot have same point twice in a row");
                    }
                    if(TestCircleLine(c, cur, next))
                    {
                        //OPTIMIZE: This is (kinda) recalculating norm
                        //EXCEPTION: What if cross is 0? - shouldn't happen though
                        Vector2 norm = (next - cur).Rotated(Math.Sign(Vector2.Cross(next-cur,c.c1-cur)) * 90).Normalized();
                        float norm1 = Vector2.Dot(norm, c.c1 - next) - player.radius;
                        float norm2 = Vector2.Dot(norm, c.c2 - next) - player.radius;
                        if (norm1 != norm2 && norm1 < minCollision*(norm1-norm2))
                        {
                            minCollision = norm1 / (norm1 - norm2);
                            finalPos = c.c1 + minCollision * (c.c2 - c.c1);
                        }
                    }
                }
            }

            if(minCollision != 1)
            {
                player.position = finalPos;
                player.velocity.X = -player.velocity.X * 0.75f;
                player.throttle /= 2;
            }
            
            //This checks for crossing on every frame, probably needs to be optimized later
            //Checks if player crosses the finish line
            if (TestLineLine(pastPos, player.position, track.finish.Item1, track.finish.Item2))
            {
                if (Vector2.Dot(player.position - pastPos, (track.finish.Item2 - track.finish.Item1).Rotated(90)) > 0 == track.finish.Item3)
                {
                    lapCount++;
                }
                else
                {
                    lapCount = lapDisplay - 1;
                }
                lapDisplay = Math.Max(lapDisplay, lapCount);
            }
        }

        public static int GetPhysicsID(Vector2 position)
        {
            //Should implement bounding box idea
            for(int i = track.interactable.Count - 1; i >= 0; i--)
            {
                if(TestPointPoly(position, track.interactable[i]))
                {
                    return track.interactable[i].id;
                }
            }
            return -1;
        }

        //OPTIMIZATION: Should be faster to directly calculate instead of using Vector2 methods
        //OPTIMIZATION: I could remove some edge cases (e.g., point-point, intersect by edge)
        //tests if line segments p and q intersect
        public static bool TestLineLine(Vector2 p1, Vector2 p2, Vector2 q1, Vector2 q2)
        {
            int oq1 = Math.Sign(Vector2.Cross(p2 - p1, q1 - p1));
            int op1 = Math.Sign(Vector2.Cross(q2 - q1, p1 - q1));

            if (oq1 == Math.Sign(Vector2.Cross(p2 - p1, q2 - p1)) ||
                op1 == Math.Sign(Vector2.Cross(q2 - q1, p2 - q1)))
            {
                //if all points are colinear
                if (op1 == 0 && oq1 == 0 && Math.Max(p1.X, p2.X) >= Math.Min(q1.X, q2.X) && Math.Max(q1.X, q2.X) >= Math.Min(p1.X, p2.X))
                {
                    if(p1.Equals(p2) && q1.Equals(q2))
                    {
                        return p1.Equals(q1);
                    }
                    return true;
                }
                return false;
            }
            return true;
        }

        //tests if a point is within a physics polygon
        public static bool TestPointPoly(Vector2 point, PhysicsPolygon poly)
        {
            bool odd = false;
            for(int i = 0; i < poly.points.Length - 1; i++)
            {
                odd ^= TestLineHorRay(poly.points[i], poly.points[i+1], point);
            }
            return odd ^ TestLineHorRay(poly.points[poly.points.Length - 1], poly.points[0], point);
        }

        //tests if line segment p intersects the horizontal (pointing right) ray from r
        public static bool TestLineHorRay(Vector2 p1, Vector2 p2, Vector2 r)
        {
            if(Math.Min(p1.Y, p2.Y) <= r.Y && r.Y <= Math.Max(p1.Y, p2.Y))
            {
                if(p1.Y == p2.Y)
                {
                    return Math.Max(p1.X, p2.X) >= r.X;
                }
                else
                {
                    return p1.X + (p2.X - p1.X) * (r.Y - p1.Y) / (p2.Y - p1.Y) >= r.X;
                }
            }
            return false;
        }

        //tests if the path of a circle (with radius r from c1 to c2) will intersect segment p
        public static bool TestCircleLine(CirclePath c, Vector2 p1, Vector2 p2)
        {
            Vector2 norm = c.r * (p2 - p1).Rotated(90).Normalized();
            return TestLineLine(c.c1 - norm, c.c2 + norm, p1, p2) ||
                TestLineLine(c.c1 + norm, c.c2 - norm, p1, p2) ||
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
                return (Vector2.Dot(p - c.c1, p - c.c1) - dot * dot/ Vector2.Dot(c.c2 - c.c1, c.c2 - c.c1)) < c.r * c.r;
            }
        }

        //tests if the path of a circle a intersects the path of a circle b
        public static bool TestCircleCircle(CirclePath a, CirclePath b)
        {
            //TODO!
            return false;
        }

        //Tests if path a intersects static circle at pos with radius rad
        public static bool TestCircleStaticCircle(CirclePath a, Vector2 pos, float rad)
        {
            //TODO:
            return false;
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

