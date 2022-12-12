using System;
using System.Collections.Generic;

namespace Mooyash.Services
{
    public static class MenuSystem
    {
        private static Screen[] ScreenStack = new Screen[7];

        private static int CurScreen = 0;

        public static Dictionary<string, int> SettingtoID = new Dictionary<string, int>();
        public static List<string> Settings = new List<String>();

        public static void loadTextures()
        {
            Texture menu = Engine.LoadTexture("fallengong.png");
            Texture menuNoGong = Engine.LoadTexture("fallinggong.png");

            SettingtoID["play"] = 0;

            SettingtoID["timetrial"] = 0;
            SettingtoID["grandprix"] = 1;

            SettingtoID["50cc"] = 0;
            SettingtoID["100cc"] = 1;

            SettingtoID["william"] = 0;
            SettingtoID["suyash"] = 1;

            SettingtoID["map1"] = 0;
            SettingtoID["map2"] = 1;
            SettingtoID["map3"] = 2;

            SettingtoID["replay"] = 0;
            SettingtoID["return"] = 1;
            SettingtoID["credits"] = 2;

            //play mode cc character map return

            //play
            Texture[] text0 = new Texture[] { menu };
            Vector2[] posi0 = new Vector2[] { new Vector2(0, 0)};
            Vector2[] size0 = new Vector2[] { new Vector2(320, 180)};
            Dictionary<int, Button> tons0 = new Dictionary<int, Button>();
            tons0[0] = new Button(Color.Black, new Vector2(120, 90), new Vector2(80, 30), "play", Color.White);
            ScreenStack[0] = new Screen(text0, posi0, size0, tons0, 0);

            //mode
            Texture[] text1 = new Texture[] { menuNoGong };
            Vector2[] posi1 = new Vector2[] { new Vector2(0, 0) };
            Vector2[] size1 = new Vector2[] { new Vector2(320, 180) };
            Dictionary<int, Button> tons1 = new Dictionary<int, Button>();
            tons1[0] = new Button(Color.Black, new Vector2(45, 60), new Vector2(100, 30), "timetrial", Color.White);
            tons1[1] = new Button(Color.Black, new Vector2(175, 60), new Vector2(100, 30), "grandprix", Color.White);
            ScreenStack[1] = new Screen(text1, posi1, size1, tons1, 0);

            //cc
            Texture[] text2 = new Texture[] { menuNoGong };
            Vector2[] posi2 = new Vector2[] { new Vector2(0, 0) };
            Vector2[] size2 = new Vector2[] { new Vector2(320, 180) };
            Dictionary<int, Button> tons2 = new Dictionary<int, Button>();
            tons2[0] = new Button(Color.Black, new Vector2(76, 60), new Vector2(75, 30), "50cc", Color.White);
            tons2[1] = new Button(Color.Black, new Vector2(176, 60), new Vector2(75, 30), "100cc", Color.White);
            ScreenStack[2] = new Screen(text2, posi2, size2, tons2, 0);

            //character
            Texture[] text3 = new Texture[] { menuNoGong };
            Vector2[] posi3 = new Vector2[] { new Vector2(0, 0) };
            Vector2[] size3 = new Vector2[] { new Vector2(320, 180) };
            Dictionary<int, Button> tons3 = new Dictionary<int, Button>();
            tons3[0] = new Button(Color.Black, new Vector2(76, 60), new Vector2(75, 30), "william", Color.White);
            tons3[1] = new Button(Color.Black, new Vector2(176, 60), new Vector2(75, 30), "suyash", Color.White);
            ScreenStack[3] = new Screen(text3, posi3, size3, tons3, 0);

            //map
            Texture[] text4 = new Texture[] { menuNoGong };
            Vector2[] posi4 = new Vector2[] { new Vector2(0, 0) };
            Vector2[] size4 = new Vector2[] { new Vector2(320, 180) };
            Dictionary<int, Button> tons4 = new Dictionary<int, Button>();
            tons4[0] = new Button(Color.Black, new Vector2(22, 60), new Vector2(75, 30), "map1", Color.White);
            tons4[1] = new Button(Color.Black, new Vector2(122, 60), new Vector2(75, 30), "map2", Color.White);
            tons4[2] = new Button(Color.Black, new Vector2(222, 60), new Vector2(75, 30), "map3", Color.White);
            
            ScreenStack[4] = new Screen(text4, posi4, size4, tons4, 0);

            //return
            Texture[] text5 = new Texture[] { menuNoGong };
            Vector2[] posi5 = new Vector2[] { new Vector2(0, 0) };
            Vector2[] size5 = new Vector2[] { new Vector2(320, 180) };
            Dictionary<int, Button> tons5 = new Dictionary<int, Button>();
            tons5[0] = new Button(Color.Black, new Vector2(76, 60), new Vector2(75, 30), "replay", Color.White);
            tons5[1] = new Button(Color.Black, new Vector2(176, 60), new Vector2(75, 30), "return", Color.White);
            tons5[2] = new Button(Color.Black, new Vector2(125, 100), new Vector2(75, 30), "credits", Color.White);
                //score
                tons5[3] = new Button(new Color(0,0,0,0), new Vector2(152.5f, 30), new Vector2(20, 20), "", Color.Black);
            
            ScreenStack[5] = new Screen(text5, posi5, size5, tons5, 0);

            //credits
            Texture[] text6 = new Texture[] { menuNoGong };
            Vector2[] posi6 = new Vector2[] { new Vector2(0, 0) };
            Vector2[] size6 = new Vector2[] { new Vector2(320, 180) };
            Dictionary<int, Button> tons6 = new Dictionary<int, Button>();
            tons6[0] = new Button(Color.Black, new Vector2(76, 140), new Vector2(75, 30), "replay", Color.White);
            tons6[1] = new Button(Color.Black, new Vector2(176, 140), new Vector2(75, 30), "return", Color.White);

            tons6[2] = new Button(new Color(0, 0, 0, 0), new Vector2(175,10), new Vector2(1,1), "attribution - mario kart 1992", Color.Black);
            tons6[3] = new Button(new Color(0, 0, 0, 0), new Vector2(175, 35), new Vector2(1, 1), "created by - steven h sebastian k ", Color.Black);
            tons6[4] = new Button(new Color(0, 0, 0, 0), new Vector2(175, 60), new Vector2(1, 1), "william g suyash m and davis y", Color.Black);
            tons6[5] = new Button(new Color(0, 0, 0, 0), new Vector2(175, 85), new Vector2(1, 1), "special thanks to - andrew martz", Color.Black);
            tons6[6] = new Button(new Color(0, 0, 0, 0), new Vector2(175, 110), new Vector2(1, 1), "and mrs. kankelborg", Color.Black);

            ScreenStack[6] = new Screen(text6, posi6, size6, tons6, 0);


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
            if (Engine.GetKeyDown(Key.Return))
            {
                if(CurScreen < 5)
                {
                    Settings.Add(cur.Select());
                }

                CurScreen++;

                if (CurScreen >= 5)
                {
                    String select = cur.Select();
                    if (select.Equals("replay"))
                    {
                        CurScreen = 4;
                    }
                    if (select.Equals("return"))
                    {
                        Settings.Clear();
                        CurScreen = 0;
                    }
                    if (select.Equals("credits"))
                    {
                        CurScreen = 6;
                    }
                }

                if(CurScreen == 5)
                {
                    for(int i = 0; i < ScreenStack.Length; i++)
                    {
                        ScreenStack[i].resetSelected();
                    }
                    return true; //create new way to move on
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

        public static void SetFinalTime(float finalTime)
        {
            float time = finalTime;
            String timer = "0" + (int)time / 60 + "." + time % 60 + "000";
            if (time % 60 < 10)
            {
                timer = "0" + (int)time / 60 + ".0" + time % 60 + "000";
            }
            timer = timer.Substring(0, 8);
            Dictionary<int, Button> buttons = ScreenStack[CurScreen].getButton();
            buttons[buttons.Count - 1].Function(timer);
        }

        public static bool SettingsContain(string key)
        {
            return SettingtoID.ContainsKey(key);
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
            if(curButton == buttons.Count-2 && buttons.Count == 4)
            {
                return;
            }
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

        public void resetSelected()
        {
            curButton = 0;
        }
    }

    public class Button
    {
        private Vector2 position;
        private Vector2 size;
        private string func;
        private Color color;
        private Color fontColor;

        private static Font font = Engine.LoadFont("Mario-Kart-DS.ttf", 18);

        public Button(Color color, Vector2 position, Vector2 size, string func, Color fontColor)
        {
            this.position = position;
            this.size = size;
            this.func = func;
            this.color = color;
            this.fontColor = fontColor;
        }

        public void DrawButton()
        {
            Engine.DrawRectSolid(new Bounds2(position.X, position.Y, size.X, size.Y), color);
            Engine.DrawString(func, new Vector2(position.X+size.X/2,position.Y+Game.Resolution.Y/22), fontColor, font, TextAlignment.Center);
        }

        public void DrawSelectedButton()
        {
            Engine.DrawRectSolid(new Bounds2(position.X, position.Y, size.X, size.Y), Color.AliceBlue);
            Engine.DrawRectSolid(new Bounds2(position.X+2, position.Y+2, size.X-4, size.Y-4), color);
            Engine.DrawString(func, new Vector2(position.X + size.X / 2, position.Y + Game.Resolution.Y / 22), fontColor, font, TextAlignment.Center);
        }

        public string Function()
        {
            return func;
        }

        public void Function(string s)
        {
            func = s;
        }
    }
}

