using System;
using System.Collections.Generic;

namespace Mooyash.Services
{
    public static class MainMenu
    {
        private static int frameCount = 0;
        private static Texture[] loads = new Texture[100];
        private static Vector2[] loadPositions = new Vector2[100];
        private static Vector2[] sizes;
        private static Stack<Screen> ScreenStack;

        public static void loadTextures()
        {
            for (int i = 1; i <= 100; i++)
            {
                loads[i - 1] = Engine.LoadTexture("Loading" + i + ".png");
                loadPositions[i - 1] = new Vector2(20, 45);
            }

            ScreenStack.Push(new Screen(Engine.LoadTexture("TitleScreen.png"), Vector2.Zero, null));
            ScreenStack.Push(new Screen(loads, loadPositions, null, null));
            


            //will initialize this with actual buttons when the title screen is finished
            //button[] menuButtons = new button[1] { new button(Vector2.Zero, 320, 180) };
     
        }

        public static void updateMenu()
        {
            frameCount++;
            if (frameCount > 99)
            {
                ScreenStack.Pop();
            }
        }

        public static int count()
        {
            return frameCount;
        }

        public static Stack<Screen> getScreens()
        {
            return ScreenStack;
        }

    }



    //button object, supports squares and rectangles
    public class button
    {
        //coordinates start from the top left, width determines size of the button
        Vector2 dimensions;
        int xWidth;
        int yWidth;
        Texture texture;

        public button(Vector2 dimensions, int xWidth, int yWidth, Texture texture)
        {
            this.dimensions = dimensions;
            this.xWidth = xWidth;
            this.yWidth = yWidth;
            this.texture = texture;
        }

        public button(Vector2 dimensions, int xWidth, int yWidth)
        {
            this.dimensions = dimensions;
            this.xWidth = xWidth;
            this.yWidth = yWidth;
        }


        //draws the texture associated with the button
        public void draw()
        {
            Engine.DrawTexture(texture, dimensions);
        }

        //returns whether the mouse is within the bounds of the button
        private bool withinButton()
        {
            int mouseX = (int)Engine.MousePosition.X;
            int mouseY = (int)Engine.MousePosition.Y;

            return (mouseX < (dimensions.X + xWidth)) && (mouseY < (dimensions.Y + yWidth));
        }


        //returns whether a mouse button has been clicked
        public bool isMouseClicked(MouseButton button)
        {
            return withinButton() && Engine.GetMouseButtonDown(button);
        }

        //returns whether a key has been pressed
        public bool isClickedKey(Key key)
        {
            return withinButton() && Engine.GetKeyDown(key);
        }


    }


    public class Screen
    {
        private Texture[] textures;
        private Vector2[] positions;
        private Vector2[] sizes;
        private button[] buttons;


        public Screen(Texture texture, Vector2 position, button button)
        {
            textures = new Texture[] { texture };
            positions = new Vector2[] { position };
            buttons = new button[] { button };
        }

        public Screen(Texture[] textures, Vector2[] positions, Vector2[] sizes, button[] buttons)
        {
            this.textures = textures;
            this.positions = positions;
            this.sizes = sizes;
            this.buttons = buttons;
        }

        

        public void DrawAnimation(int frameCount)
        {

            Engine.DrawTexture(textures[frameCount], positions[frameCount]);
            
        }

        public void DrawScreen()
        {
            Engine.DrawTexture(textures[0], positions[0]);
        }

        public button[] getAllButtons()
        {
            return buttons;
        }

        public button getButton(int index)
        {
            return buttons[index];
        }
    }
}

