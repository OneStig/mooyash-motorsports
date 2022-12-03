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

        public static void init()
        {
            player = new Kart();
            gameObjects = new Dictionary<string, GameObject>();
            gameObjects.Add("player", player);
            lapCount = 0;
        }

        public static void update(float dt)
        {
            Vector2 pastPos = new Vector2(player.position.X, player.position.Y);

            player.updateInput(dt);
            player.update(dt);
            
            // This checks for crossing on every frame, probably needs to be optimized later
            if (TestLineLine(pastPos, player.position, Track.defaultTrack.checkpoints[0].Item1, Track.defaultTrack.checkpoints[0].Item2))
            {
                lapCount++;
                Console.WriteLine(lapCount);
            }

            RenderEngine.camera.followKart(player);
        }
        
        //tests if line segments p and q intersect
        public static bool TestLineLine(Vector2 p1, Vector2 p2, Vector2 q1, Vector2 q2)
        {
            int o = Math.Sign(Vector2.Cross(p2 - p1, q1 - p1));

            if(o == Math.Sign(Vector2.Cross(p2 - p1, q2 - p1)) || Math.Sign(Vector2.Cross(q2 - q1, p1 - q1)) == Math.Sign(Vector2.Cross(q2 - q1, p2 - q1)))
            {
                //if all points are colinear
                if(o == 0 && Math.Max(p1.X, p2.X) > Math.Min(q1.X, q2.X) && Math.Max(q1.X, q2.X) > Math.Min(p1.X, p2.X))
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

        //tests if the path of a circle will intersect line
        public static bool TestCircleLine(Vector2 c1, Vector2 c2, float r, Vector2 p1, Vector2 p2)
        {
            return true;
        }
    }
}

