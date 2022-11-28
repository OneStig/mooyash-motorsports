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

    bool playing; // (saves 31 bits of overhead yay)

    public Game()
    {
        // Initialize game objects
        // Load textures into static member of various GameObjects
        MainMenu.loadTextures();
        
        
        
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
        }
        else
        {
            //in the main menu, so check for user input and change the playing bool
            if (MainMenu.count() > 99)
            {
                MainMenu.getScreens().Peek().DrawAnimation(MainMenu.count());
            } else
            {
                MainMenu.getScreens().Peek().DrawScreen();
            }
            
            //temporary: if(ScreenStack.Peek().getButton(0).isMouseClicked() || ScreenStack.Peek().getButton(0).isClickedKey())
            //if this is true, set bool to true
            if (MainMenu.getScreens().Peek().getButton(0).isMouseClicked(MouseButton.Left))
            { 
                playing  = true;
            }
            MainMenu.updateMenu();
        }
    }
}
