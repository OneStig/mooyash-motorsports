using System;
using System.Collections.Generic;
using SDL2;
using Mooyash.Modules;
using Mooyash.Services;

class Game
{
    public static readonly string Title = "Mooyash Motorsport";
    public static readonly Vector2 Resolution = new Vector2(320, 180);

    public Dictionary<string, GameObject> gameObjects;

    bool playing; // (saves 31 bits of overhead yay)
    bool debugging;
    public float speed = 1f;
    public static IntPtr joystick;
    //int lasta = 0;
    //int lastb = 0;

    Kart player;

    public Game()
    {
        // Initialize game objects
        PhysicsEngine.init();
        player = new Kart();
        gameObjects = new Dictionary<string, GameObject>();
        gameObjects.Add("player", player);
        
        // Load textures into static member of various GameObjects

        // First mode is false (menu)
        playing = true; // SET TO FALSE LATER
        debugging = false; // set true for diagnostics
        //SDL.SDL_Init(SDL.SDL_INIT_GAMECONTROLLER);
        //joystick = SDL.SDL_GameControllerOpen(0);

        RenderEngine.camera = new Camera(new Vector2(125, -30), Math.PI/2, 25, Math.PI/2, 20);
    }

    public void Update()
    {
        if (debugging)
        {
            System.Diagnostics.Debug.WriteLine(1 / Engine.TimeDelta);
            Console.WriteLine(1 / Engine.TimeDelta);

            //int x = SDL.SDL_GameControllerGetAxis(joystick, SDL.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_LEFTX);
            //int y = SDL.SDL_GameControllerGetAxis(joystick, SDL.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_TRIGGERRIGHT);
            //int a = SDL.SDL_GameControllerGetButton(joystick, SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_A);
            //int b = SDL.SDL_GameControllerGetButton(joystick, SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_B);

            //if (Math.Abs(x) < 3000)
            //{
            //    x = 0;
            //}

            //if (a > 0 && lasta == 0)
            //{
            //    speed = Math.Max(speed - 1, 1);
            //}
            //else if (b > 0 && lastb == 0)
            //{
            //    speed = Math.Min(speed + 1, 5);
            //}

            //lasta = a;
            //lastb = b;
            //Console.WriteLine(a);

            //RenderEngine.camera.position += y / 32000f * speed * new Vector2(RenderEngine.camera.cos, RenderEngine.camera.sin);
            //RenderEngine.camera.changeAngle(x / 32000f * 0.02);

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
            player.updateInput();
            player.update(Math.Min(Engine.TimeDelta, 1f / 60f));

            RenderEngine.camera.followKart(player);
        }
        else
        {
            //  handled by menu class
        }
    }
}