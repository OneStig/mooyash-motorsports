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
            size = new Vector2(500, 500);
            resolution = new Vector2(32, 32);

            radius = 30f;
        }

        public override void collide(Kart k)
        {
            if(k.score < 2)
            {
                k.score = 0;
            } else
            {
                k.score -= 2;
            }

            k.stunTime = 0;

            PhysicsEngine.gameObjects.Remove(this);
        }
    }
}

