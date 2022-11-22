using System;
using System.Collections.Generic;

namespace Mooyash.Services
{
    public static class MenuSystem
    {
        private static int frameCount = 0;
        private static Texture[] loads;
        private static Vector2[,] positions;
        private static Vector2[,] sizes;
        private static Stack<Screen> ScreenStack;


        public static void loadMenuAndBackground()
        {
            /*ScreenStack = new Stack<Screen>();
            titles = new Texture[1];
            positions = new Vector2[2,3];
            sizes = new Vector2[2,3];*/
            loads = new Texture[100];
            for (int i = 1; i <= 100; i++)
            {
                loads[i - 1] = Engine.LoadTexture("Loading" + i + ".png");
            }

        }

        public static void updateMenuAndBackground()
        {

            Engine.DrawTexture(loads[frameCount], new Vector2(20, 45));
            frameCount++;
            if(frameCount > 99)
            {
                frameCount = 0;
            }








            //increment the framCount int by one and load the next frame every time this is called
            

        }

        //button object, supports squares and rectangles
        public class Button
        {
            //coordinates start from the top left, width determines size of the button
            Vector2 dimensions;
            int xWidth;
            int yWidth;

            public Button(Vector2 dimensions, int xWidth, int yWidth)
            {
                this.dimensions = dimensions;
                this.xWidth = xWidth;
                this.yWidth = yWidth;
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

            public Screen(Texture[] textures, Vector2[] positions, Vector2[] sizes)
            {

                this.textures = textures;
                this.positions = positions;
                this.sizes = sizes;
            }

            public void DrawScreen(int frame)
            {
                int i = frame % textures.Length;
                Engine.DrawTexture(textures[i], positions[i], size: sizes[i]);
            }
            
        }
    }
}
