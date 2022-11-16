using System;

namespace Mooyash.Services
{
    public static class MenuSystem
    {
        private static int frameCount = 0;
        private static Texture title1;
        private static Texture title2;
        private static Texture title3;
        private static Texture title4;
        private static Texture title5;
        private static Texture title6;
        private static Texture title7;

        public static void loadMenuAndBackground()
        {
            title1 = Engine.LoadTexture("Title1.png");
            title2 = Engine.LoadTexture("Title2.png");
            title3 = Engine.LoadTexture("Title3.png");
            title4 = Engine.LoadTexture("Title4.png");
            title5 = Engine.LoadTexture("Title5.png");
            title6 = Engine.LoadTexture("Title6.png");
            title7 = Engine.LoadTexture("Title7.png");

        }

        public static void updateMenuAndBackground()
        {
            frameCount++;
            if (frameCount == 1)
            {
                Engine.DrawTexture(title1, new Vector2(100, 25));
            } else if (frameCount == 2)
            {
                Engine.DrawTexture(title2, new Vector2(100, 25));
            }
            else if (frameCount == 3)
            {
                Engine.DrawTexture(title3, new Vector2(100, 25));
            }
            else if (frameCount == 4)
            {
                Engine.DrawTexture(title4, new Vector2(100, 25));
            }
            else if (frameCount == 5)
            {
                Engine.DrawTexture(title5, new Vector2(100, 25));
            }
            else if (frameCount == 6)
            {
                Engine.DrawTexture(title6, new Vector2(100, 25));
            }
            else if (frameCount == 7)
            {
                Engine.DrawTexture(title7, new Vector2(100, 25));
                frameCount = 0;
            }

            //increment the framCount int by one and load the next frame every time this is called
        }
    }
}

