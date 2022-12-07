using System;
using System.Collections.Generic;

namespace Mooyash.Services
{
    public static class MenuSystem
    {
        private static Screen[] ScreenStack = new Screen[3];

        private static int CurScreen = 0;

        public static Dictionary<string, int> SettingtoID = new Dictionary<string, int>();
        public static List<string> Settings = new List<String>();

        public static void loadTextures()
        {
            Texture menu = Engine.LoadTexture("fallengong.png");
            Texture menuNoGong = Engine.LoadTexture("fallinggong.png");

            SettingtoID["play"] = 1;
            SettingtoID["map1"] = 2;
            SettingtoID["map2"] = 3;
            SettingtoID["map3"] = 4;
            SettingtoID["replay"] = 5;
            SettingtoID["return"] = 6;

            Texture[] text0 = new Texture[] { menu };
            Vector2[] posi0 = new Vector2[] { new Vector2(0, 0)};
            Vector2[] size0 = new Vector2[] { new Vector2(320, 180)};
            Dictionary<int, Button> tons0 = new Dictionary<int, Button>();
            tons0[0] = new Button(Color.Black, new Vector2(120, 100), new Vector2(80, 30), "play");
            ScreenStack[0] = new Screen(text0,posi0,size0,tons0,0);

            Texture[] text1 = new Texture[] { menuNoGong };
            Vector2[] posi1 = new Vector2[] { new Vector2(0, 0) };
            Vector2[] size1 = new Vector2[] { new Vector2(320, 180) };
            Dictionary<int, Button> tons1 = new Dictionary<int, Button>();
            tons1[0] = new Button(Color.Black, new Vector2(22, 60), new Vector2(75, 30), "map1");
            tons1[1] = new Button(Color.Black, new Vector2(122, 60), new Vector2(75, 30), "map2");
            tons1[2] = new Button(Color.Black, new Vector2(222, 60), new Vector2(75, 30), "map3");
            
            ScreenStack[1] = new Screen(text1, posi1, size1, tons1, 0);

            Texture[] text2 = new Texture[] { menuNoGong };
            Vector2[] posi2 = new Vector2[] { new Vector2(0, 0) };
            Vector2[] size2 = new Vector2[] { new Vector2(320, 180) };
            Dictionary<int, Button> tons2 = new Dictionary<int, Button>();
            tons2[0] = new Button(Color.Black, new Vector2(76, 60), new Vector2(75, 30), "replay");
            tons2[1] = new Button(Color.Black, new Vector2(176, 60), new Vector2(75, 30), "return");
            

            ScreenStack[2] = new Screen(text2, posi2, size2, tons2, 0);
            

        }

        public static bool UpdateMenu()
        {
          
            Screen cur = ScreenStack[CurScreen];
            cur.DrawScreen();
            if (Engine.GetKeyDown(Key.W) || Engine.GetKeyDown(Key.Up) || Engine.GetKeyDown(Key.Left) || Engine.GetKeyDown(Key.A))
            {
                cur.Up();
            }
            if (Engine.GetKeyDown(Key.S) || Engine.GetKeyDown(Key.Down) || Engine.GetKeyDown(Key.Right) || Engine.GetKeyDown(Key.D))
            {
                cur.Down();
            }
            if (Engine.GetKeyDown(Key.Space))
            {
                if (!cur.Select().Equals(" "))
                {
                    Settings.Add(cur.Select());
                    CurScreen++;
                    if(CurScreen == 3)
                    {
                        Dictionary<int, Button> buttons = cur.getButton();
                        for (int i = 0; i < buttons.Count; i++)
                        {
                            if (buttons[i].Function().Equals("replay"))
                            {
                                CurScreen = 1;
                            } 
                            if (buttons[i].Function().Equals("return"))
                            {
                                CurScreen = 0;
                            }
                        }
                    }
                    if(CurScreen >= 2)
                    {
                        
                        return true; //create new way to move on
                    }

                }
            }
            return false;
        }

        public static List<int> GetSettings()
        {
            List<int> ids = new List<int>();
            foreach(string i in Settings)
            {
                 ids.Add(SettingtoID[i]);
            }
            return ids;
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

        public Screen(Texture[] textures, Vector2[] positions, Vector2[] sizes, Dictionary<int, Button> buttons, int curButton)
        {
            this.textures = textures;
            this.positions = positions;
            this.sizes = sizes;
            this.buttons = buttons;
            this.curButton = curButton;
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

        public Dictionary<int, Button> getButton()
        {
            return buttons;
        }
    }

    public class Button
    {
        private Vector2 position;
        private Vector2 size;
        private string func;
        private Color color;

        private static Font font = Engine.LoadFont("Mario-Kart-DS.ttf", 18);

        public Button(Color color, Vector2 position, Vector2 size)
        {
            this.position = position;
            this.size = size;
            this.color = color;
        }

        public Button(Color color, Vector2 position, Vector2 size, string func)
        {
            this.position = position;
            this.size = size;
            this.func = func;
            this.color = color;
        }

        public void DrawButton()
        {
            Engine.DrawRectSolid(new Bounds2(position.X, position.Y, size.X, size.Y), color);
            Engine.DrawString(func, new Vector2(position.X+size.X/2,position.Y+Game.Resolution.Y/22), Color.White, font, TextAlignment.Center);
        }

        public void DrawSelectedButton()
        {
            Engine.DrawRectSolid(new Bounds2(position.X, position.Y, size.X, size.Y), Color.AliceBlue);
            Engine.DrawRectSolid(new Bounds2(position.X+2, position.Y+2, size.X-4, size.Y-4), color);
            Engine.DrawString(func, new Vector2(position.X + size.X / 2, position.Y + Game.Resolution.Y / 22), Color.White, font, TextAlignment.Center);
        }

        public string Function()
        {
            return func;
        }
    }
}

