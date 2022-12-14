using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using Mooyash.Modules;
using Mooyash.Services;
using Newtonsoft.Json;
using SDL2;

class Game
{
    public static readonly string Title = "Mooyash Motorsport";
    public static readonly Vector2 Resolution = new Vector2(1600, 900);

    public Dictionary<string, GameObject> gameObjects;
    public string[] allObjects;

    public Font courier = Engine.LoadFont("cour.ttf", 24);
    public Font courSm = Engine.LoadFont("cour.ttf", 15);

    public static float scaleFactor = 10f;

    Color[] colors = new Color[]
    {
        Color.SlateGray,
        Color.LawnGreen,
        Color.SandyBrown,
        Color.Black,
        Color.White,
        Color.Red,
        Color.Yellow,
        Color.Blue,
        Color.Magenta,
    };

    string label = "";

    int cc = 0;

    Track curTrack;
    Polygon curPoly;

    bool playing; // (saves 31 bits of overhead yay)

    int frames = 0;
    float dt = 0;
    int fps = 0;

    public Game()
    {
        // Initialize game objects
        // Load textures into static member of various GameObjects

        // First mode is false (menu)
        playing = false;
        curPoly = new Polygon(new Vector2[0], colors[cc]);
        //curTrack = new Track(new List<Polygon>(), new List<Polygon>());
        curTrack = new Track(new List<Polygon>(), new List<PhysicsPolygon>(), new List<Polygon>(), new List<Vector2>(), new List<Tuple<Vector2, Vector2>>());

        //TrackLoader test = new TrackLoader();
        //test.startPos = new Vector2(1, 2);

        //string txt = JsonConvert.SerializeObject(test, Formatting.Indented);
        //File.WriteAllText("gentrack.json", txt);

        try
        {
            string test = File.ReadAllText("Assets/gentrack.json");
            TrackLoader loaded = JsonConvert.DeserializeObject<TrackLoader>(test);

            List<Polygon> collidable = new List<Polygon>();

            for (int i = 0; i < loaded.collidable.Length; i++)
            {
                collidable.Add(new Polygon(loaded.collidable[i], loaded.collidableColor[i]));
            }

            List<PhysicsPolygon> interactable = new List<PhysicsPolygon>();

            for (int i = 0; i < loaded.interactable.Length; i++)
            {
                interactable.Add(new PhysicsPolygon(loaded.interactable[i], loaded.interactableColor[i], 0));
            }

            List<Polygon> visual = new List<Polygon>();

            for (int i = 0; i < loaded.visual.Length; i++)
            {
                visual.Add(new Polygon(loaded.visual[i], loaded.visualColor[i]));
            }

            curTrack = new Track(collidable, interactable, visual, new List<Vector2>(), new List<Tuple<Vector2, Vector2>>());
        }
        catch
        {
            Console.WriteLine("nothing to load");
        }
    }

    public void Update()
    {
        float inc = 25f;

        if (Engine.GetKeyHeld(Key.O))
        {
            inc = 10f;
        }

        Vector2 mpos = Engine.MousePosition;

        if (Engine.GetKeyHeld(Key.P))
        {
            mpos = new Vector2((float)Math.Round(mpos.X / inc) * inc, (float)Math.Round(mpos.Y / inc) * inc);
        }

        foreach (Polygon p in curTrack.interactable) {
            Engine.DrawConvexPolygon(p);
        }

        foreach (Polygon p in curTrack.collidable)
        {
            Engine.DrawConvexPolygon(p);
        }

        foreach (Polygon p in curTrack.visual)
        {
            Engine.DrawConvexPolygon(p);
        }

        if (Engine.GetKeyHeld(Key.P))
        {
            for (int i = 0; i < Game.Resolution.X; i += (int)inc)
            {
                Engine.DrawLine(new Vector2(i, 0), new Vector2(i, Game.Resolution.Y), Color.Cyan);
            }

            for (int j = 0; j < Game.Resolution.Y; j += (int)inc)
            {
                Engine.DrawLine(new Vector2(0, j), new Vector2(Game.Resolution.X, j), Color.Cyan);
            }
        }

        if (curPoly.points != null)
        {
            if (curPoly.points.Length >= 3)
            {
                Engine.DrawConvexPolygon(curPoly);
            }

            for (int i = 0; i < curPoly.points.Length; i++)
            {
                Engine.DrawLine(new Vector2(curPoly.points[i].X, curPoly.points[i].Y), new Vector2(curPoly.points[(i + 1) % curPoly.points.Length].X, curPoly.points[(i + 1) % curPoly.points.Length].Y), Color.HotPink);
            }
        }

        if (Engine.GetMouseButtonDown(MouseButton.Left))
        {

            if (curPoly.points == null)
            {
                curPoly = new Polygon(new Vector2[] { mpos }, colors[cc]);
                label = "started new polygon";
            }
            else
            {
                Vector2[] cpy = new Vector2[curPoly.points.Length + 1];

                for (int i = 0; i < curPoly.points.Length; i++)
                {
                    cpy[i] = new Vector2(curPoly.points[i].X, curPoly.points[i].Y);
                }

                cpy[curPoly.points.Length] = mpos;

                curPoly = new Polygon(cpy, curPoly.color);
            }
        }

        if (Engine.GetKeyDown(Key.E))
        {
            if (curPoly.points != null && curPoly.points.Length >= 3)
            {
                curTrack.interactable.Add(new PhysicsPolygon(curPoly.points, curPoly.color, cc));

                curPoly = new Polygon(new Vector2[0], colors[cc]);
                label = "Commit poly to interatable";
            }
        }

        if (Engine.GetKeyDown(Key.R))
        {
            if (curPoly.points != null && curPoly.points.Length >= 3)
            {
                curTrack.visual.Add(curPoly);
                curPoly = new Polygon(new Vector2[0], colors[cc]);
                label = "Commit poly to visual";
            }
        }

        if (Engine.GetKeyDown(Key.T))
        {
            if (curPoly.points != null && curPoly.points.Length >= 3)
            {
                curTrack.collidable.Add(curPoly);
                curPoly = new Polygon(new Vector2[0], colors[cc]);
                label = "Commit poly to collidable";
            }
        }

        if (Engine.GetKeyDown(Key.W))
        {
            curPoly = new Polygon(new Vector2[0], colors[cc]);
            label = "reset active poly to blank";
        }


        if (Engine.GetKeyDown(Key.Space))
        {
            Console.WriteLine("Saved new track file");
            label = "saved to track file";
            // string raw = JsonConvert.SerializeObject(curTrack, Formatting.None);
            TrackLoader tst = new TrackLoader();
            tst.startPos = new Vector2(0, 0);
            tst.startAngle = 0f;

            tst.collidable = new Vector2[curTrack.collidable.Count][];
            tst.collidableColor = new Color[curTrack.collidable.Count];
            for (int i = 0; i < curTrack.collidable.Count; i++)
            {
                tst.collidable[i] = curTrack.collidable[i].points;
                tst.collidableColor[i] = curTrack.collidable[i].color;
            }

            tst.interactable = new Vector2[curTrack.interactable.Count][];
            tst.interactableColor = new Color[curTrack.interactable.Count];
            for (int i = 0; i < curTrack.interactable.Count; i++)
            {
                tst.interactable[i] = curTrack.interactable[i].points;
                tst.interactableColor[i] = curTrack.interactable[i].color;
            }

            tst.visual = new Vector2[curTrack.visual.Count][];
            tst.visualColor = new Color[curTrack.visual.Count];
            for (int i = 0; i < curTrack.visual.Count; i++)
            {
                tst.visual[i] = curTrack.visual[i].points;
                tst.visualColor[i] = curTrack.visual[i].color;
            }

            tst.splines = new Vector2[] { new Vector2(0, 0) };
            tst.checkpoint = new Tuple<Vector2, Vector2>(Vector2.Zero, Vector2.Zero);

            string txt = JsonConvert.SerializeObject(tst, Formatting.Indented);
            File.WriteAllText("gentrack.json", txt);
        }

        if (Engine.GetKeyDown(Key.NumRow1))
        {
            cc = 0;
        }
        else if (Engine.GetKeyDown(Key.NumRow2))
        {
            cc = 1;
        }
        else if (Engine.GetKeyDown(Key.NumRow3))
        {
            cc = 2;
        }
        else if (Engine.GetKeyDown(Key.NumRow4))
        {
            cc = 3;
        }
        else if (Engine.GetKeyDown(Key.NumRow5))
        {
            cc = 4;
        }
        else if (Engine.GetKeyDown(Key.NumRow6))
        {
            cc = 5;
        }
        else if (Engine.GetKeyDown(Key.NumRow7))
        {
            cc = 6;
        }
        else if (Engine.GetKeyDown(Key.NumRow8))
        {
            cc = 7;
        }
        else if (Engine.GetKeyDown(Key.NumRow9))
        {
            cc = 8;
        }

        if (playing)
        {
            //  input handling
            //  physics handled by physics engine
            //  rendering handled by rendering engine
        }
        else
        {
            //  handled by menu class
        }

        dt += Engine.TimeDelta;
        frames++;

        if (dt >= 1)
        {
            fps = (int)(frames / dt);
            frames = 0;
            dt = 0;
        }

        Engine.DrawString("fps: " + fps + " " + label + " " + mpos, new Vector2(400, 10), colors[cc], courier);

        for (int x = 0; x < 16; x++)
        {
            Engine.DrawString((int)(x * scaleFactor) + "m", new Vector2(x * 100, 880), new Color(255, 255, 255), courSm);
        }

        for (int y = 0; y < 9; y++)
        {
            Engine.DrawString((int)(y * scaleFactor) + "m", new Vector2(20, y * 100), new Color(255, 255, 255), courSm);
        }
    }
}
