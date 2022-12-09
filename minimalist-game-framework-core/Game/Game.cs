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
    public static Font font = Engine.LoadFont("Mario-Kart-DS.ttf", 17);

    public static bool debugging;


    public static bool playing; // (saves 31 bits of overhead yay)

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
            //System.Diagnostics.Debug.WriteLine(PhysicsEngine.TestCircleLine(
                //new CirclePath(new Vector2(4946.457f, 106.707f), new Vector2(4951.442f, 109.4627f), 50),
                //new Vector2(5000,-5000), new Vector2(5000,5000)));
            System.Diagnostics.Debug.WriteLine("POSITION: " + PhysicsEngine.player.position + " FPS: " + 1 / Engine.TimeDelta);
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
            RenderEngine.drawPlayer();
            RenderEngine.drawObjects();
            RenderEngine.drawUI();

            //if (Engine.GetKeyDown(Key.Escape))
            //{
            //    playing = false;
            //    time = 0;
            //}
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