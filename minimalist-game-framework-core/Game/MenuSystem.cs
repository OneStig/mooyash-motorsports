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

        private static Frame loadFrame;
        private static Stack<Frame> ScreenStack;
        




        public static Frame loadMenu()
        {


            for (int i = 1; i <= 100; i++)
            {
                loads[i - 1] = Engine.LoadTexture("Loading" + i + ".png");
                loadPositions[i - 1] = new Vector2(20, 45);
            }

            return new Frame(loads, loadPositions, null);
        }
        public static void updateMenu()
        {
            frameCount++;
            if (frameCount > 99)
            {
                frameCount = 0;
            }
        }

        public static int count()
        {
            return frameCount;
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


    public class Frame
    {
        private Texture[] textures;
        private Vector2[] positions;
        private Vector2[] sizes;
        private button[] buttons;

        public Frame(Texture[] textures, Vector2[] positions, button[] buttons)
        {
            this.textures = textures;
            this.positions = positions;
            this.buttons = buttons;
        }

        public Frame(Texture[] textures, Vector2[] positions, Vector2[] sizes, button[] buttons)
        {
            this.textures = textures;
            this.positions = positions;
            this.sizes = sizes;
            this.buttons = buttons;
        }

        public void DrawFrame(int frameCount)
        {

            Engine.DrawTexture(textures[frameCount], positions[frameCount]);
            
        }
    }
}

