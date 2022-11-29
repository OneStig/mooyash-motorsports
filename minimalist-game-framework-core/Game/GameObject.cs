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
        public bool reverse = false;

        //determines acceleration
        private readonly float throttleConst = 1200; //multiplies throttle
        private readonly float dragConst = 1; //deceleration based on velocity
        private readonly float naturalDecel = 1; //constant deceleration

        //determines turning
        private readonly float steerConst = 30f * (float)Math.PI / 180f; // MAXIMUM steering angle in radians
        private readonly float steerLimit = 0.001f; // reduces maximum steering angle at higher speeds
        private readonly float kartLength = 100f; // length of wheel base from back to front axle

        //determines input
        private readonly float tInputScale = 0.2f;
        private readonly float sInputScale = 8f;

        //steer and throttle decay to 0
        private readonly float steerDecay = 4f;
        private readonly float throttleDecay = 1f;

        public Kart() : base()
        {
            velocity = new Vector2(0, 0);
        }

        private float decay(float value, float constant, float dt)
        {
            if (Math.Abs(value) - constant * dt < 0)
            {
                return 0;
            }
            else
            {
                return value - constant * dt * Math.Sign(value);
            }
        }

        public void updateInput(float dt)
        {
            reverse = false;
            if (Engine.GetKeyHeld(Key.W))
            {
                throttle = Math.Min(1, throttle + tInputScale * dt);
            }
            else if (Engine.GetKeyHeld(Key.S))
            {
                //HOW ARE WE HANDLING BRAKE AND REVERSE?
                throttle = Math.Max(-0.5f, throttle - tInputScale * dt);
            }
            else
            {
                throttle = decay(throttle, throttleDecay, dt);
            }

            if (Engine.GetKeyHeld(Key.A))
            {
                steer = Math.Max(-1, steer - sInputScale * dt);
            }
            else if (Engine.GetKeyHeld(Key.D))
            {
                steer = Math.Min(1, steer + sInputScale * dt);
            }
            else
            {
                steer = decay(steer, steerDecay, dt);
            }
        }

        public void update(float dt)
        {
            float tempA = throttle * throttleConst - velocity.X * dragConst - naturalDecel;
            //if acceleration and tempA have opposite signs
            if (Math.Sign(acceleration)*Math.Sign(tempA) == -1)
            {
                acceleration = 0;
            }
            else
            {
                acceleration = tempA;
            }

            float tempV = velocity.X + acceleration * dt;
            //if velocity and tempV have opposite signs
            if (Math.Sign(velocity.X)*Math.Sign(tempV) == -1)
            {
                velocity = new Vector2(0, 0);
            }
            else
            {
                velocity = new Vector2(tempV, 0);
            }

            float steerAngle = steer * steerConst / (steerLimit * Math.Abs(velocity.X) + 1);
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