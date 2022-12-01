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
    public float time;
    public int laps = 1;
    public Font font = Engine.LoadFont("Mario-Kart-DS.ttf", 17);

    bool playing; // (saves 31 bits of overhead yay)
    bool debugging;

    public Game()
    {
        // Initialize game objects
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
            //Console.WriteLine(1 / Engine.TimeDelta);
            Console.WriteLine(Track.genTrack.isConvex());

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
        time += Engine.TimeDelta;
        String timer = "0" + (int) time / 60 + "." + time % 60;
        if (time % 60 < 10)
        {
            timer = "0" + (int) time / 60 + ".0" + time % 60;
        }
        timer = timer.Substring(0, 8);
        Engine.DrawString(timer, new Vector2(250, 15), Color.White, font);
        Engine.DrawString("Lap - " + laps, new Vector2(250, 30), Color.White, font);
        
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