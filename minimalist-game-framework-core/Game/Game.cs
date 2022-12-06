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
        Track.LoadTracks();

        // Load textures into static member of various GameObjects

        //set playing to false
        playing = false;

        //DEBUGGING
        playing = true; // SET TO FALSE LATER
        debugging = true; // set true for diagnostics
        PhysicsEngine.track = Track.tracks[0]; // should be handled by menu
        RenderEngine.camera = new Camera(new Vector2(300,100), 25, Math.PI/2, 20);
    }

    public void Update()
    {
        if (debugging)
        {
            System.Diagnostics.Debug.WriteLine("LAP COUNT: " + PhysicsEngine.lapCount + "LAP DISPLAY: " + PhysicsEngine.lapDisplay + " POSITION: " + PhysicsEngine.player.position);
            System.Diagnostics.Debug.WriteLine(PhysicsEngine.GetPhysicsID(PhysicsEngine.player.position));
            //System.Diagnostics.Debug.WriteLine(PhysicsEngine.TestPointPoly(new Vector2(-0.4f,0), new PhysicsPolygon(new float[] {-1, 0, 1 }, new float[] {-1, 1, -1 }, Color.AliceBlue, 0) ));

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
            //  handled by menu class
        }
    }
}