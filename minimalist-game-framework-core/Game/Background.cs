using System;
using Mooyash.Services;
using static System.Net.Mime.MediaTypeNames;

namespace Mooyash.Modules
{
    public class Background
    {
        public float depth;

        public virtual void draw(Camera c)
        {

        }
    }

    public class Tree : Background
    {
        public static Texture texture = Engine.LoadTexture("R.jpg");
        public static Vector2 size = new Vector2(10,20);

        public float angle;

        public Tree(float depth, float angle)
        {
            this.depth = depth;
            this.angle = angle;
        }

        public override void draw(Camera c)
        {
            float xPos = Game.Resolution.X/2 + (float)Math.Tan(angle - c.angle) * c.angleScale;
            Vector2 newSize = size * c.screen * c.scale * Game.ResolutionScale / depth;
            Engine.DrawTexture(texture, new Vector2(xPos, c.ground.Position.Y - newSize.Y), size: newSize);
        }
    }
}

