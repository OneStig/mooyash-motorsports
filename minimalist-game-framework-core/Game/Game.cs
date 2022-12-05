using System;
using System.Collections.Generic;
using Mooyash.Modules;
using Mooyash.Services;

class Game
{
    public static readonly string Title = "Mooyash Motorsport";
    public static readonly Vector2 Resolution = new Vector2(320, 180);

    public static List<int> GameSettings;
    private static Font font = Engine.LoadFont("Mario-Kart-DS.ttf", 10);

    public Dictionary<string, GameObject> gameObjects;
    public string[] allObjects;

    bool playing; // (saves 31 bits of overhead yay)

    public Game()
    {
        // Initialize game objects
        // Load textures into static member of various GameObjects
        MenuSystem.loadTextures();
        
        
        
        // First mode is false (menu)
        playing = false;
    }

    public void Update()
    {
        // Engine.DrawRectSolid(new Bounds2(0, 0, 50, 50), Color.Aqua);
        if (playing)
        {
            //  input handling
            //  physics handled by physics engine
            //  rendering handled by rendering engine

            foreach(int i in GameSettings)
            {
                Engine.DrawString(i + "", new Vector2(10, 10 + i * 20), Color.AliceBlue, font);
            }

        }
        else
        {
            bool temp = MenuSystem.UpdateMenu();
            if (temp)
            {
                playing = true;
                GameSettings = MenuSystem.GetSettings();
            }
        }
    }
}
