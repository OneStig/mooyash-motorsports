using System;

namespace Mooyash.Modules
{
    public class GameObject
    {
        public Vector2 position;
        public double angle; //0 = positive x, pi/2 = positive y
        public Polygon hitbox;
        public Texture[] textures;
    }

    public class Kart : GameObject
    {
        public float velocity;
        public float acceleration;
        public float throttle;
        public float steer;
        public bool stunned;

        public void update(float dt)
        {
            float scalar = velocity * dt + 0.5f * acceleration * dt * dt;
            position += scalar * new Vector2((float) Math.Cos(angle), (float) Math.Sin(angle));
            velocity += acceleration;
        }
    }

    public class Item : GameObject
    {
        public String name;
        public Item()
        {

        }

    }
}