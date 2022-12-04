using System;
using System.Collections.Generic;
using SDL2;
using Mooyash.Modules;
using Mooyash.Services;

class Game
{
    public static readonly string Title = "Mooyash Motorsport";
    public static readonly Vector2 Resolution = new Vector2(320, 180);
    public static bool debugging;

    bool playing; // (saves 31 bits of overhead yay)

    public Game()
    {
        // Initialize game objects
        PhysicsEngine.init();

        // Load textures into static member of various GameObjects

        // First mode is false (menu)
        playing = false; // SET TO FALSE LATER
        debugging = true; // set true for diagnostics

        RenderEngine.camera = new Camera(new Vector2(125, -30), new Vector2(300,100), Math.PI/2, 25, Math.PI/2, 20);
    }

    public void Update()
    {
        if (debugging)
        {
            //System.Diagnostics.Debug.WriteLine(1 / Engine.TimeDelta);
            System.Diagnostics.Debug.WriteLine(PhysicsEngine.
                TestCircleLine(new CirclePath(new Vector2(-8,5), new Vector2(5,-8), 5f), 
                new Vector2(1,1), new Vector2(-1,-1)));

            if (Engine.GetKeyHeld(Key.Up))
            {
                RenderEngine.camera.height += 1;
            }
            if (Engine.GetKeyHeld(Key.Down))
            {
                RenderEngine.camera.height -= 1;
            }
        }
        
        if (playing)
        {
            //  input handling
            //  physics handled by physics engine
            //  rendering handled by rendering engine

            RenderEngine.drawPerTrack(Track.defaultTrack);
            PhysicsEngine.update(Math.Min(Engine.TimeDelta, 1f / 60f));
            RenderEngine.camera.followKart(PhysicsEngine.player);
            RenderEngine.drawPlayer();
        }
        else
        {
            //  handled by menu class
        }
    }
}