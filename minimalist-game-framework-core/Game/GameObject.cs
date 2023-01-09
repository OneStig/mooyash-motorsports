using System;
using System.Numerics;
using Mooyash.Services;

namespace Mooyash.Modules
{
    public class GameObject
    {
        public string selfId;

        public Vector2 position;
        public float angle; //0 = positive x, pi/2 = positive y
        public Polygon hitbox;

        public int curTex; // currentTexture
        public Texture texture; //sprite sheet for this gameObject
        public Vector2 size; // width and height of the object in game
        public Vector2 resolution; // width and height of each costume
        public int numTex; // number of textures

        public bool exists;

        public GameObject()
        {
            position = new Vector2();
            angle = 0;
            curTex = 0;
            exists = true;
            // need to add hitbox and textures later
        }

        public virtual void collide(Kart k) { }
        
        public void chooseTextureCam(Camera c)
        {
            float cameraAng = (float)Math.Atan2(c.position.Y - position.Y, c.position.X - position.X);
            float angDiff = cameraAng - angle;

            angDiff %= 2f * (float)Math.PI;
            angle %= 2f * (float)Math.PI;

            if (angDiff < 0)
            {
                angDiff += 2f * (float)Math.PI;
            }

            bool parity = angDiff <= (float)Math.PI;
            float intervalAngle = (float)Math.PI / (numTex - 1);

            if (parity)
            {
                angDiff = (float)Math.PI - angDiff + intervalAngle / 2f;
            }
            else
            {
                angDiff -= (float)Math.PI;
                angDiff += intervalAngle / 2f;
            }

            curTex = (int)Math.Floor(angDiff / intervalAngle);

            if (!parity)
            {
                curTex *= -1;
            }
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
        public bool isAI;

        public int itemHeld;
        public float stunTime = float.MaxValue / 2; // time passed since last stun
        public float boostTime = float.MaxValue / 2; // time passed since last speed boost
        public float rollItemTime = float.MaxValue / 2; // time passed since rolled item

        private float stunDrag = 1f;

        // Constants to determine effect intensity
        private readonly float boostMultiplier = 2f;
        private readonly float stunMultiplier = 6f;

        // Constants to determine how long certain effects will last (in seconds)
        private readonly float stunConst = 3f;
        private readonly float speedBoostConst = 3f;
        private readonly float rollConst = 2f;

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

        public Kart(string kartName, float throttleConst, bool isAI) : base()
        {
            texture = Engine.LoadTexture(kartName + "_sheet.png");
            numTex = 15;
            size = new Vector2(500, 500);
            resolution = new Vector2(32, 32);

            velocity = new Vector2(0, 0);
            position = new Vector2(4500, 0);
            radius = 24f;

            itemHeld = 0;

            this.isAI = isAI;
            this.throttleConst = throttleConst;
        }

        private void useItem()
        {
            // "nothing", "banana", "green shell", "mushroom"
            if (itemHeld == 1) // banana
            {
                float sin = (float)Math.Sin(angle);
                float cos = (float)Math.Cos(angle);

                Vector2 spawnPosition = position - new Vector2(cos, sin) * 60;

                PhysicsEngine.gameObjects.Add("banana" + PhysicsEngine.gameObjects.Count, new Banana(spawnPosition));
            }
            else if (itemHeld == 2) // green shell
            {
                float sin = (float)Math.Sin(angle);
                float cos = (float)Math.Cos(angle);

                Vector2 spawnPosition = position + new Vector2(cos, sin) * 60;

                PhysicsEngine.gameObjects.Add("shell" + PhysicsEngine.gameObjects.Count, new Shell(spawnPosition, angle));
            }
            else if (itemHeld == 3) // mushroom
            {
                if (boostTime < speedBoostConst)
                {
                    boostTime -= speedBoostConst;
                }
                else
                {
                    boostTime = 0;
                }
            }

            itemHeld = 0;
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

            if (Engine.GetKeyDown(Key.Space))
            {
                if (itemHeld > 0)
                {
                    useItem();
                }
            }
        }

        public void update(float dt, Tuple<float, float, float> terrainConst)
        {
            // update various timers
            stunTime += dt;
            boostTime += dt;
            rollItemTime += dt;

            // when itemRolled

            if (rollItemTime >= rollConst && itemHeld == -1)
            {
                Random r = new Random();
                itemHeld = r.Next(0, ItemBox.validItems.Length) + 1;
            }

            // when stunned

            if (stunTime < stunConst)
            {
                stunDrag = stunMultiplier;
                throttle = 0;
                steer = 0;
            }
            else
            {
                stunDrag = 1f;
            }

            // when boosting

            if (boostTime < speedBoostConst)
            {
                throttle *= boostMultiplier;
            }

            //acceleration due to drag (quadratic) and friction
            float tempA = -velocity.X*Math.Abs(velocity.X) * terrainConst.Item1 * quadDragConst * stunDrag 
                - velocity.X * terrainConst.Item2 * linDragConst 
                - Math.Sign(velocity.X) * terrainConst.Item3 * naturalDecel;

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
                if(Math.Abs(tempA) <= terrainConst.Item3 * naturalDecel)
                {
                    tempA = 0;
                }
                else
                {
                    tempA -= Math.Sign(tempA) * terrainConst.Item3 * naturalDecel;
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

            // reset throttle if boosting

            if (boostTime < speedBoostConst)
            {
                throttle /= boostMultiplier;
            }

            if (!isAI)
            {
                choosePlayerTexture(angularVelo);
            }
            else
            {
                chooseTextureCam(RenderEngine.camera);
            }
        }

        public void choosePlayerTexture(float angularVelo)
        {
            if (angularVelo < -0.8)
            {
                curTex = -2;
            }
            else if (angularVelo > 0.8)
            {
                curTex = 2;
            }
            else if (angularVelo < -0.3)
            {
                curTex = -1;
            }
            else if (angularVelo > 0.3)
            {
                curTex = 1;
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