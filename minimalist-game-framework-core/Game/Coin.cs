using System;

namespace Mooyash.Modules
{
    public class Coin : GameObject
    {
        public float radius;

        public Coin(Vector2 position) : base()
        {
            this.position = position;

            texture = Engine.LoadTexture("coin.png");
            size = new Vector2(400, 400);
            resolution = new Vector2(32, 32);

            radius = 40f;


            
        }


        public override void collide(Kart k)
        {
            exists = false;
            k.score += 1;
            //Engine.DrawString("score " + k.score, new Vector2(100, 5), Color.White, );
        }
    }

}

