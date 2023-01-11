using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        //parallel array with textures to indicate how big sprites should be drawn
        public Vector2[] sizes;

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

        //Waypoint variables for ai driving
        public int currentWaypoint;
        public int previousWaypoint;
        public List<Vector2> allWaypoints;

        public float distanceTraveled;
        public float prevPercent;
        public float curPercent;
        public float percentageAlongTrack;

        //Kart dependent lapCount and lapDisplay variables
        public int lapCount;
        public int lapDisplay;

        /*
         * two separate variables, controls the radius at which a random point is found around a waypoint
         * and the radius at which the kart will decide it reaches the waypoint.
         * */
        public float minDistanceToReachWaypoint;
        public float randomDrivingRadius;

        public float randAngle;
        public Vector2 newRandomWaypoint;
        public Vector2 prevRandomWaypoint;

        public float angleToWaypoint;
        public Random rand = new Random();

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
            //Kart physics/properties
            velocity = new Vector2(0, 0);
            textures = new Texture[5];
            sizes = new Vector2[5];
            position = new Vector2(4500, 0);
            radius = 24f;
            this.throttleConst = throttleConst;

            //Waypoint initialiazation
            this.allWaypoints = Track.tracks[0].splines;
            currentWaypoint = 0;
            previousWaypoint = 0;

            newRandomWaypoint = allWaypoints[0];
            prevRandomWaypoint = allWaypoints[0];
            minDistanceToReachWaypoint = rand.Next(450, 500);

            //Kart-dependent lap
            lapCount = 0;
            lapDisplay = 1;

            //Loading textures
            for (int i = 0; i < textures.Length; i++)
            {
                textures[i] = Engine.LoadTexture("player_" + i + ".png");
                sizes[i] = new Vector2(500, 500);
            }
        }

        public void percentDone()
        {
            if(previousWaypoint == 0)
            {
                percentageAlongTrack = Track.tracks[0].lens[0] *
                                        Splines.getPercentageProgress(prevRandomWaypoint, newRandomWaypoint, position) / Track.tracks[0].totalLen;
                return;
            }
            float curDist = Track.tracks[0].lens[previousWaypoint] * Splines.getPercentageProgress(prevRandomWaypoint, newRandomWaypoint, position) / 100;
            float prevDist = Track.tracks[0].lensToPoint[previousWaypoint - 1];
            percentageAlongTrack = (curDist + prevDist) / Track.tracks[0].totalLen * 100;
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
                if (velocity.X < 0)
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
                if (velocity.X > 0)
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

        public void updateInputAI(float dt)
        {
            angle %= 2*(float)Math.PI;
            //target is current waypoint

            Vector2 distToWaypoint = new Vector2(allWaypoints[currentWaypoint].X - position.X, allWaypoints[currentWaypoint].Y - position.Y);
            if (Math.Sqrt(distToWaypoint.X * distToWaypoint.X + distToWaypoint.Y * distToWaypoint.Y) < minDistanceToReachWaypoint)
            {
                minDistanceToReachWaypoint = rand.Next(450, 500);
                previousWaypoint = currentWaypoint;
                currentWaypoint = (currentWaypoint + 1) % allWaypoints.Count;

                randomDrivingRadius = rand.Next(0, 30);
                randAngle = (float)(rand.NextDouble() * 2) * (float)Math.PI;
                prevRandomWaypoint = newRandomWaypoint;
                newRandomWaypoint = new Vector2((float)(allWaypoints[currentWaypoint].X + Math.Cos(randAngle) * randomDrivingRadius),
                                                (float)(allWaypoints[currentWaypoint].Y + Math.Sin(randAngle) * randomDrivingRadius));
            }


            braking = false;
            throttle = Math.Min(1, throttle + tInputScale * dt);    

            angleToWaypoint = (float)Math.Atan2(newRandomWaypoint.Y - position.Y,
                                                    newRandomWaypoint.X - position.X);
            angleToWaypoint %= 2 * (float)Math.PI;


            if (Math.Abs(angleToWaypoint - angle) > .1)
            { 
                float angleDiff = (angleToWaypoint - angle) % (2 * (float)Math.PI);
                if (Math.Abs(angleDiff) < Math.PI)
                {
                    if (angleToWaypoint - angle < 0)
                    {
                        if (Math.Abs(angleDiff) < .15)
                        {
                            steer = decay(steer, steerDecay, dt);
                        }
                        else
                        {
                            //turn left
                            steer = Math.Max(-1, steer - sInputScale * dt);
                        }
                    }
                    else if (angleToWaypoint - angle > 0)
                    {
                        if(Math.Abs(angleDiff) < .15)
                        {
                            steer = decay(steer, steerDecay, dt);
                        }
                        else
                        {
                            //turn right
                            steer = Math.Min(1, steer + sInputScale * dt);
                        }
                    }
                }
                else
                {
                    if (angleToWaypoint - angle > 0)
                    {
                        if (Math.Abs(angleDiff) < .15)
                        {
                            steer = decay(steer, steerDecay, dt);
                        }
                        else
                        {
                            //turn left
                            steer = Math.Max(-1, steer - sInputScale * dt);
                        }
                    }
                    else if (angleToWaypoint - angle < 0)
                    {
                        if (Math.Abs(angleDiff) < .15)
                        {
                            steer = decay(steer, steerDecay, dt);
                        }
                        else
                        {
                            //turn right
                            steer = Math.Min(1, steer + sInputScale * dt);
                        }
                    }
                }
            }
            else
            {
                steer = decay(steer, steerDecay, dt);
                angle = angleToWaypoint;
            }
            percentDone();
        }


        public void update(float dt, Tuple<float, float, float> terrainConst)
        {
            //acceleration due to drag (quadratic) and friction
            float tempA = -velocity.X * Math.Abs(velocity.X) * terrainConst.Item1 * quadDragConst
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
            if (velocity.X == 0)
            {
                if (Math.Abs(tempA) <= terrainConst.Item3 * naturalDecel)
                {
                    tempA = 0;
                }
                else
                {
                    tempA -= Math.Sign(tempA) * terrainConst.Item3 * naturalDecel;
                }
            }
            //if acceleration and tempA have opposite signs
            if (Math.Sign(acceleration) * Math.Sign(tempA) == -1)
            {
                acceleration = 0;
            }
            else
            {
                acceleration = tempA;
            }

            float tempV = velocity.X + acceleration * dt;
            //if velocity and tempV have opposite signs
            if (Math.Sign(velocity.X) * Math.Sign(tempV) == -1)
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

            chooseTexture(angularVelo);
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