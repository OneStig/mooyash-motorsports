﻿using System;
using Mooyash.Services;

namespace Mooyash.Modules
{
    public class GameObject
    {
        public Vector2 position;
        public float angle; //0 = positive x, pi/2 = positive y
        public Polygon hitbox;
        public int curTex;
        public Texture[] textures;

        public GameObject()
        {
            position = new Vector2();
            angle = 0;
            curTex = 0;
            // need to add hitbox and textures later
        }
    }

    public class Kart : GameObject
    {
        public Vector2 velocity;
        public float acceleration;
        //throttle could be signed or unsigned, it doesn't matter that much
        public float throttle;
        public float steer;
        public float radius;
        public bool stunned;
        public bool braking;

        //determines acceleration
        private readonly float throttleConst = 1200; //multiplies throttle
        private readonly float linDragConst = 0.5f; //deceleration linearly based on velocity
        private readonly float quadDragConst = 0.002f; //deceleration quadratically based on velocity
        private readonly float naturalDecel = 1; //constant deceleration
        private readonly float brakeConst = 300; //deceleration due to braking

        //determines turning
        private readonly float steerConst = 20f * (float)Math.PI / 180f; // MAXIMUM steering angle in radians
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
            textures = new Texture[5];
            position = new Vector2(4500, 0);
            radius = 50f;

            for (int i = 0; i < textures.Length; i++)
            {
                textures[i] = Engine.LoadTexture("player_" + i + ".png");
            }
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
            braking = false;
            if (Engine.GetKeyHeld(Key.W))
            {
                if(velocity.X < 0)
                {
                    throttle = 0;
                    braking = true;
                }
                else
                {
                    throttle = Math.Min(1, throttle + tInputScale * dt);
                }
            }
            else if (Engine.GetKeyHeld(Key.S))
            {
                if(velocity.X > 0)
                {
                    throttle = 0;
                    braking = true;
                }
                else
                {
                    throttle = Math.Max(-0.5f, throttle - tInputScale * dt);
                }
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

        public void update(float dt, Tuple<float, float> terrainConst)
        {
            //acceleration due to drag (quadratic) and friction
            float tempA = -velocity.X*Math.Abs(velocity.X) * terrainConst.Item1 * quadDragConst 
                - velocity.X * terrainConst.Item2 * linDragConst 
                - Math.Sign(velocity.X) * naturalDecel;
            if (braking)
            {
                //acceleration due to braking
                tempA += -Math.Sign(velocity.X) * brakeConst;
            }
            else
            {
                //acceleration due to throttle
                tempA += throttle * throttleConst;
            }
            //static friction
            if(velocity.X == 0)
            {
                if(Math.Abs(tempA) <= naturalDecel)
                {
                    tempA = 0;
                }
                else
                {
                    tempA -= Math.Sign(tempA) * naturalDecel;
                }
            }
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

            float angularVelo;
            float steerAngle = steer * steerConst / (steerLimit * Math.Abs(velocity.X) + 1);
            float turnRad = kartLength / (float)Math.Sin(steerAngle);
            float backRad = turnRad * (float)Math.Cos(steerAngle);

            if (steerAngle == 0)
            {
                angularVelo = 0;
            }
            else
            {
                angularVelo = velocity.X / turnRad;
            }
            angle += angularVelo * dt;
            position += velocity.Rotated(angle * 180f / (float)Math.PI) * dt;

            if (angularVelo < -0.8)
            {
                curTex = 3;
            }
            else if (angularVelo > 0.8)
            {
                curTex = 4;
            }
            else if (angularVelo < -0.3)
            {
                curTex = 1;
            }
            else if (angularVelo > 0.3)
            {
                curTex = 2;
            }
            else
            {
                curTex = 0;
            }

            // Console.WriteLine("throt: " + throttle + " velo: " + velocity / 100f + " accel: " + acceleration);
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