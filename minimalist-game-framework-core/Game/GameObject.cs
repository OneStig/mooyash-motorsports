using System;
using Mooyash.Services;

namespace Mooyash.Modules
{
    public class GameObject
    {
        public Vector2 position;
        public float angle; //0 = positive x, pi/2 = positive y
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
        public Vector2 velocity;
        public float acceleration;
        public float throttle;
        public float steer;
        public bool stunned;

        private readonly float throttleConst = 500;
        private readonly float steerConst = 40f * (float)Math.PI / 180f; // MAXIMUM steering angle in radians
        private readonly float steerLimit = 0.001f; // reduces maximum steering angle at higher speeds
        private readonly float dragConst = 1;
        private readonly float naturalDecel = 1;

        private readonly float steerDecay = 0.2f;

        private readonly float kartLength = 100f; // length of wheel base from back to front axle

        public Kart() : base()
        {
            velocity = new Vector2(0, 0);
        }

        public void updateInput()
        {
            throttle = 0;
            steer = 0;
            if (Engine.GetKeyHeld(Key.W))
            {
                throttle = 1;
            }
            if (Engine.GetKeyHeld(Key.S))
            {
                throttle = -1;
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
            acceleration = throttle * throttleConst - velocity.X * dragConst - naturalDecel;

            if (Math.Abs(acceleration) < 0.1)
            {
                acceleration = 0;
            }

            velocity += new Vector2(acceleration * dt, 0);

            if (Math.Abs(velocity.X) < 0.005)
            {
                velocity = new Vector2(0, 0);
            }

            if (Math.Abs(steer) - steerDecay * dt < 0)
            {
                steer = 0;
            }
            else
            {
                steer -= steerDecay * dt * Math.Sign(steer);
            }

            float steerAngle = steer * steerConst / (steerLimit * velocity.X + 1);
            float turnRad = kartLength / (float)Math.Sin(steerAngle);
            // float backRad = frontRad * (float)Math.Cos(steerAngle);
            float angularVelo;

            if (steerAngle == 0)
            {
                angularVelo = 0;
            }
            else
            {
                angularVelo = velocity.X / turnRad;
            }

            position += velocity.Rotated(angle * 180f / (float)Math.PI) * dt;
            angle += angularVelo * dt;


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