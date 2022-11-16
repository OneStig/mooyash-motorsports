using System;

namespace Mooyash.Services
{
    public static class MenuSystem
    {
        
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

