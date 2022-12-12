using System;
using System.Collections.Generic;
using SDL2;
using Mooyash.Modules;
using Mooyash.Services;

class Game
{
    public static readonly string Title = "Mooyash Motorsport";
    public static readonly Vector2 Resolution = new Vector2(320, 180);

    public static List<int> GameSettings;
    private static Font font = Engine.LoadFont("Mario-Kart-DS.ttf", 10);

    public static bool debugging;


    bool playing; // (saves 31 bits of overhead yay)

    public Game()
    {
        // Load textures
        MenuSystem.loadTextures();
        Track.LoadTracks();
        
        //set playing to false
        playing = false;

        //DEBUGGING
        debugging = true; // set true for diagnostics
        PhysicsEngine.track = Track.tracks[0]; // should be handled by menu
        RenderEngine.camera = new Camera(new Vector2(300,100), 25, Math.PI/2, 20);
    }

    public void Update()
    {
        if (debugging && playing)
        {
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
            PhysicsEngine.update(Math.Min(Engine.TimeDelta, 1f / 60f));

            RenderEngine.camera.followKart(PhysicsEngine.player);
            RenderEngine.drawPerTrack(PhysicsEngine.track);
            RenderEngine.drawUI();
            RenderEngine.drawObjects();
        }
        else
        {
            if (MenuSystem.UpdateMenu())
            {
                playing = true;
                GameSettings = MenuSystem.GetSettings();
                PhysicsEngine.init();
            }
        }
    }
}