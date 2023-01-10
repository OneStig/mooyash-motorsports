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
            numTex = 1;
            radius = 40f;
        }


        public void update()
        {
            
           
        }


        public override void collide(Kart k)
        {
            if(RenderEngine.score == 0)
            {
                k.score = 0;
            }
            k.score += 1;
            
            RenderEngine.score = k.score;
            
            PhysicsEngine.gameObjects.Remove(this);
        }
    }

}

