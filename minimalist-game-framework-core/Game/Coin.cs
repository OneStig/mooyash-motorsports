using System;
using Mooyash.Services;
using Mooyash.Modules;

namespace Mooyash.Modules
{
    public class Coin : GameObject
    {
        public Coin(Vector2 position) : base()
        {
            this.position = position;
            texture = Engine.LoadTexture("temp.png");
            size = new Vector2(400, 400);
            resolution = new Vector2(32, 32);

            radius = 40f;
        }


        public void update(float dt)
        {

        }


        public override void collide(Kart k)
        {
            k.score += 1;
            //Engine.DrawString("score " + k.score, new Vector2(100, 5), Color.White, );

            PhysicsEngine.gameObjects.Remove(this);
        }
    }

}

