using System;
using Mooyash.Services;
using Mooyash.Modules;

namespace Mooyash.Modules
{
    public class Shell : Projectile
    {
        private float timer;

        private readonly float lifespan = 10;

        public Shell(Vector2 position, float angle) : base()
        {
            this.position = position;
            this.angle = angle;

            texture = Engine.LoadTexture("hay_bale.png");
            size = new Vector2(42f, 42f);
            resolution = new Vector2(16, 16);

            radius = 30f;
            timer = 0;

            velocity = new Vector2(2000, 0);
        }

        public override void update(float dt)
        {
            base.update(dt);

            timer += dt;

            if (timer >= lifespan)
            {
                PhysicsEngine.gameObjects.Remove(this);
                PhysicsEngine.projectiles.Remove(this);
            }
        }

        public override void collide(Kart k)
        {
            if (k.coins < 2)
            {
                k.coins = 0;
            }
            else
            {
                k.coins -= 2;
            }

            if(!k.isLarge)
            {
                k.hit();
            }
            

            PhysicsEngine.gameObjects.Remove(this);
            PhysicsEngine.projectiles.Remove(this);
        }
    }
}

