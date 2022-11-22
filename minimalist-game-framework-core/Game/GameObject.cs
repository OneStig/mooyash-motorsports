using System;
using Mooyash.Services;

namespace Mooyash.Modules
{
    public class GameObject
    {
        public Vector2 position;
        public double angle; //0 = positive x, pi/2 = positive y
        public Polygon hitbox;
        public Texture[] textures;

        public GameObject()
        {
            position = new Vector2();
            angle = 0;
            // need to add hitbox and textures later
        }
    }

    public class Kart : GameObject
    {
        public float velocity;
        public float acceleration;
        public float throttle;
        public float steer;
        public bool stunned;

        private readonly float throttleConst = 500;
        private readonly float steerConst = 3;
        private readonly float dragConst = 1;
        private readonly float naturalDecel = 1;

        private readonly float steerDecay = 0.2f;

        public Kart() : base()
        {
            
        }

        public void updateInput()
        {
            throttle = 0;
            steer = 0;
            if (Engine.GetKeyHeld(Key.W))
            {
                throttle = 1f;
            }
            if (Engine.GetKeyHeld(Key.S))
            {
                throttle = -1f;
            }
            if (Engine.GetKeyHeld(Key.A))
            {
                steer = -1;
            }
            if (Engine.GetKeyHeld(Key.D))
            {
                steer = 1;
            }
        }

        public void update(float dt)
        {
            acceleration = throttle * throttleConst - velocity * dragConst - naturalDecel;

            if (Math.Abs(steer) - steerDecay * dt < 0)
            {
                steer = 0;
            }
            else
            {
                steer -= steerDecay * dt * Math.Sign(steer);
            }
           
            angle += steer * steerConst * dt;
            velocity += acceleration * dt;
            float scalar = velocity * dt + 0.5f * acceleration * dt * dt;
            position += scalar * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

            Console.WriteLine("throt: " + throttle + " velo: " + velocity / 100f + " accel: " + acceleration);
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