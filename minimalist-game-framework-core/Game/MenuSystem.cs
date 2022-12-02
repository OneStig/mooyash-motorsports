using System;
using System.Collections.Generic;

namespace Mooyash.Services
{
    public static class MainMenu
    {
        private static int frameCount = 0;
        private static Screen[] ScreenStack = new Screen[3];
        

        public static void loadTextures()
        {
            ScreenStack[0] = new Screen(Engine.LoadTexture("TitleScreen.png"), Vector2.Zero);

     
        }

        public static void updateMenu(Key key)
        {
            if(Engine.GetKeyDown(Key.W))
            {
                // go up
            }

            if(Engine.GetKeyDown(Key.S))
            {
                // go down
            }

            if (Engine.GetKeyDown(Key.Space))
            {
                // select
            }
        }

        public static Screen[] getScreens()
        {
            return ScreenStack;
        }

    }



   


    public class Screen
    {
        private Texture[] textures;
        private Vector2[] positions;
        private Vector2[] sizes;
        private Dictionary<int, string>


        public Screen(Texture texture, Vector2 position)
        {
            textures = new Texture[] { texture };
            positions = new Vector2[] { position };
        }

        public Screen(Texture[] textures, Vector2[] positions, Vector2[] sizes)
        {
            this.textures = textures;
            this.positions = positions;
            this.sizes = sizes;
        }

        public void DrawAnimation(int frameCount)
        {

            Engine.DrawTexture(textures[frameCount], positions[frameCount]);
            
        }

        public void DrawScreen()
        {
            Engine.DrawTexture(textures[0], positions[0], size:sizes[0]);
        }

    }
}

