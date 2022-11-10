using System;
using System.Collections.Generic;

class Game
{
    public static readonly string Title = "Minimalist Game Framework";
    public static readonly Vector2 Resolution = new Vector2(640, 480);

    public Game()
    {
    }

    public void Update()
    {
        Polygon p = new Polygon(new int[] { 30, 200, 230, 100, -30 }, new int[] { 40, -50, 100, 150, 75 });
        Engine.DrawConvexPolygon(p, new Color(255, 255, 255));
    }
}
