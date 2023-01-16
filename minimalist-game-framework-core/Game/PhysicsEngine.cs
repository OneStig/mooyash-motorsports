using System;
using System.Linq;
using System.Collections.Generic;
using Mooyash.Modules;

namespace Mooyash.Services
{
    public static class PhysicsEngine
    {
        public static HashSet<Kart> karts = new HashSet<Kart>();
        public static HashSet<Projectile> projectiles = new HashSet<Projectile>();
        public static HashSet<GameObject> gameObjects = new HashSet<GameObject>();
        public static Kart player;
        public static Track track;

        public static float time;
        public static float finalTime;


        public static Kart[] aiKarts = new Kart[0];
        public static Kart ai1;
        public static Kart ai2;
        public static Kart ai3;
        public static Kart ai4;
        public static Kart ai5;
        //public static Kart ai6;

        //Item 1 is for quadratic drag, Item2 is for linear drag, Item3 is for naturalDecel
        public static Tuple<float,float,float>[] terrainConsts = new Tuple<float,float,float>[] {
            new Tuple<float, float, float>(1,1,1), new Tuple<float, float, float>(2,2,2), new Tuple<float, float, float>(3,3,3)};

        public static void init()
        {
            //LAP COUNT + LAP DISPLAY!!!!
            //GameSettings[2]: 0 = 50cc, 1 = 100cc
            gameObjects = new HashSet<GameObject>();
            projectiles = new HashSet<Projectile>();
            karts = new HashSet<Kart>();

            player = new Kart(2400 * (Game.GameSettings[2]+1), false, "mario", Color.Red);

            gameObjects.Add(player);
            karts.Add(player);
            player.position = track.startPos;
            player.angle = track.startAngle;
            player.currentWaypoint = 1;

            time = 0;

            Random rand = new Random();

            int aiKartThrottle = 2400 + rand.Next(100, 450) * (Game.GameSettings[2]);
            ai1 = new Kart(aiKartThrottle * (Game.GameSettings[2] + 1), true, "mario", Color.Blue);
            ai1.position = track.startPos - new Vector2(100,-25);
            ai1.angle = track.startAngle;

            aiKartThrottle = 2400 + rand.Next(100, 450) * (Game.GameSettings[2]);
            ai2 = new Kart(aiKartThrottle * (Game.GameSettings[2] + 1), true, "mario", Color.Green);
            ai2.position = track.startPos - new Vector2(100, 100);
            ai2.angle = track.startAngle;

            aiKartThrottle = 2400 + rand.Next(100, 450) * (Game.GameSettings[2]);
            ai3 = new Kart(aiKartThrottle * (Game.GameSettings[2] + 1), true, "mario", Color.Orange);
            ai3.position = track.startPos - new Vector2(-100, 50);
            ai3.angle = track.startAngle;

            aiKartThrottle = 2400 + rand.Next(100, 450) * (Game.GameSettings[2]);
            ai4 = new Kart(aiKartThrottle * (Game.GameSettings[2] + 1), true, "mario", Color.Yellow);
            ai4.position = track.startPos - new Vector2(-100, 175);
            ai4.angle = track.startAngle;

            aiKartThrottle = 2400 + rand.Next(100, 450) * (Game.GameSettings[2]);
            ai5 = new Kart(aiKartThrottle * (Game.GameSettings[2] + 1), true, "mario", Color.Purple);
            ai5.position = track.startPos - new Vector2(0, 150);
            ai5.angle = track.startAngle;

            //ai6 = new Kart(2400 * (Game.GameSettings[2] + 1));
            //gameObjects.Add("ai6", ai6);
            //ai6.position = track.startPos - new Vector2(100, 130);
            //ai6.angle = track.startAngle;

            aiKarts = new Kart[] { ai1, ai2, ai3, ai4, ai5 };
            if (Game.GameSettings[1] == 1)
            {
                foreach (Kart aiKart in aiKarts)
                {
                    gameObjects.Add(aiKart);
                    karts.Add(aiKart);
                }
            }

            for (int i = 0; i < track.boxes.Length; i++)
            {
                gameObjects.Add(new ItemBox(track.boxes[i]));
            }

            for (int i = 0; i < track.coins.Length; i++)
            {
                gameObjects.Add(new Coin(track.coins[i]));
            }
        }

        public static void update(float dt)
        {
            time += dt;

            //sees if game ends
            if (player.lapDisplay > 3)
            {
                player.lapDisplay = 3;
                Game.playing = false;
                finalTime = time;
                MenuSystem.SetFinalTime(finalTime);
            }

            player.updateInput(dt);

            foreach (Kart kart in karts)
            {
                kart.update(dt);
            }

            Vector2[] pastPosAIs = new Vector2[aiKarts.Length];
            for(int i = 0; i < aiKarts.Length; i++)
            {
                pastPosAIs[i] = aiKarts[i].position;
            }

            if (Game.GameSettings[1] == 1)
            {
                for (int i = 0; i < aiKarts.Length; i++)
                {
                    aiKarts[i].updateInputAI(dt);
                    // aiKarts[i].update(dt);
                }
            }

            foreach(Projectile projectile in projectiles)
            {
                projectile.update(dt);
            }
            foreach(GameObject obj in gameObjects)
            {
                foreach(Kart kart in karts)
                {
                    if(!obj.Equals(kart) && obj.testCollision(dt, kart))
                    {
                        obj.collide(kart);
                    }
                }

                // This is scuffed for coin rotation, replace with dynamic obj later

                if (obj.GetType() == typeof(Coin))
                {
                    
                    Coin c = (Coin)obj;
                    c.update(dt);
                }
            }

            List<Kart> kartList = PhysicsEngine.karts.ToList();
            kartList.Sort(ComparePosition);

            for (int i = 0; i < kartList.Count; i++)
            {
                Kart curK = kartList[i];

                curK.place = i + 1;

                if (TestLineLine(curK.prevPosition, curK.position, track.finish.Item1, track.finish.Item2))
                {
                    if (Vector2.Dot(curK.position - curK.prevPosition, (track.finish.Item2 - track.finish.Item1).Rotated(90)) > 0 == track.finish.Item3)
                    {
                        curK.lapCount++;
                        curK.distanceTraveled = 0;
                    }
                    else
                    {
                        curK.lapCount = curK.lapDisplay - 1;
                    }
                    curK.lapDisplay = Math.Max(curK.lapDisplay, curK.lapCount);
                }
            }
        }

        public static int ComparePosition(Kart k1, Kart k2)
        {
            if (k1.lapCount == k2.lapCount)
            {
                return k2.percentageAlongTrack.CompareTo(k1.percentageAlongTrack);
            }

            return k2.lapCount.CompareTo(k1.lapCount);
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
            return 0;
        }

        // distance formula
        public static float GetDistance(Vector2 p1, Vector2 p2)
        {
            return (float)Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
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

        public static bool TestCircles(Vector2 c1, float r1, Vector2 c2, float r2)
        {
            return (c1 - c2).Length() < (r1 + r2);
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

