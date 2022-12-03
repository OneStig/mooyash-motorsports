using System;
using System.Collections.Generic;

namespace Mooyash.Services
{
    public static class MenuSystem
    {
        private static Screen[] ScreenStack = new Screen[3];
        private static int CurScreen = 0;
        private static Dictionary<string, int> SettingtoID;
        private static List<string> Settings = new List<String>();

        private static Font font = Engine.LoadFont("Mario-Kart-DS.ttf", 10);

        public static void loadTextures()
        {
            Texture test = Engine.LoadTexture("R.jpg");
            Texture menu = Engine.LoadTexture("TitleScreen.png");
                


            Texture[] text0 = new Texture[] { menu };
            Vector2[] posi0 = new Vector2[] { new Vector2(0, 0)};
            Vector2[] size0 = new Vector2[] { new Vector2(50, 50)};
            Dictionary<int, Button> tons0 = new Dictionary<int, Button>();
            tons0[0] = new Button(test, new Vector2(120, 10), new Vector2(60, 30), "Hello");
            tons0[1] = new Button(test, new Vector2(120, 60), new Vector2(60, 30), "World");
            tons0[2] = new Button(test, new Vector2(120, 120), new Vector2(60, 30), "!");

            Texture[] text1 = new Texture[] { test };
            Vector2[] posi1 = new Vector2[] { new Vector2(0, 0) };
            Vector2[] size1 = new Vector2[] { new Vector2(50, 50) };
            Dictionary<int, Button> tons1 = new Dictionary<int, Button>();
            tons1[0] = new Button(menu, new Vector2(120, 10), new Vector2(60, 30), "Hello");
            tons1[1] = new Button(menu, new Vector2(120, 60), new Vector2(60, 30), "World!");
            tons1[2] = new Button(menu, new Vector2(120, 120), new Vector2(60, 30), "!");

            ScreenStack[0] = new Screen(text0,posi0,size0,tons0);
            ScreenStack[1] = new Screen(text1, posi1, size1, tons1);
            ScreenStack[2] = ScreenStack[0];
 
        }

        public static bool UpdateMenu()
        {
            Screen cur = ScreenStack[CurScreen];
            cur.DrawScreen();
            if (Engine.GetKeyDown(Key.W))
            {
                cur.Up();
            }
            if (Engine.GetKeyDown(Key.S))
            {
                cur.Down();
            }
            if (Engine.GetKeyDown(Key.Space))
            {
                if (!cur.Select().Equals(" "))
                {
                    Settings.Add(cur.Select());
                    CurScreen++;
                    if(CurScreen >= ScreenStack.Length)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

    }



   


    public class Screen
    {
        private Texture[] textures;
        private Vector2[] positions;
        private Vector2[] sizes;
        private Dictionary<int, Button> buttons;
        private int curButton = -1;


        public Screen(Texture texture, Vector2 position)
        {
            textures = new Texture[] { texture };
            positions = new Vector2[] { position };
        }

        public Screen(Texture[] textures, Vector2[] positions, Vector2[] sizes, Dictionary<int, Button> buttons)
        {
            this.textures = textures;
            this.positions = positions;
            this.sizes = sizes;
            this.buttons = buttons;
        }

        public void DrawScreen()
        {
            for(int i = 0; i < textures.Length; i++)
            {
                Engine.DrawTexture(textures[i], positions[i], size: sizes[i]);
            }

            for(int i = 0; i < buttons.Count; i++)
            {
                if(i == curButton)
                {
                    buttons[i].DrawSelectedButton();
                }
                else
                {
                    buttons[i].DrawButton();
                }
            }
        }

        public void Up()
        {
            if(curButton > 0)
            {
                curButton--;
            }
            if(curButton == -1)
            {
                curButton = 0;
            }
        }

        public void Down()
        {
            if(curButton < buttons.Count-1)
            {
                curButton++;
            }
            if (curButton == -1)
            {
                curButton = 0;
            }
        }

        public string Select()
        {
            if(curButton != -1)
            {
                return buttons[curButton].Function();
            }
            return " ";
        }

    }

    public class Button
    {
        private Texture texture;
        private Vector2 position;
        private Vector2 size;
        private string func;

        public Button(Texture texture, Vector2 position, Vector2 size)
        {
            this.texture = texture;
            this.position = position;
            this.size = size;
        }

        public Button(Texture texture, Vector2 position, Vector2 size, string func)
        {
            this.texture = texture;
            this.position = position;
            this.size = size;
            this.func = func;
        }

        public void DrawButton()
        {
            Engine.DrawTexture(texture, position, size: this.size);
        }

        public void DrawSelectedButton()
        {
            Engine.DrawRectSolid(new Bounds2(position.X-5, position.Y-5, size.X+10, size.Y+10), Color.AliceBlue);
            DrawButton();
        }

        public string Function()
        {
            return func;
        }
    }
}

