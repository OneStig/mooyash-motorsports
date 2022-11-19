using System;
using System.Collections.Generic;
using Mooyash.Modules;
using Mooyash.Services;

class Game
{
    public static readonly string Title = "Mooyash Motorsport";
    public static readonly Vector2 Resolution = new Vector2(800, 800);

    public Dictionary<string, GameObject> gameObjects;
    public string[] allObjects;

    bool playing; // (saves 31 bits of overhead yay)

    Polygon p = new Polygon(new float[] { 300, 200, 300, 400, 550 }, new float[] { 400, 50, 75, 175, 400 }, Color.White);

    public Game()
    {
        // Initialize game objects
        // Load textures into static member of various GameObjects

        // First mode is false (menu)
        playing = false;
        p.splice(120);
    }

    public void Update()
    {
        Engine.DrawConvexPolygon(p);

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
    }
}
