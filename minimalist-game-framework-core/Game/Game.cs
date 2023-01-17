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
    public static Font placeFont = Engine.LoadFont("MarioKart.ttf", 17 * ResolutionScale);
    public static Font font = Engine.LoadFont("MarioKart.ttf", 12 * ResolutionScale);
    public static Font diagnosticFont = Engine.LoadFont("cour.ttf", 12);

    public static float countDownConst = 4;
    public static float countDown;

    public static float goConst = 1;
    public static float go;

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

        countDown = 1;
        go = 0;

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
            if (countDown <= countDownConst)
            {
                RenderEngine.draw();
                float dt = Math.Min(Engine.TimeDelta, 1f / 30f);
                Engine.DrawString(Math.Floor(countDown) + "", new Vector2(160, 60) * ResolutionScale, Color.White, placeFont, TextAlignment.Center);
                countDown += dt;
            }
            else
            {
                PhysicsEngine.update(Math.Min(Engine.TimeDelta, 1f / 30f));
                RenderEngine.draw();
                if (go <= goConst)
                {
                    float dt = Math.Min(Engine.TimeDelta, 1f / 30f);
                    Engine.DrawString("GO!", new Vector2(160, 60) * ResolutionScale, Color.White, placeFont, TextAlignment.Center);
                    go += dt;
                }
            }
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