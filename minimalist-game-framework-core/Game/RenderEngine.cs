using System;
using Mooyash.Modules;

namespace Mooyash.Services
{
    public class Camera
    {
        public Vector2 position;
        public double angle; //0 = positive x, pi/2 = positive y
        public double height;
        public double hfov; //in radians
        public double screen; //how far (in cm) the screen is in front of camera
    }

    public static class RenderEngine
    {
        public static Camera camera;

        public static Vector2 convert(int x, int y)
        {
            Vector2 temp;
            Vector2 result;
            //convert x,y to be relative to camera
            temp.X = x - camera.position.X;
            temp.Y = y - camera.position.Y;
            //convert coordinate system (+y is away from camera)
            result.X = Vector2.Dot(new Vector2(-(float)Math.Sin(camera.angle), (float)Math.Cos(camera.angle)), temp);
            result.Y = Vector2.Dot(new Vector2( (float)Math.Cos(camera.angle), (float)Math.Sin(camera.angle)), temp);
            //project coordinates onto screen

            //WHAT IF RESULT.Y IS TINY?!
            result.X = (float)  camera.screen * result.X / result.Y;
            result.Y = (float) -camera.screen * (float) camera.height / result.Y;
            System.Diagnostics.Debug.WriteLine(result.X);
            System.Diagnostics.Debug.WriteLine(result.Y);
            //scale according to FOV
            float scale = Game.Resolution.X / (float) (2 * camera.screen * Math.Tan(camera.hfov / 2));
            result.X = result.X * scale;
            result.Y = result.Y * scale;
            //convert to MGF coordinate system
            result.X += Game.Resolution.X / 2;
            result.Y = Game.Resolution.Y - result.Y;
            return result;
            /*
            result.Y = (float) Math.Min(Game.Resolution.Y-camera.horizon,camera.scale * (x*Math.Cos(camera.angle) + y*Math.Sin(camera.angle)));
            result.X = (float) (camera.scale * (-x*Math.Sin(camera.angle) + y*Math.Cos(camera.angle)) *
                (1-result.Y/((float) camera.vanish)));
            result.X += Game.Resolution.X/2;
            result.Y = Game.Resolution.Y - result.Y;
            return new Vector2(x,y);
            */
        }

        public static void drawPerPolygon(Polygon p)
        {
            Vector2[] points = new Vector2[p.points.Length];
            bool draw = false;
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

