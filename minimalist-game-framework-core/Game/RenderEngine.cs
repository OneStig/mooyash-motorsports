using System;
using Mooyash.Modules;

namespace Mooyash.Services
{
    public class Camera
    {
        public Vector2 position;
        public double angle; //0 = positive x, pi/2 = positive y
        public float height;
        public double hfov; //in radians
        public float screen; //how far (in cm) the screen is in front of camera
    }

    public static class RenderEngine
    {
        public static Camera camera;

        public static Vector2 rotate(Vector2 input)
        {
            Vector2 temp;
            Vector2 result;
            //convert x,y to be relative to camera
            temp.X = input.X - camera.position.X;
            temp.Y = input.Y - camera.position.Y;
            //convert coordinate system (+y is away from camera)
            result.X = Vector2.Dot(new Vector2(-(float)Math.Sin(camera.angle), (float)Math.Cos(camera.angle)), temp);
            result.Y = Vector2.Dot(new Vector2((float)Math.Cos(camera.angle), (float)Math.Sin(camera.angle)), temp);
            return result;
        }

        public static Vector2 project(Vector2 input)
        {
            Vector2 result = new Vector2();
            //project coordinates onto screen
            result.X = camera.screen * input.X / input.Y;
            result.Y = -camera.screen * camera.height / input.Y;
            //scale according to FOV
            float scale = Game.Resolution.X / (float)(2 * camera.screen * Math.Tan(camera.hfov / 2));
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
            Polygon temp = new Polygon(p.points);
            //Vector2[] points = new Vector2[p.points.Length];
            bool draw = true;
            bool splice = true;
            //check whether polygon should be drawn (return if shouldn't) or spliced
            if(!draw) { return; }
            for (int i = 0; i < temp.points.Length; i++)
            {
                temp.points[i] = rotate(p.points[i]);
            }
            if (splice)
            {
                temp.splice();
            }
            for (int i = 0; i < points.Length; i++)
            {
                temp.points[i] = project(temp.points[i]);
            }
            Engine.DrawConvexPolygon(temp);
        }

        //don't draw everything in the track
        public static void drawPerTrack(Track t)
        {
            foreach (Polygon p in t.interactable)
            {
                drawPerPolygon(p);
            }
            foreach (Polygon p in t.visual)
            {
                drawPerPolygon(p);
            }
        }
    }
}

