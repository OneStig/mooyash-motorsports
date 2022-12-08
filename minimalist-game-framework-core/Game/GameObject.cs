using System;
using Mooyash.Services;

namespace Mooyash.Modules
{
    public class GameObject
    {
        public Vector2 position;
        public float angle; //0 = positive x, pi/2 = positive y
        public int curTex;
        public Texture[] textures;

        //public Polygon hitbox;


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
        public Vector2 acceleration;
        public Vector2 dir;
        //throttle could be signed or unsigned, it doesn't matter that much
        public float throttle;
        public float steer;
        public bool braking;

        //for collisions
        public float radius;
        //norm is for if you're skating along a wall
        public Nullable<Vector2> norm;

        //public bool stunned;

        //determines acceleration
        private readonly float throttleConst = 1200; //multiplies throttle
        private readonly float linDragConst = 0.5f; //deceleration linearly based on velocity
        private readonly float quadDragConst = 0.002f; //deceleration quadratically based on velocity
        private readonly float kinDecel = 1; //kinetic friction
        private readonly float statDecel = 1; //static friction
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
            dir = new Vector2((float) Math.Cos(angle), (float) Math.Sin(angle));
            norm = null;

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
                if(Vector2.Dot(velocity, dir) < 0)
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
                if(Vector2.Dot(velocity, dir) > 0)
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
            if(Engine.GetKeyDown(Key.P))
            {
                int a = 0;
            }

            //acceleration due to drag (quadratic) and friction
            acceleration = -velocity.Length() * terrainConst.Item1 * quadDragConst * velocity
                - terrainConst.Item2 * linDragConst * velocity
                - kinDecel * velocity.Normalized();
            if (braking)
            {
                //acceleration due to braking
                acceleration -= brakeConst * velocity.Normalized();
            }
            else
            {
                //acceleration due to throttle
                acceleration += throttle * throttleConst * dir;
            }

            //static friction - NEEDS TO BE ADJUSTED FOR DRIFTING?
            if(velocity.Length() == 0)
            {
                if(acceleration.Length() <= statDecel)
                {
                    acceleration = new Vector2(0,0);
                }
                else
                {
                    acceleration -= statDecel * acceleration.Normalized();
                }
            }

            /*
            float angularVelo;
            float steerAngle = steer * steerConst / (steerLimit * Math.Abs(Vector2.Dot(velocity, dir)) + 1);
            float turnRad = kartLength / (float)Math.Sin(steerAngle);

            if (steerAngle == 0)
            {
                angularVelo = 0;
            }
            else
            {
                angularVelo = Vector2.Dot(velocity, dir) / turnRad;
            }
            
            angle += angularVelo * dt;
            float deltaAngle = -angularVelo * dt * 180f / (float)Math.PI;
            dir = dir.Rotated(deltaAngle);
            velocity = velocity.Rotated(deltaAngle);
            position += velocity * dt;
            */
            
            //ACTUALLY FIGURE OUT PHYSICS
            float curvature = (float)(2 * Math.Sin(steer * steerConst / 2)) / kartLength;
            acceleration += Vector2.Dot(velocity, dir)*Vector2.Dot(velocity, dir)*curvature * dir.Rotated(90);

            if(norm != null)
            {
                float dot = Vector2.Dot(acceleration, (Vector2) norm);
                if(dot < 0)
                {
                    acceleration -= dot * (Vector2) norm;
                }
            }

            Vector2 tempV = velocity + acceleration * dt;
            //if velocity and tempV are in opposite directions
            if (Vector2.Dot(tempV, velocity) < 0)
            {
                velocity = new Vector2(0, 0);
            }
            else
            {
                velocity = tempV;
            }

            float dAngle;
            if(velocity.Length() == 0)
            {
                dAngle = 0;
            }
            else
            {
                dAngle = (float) Math.Atan(dt * Vector2.Dot(velocity, dir) * Vector2.Dot(velocity, dir) * curvature / velocity.Length());
            }
            angle += dAngle;
            dir = dir.Rotated(-dAngle * (float) (180/(2*Math.PI)) );

            position += velocity * dt;

            chooseTexture(curvature);
        }

        public void chooseTexture(float angularVelo)
        {
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