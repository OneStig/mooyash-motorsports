using System;
using Mooyash.Services;

namespace Mooyash.Modules
{
    public class Banana : GameObject
    {
        public Banana(Vector2 position) : base()
        {
            this.position = position;

            texture = Engine.LoadTexture("banana_peel.png");
            size = new Vector2(62.5f, 62.5f);
            resolution = new Vector2(32, 32);

            radius = 30f;
        }

        public override void collide(Kart k)
        {
            if (k.coins < 2)
            {
                k.coins = 0;
            } else
            {
                k.coins -= 2;
            }

            if(!k.isLarge)
            {
                k.hit();
            }
            

            PhysicsEngine.gameObjects.Remove(this);
        }
    }
}

