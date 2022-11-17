using System;
using Mooyash.Modules;

namespace Mooyash.Services
{
    public class Camera
    {
        public Vector2 position;
        public double angle; //0 = positive x, pi/2 = positive y
        public double vanish; //in pixels in front of camera
        public double scale;
    }

    public static class RenderEngine
    {
        public static Camera camera;

        public static Vector2 convert(int x, int y)
        {
            Vector2 result = new Vector2();
            x = (int) (x - camera.position.X);
            y = (int) (y - camera.position.Y);
            //dot products
            result.Y = (float) (camera.scale * (x*Math.Cos(camera.angle) + y*Math.Sin(camera.angle)));
            result.X = (float) (camera.scale * (x*Math.Sin(camera.angle) - y*Math.Sin(camera.angle)) *
                (1-result.Y/((float) camera.vanish)));
            result.X += Game.Resolution.X/2;
            result.Y = Math.Max(Game.Resolution.Y - result.Y, (float) camera.vanish);
            return result;
        }

        public static void drawPerPolygon(Polygon p)
        {
            Vector2[] points = new Vector2[p.points.Length];
            bool draw = true;
            for(int i = 0; i < points.Length; i++)
            {
                points[i] = convert(p.points[i].x, p.points[i].y);
            }
            if(draw)
            {
                Engine.DrawConvexPolygon(new Polygon(points, p.color));
            }
        }

        //don't draw everything in the track
        public static void drawPerTrack(Track t)
        {
            foreach(Polygon p in t.interactable)
            {
                drawPerPolygon(p);
            }
            foreach(Polygon p in t.visual)
            {
                drawPerPolygon(p);
            }
        }
    }
}

