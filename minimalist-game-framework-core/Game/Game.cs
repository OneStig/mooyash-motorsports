using System;
using System.Collections.Generic;
using Mooyash.Modules;
using Mooyash.Services;

class Game
{
    public static readonly string Title = "Mooyash Motorsport";
    public static readonly Vector2 Resolution = new Vector2(320, 180);

    public Dictionary<string, GameObject> gameObjects;
    public string[] allObjects;

    bool playing; // (saves 31 bits of overhead yay)

    public Game()
    {
        // Initialize game objects
        // Load textures into static member of various GameObjects

        // First mode is false (menu)
        playing = false;

        RenderEngine.camera = new Camera();
        RenderEngine.camera.position = new Vector2(235, 350);
        RenderEngine.camera.angle = Math.PI/2;
        RenderEngine.camera.height = 1;
        RenderEngine.camera.hfov = Math.PI/2;
        RenderEngine.camera.screen = 1;
    }

    public void Update()
    {
        RenderEngine.drawPerTrack(Track.templateTrack);
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