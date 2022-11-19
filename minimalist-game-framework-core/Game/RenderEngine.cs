using System;
using Mooyash.Modules;

namespace Mooyash.Services
{
    public class Camera
    {
        public Vector2 position;
        public float height;
        public double hfov { get; private set; } //in radians
        public double angle { get; private set; } //0 = positive x, pi/2 = positive y
        public float screen { get; private set; } //how far (in cm) the screen is in front of camera

        //precalculate values to speed up rendering
        public float sin { get; private set; } // of angle
        public float cos { get; private set; } // of angle
        public float scale { get; private set; } //based on hfov and screen
        public float hslope { get; private set; } //for handling drawing conditions
        public float vslope { get; private set; } //for handling drawing conditions

        public Camera(Vector2 position, double angle, float height, double hfov, float screen)
        {
            this.position = position;
            this.angle = angle;
            this.height = height;
            this.hfov = hfov;
            this.screen = screen;

            sin = (float) Math.Sin(angle);
            cos = (float) Math.Cos(angle);
            hslope = (float) Math.Tan(hfov / 2);
            vslope = (float)Game.Resolution.Y * hslope / Game.Resolution.X;
            scale = Game.Resolution.X / (float)(2 * screen * hslope);
        }

        public void changeAngle(double dAngle)
        {
            angle += dAngle;
            sin = (float)Math.Sin(angle);
            cos = (float)Math.Cos(angle);
        }
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
            result.X = Vector2.Dot(new Vector2(-camera.sin, camera.cos), temp);
            result.Y = Vector2.Dot(new Vector2(camera.cos, camera.sin), temp);
            return result;
        }

        public static Vector2 project(Vector2 input)
        {
            Vector2 result = new Vector2();
            //project coordinates onto screen
            result.X = camera.screen * input.X / input.Y;
            result.Y = -camera.screen * camera.height / input.Y;
            //scale according to FOV
            result.X = result.X * camera.scale;
            result.Y = result.Y * camera.scale;
            //convert to MGF coordinate system
            result.X += Game.Resolution.X / 2;
            result.Y = Game.Resolution.Y/2 - result.Y;
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
            Vector2[] tempPoints = new Vector2[p.vertices];

            for (int i = 0; i < p.vertices; i++)
            {
                tempPoints[i] = p.points[i];
            }

            Polygon temp = new Polygon(tempPoints, p.color);
            bool draw = false;
            bool splice = false;
            bool inside;

            for (int i = 0; i < temp.points.Length; i++)
            {
                temp.points[i] = rotate(temp.points[i]);
                inside = (camera.vslope * temp.points[i].Y > 1) &&
                    (camera.hslope * temp.points[i].Y + temp.points[i].X > 0) &&
                    (camera.hslope * temp.points[i].Y - temp.points[i].X > 0);
                draw = (draw || inside);
                splice = (splice || !inside);
            }
            if(!draw) { return; }
            if (splice)
            {
                temp.splice(camera.screen);
            }

            for (int i = 0; i < temp.points.Length; i++)
            {
                temp.points[i] = project(temp.points[i]);
            }
            Engine.DrawConvexPolygon(new Polygon(temp.points, temp.color));
        }

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

