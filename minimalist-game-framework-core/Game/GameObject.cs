using System;
using Mooyash.Services;

namespace Mooyash.Modules
{
    public class GameObject
    {
        public Vector2 position;
        //technically unnecessary for StaticObjects
        public float angle; //0 = positive x, pi/2 = positive y
        public float radius;
        public int curTex;
        public Texture[] textures;
        //parallel array with textures to indicate how big sprites should be drawn
        public Vector2[] sizes;

        //updates object and returns true if there is a collision
        public bool testCollision(float dt, Kart kart)
        {
            return false;
        }

        public void collide(Kart kart)
        { }
    }

    public class StaticObject : GameObject
    {
        new public bool testCollision(float dt, Kart kart)
        {
            return PhysicsEngine.TestCircleStaticCircle(kart.getCirclePath(), position, radius);
        }
    }

    //I think, Kart could inherit from projectile - but it's not critical
    public class Projectile : GameObject
    {
        public Vector2 prevPosition;
        public Vector2 velocity;

        new public bool testCollision(float dt, Kart kart)
        {
            return PhysicsEngine.TestCircleCircle(kart.getCirclePath(), getCirclePath());
        }

        public CirclePath getCirclePath()
        {
            return new CirclePath(prevPosition, position, radius);
        }

        public void update(float dt)
        {
            prevPosition = position;
            position += velocity.Rotated(angle * 180f / (float)Math.PI) * dt;

            float minCollision = 1;
            Vector2 finalPos = new Vector2();
            Vector2 cur;
            Vector2 next;
            CirclePath c = new CirclePath(prevPosition, position, radius);

            //Handles collisions between player and walls of polygon
            //Should implement bounding box idea
            foreach (Polygon p in PhysicsEngine.track.collidable)
            {
                for (int i = 0; i < p.vertices; i++)
                {
                    //if p has the same point twice in a row, this fails
                    cur = p.points[i];
                    next = p.points[(i + 1) % p.vertices];
                    if (cur.Equals(next))
                    {
                        throw new Exception("Polygon cannot have same point twice in a row");
                    }
                    if (PhysicsEngine.TestCircleLine(c, cur, next))
                    {
                        //OPTIMIZE: This is (kinda) recalculating norm
                        //EXCEPTION: What if cross is 0? - shouldn't happen though
                        Vector2 norm = (next - cur).Rotated(Math.Sign(Vector2.Cross(next - cur, c.c1 - cur)) * 90).Normalized();
                        float norm1 = Vector2.Dot(norm, c.c1 - next) - radius;
                        float norm2 = Vector2.Dot(norm, c.c2 - next) - radius;
                        if (norm1 != norm2 && norm1 < minCollision * (norm1 - norm2))
                        {
                            minCollision = norm1 / (norm1 - norm2);
                            finalPos = c.c1 + minCollision * (c.c2 - c.c1);
                        }
                    }
                }
            }

            if (minCollision != 1)
            {
                position = finalPos;
                wallCollide();
            }
        }

        public void wallCollide()
        {
            velocity.X = -velocity.X;
        }
    }

    public class Kart : Projectile
    {
        public float acceleration;
        //throttle could be signed or unsigned, it doesn't matter that much
        public float throttle;
        public float steer;

        public bool stunned;
        public bool braking;

        //for lap completion
        public float lapCount;
        public float lapDisplay;

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

            
        public Kart(float throttleConst) : base()
        {
            velocity = new Vector2(0, 0);
            textures = new Texture[5];
            sizes = new Vector2[5];
            position = new Vector2(4500, 0);
            radius = 24f;
            this.throttleConst = throttleConst;

            for (int i = 0; i < textures.Length; i++)
            {
                textures[i] = Engine.LoadTexture("player_" + i + ".png");
                sizes[i] = new Vector2(500, 500);
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

        new public void update(float dt)
        {
            Tuple<float, float, float> terrainConst = PhysicsEngine.terrainConsts[PhysicsEngine.GetPhysicsID(position)];

            //acceleration due to drag (quadratic) and friction
            float tempA = -velocity.X*Math.Abs(velocity.X) * terrainConst.Item1 * quadDragConst 
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
            chooseTexture(angularVelo);

            base.update(dt);


            if (PhysicsEngine.TestLineLine(prevPosition, position, PhysicsEngine.track.finish.Item1, PhysicsEngine.track.finish.Item2))
            {
                if (Vector2.Dot(position - prevPosition, (PhysicsEngine.track.finish.Item2 - PhysicsEngine.track.finish.Item1).Rotated(90)) > 0
                    == PhysicsEngine.track.finish.Item3)
                {
                    lapCount++;
                }
                else
                {
                    lapCount = lapDisplay - 1;
                }
                lapDisplay = Math.Max(lapDisplay, lapCount);
            }
        }

        new public void wallCollide()
        {
            velocity.X = -velocity.X * 0.75f;
            throttle /= 2;
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