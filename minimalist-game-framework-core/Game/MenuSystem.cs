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
            if(frameCount == 42)
            {
                frameCount = 0;
            } else
            {
                frameCount++;
            }
            
        }

    //button object, supports squares and rectangles
    public class button
    {
        //coordinates start from the top left, width determines size of the button
        Vector2 dimensions;
        int xWidth;
        int yWidth;

        public button(Vector2 dimensions, int xWidth, int yWidth)
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
}
