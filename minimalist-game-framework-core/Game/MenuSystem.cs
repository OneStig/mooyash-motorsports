using System;

namespace Mooyash.Services
{
    public static class MenuSystem
    {
        private static int frameCount = 0;
        private static Texture[] titles = new Texture[43];


        public static void loadMenuAndBackground()
        {
            for (int i = 1; i < 44; i++)
            {
                titles[i - 1] = Engine.LoadTexture("Title" + i + ".png");

            }

        }

        public static void updateMenuAndBackground()
        {
            //increment the framCount int by one and load the next frame every time this is called
            Engine.DrawTexture(titles[frameCount], new Vector2(70, 25));
            if (frameCount == 42)
            {
                frameCount = 0;
            }
            else
            {
                frameCount++;
            }

        }

        //button object, supports squares and rectangles
        public class button
        {
            //coordinates start from the top left, width determines size of the button
            int x0;
            int y0;
            int xWidth;
            int yWidth;

            public button(int x0, int y0, int xWidth, int yWidth)
            {
                this.x0 = x0;
                this.y0 = y0;
                this.xWidth = xWidth;
                this.yWidth = yWidth;
            }

            //returns whether the mouse is within the bounds of the button
            private bool withinButton()
            {
                int mouseX = (int)Engine.MousePosition.X;
                int mouseY = (int)Engine.MousePosition.Y;

                return (mouseX < (x0 + xWidth)) && (mouseY < (y0 + yWidth));
            }

            //whether the user has left clicked the button
            public bool isClickedLeft()
            {
                return withinButton() && Engine.GetMouseButtonDown(MouseButton.Left);
            }

            //whether the user has right clicked the button
            public bool isClickedRight()
            {
                return withinButton() && Engine.GetMouseButtonDown(MouseButton.Right);

            }
        }
    }
    public class Screen
    {
        public Texture[] textures;
        public Vector2[] positions;
        public Vector2[] sizes;

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
