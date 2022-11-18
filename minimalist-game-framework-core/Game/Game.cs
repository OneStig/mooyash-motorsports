using System;
using System.Collections.Generic;
using Mooyash.Modules;
using Mooyash.Services;

class Game
{
    public static readonly string Title = "Mooyash Motorsport";
    public static readonly Vector2 Resolution = new Vector2(1280, 720);

    public Dictionary<string, GameObject> gameObjects;
    public string[] allObjects;

    bool playing; // (saves 31 bits of overhead yay)

    public Game()
    {
        // Initialize game objects
        // Load textures into static member of various GameObjects

        // First mode is false (menu)
        playing = false;
    }

    public void Update()
    {
        if (playing)
        {
            //  input handling
            //  physics handled by physics engine
            //  rendering handled by rendering engine

            Render
        }
        else
        {
            //  handled by menu class
            if (Engine.GetKeyDown(Key.Space))
            {
                playing = true;
            }
        }
    }
}
