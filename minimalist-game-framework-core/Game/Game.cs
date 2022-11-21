﻿using System;
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
    bool debugging;

    public Game()
    {
        // Initialize game objects
        PhysicsEngine.init();

        // Load textures into static member of various GameObjects

        // First mode is false (menu)
        playing = true; // SET TO FALSE LATER
        debugging = true; // set true for diagnostics

        RenderEngine.camera = new Camera(new Vector2(125, -30), Math.PI/2, 25, Math.PI/2, 20);
    }

    public void Update()
    {
        if (debugging)
        {
            System.Diagnostics.Debug.WriteLine(1 / Engine.TimeDelta);
            Console.WriteLine(1 / Engine.TimeDelta);

            if (Engine.GetKeyHeld(Key.W))
            {
                RenderEngine.camera.position += new Vector2(RenderEngine.camera.cos, RenderEngine.camera.sin);
            }
            if (Engine.GetKeyHeld(Key.S))
            {
                RenderEngine.camera.position -= new Vector2(RenderEngine.camera.cos, RenderEngine.camera.sin);
            }
            if (Engine.GetKeyHeld(Key.A))
            {
                RenderEngine.camera.changeAngle(-0.02);
            }
            if (Engine.GetKeyHeld(Key.D))
            {
                RenderEngine.camera.changeAngle(0.02);
            }
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

            RenderEngine.drawPerTrack(Track.genTrack);
        }
        else
        {
            //  handled by menu class
        }
    }
}