using System;

namespace Mooyash.Modules
{
    public class Shell : GameObject
    {
        public float radius;
        public float angle;

        private float timer;

        private Vector2 trig;

        private readonly float speed = 2000;
        private readonly float lifespan = 10;

        public Shell(Vector2 position, float angle) : base()
        {
            this.position = position;
            this.angle = angle;

            trig = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

            texture = Engine.LoadTexture("shell.png");
            size = new Vector2(300, 300);
            resolution = new Vector2(16, 16);

            radius = 50f;

            timer = 0;
        }

        public void update(float dt)
        {
            position += trig * speed * dt;
            timer += dt;

            if (timer >= lifespan)
            {
                exists = false;
            }
        }

        public override void collide(Kart k)
        {
            exists = false;
            k.stunTime = 0;
        }
    }
}

