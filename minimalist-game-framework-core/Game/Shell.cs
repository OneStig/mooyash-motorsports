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

            texture = Engine.LoadTexture("shell.png");
            size = new Vector2(37.5f, 37.5f);
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
            PhysicsEngine.gameObjects.Remove(this);
            PhysicsEngine.projectiles.Remove(this);

            k.hit();
        }
    }
}

