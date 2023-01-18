using System;
using System.Collections.Generic;
using System.Linq;
using Mooyash.Modules;
namespace Mooyash.Services

{
    public static class MenuSystem
    {
        private static Screen[] ScreenStack = new Screen[9];

        public static Dictionary<string, string> displayNames;

        public static int CurScreen = 8;

        public static Dictionary<string, int> SettingtoID = new Dictionary<string, int>();
        public static List<string> Settings = new List<String>();

        public static string finTime;

        public static float alpha; // menu opacity from 0-1, 1 being opaque

        // Menu timers for animation
        public static float endTimer = float.MaxValue / 2;

        public static void loadTextures()
        {
            Texture Menu = Engine.LoadTexture("MooyashMenu.png");
            Texture CreditsScreen = Engine.LoadTexture("Credits.png");
            Texture HowToPlay = Engine.LoadTexture("ControlsMenu.png");

            displayNames = new Dictionary<string, string>() {
                { "mooyash_red","Mooyash" },
                { "gilliam_orange","Gilliam" },
                { "davis_green","Davis" },
                { "mooyash_blue", "Blueyash" },
                { "mooyash_yellow", "Suyash" }
            };
            SettingtoID["How To Play"] = 0;

            SettingtoID["Back"] = 0;
            SettingtoID["Play"] = 0;

            SettingtoID["Time Trial"] = 0;
            SettingtoID["Grand Prix"] = 1;

            SettingtoID["50CC"] = 0;
            SettingtoID["100CC"] = 1;


            SettingtoID["William"] = 0;
            SettingtoID["Suyash"] = 1;

            SettingtoID["map1"] = 0;
            SettingtoID["map2"] = 1;
            SettingtoID["map3"] = 2;

            SettingtoID["Replay"] = 0;
            SettingtoID["Return"] = 1;
            SettingtoID["Credits"] = 2;

            SettingtoID["Resume"] = 0;

            //play mode cc character map return

            //play
            Texture[] MenuTextures = new Texture[] { Menu };
            Vector2[] MenuTexturePositions = new Vector2[] { new Vector2(0, 0) };
            Vector2[] MenuTextureSizes = new Vector2[] { new Vector2(320, 180) };
            Dictionary<int, Button> MenuButtons = new Dictionary<int, Button>();
            MenuButtons[0] = new Button(Color.Black, new Vector2(220, 70), new Vector2(80, 30), "Play", Color.White);
            MenuButtons[1] = new Button(Color.Black, new Vector2(195, 105), new Vector2(110, 30), "How to Play", Color.White);
            ScreenStack[0] = new Screen(MenuTextures, MenuTexturePositions, MenuTextureSizes, MenuButtons, 0);

            //mode
            Texture[] GamemodeTextures = new Texture[] { Menu };
            Vector2[] GamemodeTexturePositions = new Vector2[] { new Vector2(0, 0) };
            Vector2[] GamemodeTextureSizes = new Vector2[] { new Vector2(320, 180) };
            Dictionary<int, Button> GamemodeButtons = new Dictionary<int, Button>();
            GamemodeButtons[0] = new Button(Color.Black, new Vector2(220, 70), new Vector2(100, 30), "Time Trial", Color.White);
            GamemodeButtons[1] = new Button(Color.Black, new Vector2(195, 105), new Vector2(100, 30), "Grand Prix", Color.White);
            GamemodeButtons[2] = new Button(Color.Black, new Vector2(160, 160), new Vector2(50, 25), "Back", Color.White);
            ScreenStack[1] = new Screen(GamemodeTextures, GamemodeTexturePositions, GamemodeTextureSizes, GamemodeButtons, 0);

            //cc
            Texture[] CCTextures = new Texture[] { Menu };
            Vector2[] CCTexturePositions = new Vector2[] { new Vector2(0, 0) };
            Vector2[] CCTextureSizes = new Vector2[] { new Vector2(320, 180) };
            Dictionary<int, Button> CCButtons = new Dictionary<int, Button>();
            CCButtons[0] = new Button(Color.Black, new Vector2(76, 60), new Vector2(75, 30), "50CC", Color.White);
            CCButtons[1] = new Button(Color.Black, new Vector2(176, 60), new Vector2(75, 30), "100CC", Color.White);
            CCButtons[2] = new Button(Color.Black, new Vector2(160, 120), new Vector2(50, 25), "Back", Color.White);
            ScreenStack[2] = new Screen(CCTextures, CCTexturePositions, CCTextureSizes, CCButtons, 0);

            //character
            Texture[] CharacterTextures = new Texture[] { Menu };
            Vector2[] CharacterTexturePosition = new Vector2[] { new Vector2(0, 0) };
            Vector2[] CharacterTextureSizes = new Vector2[] { new Vector2(320, 180) };
            Dictionary<int, Button> CharacterButtons = new Dictionary<int, Button>();
            CharacterButtons[0] = new Button(Color.Black, new Vector2(76, 60), new Vector2(75, 30), "William", Color.White);
            CharacterButtons[1] = new Button(Color.Black, new Vector2(176, 60), new Vector2(75, 30), "Suyash", Color.White);
            CharacterButtons[2] = new Button(Color.Black, new Vector2(160, 120), new Vector2(50, 25), "Back", Color.White);
            ScreenStack[3] = new Screen(CharacterTextures, CharacterTexturePosition, CharacterTextureSizes, CharacterButtons, 0);

            //map
            Texture[] MapTextures = new Texture[] { Menu };
            Vector2[] MapTexturePositions = new Vector2[] { new Vector2(0, 0) };
            Vector2[] MapTextureSizes = new Vector2[] { new Vector2(320, 180) };
            Dictionary<int, Button> MapButtons = new Dictionary<int, Button>();
            MapButtons[0] = new Button(Color.Black, new Vector2(22, 60), new Vector2(75, 30), "map1", Color.White);
            MapButtons[1] = new Button(Color.Black, new Vector2(122, 60), new Vector2(75, 30), "map2", Color.White);
            MapButtons[2] = new Button(Color.Black, new Vector2(222, 60), new Vector2(75, 30), "map3", Color.White);
            MapButtons[3] = new Button(Color.Black, new Vector2(160, 120), new Vector2(50, 25), "Back", Color.White);

            ScreenStack[4] = new Screen(MapTextures, MapTexturePositions, MapTextureSizes, MapButtons, 0);

            //return
            Texture[] EndTextures = new Texture[] { };
            Vector2[] EndTexturePositions = new Vector2[] { };
            Vector2[] EndTextureSizes = new Vector2[] { };
            Dictionary<int, Button> EndButtons = new Dictionary<int, Button>();
            EndButtons[0] = new Button(Color.Black, new Vector2(15.5f, 150), new Vector2(75, 24), "Replay", Color.White);
            EndButtons[1] = new Button(Color.Black, new Vector2(122, 150), new Vector2(75, 24), "Return", Color.White);
            EndButtons[2] = new Button(Color.Black, new Vector2(228, 150), new Vector2(75, 24), "Credits", Color.White);

            ScreenStack[5] = new Screen(EndTextures, EndTexturePositions, EndTextureSizes, EndButtons, 0);

            //credits
            Texture[] CreditTextures = new Texture[] { CreditsScreen };
            Vector2[] CreditTexturePositions = new Vector2[] { new Vector2(0, 0) };
            Vector2[] CreditTextureSizes = new Vector2[] { new Vector2(320, 180) };
            Dictionary<int, Button> CreditButtons = new Dictionary<int, Button>();
            CreditButtons[0] = new Button(Color.Black, new Vector2(76, 140), new Vector2(60, 30), "Replay", Color.White);
            CreditButtons[1] = new Button(Color.Black, new Vector2(176, 140), new Vector2(60, 30), "Return", Color.White);


            ScreenStack[6] = new Screen(CreditTextures, CreditTexturePositions, CreditTextureSizes, CreditButtons, 0);


            Texture[] PauseTextures = new Texture[] {  };
            Vector2[] PauseTexturePositions = new Vector2[] { new Vector2(0, 0) };
            Vector2[] PauseTextureSizes = new Vector2[] { new Vector2(320, 180) };
            Dictionary<int, Button> PauseButtons = new Dictionary<int, Button>();
            PauseButtons[0] = new Button(Color.Black, new Vector2(76, 120), new Vector2(75, 30), "Resume", Color.White);
            PauseButtons[1] = new Button(Color.Black, new Vector2(176, 120), new Vector2(75, 30), "Return", Color.White);

            ScreenStack[7] = new Screen(PauseTextures, PauseTexturePositions, PauseTextureSizes, PauseButtons, 0);


            Texture[] HTPTextures = new Texture[] { HowToPlay };
            Vector2[] HTPPositions = new Vector2[] { new Vector2(0, 0) };
            Vector2[] HTPSizes = new Vector2[] { new Vector2(320, 180) };
            Dictionary<int, Button> HTPButtons = new Dictionary<int, Button>();
            HTPButtons[0] = new Button(Color.Black, new Vector2(76, 120), new Vector2(75, 30), "Continue", Color.White);

            ScreenStack[8] = new Screen(HTPTextures, HTPPositions, HTPSizes, HTPButtons, 0);


        }

        public static string timeToString(float time)
        {
            if (time == float.MaxValue)
            {
                return "---";
            }
            if(time == 0)
            {
                return "00.00.00";
            }

            String timer = "0" + (int)time / 60 + "." + time % 60 + "00.00.00.00";
            if (time / 60 > 9)
            {
                timer = (int)time / 60 + "." + time % 60 + "00.00.00.00";
            }
            if (time % 60 < 10)
            {
                timer = "0" + (int)time / 60 + ".0" + time % 60 + "00.00.00.00";
                if (time / 60 > 9)
                {
                    timer = (int)time / 60 + ".0" + time % 60 + "00.00.00.00";
                }
            }
            timer = timer.Substring(0, 8);
            return timer;
        }

        public static void DrawHighlightedString(string s, Vector2 position, Color c, Font f)
        {
            Engine.DrawString(s, (position + new Vector2(-1, +1)) * Game.ResolutionScale, c, f, TextAlignment.Center);
            Engine.DrawString(s, (position + new Vector2(-1, -1)) * Game.ResolutionScale, c, f, TextAlignment.Center);
            Engine.DrawString(s, (position + new Vector2(1, -1)) * Game.ResolutionScale, c, f, TextAlignment.Center);
            Engine.DrawString(s, (position + new Vector2(1, 1)) * Game.ResolutionScale, c, f, TextAlignment.Center);
            Engine.DrawString(s, position * Game.ResolutionScale, Color.White, f, TextAlignment.Center);
        }

        public static bool UpdateMenu()
        {
            alpha = 1;

            //Update timers
            endTimer += Engine.TimeDelta;

            Screen cur = ScreenStack[CurScreen];

            if (CurScreen == 7)
            {
                RenderEngine.draw();
            }

            if (CurScreen != 5)
            {
                cur.DrawScreen();
            }
            

            if (CurScreen == 5)
            {
                PhysicsEngine.update(Math.Min(Engine.TimeDelta, 1f / 30f));
                RenderEngine.draw();

                if (endTimer > 0.5f)
                {
                    alpha = Math.Min((endTimer - 0.5f) / 0.6f, 1f);

                    cur.DrawScreen();

                    Engine.DrawRectSolid(new Bounds2(95 * Game.ResolutionScale, 25 * Game.ResolutionScale, 130 * Game.ResolutionScale, 115 * Game.ResolutionScale), Color.Black * alpha);

                    if (GetSettings()[1] == 0)
                    {
                        Engine.DrawString("Time: " + finTime, new Vector2(163, 33) * Game.ResolutionScale, Color.Yellow * alpha, Game.font, TextAlignment.Center);
                        Engine.DrawString("Leaderboard", new Vector2(163,48) * Game.ResolutionScale, Color.White * alpha, Game.font, TextAlignment.Center);
                        List<float> scores = LeaderboardLoader.getScores(Game.GameSettings[4], Game.GameSettings[2]);
                        for(int i = 0; i < scores.Count; i++)
                        {
                            Engine.DrawString(RenderEngine.toPlace(i + 1) + ":", new Vector2(100, 63 + 15 * i) * Game.ResolutionScale,
                                (finTime.Equals(timeToString(scores[i])) ? Color.Yellow : Color.White) * alpha, Game.font, TextAlignment.Left);
                            Engine.DrawString(timeToString(scores[i]), new Vector2(220, 63+15*i) * Game.ResolutionScale,
                                (finTime.Equals(timeToString(scores[i])) ? Color.Yellow : Color.White) * alpha, Game.font, TextAlignment.Right);
                        }
                    }
                    else
                    {
                        Kart player = PhysicsEngine.player;

                        List<Kart> kartList = PhysicsEngine.karts.ToList();
                        kartList.Sort(PhysicsEngine.ComparePosition);

                        for (int i = 0; i < kartList.Count; i++)
                        {
                            Color txtColor = Color.White * alpha;

                            if (kartList[i] == player)
                            {
                                txtColor = Color.Yellow * alpha;
                            }

                            Engine.DrawString(displayNames[kartList[i].selfId], new Vector2(100, 31 + 15 * i) * Game.ResolutionScale, txtColor, Game.font);

                            Engine.DrawString(timeToString(kartList[i].finTime), new Vector2(220, 31 + 15 * i) * Game.ResolutionScale, txtColor, Game.font, TextAlignment.Right);
                        }

                        Engine.DrawString("Score: " + PhysicsEngine.player.score, new Vector2(100, 123) * Game.ResolutionScale, Color.White * alpha, Game.font);
                    }
                }
            }

            if (Engine.GetKeyDown(Key.W) || Engine.GetKeyDown(Key.Up) || Engine.GetKeyDown(Key.Left) || Engine.GetKeyDown(Key.A))
            {
                cur.Up();
            }
            if (Engine.GetKeyDown(Key.S) || Engine.GetKeyDown(Key.Down) || Engine.GetKeyDown(Key.Right) || Engine.GetKeyDown(Key.D))
            {
                cur.Down();
            }
            if (Engine.GetKeyDown(Key.Space) || Engine.GetKeyDown(Key.Return))
            {
                if(CurScreen == 0)
                {
                    string select = cur.Select();
                    if(select.Equals("How to Play"))
                    {
                        CurScreen = 8;
                    }
                    else
                    {
                        Settings.Add(cur.Select());
                        CurScreen++;
                        ScreenStack[CurScreen].curButton = 0;
                    }
                }
                else if (CurScreen == 8)
                {
                    CurScreen = 0;
                }
                else if(CurScreen == 7)
                {
                    string select = cur.Select();
                    if (select.Equals("Resume"))
                    {
                        CurScreen = 5;
                        Game.pause = false;
                    }
                    if (select.Equals("Return"))
                    {
                        Settings.Clear();
                        CurScreen = 0;
                        Settings.Clear();
                        Game.pause = false;
                        Sounds.playMenuMusic();
                        Game.playing = false;
                        Engine.StopSound(PhysicsEngine.player.rev, fadeTime: 0.2f);
                        if (PhysicsEngine.player.terrain != null)
                        {
                            Engine.StopSound(PhysicsEngine.player.terrain, fadeTime: 0.2f);
                        }
                        Game.countDown = 1;
                    }
                }
                else if (CurScreen >= 5)
                {
                    String select = cur.Select();
                    if (select.Equals("Replay"))
                    {
                        CurScreen = 4;
                        Settings.RemoveAt(Settings.Count - 1);
                    }
                    if (select.Equals("Return"))
                    {
                        Settings.Clear();
                        CurScreen = 0;
                        Settings.Clear();
                    }
                    if (select.Equals("Credits"))
                    {
                        CurScreen = 6;
                        Engine.StopSound(PhysicsEngine.player.rev, fadeTime: 0.2f);
                        if (PhysicsEngine.player.terrain != null)
                        {
                            Engine.StopSound(PhysicsEngine.player.terrain, fadeTime: 0.2f);
                        }
                    }
                    Game.go = 0;
                }
                else if (CurScreen < 5 && !cur.Select().Equals("Back"))
                {
                    Settings.Add(cur.Select());
                    CurScreen++;
                    ScreenStack[CurScreen].curButton = 0;
                }
                else
                {
                    CurScreen--;
                    if (CurScreen == 8)
                    {
                        CurScreen = 0;
                    }
                    else if (Settings.Count > 0)
                    {
                        Settings.RemoveAt(Settings.Count - 1);
                    }
                    ScreenStack[CurScreen].curButton = 0;
                }

                if (CurScreen == 5)
                {
                    for (int i = 0; i < ScreenStack.Length; i++)
                    {
                        ScreenStack[i].resetSelected();
                    }
                    return true; //create new way to move on
                }
            }
            if (Engine.GetKeyDown(Key.Escape) && CurScreen > 0)
            {
                if (CurScreen == 8)
                {
                    CurScreen = 0;
                }
                else if(Settings.Count > 0)
                {
                    CurScreen--;
                    Settings.RemoveAt(Settings.Count - 1);
                }
                ScreenStack[CurScreen].curButton = 0;
            }
            return false;
        }

        public static List<int> GetSettings()
        {
            List<int> ids = new List<int>();
            foreach (string i in Settings)
            {
                ids.Add(SettingtoID[i]);
            }
            return ids;
        }

        public static void SetFinalTime(float finalTime)
        {
            float time = finalTime;
            String timer = "0" + (int)time / 60 + "." + time % 60 + "00000000";
            if (time / 60 > 9)
            {
                timer = (int)time / 60 + "." + time % 60 + "00000000";
            }
            if (time % 60 < 10)
            {
                timer = "0" + (int)time / 60 + ".0" + time % 60 + "00000000";
                if (time / 60 > 9)
                {
                    timer = (int)time / 60 + ".0" + time % 60 + "00000000";
                }
            }
            timer = timer.Substring(0, 8);

            finTime = timer;
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
        public int curButton = -1;


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
            for (int i = 0; i < textures.Length; i++)
            {
                Engine.DrawTexture(textures[i], positions[i] * Game.ResolutionScale, size: sizes[i] * Game.ResolutionScale, scaleMode: TextureScaleMode.Nearest, color: Color.White * MenuSystem.alpha);
            }

            for (int i = 0; i < buttons.Count; i++)
            {
                if (i == curButton)
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
            if (curButton > 0)
            {
                curButton--;
            }
            if (curButton == -1)
            {
                curButton = 0;
            }
        }

        public void Down()
        {
            if (curButton == buttons.Count - 1)
            {
                return;
            }
            if (curButton < buttons.Count - 1)
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
            if (curButton != -1)
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

        private static Font font = Game.font;

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
            //Engine.DrawRectEmpty(new Bounds2(position * Game.ResolutionScale, size * Game.ResolutionScale), color * MenuSystem.alpha);

            Engine.DrawString(func, new Vector2(position.X + size.X / 2, position.Y + size.Y / 2 - 6) * Game.ResolutionScale, fontColor * MenuSystem.alpha, font, TextAlignment.Center);

        }

        public void DrawSelectedButton()
        {
            Engine.DrawRectEmpty(new Bounds2(position * Game.ResolutionScale, size * Game.ResolutionScale), Color.AliceBlue * MenuSystem.alpha);
            Engine.DrawRectSolid(new Bounds2((position + new Vector2(2f, 2f)) * Game.ResolutionScale, (size + new Vector2(-4f, -4f)) * Game.ResolutionScale), color * MenuSystem.alpha);
            Engine.DrawString(func, new Vector2(position.X + size.X / 2, position.Y + size.Y / 2 - 6) * Game.ResolutionScale, fontColor * MenuSystem.alpha, font, TextAlignment.Center);

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

