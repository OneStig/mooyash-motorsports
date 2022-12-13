using System;
using Mooyash.Modules;

namespace Mooyash.Services
{
    public class Camera
    {
        public Vector2 position;
        public float height;
        public Vector2 follow; //x = followBack, y = followUp

        public double hfov { get; private set; } //in radians
        public double angle { get; private set; } //0 = positive x, pi/2 = positive y
        public float screen { get; private set; } //how far (in cm) the screen is in front of camera

        //precalculate values to speed up rendering
        public float sin { get; private set; } // of angle
        public float cos { get; private set; } // of angle
        public float scale { get; private set; } //based on hfov and screen
        public float hslope { get; private set; } //for handling drawing conditions

        //don't use camera until callling followKart
        public Camera(Vector2 follow, float height, double hfov, float screen)
        {
            this.follow = follow;
            this.height = height;
            this.hfov = hfov;
            this.screen = screen;

            hslope = (float) Math.Tan(hfov / 2);
            scale = Game.Resolution.X / (float)(2 * screen * hslope);
        }

        public void changeAngle(double dAngle)
        {
            angle += dAngle;
            sin = (float)Math.Sin(angle);
            cos = (float)Math.Cos(angle);
        }

        public void followKart(Kart kart)
        {
            angle = kart.angle;
            sin = (float) Math.Sin(angle);
            cos = (float) Math.Cos(angle);

            position = kart.position - new Vector2(cos, sin) * follow.X;

            height = follow.Y;
        }
    }

    public static class RenderEngine
    {
        public static Camera camera;
        public static float renderDistance = 3000f;

        // for debugging
        private static bool drawhitboxes = false;

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
            result.Y = Game.Resolution.Y / 2 - result.Y;
            return result;
        }

        public static void drawPerPolygon(Polygon p)
        {
            Vector2[] tempPoints = new Vector2[p.vertices];

            for (int i = 0; i < p.vertices; i++)
            {
                tempPoints[i] = p.points[i];
            }

            Polygon temp = new Polygon(tempPoints, p.color);
            //used to check if polygon should be drawn and/or spliced
            bool leftCut = true;
            bool rightCut = true;
            bool botCut = true;
            bool topCut = true;
            bool splice = false;

            for (int i = 0; i < temp.points.Length; i++)
            {
                temp.points[i] = rotate(temp.points[i]);
                leftCut &= (camera.hslope * temp.points[i].Y + temp.points[i].X < 0);
                rightCut &= (camera.hslope * temp.points[i].Y - temp.points[i].X < 0);
                //technically, we could be more aggressive with botCut, but should be unnecessary
                botCut &= (temp.points[i].Y < camera.screen);
                topCut &= (temp.points[i].Y > renderDistance);
                splice |= (temp.points[i].Y < camera.screen);
            }
            if (leftCut || rightCut || botCut || topCut) { return; }
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
            Engine.DrawRectSolid(new Bounds2(Vector2.Zero, Game.Resolution), Color.DeepSkyBlue);

            foreach (Polygon p in t.interactable)
            {
                drawPerPolygon(p);
            }
            foreach (Polygon p in t.collidable)
            {
                drawPerPolygon(p);
            }
            foreach (Polygon p in t.visual)
            {
                drawPerPolygon(p);
            }

            // Engine.DrawLine(project(rotate(Track.defaultTrack.checkpoints[0].Item1)), project(rotate(Track.defaultTrack.checkpoints[0].Item2)), Color.HotPink);
        }

        public static void drawPlayer()
        {
            if (drawhitboxes)
            {
                Vector2[] offsets = new Vector2[12];

                for (int i = 0; i < offsets.Length; i++)
                {
                    float dist = i * 2f / offsets.Length * (float)Math.PI;
                    offsets[i] = new Vector2((float)Math.Sin(dist), (float)Math.Cos(dist)) * 40;

                    offsets[i] = offsets[i].Rotated(PhysicsEngine.player.angle / (float)Math.PI * 180 - 90);
                    offsets[i] += PhysicsEngine.player.position;
                }

                drawPerPolygon(new Polygon(offsets, new Color(0, 0, 0, 100)));
            }

            Vector2 screenPlayer = project(rotate(PhysicsEngine.player.position));

            screenPlayer = new Vector2((float)Math.Round(screenPlayer.X), (float)Math.Round(screenPlayer.Y));
            Engine.DrawTexture(PhysicsEngine.player.textures[PhysicsEngine.player.curTex], new Vector2(-15, -24)+ screenPlayer);
        }

        public static void drawUI()
        {
            String timer = "0" + (int) PhysicsEngine.time / 60 + "." + PhysicsEngine.time % 60 + "000";
            if (PhysicsEngine.time % 60 < 10)
            {
                timer = "0" + (int) PhysicsEngine.time / 60 + ".0" + PhysicsEngine.time % 60 + "000";
            }
            timer = timer.Substring(0, 8);
            Engine.DrawString(timer, new Vector2(250, 5), Color.White, Game.font);
            Engine.DrawString("lap " + PhysicsEngine.lapDisplay + " of 3", new Vector2(240, 20), Color.White, Game.font);
        }

        public static void drawObjects()
        {
            //TO DO
        }
    }
}

