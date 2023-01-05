using System;

namespace Mooyash.Modules
{
    public class Banana : GameObject // Equivalent of mario kart mystery box
    {
        public float radius;

        public Banana(Vector2 position) : base()
        {
            this.position = position;

            textures = new Texture[1] { Engine.LoadTexture("banana_peel.png") };
            sizes = new Vector2[1] { new Vector2(500, 500) };

            radius = 50f;
        }

        public override void collide(Kart k)
        {
            exists = false;
            k.stunTime = 0;
        }
    }
}

