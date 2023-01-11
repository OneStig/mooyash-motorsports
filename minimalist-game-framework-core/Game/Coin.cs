using System;
using Mooyash.Services;
using Mooyash.Modules;

namespace Mooyash.Modules
{
    public class Coin : GameObject
    {
        public static float angularVelo = (float)Math.PI * 2;

        public Coin(Vector2 position) : base()
        {
            this.position = position;
            texture = Engine.LoadTexture("Coin.png");
            size = new Vector2(400, 400);
            resolution = new Vector2(32, 32);
            numTex = 5;
            radius = 20f;
        }

        public void update(float dt)
        {
            angle = (angle + angularVelo * dt) % ((float)Math.PI * 2);
        }

        public override void collide(Kart k)
        {
            k.score += 1;
            //Engine.DrawString("score " + k.score, new Vector2(100, 5), Color.White, );

            PhysicsEngine.gameObjects.Remove(this);
        }
    }

}

