using System;
using System.IO;
using System.Collections.Generic;
using Mooyash.Modules;
using Mooyash.Services;
using Newtonsoft.Json;

class Game
{
    public static readonly string Title = "Mooyash Motorsport";
    public static readonly Vector2 Resolution = new Vector2(1600, 900);

    public Dictionary<string, GameObject> gameObjects;
    public string[] allObjects;

    public Font courier = Engine.LoadFont("cour.ttf", 24);
    public Font courSm = Engine.LoadFont("cour.ttf", 15);

    Color[] colors = new Color[]
    {
        new Color(255, 255, 255),
        new Color(155, 118, 83),
        new Color(31, 109, 4),
        Color.Red,
        Color.Blue,
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
        curTrack = new Track(new List<Polygon>(), new List<Polygon>());
    }

    public void Update()
    {
        foreach (Polygon p in curTrack.interactable) {
            Engine.DrawConvexPolygon(p);
        }

        foreach (Polygon p in curTrack.visual)
        {
            Engine.DrawConvexPolygon(p);
        }

        if (curPoly.points != null && curPoly.points.Length >= 3)
        {
            Engine.DrawConvexPolygon(curPoly);
        }

        if (Engine.GetMouseButtonDown(MouseButton.Left))
        {
            if (curPoly.points == null)
            {
                curPoly = new Polygon(new Vector2[] { new Vector2(Engine.MousePosition.X, Engine.MousePosition.Y) }, colors[cc]);
                label = "started new polygon";
            }
            else
            {
                Vector2[] cpy = new Vector2[curPoly.points.Length + 1];

                for (int i = 0; i < curPoly.points.Length; i++)
                {
                    cpy[i] = new Vector2(curPoly.points[i].x, curPoly.points[i].y);
                }

                cpy[curPoly.points.Length] = new Vector2(Engine.MousePosition.X, Engine.MousePosition.Y);

                curPoly = new Polygon(cpy, curPoly.color);
            }
        }

        if (Engine.GetKeyDown(Key.E))
        {
            if (curPoly.points != null && curPoly.points.Length >= 3)
            {
                curTrack.interactable.Add(curPoly);
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

            string raw = "public static readonly Track genTrack = new Track(\n";
            raw += "new List<Polygon>() {\n";

            foreach (Polygon p in curTrack.interactable)
            {
                raw += "new Polygon(";
                raw += "new float[] {";
                for (int i = 0; i < p.points.Length; i++)
                {
                    raw += p.points[i].x;
                    if (i != p.points.Length - 1)
                    {
                        raw += ", ";
                    }
                }


                raw += "},\nnew float[] {";

                for (int i = 0; i < p.points.Length; i++)
                {
                    raw += p.points[i].y;
                    if (i != p.points.Length - 1)
                    {
                        raw += ", ";
                    }
                }


                raw += "}, new Color" + p.color.ToString() + "),\n";

            }

            raw += "},\n new List<Polygon> () {\n";

            foreach (Polygon p in curTrack.visual)
            {
                raw += "new Polygon(";
                raw += "new float[] {";
                for (int i = 0; i < p.points.Length; i++)
                {
                    raw += p.points[i].x;
                    if (i != p.points.Length - 1)
                    {
                        raw += ", ";
                    }
                }


                raw += "},\nnew float[] {";

                for (int i = 0; i < p.points.Length; i++)
                {
                    raw += p.points[i].y;
                    if (i != p.points.Length - 1)
                    {
                        raw += ", ";
                    }
                }


                raw += "}, new Color" + p.color.ToString() + "),\n";

            }

            raw += "});";

            File.WriteAllText("GeneratedTrack.json", raw);
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

        Engine.DrawString("fps: " + fps + " " + label, new Vector2(400, 10), colors[cc], courier);

        for (int x = 0; x < 16; x++)
        {
            Engine.DrawString("" + (int)(x * 100), new Vector2(x * 100, 880), new Color(255, 255, 255), courSm);
        }

        for (int y = 0; y < 9; y++)
        {
            Engine.DrawString("" + (int)(y * 100), new Vector2(20, y * 100), new Color(255, 255, 255), courSm);
        }
    }
}
