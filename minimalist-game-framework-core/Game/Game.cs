using System;
using System.Collections.Generic;
using Mooyash.Modules;
using Mooyash.Services;
using SDL2;

class Game
{
    public static readonly string Title = "Mooyash Motorsport";
    public static readonly Vector2 Resolution = new Vector2(1280, 720);
    public static readonly Vector2 VirtualResolution = new Vector2(320, 180);
    public static readonly int ResolutionScale = 4;

    public static List<int> GameSettings;
    public static Font font = Engine.LoadFont("Mario-Kart-DS.ttf", 17 * ResolutionScale);
    public static Font diagnosticFont = Engine.LoadFont("cour.ttf", 20);

    public static bool debugging;

    
    public static bool playing; // (saves 31 bits of overhead yay)

    public Game()
    {
        Engine.Fullscreen = true;
        SDL.SDL_SetWindowFullscreen(Engine.Window, Engine.Fullscreen ? (uint)SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN_DESKTOP : 0);

        // Load textures
        MenuSystem.loadTextures();
        Track.LoadTracks();
        
        //set playing to false
        playing = false;

        //DEBUGGING
        debugging = false; // set true for diagnostics
        PhysicsEngine.track = Track.tracks[0]; // should be handled by menu
        RenderEngine.camera = new Camera(new Vector2(250, 150), Math.PI / 2, 20, (float) Math.PI/12);
    }

    public void Update()
    {
        if (Engine.GetKeyDown(Key.P))
        {
            debugging = !debugging;
        }
        
        if (playing)
        {
            //  input handling
            //  physics handled by physics engine
            //  rendering handled by rendering engine       
            PhysicsEngine.update(Math.Min(Engine.TimeDelta, 1f / 30f));
            RenderEngine.draw();
        }
        else
        {
            if (MenuSystem.UpdateMenu())
            {
                GameSettings = MenuSystem.GetSettings();
                PhysicsEngine.init();
                playing = true;
            }
        }
    }
}