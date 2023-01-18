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
    public static Font placeFont = Engine.LoadFont("MarioKart.ttf", 25 * ResolutionScale);
    public static Font font = Engine.LoadFont("MarioKart.ttf", 12 * ResolutionScale);
    public static Font menuFont = Engine.LoadFont("MarioKart.ttf", 10 * ResolutionScale);
    public static Font diagnosticFont = Engine.LoadFont("cour.ttf", 12);

    public static float countDownConst = 4;
    public static float countDown;

    public static float goConst = 1;
    public static float go;

    public static bool debugging;

    public static bool pause;

    
    public static bool playing; // (saves 31 bits of overhead yay)
    public Game()
    {
        Engine.Fullscreen = true;
        SDL.SDL_SetWindowFullscreen(Engine.Window, Engine.Fullscreen ? (uint)SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN_DESKTOP : 0);

        // Load textures
        MenuSystem.loadTextures();
        Track.LoadTracks();
        Sounds.loadSounds();

        //start menu music
        Sounds.playMenuMusic();
        
        //set playing to false
        playing = false;

        countDown = 1;
        go = 0;

        //DEBUGGING
        debugging = false; // set true for diagnostics
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
                MenuSystem.DrawHighlightedString(4 - Math.Floor(countDown) + "",new Vector2(160,60), Color.Black, placeFont);

                countDown += dt;
                //  rendering handled by rendering engine     
            }
            else
            {
                if (Engine.GetKeyDown(Key.Escape))
                {
                    pause = !pause;
                }
                if (pause)
                {
                    MenuSystem.CurScreen = 7;
                    MenuSystem.UpdateMenu();
                }
                if(!pause)
                {
                    PhysicsEngine.update(Math.Min(Engine.TimeDelta, 1f / 30f));
                    RenderEngine.draw();
                }
                if (go <= goConst)
                {
                    float dt = Math.Min(Engine.TimeDelta, 1f / 30f);
                    MenuSystem.DrawHighlightedString("GO!", new Vector2(160,60), Color.Black, placeFont);
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
                //start playing game music
                Sounds.playGameMusic();
            }
        }
    }
}