using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using Mooyash.Modules;
using Newtonsoft.Json;

namespace Mooyash.Services
{
    public class Camera
    {
        public Vector2 position;
        public float height;
        public Vector2 follow; //x = followBack, y = followUp

        public float tilt { get; private set; } //in radians, positive = down;
        public double hfov { get; private set; } //in radians
        public double angle { get; private set; } //0 = positive x, pi/2 = positive y
        public float screen { get; private set; } //how far (in cm) the screen is in front of camera

        //precalculate values to speed up rendering
        public float sin { get; private set; } // of angle
        public float cos { get; private set; } // of angle
        public float scale { get; private set; } //based on hfov and screen
        public float hslope { get; private set; } //for handling drawing conditions
        public float tsin { get; private set; } // of tilt angle
        public float tcos { get; private set; } // of tilt angle
        public float angleScale { get; private set; } //for drawing background stuff
        public Bounds2 ground { get; private set; } //for drawing the ground

        
        //don't use camera until callling followKart
        public Camera(Vector2 follow, double hfov, float screen, float tilt)
        {
            this.follow = follow;
            this.hfov = hfov;
            this.screen = screen;
            this.tilt = tilt;

            hslope = (float) Math.Tan(hfov / 2);
            scale = Game.VirtualResolution.X / (float)(2 * screen * hslope);
            tcos = (float)Math.Cos(tilt);
            tsin = (float)Math.Sin(tilt);
            angleScale = screen * scale * Game.ResolutionScale;
            float groundTemp = (float)Math.Tan(tilt) * angleScale;
            ground = new Bounds2(new Vector2(0, Game.Resolution.Y / 2 - groundTemp),
                new Vector2(Game.Resolution.X, Game.Resolution.Y /2 + groundTemp));
        }

        public void followKart(Kart kart)
        {
             angle = (kart.camFlipped ? (kart.angle + (float)Math.PI) % (2 * (float)Math.PI) : kart.angle);
            sin = (float) Math.Sin(angle);
            cos = (float) Math.Cos(angle);

            position = kart.position - new Vector2(cos, sin) * follow.X;

            height = follow.Y;
        }
    }

    public static class RenderEngine
    {
        public static Camera camera;
        private static Texture itemRoulette = Engine.LoadTexture("roulette.png");
        private static int lastItem = 0;
        private static float lastItemTimer = 0;

        //just has to be greater than 0.1f to avoid error
        private static float boostLineTimer = 0.2f;
        private static float[] angles = new float[] { -0.5f, 0, 0.5f, 2.5f, 3, 3.5f };
        private static Polygon[] boostLines = new Polygon[angles.Length];

        public static float renderDistance = 4000f;

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
            float distance = camera.tcos * input.Y + camera.tsin * camera.height;
            result.X = camera.screen * input.X / distance;
            result.Y = (camera.tsin*input.Y-camera.tcos*camera.height) * camera.screen / distance;
            //scale according to FOV
            result.X = result.X * camera.scale;
            result.Y = result.Y * camera.scale;
            //convert to MGF coordinate system
            result.X += Game.VirtualResolution.X / 2;
            result.Y = Game.VirtualResolution.Y / 2 - result.Y;
            return result;
        }

        public static Vector2 project(Vector2 input, float cos, float sin)
        {
            Vector2 result = new Vector2();
            //project coordinates onto screen
            float distance = cos * input.Y + sin * camera.height;
            result.X = camera.screen * input.X / distance;
            result.Y = (sin * input.Y - cos * camera.height) * camera.screen / distance;
            //scale according to FOV
            result.X = result.X * camera.scale;
            result.Y = result.Y * camera.scale;
            //convert to MGF coordinate system
            result.X += Game.VirtualResolution.X / 2;
            result.Y = Game.VirtualResolution.Y / 2 - result.Y;
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

            if (!testDraw(temp)) { return; }
            if (testSplice(temp))
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
            Engine.DrawRectSolid(camera.ground, t.background);

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
            foreach (GameObject o in t.backObjs)
            {
                drawBackObj(o);
            }

            if (Game.debugging)
            {
                for (int i = 0; i < t.splines.Count; i++)
                {
                    drawWaypoint(t.splines[i]);
                }
            }
            
            // Engine.DrawLine(project(rotate(Track.defaultTrack.checkpoints[0].Item1)), project(rotate(Track.defaultTrack.checkpoints[0].Item2)), Color.HotPink);
        }

        public static bool testDraw(Polygon temp)
        {
            bool leftCut = true;
            bool rightCut = true;
            bool botCut = true;
            bool topCut = true;
            for (int i = 0; i < temp.points.Length; i++)
            {
                temp.points[i] = rotate(temp.points[i]);
                leftCut &= (camera.hslope * temp.points[i].Y + temp.points[i].X < 0);
                rightCut &= (camera.hslope * temp.points[i].Y - temp.points[i].X < 0);
                //technically, we could be more aggressive with botCut, but should be unnecessary
                botCut &= (temp.points[i].Y < camera.screen);
                topCut &= (temp.points[i].Y > renderDistance);
            }
            return !(leftCut || rightCut || botCut || topCut);
        }

        public static bool testSplice(Polygon temp)
        {
            bool splice = false;
            for (int i = 0; i < temp.points.Length; i++)
            {
                splice |= (temp.points[i].Y < camera.screen);
            }
            return splice;
        }

        public static void drawWaypoint(Vector2 pos)
        {
            Vector2 newP = rotate(pos);

            if ((camera.hslope * newP.Y + newP.X < 0) || (camera.hslope * newP.Y - newP.X < 0) || (newP.Y < camera.screen) || (newP.Y > renderDistance))
            {
                return;
            }

            newP = project(newP);

            Vector2[] offsets = new Vector2[12];
            Vector2[] offsets2 = new Vector2[12];
            float wpRadius = 500;

            for (int i = 0; i < offsets.Length; i++)
            {
                float dist = i * 2f / offsets.Length * (float)Math.PI;
                offsets2[i] = new Vector2((float)Math.Sin(dist), (float)Math.Cos(dist));
                offsets[i] = offsets2[i] * wpRadius;

                // offsets[i] = offsets[i].Rotated(t.angle / (float)Math.PI * 180 - 90);
                offsets[i] += pos;
                offsets2[i] = offsets2[i] * 10 + pos;
            }

            drawPerPolygon(new Polygon(offsets, new Color(255, 87, 51, 100)));
            drawPerPolygon(new Polygon(offsets2, new Color(255, 87, 51, 255)));
        }

        public static void drawBackObj(GameObject t)
        {
            Vector2 newP = rotate(t.position);
            //this is repeating some code, but I don't think it needs to be in a method
            if ((camera.hslope * newP.Y + newP.X + t.size.X < 0) || (camera.hslope * newP.Y - newP.X + t.size.X < 0) || (newP.Y < camera.screen))
            {
                return;
            }

            //this is scuffed - i'm sure it's fine
            camera.height -= t.height;

            float distance = camera.tcos * newP.Y + camera.tsin * camera.height;
            Vector2 newSize = (camera.screen / distance) * t.size * camera.scale * Game.ResolutionScale;

            newP = project(newP) * Game.ResolutionScale;

            newSize.X = (float)Math.Round(newSize.X);
            newSize.Y = (float)Math.Round(newSize.Y);

            newP.X = (float)Math.Round(newP.X);
            newP.Y = (float)Math.Round(newP.Y);

            Engine.DrawTexture(t.texture,
                new Vector2((float)Math.Round(newP.X - newSize.X / 2), (float)Math.Round(newP.Y - newSize.Y)),
                size: newSize, scaleMode: TextureScaleMode.Nearest,
                source: new Bounds2(new Vector2(Math.Abs(t.curTex) * t.resolution.X, 0), t.resolution));

            camera.height += t.height;
        }

        public static bool drawObject(GameObject t)
        {
            Vector2 newP = rotate(t.position);
            //this is repeating some code, but I don't think it needs to be in a method
            if( (camera.hslope * newP.Y + newP.X + t.size.X < 0) || (camera.hslope * newP.Y - newP.X + t.size.X < 0) || (newP.Y < camera.screen) || (newP.Y > renderDistance))
            {
                return false;
            }

            if (Game.debugging)
            {
                Vector2[] offsets = new Vector2[12];

                for (int i = 0; i < offsets.Length; i++)
                {
                    float dist = i * 2f / offsets.Length * (float)Math.PI;
                    offsets[i] = new Vector2((float)Math.Sin(dist), (float)Math.Cos(dist)) * t.radius;

                    // offsets[i] = offsets[i].Rotated(t.angle / (float)Math.PI * 180 - 90);
                    offsets[i] += t.position;
                }

                // draw collider when debugging
                drawPerPolygon(new Polygon(offsets, new Color(255, 0, 0, 100)));

                Vector2 angleVec = project(rotate(t.position + new Vector2((float)Math.Cos(t.angle), (float)Math.Sin(t.angle)) * 100));

                Engine.DrawLine(project(rotate(t.position)) * Game.ResolutionScale, angleVec * Game.ResolutionScale, Color.LimeGreen);
            }

            // those 3 mystery boxes, their positions are separated by 100 units

            float distance = camera.tcos * newP.Y + camera.tsin * camera.height;
            Vector2 newSize = (camera.screen/distance)*t.size * camera.scale * Game.ResolutionScale;
            
            TextureMirror m = t.curTex >= 0 ? TextureMirror.None : TextureMirror.Horizontal;

            newP = project(newP) * Game.ResolutionScale;

            newSize.X = (float)Math.Round(newSize.X);
            newSize.Y = (float)Math.Round(newSize.Y);

            newP.X = (float)Math.Round(newP.X);
            newP.Y = (float)Math.Round(newP.Y);

            if (t.GetType() == typeof(Kart))
            {
                Kart k = (Kart)t;

                // scuffed fix to reorient texture with collider
                // also this does drifting particles now

                if (k == PhysicsEngine.player)
                {
                    newP.Y += 30;

                    if (PhysicsEngine.player.drifting && Math.Abs(PhysicsEngine.player.curTex) == 4)
                    {
                        TextureMirror tm = PhysicsEngine.player.curTex < 0 ? TextureMirror.None : TextureMirror.Horizontal;
                        
                        Vector2[] offsets =
                        {
                            new Vector2(28, 0),
                            new Vector2(-30, -15),
                            new Vector2(65, -12)
                        };

                        float[] sizes = {
                            1,
                            0.7f,
                            0.8f
                        };

                        float[] animTimes =
                        {
                            0.07f,
                            0.06f,
                            0.08f
                        };

                        for (int i = 0; i < offsets.Length; i++)
                        {
                            Vector2 smokePos = new Vector2(newP.X, newP.Y);

                            Vector2 smokeSize = new Vector2(64, 64);

                            smokeSize *= 0.7f + ((int)(PhysicsEngine.player.driftTime / animTimes[i]) % 3) * 0.2f;
                            smokePos.X -= smokeSize.X / 2;

                            Engine.DrawTexture(Kart.smoke,
                                smokePos + new Vector2(Math.Sign(PhysicsEngine.player.curTex) * offsets[i].X * -1
                                - Math.Sign(PhysicsEngine.player.curTex) *((int)(PhysicsEngine.player.driftTime / animTimes[i]) % 3) * 5, offsets[i].Y),
                                size: smokeSize * sizes[i],
                                scaleMode: TextureScaleMode.Nearest,
                                mirror: tm);
                        }

                        
                    }
                }

                if (k.stunned && (int)(k.stunTime / 0.2) % 2 == 0)
                {
                    Engine.DrawTexture(t.texture,
                    new Vector2((float)Math.Round(newP.X - newSize.X / 2), (float)Math.Round(newP.Y - newSize.Y)),
                    size: newSize, scaleMode: TextureScaleMode.Nearest,
                    source: new Bounds2(new Vector2(Math.Abs(t.curTex) * t.resolution.X, 0), t.resolution),
                    mirror: m,
                    color: Color.Red);
                    return true;
                }
            }

            Engine.DrawTexture(t.texture,
                new Vector2((float) Math.Round(newP.X - newSize.X / 2), (float) Math.Round(newP.Y - newSize.Y)),
                size: newSize, scaleMode: TextureScaleMode.Nearest,
                source: new Bounds2(new Vector2(Math.Abs(t.curTex) * t.resolution.X, 0), t.resolution),
                mirror: m);

            return true;
        }

        /*
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
        */

        //rads as proportion of Game.Resolution.X
        public static void createBoostLines(float rad1, float rad2, float width)
        {
            Random rand = new Random();

            float curRad1;
            float curRad2;
            float angle;
            float distance = camera.tcos * camera.follow.X + camera.tsin * camera.height;
            //based on kart texture size
            float kartY = project(new Vector2(0, camera.follow.X)).Y - (camera.screen / distance) * PhysicsEngine.player.size.Y/2 * camera.scale;
            Vector2[] points;
            for (int i = 0; i < angles.Length; i++)
            {
                curRad1 = Game.VirtualResolution.X * (rad1 + (float)(rand.NextDouble() - 0.5) / 45);
                curRad2 = Game.VirtualResolution.X * (rad2 + (float)(rand.NextDouble() - 0.5) / 12.5f);
                angle = angles[i] + (float)(rand.NextDouble() - 0.5) / 3.5f;
                points = new Vector2[] { new Vector2(curRad1, -width*Game.VirtualResolution.Y), new Vector2(curRad1,width*Game.VirtualResolution.Y),
                    new Vector2(curRad2, width * Game.VirtualResolution.Y), new Vector2(curRad2, - width * Game.VirtualResolution.Y) };
                for (int j = 0; j < points.Length; j++)
                {
                    points[j] = points[j].Rotated(-angle * 180 / (float)Math.PI) + new Vector2(Game.VirtualResolution.X/2, kartY);
                }
                boostLines[i] = new Polygon(points,new Color(0xC8, 0xF0, 0xFF));
            }
        }

        public static void drawBoostLines()
        {
            foreach (Polygon p in boostLines)
            {
                Engine.DrawConvexPolygon(p);
            }
        }

        public static string toPlace(int place)
        {
            //doesn't work for big numbers like 21
            if (place == 1)
            {
                return place + "st";
            }
            else if (place == 2)
            {
                return place + "nd";
            }
            else if (place == 3)
            {
                return place + "rd";
            }
            else
            {
                return place + "th";
            }
        }
        public static void drawUI()
        {
            if (Game.debugging)
            {
                Engine.DrawString("fps " + Math.Round(1 / Engine.TimeDelta), new Vector2(5, 5), Color.Red, Game.diagnosticFont);

                string info = JsonConvert.SerializeObject(PhysicsEngine.player, Formatting.Indented);

                int i = 0;
                foreach (string s in info.Split("\n"))
                {
                    Engine.DrawString(s, new Vector2(5, 35 + i * 11), Color.Black, Game.diagnosticFont);
                    i++;

                    if (35 + i * 11 > Game.Resolution.Y)
                    {
                        break;
                    }
                }

            }

            String timer = MenuSystem.timeToString(PhysicsEngine.time);


            float lineLen = 500;
            float lineHeight = 12;
            float progress;
            float start = (Game.Resolution.X - lineLen) / 2;
            
            Engine.DrawRectSolid(new Bounds2(start, 30, lineLen, lineHeight), Color.White);

            foreach (Kart k in PhysicsEngine.karts)
            {
                progress = (k.percentageAlongTrack / 100 + 0.005f) % 1f;

                Engine.DrawRectSolid(new Bounds2(start + lineLen * progress, 30 - lineHeight, lineHeight * 3, lineHeight * 3), k.iconColor);
            }

            // manually draw for player on top

            Kart player = PhysicsEngine.player;
            progress = (player.percentageAlongTrack / 100 + 0.005f) % 1f;

            Engine.DrawRectSolid(new Bounds2(start + lineLen * progress, 30 - lineHeight, lineHeight * 3, lineHeight * 3), player.iconColor);

            //Engine.DrawString(player.dists[0] + " ", new Vector2(300, 250), Color.White, Game.diagnosticFont);
            //Engine.DrawString(player.dists[1] + " ", new Vector2(300, 300), Color.White, Game.diagnosticFont);
            //Engine.DrawString(player.dists[2] + " ", new Vector2(300, 350), Color.White, Game.diagnosticFont);
            //Engine.DrawString(player.prevProgressInd + " " + player.curProgressInd, new Vector2(300, 400), Color.White, Game.diagnosticFont);

            MenuSystem.DrawHighlightedString(timer, new Vector2(247, 5) * Game.ResolutionScale, Color.Black, Game.font);
            MenuSystem.DrawHighlightedString("Lap " + PhysicsEngine.player.lapDisplay + "/3", new Vector2(253, 20) * Game.ResolutionScale, Color.Black, Game.font);

            // "banana", "projectile", "speed"
            // 26 x 18 pixels

            lastItemTimer += Engine.TimeDelta;

            int ind = PhysicsEngine.player.itemHeld;

            if (ind == -1)
            {
                if (lastItemTimer > 0.1f)
                {
                    lastItemTimer = 0;
                    lastItem = (lastItem + 1) % ItemBox.validItems.Length;
                }
                
                ind = lastItem + 1;
            }

            Engine.DrawTexture(itemRoulette, new Vector2(290, 35) * Game.ResolutionScale,
                source: new Bounds2(new Vector2(26 * ind, 0), new Vector2(26, 18)), size: new Vector2(26, 18) * Game.ResolutionScale,
                scaleMode: TextureScaleMode.Nearest);

            MenuSystem.DrawHighlightedString("Score " + PhysicsEngine.player.score, new Vector2(5, 5) * Game.ResolutionScale, Color.Black, Game.font);
            MenuSystem.DrawHighlightedString("Coins " + PhysicsEngine.player.coins, new Vector2(5, 20) * Game.ResolutionScale, Color.Black, Game.font);

            if (Game.GameSettings[1] == 1)
            {
                int place = PhysicsEngine.player.place;
                MenuSystem.DrawHighlightedString(toPlace(place), new Vector2(315, 150) * Game.ResolutionScale, Color.Black, Game.placeFont, align: TextAlignment.Right);
            }

            if (PhysicsEngine.player.boostTime < PhysicsEngine.player.speedBoostConst || PhysicsEngine.player.dBoostTime < PhysicsEngine.player.dBoostConst)
            {
                boostLineTimer += Engine.TimeDelta;
                if (boostLineTimer > 0.05f)
                {
                    createBoostLines(0.15f, 0.35f, 0.003f);
                    boostLineTimer = 0;
                }
                drawBoostLines();
            }
        }

        public static void drawObjects(List<GameObject> objs)
        {
            int objDrawn = 0;

            objs.Sort(compareDepths);
            foreach(GameObject t in objs)
            {
                if (t.GetType() == typeof(Kart))
                {
                    Kart k = (Kart)t;

                    if (k.isAI)
                    {
                        t.chooseTextureCam(RenderEngine.camera);
                    }
                }
                else if (t.GetType() == typeof(Coin))
                {
                    t.chooseTextureCam(RenderEngine.camera);
                }

                objDrawn += drawObject(t) ? 1 : 0;
            }

            if (Game.debugging)
            {
                Engine.DrawString("gameObj count " + objDrawn, new Vector2(5, 20), Color.Red, Game.diagnosticFont);
            }
        }

        private static int compareDepths(GameObject first, GameObject second)
        {
            return rotate(second.position).Y.CompareTo(rotate(first.position).Y);
        }

        public static void draw()
        {
            camera.followKart(PhysicsEngine.player);
            drawPerTrack(PhysicsEngine.track);
            drawObjects(PhysicsEngine.gameObjects.ToList());
            drawUI();
        }
    }
}
