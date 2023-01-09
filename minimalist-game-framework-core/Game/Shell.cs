using System;

namespace Mooyash.Modules
{
    public class Shell : GameObject
    {
        public float radius;

        public Shell(Vector2 position) : base()
        {
            this.position = position;

            texture = Engine.LoadTexture("banana_peel.png");
            size = new Vector2(500, 500);
            resolution = new Vector2(32, 32);

            radius = 50f;
        }

        public override void collide(Kart k)
        {
            exists = false;
            k.stunTime = 0;
        }
    }
}

