﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using Mooyash.Services;

namespace Mooyash.Modules
{
    public class GameObject
    {
        public string selfId;

        public Vector2 position;
        //technically unnecessary for StaticObjects
        public float angle; //0 = positive x, pi/2 = positive y
        public float radius;
        public int curTex; // currentTexture
        public Texture texture; //sprite sheet for this gameObject
        public Vector2 size; // width and height of the object in game
        public Vector2 resolution; // width and height of each costume
        public int numTex; // number of textures

        public GameObject()
        {
            position = new Vector2();
            angle = 0;
            curTex = 0;
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
        
        //updates object and returns true if there is a collision
        public bool testCollision(float dt, Kart kart)
        {
            return PhysicsEngine.TestCircles(kart.position, kart.radius, position, radius);
        }
    }

    //I think, Kart could inherit from projectile - but it's not critical
    public class Projectile : GameObject
    {
        public Vector2 prevPosition;
        public Vector2 velocity;

        public virtual void update(float dt)
        {
            prevPosition = new Vector2(position.X, position.Y);
            position += velocity.Rotated(angle * 180f / (float)Math.PI) * dt;

            float minCollision = 1;
            float wallAngle = 0;
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
                            wallAngle = Vector2.Angle(next - cur);
                            finalPos = c.c1 + minCollision * (c.c2 - c.c1);
                        }
                    }
                }
            }

            if (minCollision != 1)
            {
                position = finalPos;
                wallCollide(wallAngle);
            }
        }

        public void wallCollide(float wallAngle)
        {
            angle = 2 * wallAngle - angle;
        }
    }

    public class Kart : GameObject
    {
        public float acceleration;
        public Vector2 velocity;
        //throttle could be signed or unsigned, it doesn't matter that much
        public float throttle;
        public float steer;

        public Vector2 prevPosition;

        public bool stunned;
        public bool braking;
        public bool isAI;

        public int itemHeld;
        public float stunTime = float.MaxValue / 2; // time passed since last stun
        public float boostTime = float.MaxValue / 2; // time passed since last speed boost
        public float rollItemTime = float.MaxValue / 2; // time passed since rolled item

        private float stunDrag = 1f;

        // Constants to determine effect intensity
        private readonly float boostMultiplier = 1.8f;
        private readonly float stunMultiplier = 6f;

        // Constants to determine how long certain effects will last (in seconds)
        private readonly float stunConst = 3f;
        private readonly float speedBoostConst = 3f;
        private readonly float rollConst = 2f;

        //Waypoint variables for ai driving
        public int currentWaypoint;
        public int previousWaypoint;
        public List<Vector2> allWaypoints;

        public float distanceTraveled;
        public float prevPercent;
        public float curPercent;
        public float percentageAlongTrack;

        //Distance from kart to closet waypoints updated every frame
        public float[] dists;

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
        public float angleToPlayer;
        public float distanceToPlayer;
        public Random rand = new Random();

        //determines acceleration
        private readonly float throttleConst = 1200; //multiplies throttle
        private float linDragConst = 0.5f; //deceleration linearly based on velocity
        private float quadDragConst = 0.002f; //deceleration quadratically based on velocity
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

        // score
        public int score;
        public Kart(float throttleConst, bool isAI, String kartName) : base()
        {
            texture = Engine.LoadTexture(kartName + "_sheet.png");
            numTex = 15;
            size = new Vector2(62.5f, 62.5f);
            resolution = new Vector2(32, 32);

            velocity = new Vector2(0, 0);
            position = new Vector2(4500, 0);
            radius = 24f;

            //Waypoint initialiazation
            this.allWaypoints = Track.tracks[0].splines;
            currentWaypoint = 0;
            previousWaypoint = 0;

            newRandomWaypoint = allWaypoints[0];
            prevRandomWaypoint = allWaypoints[0];
            minDistanceToReachWaypoint = 500;

            //Kart-dependent lap
            lapCount = 0;
            lapDisplay = 1;

            itemHeld = 0;
            score = 0;
            
            this.isAI = isAI;
            this.throttleConst = throttleConst;
        }

        //do not call itemHeld unless the item is greater than 0
        private void useItem()
        {
            // "nothing", "banana", "green shell", "mushroom"
            if (itemHeld == 1) // banana
            {
                float sin = (float)Math.Sin(angle);
                float cos = (float)Math.Cos(angle);

                Vector2 spawnPosition = position - new Vector2(cos, sin) * 100;

                PhysicsEngine.gameObjects.Add(new Banana(spawnPosition));
            }
            else if (itemHeld == 2) // green shell
            {
                float sin = (float)Math.Sin(angle);
                float cos = (float)Math.Cos(angle);

                Vector2 spawnPosition = position + new Vector2(cos, sin) * 100;

                Shell sh = new Shell(spawnPosition, angle);

                PhysicsEngine.gameObjects.Add(sh);
                PhysicsEngine.projectiles.Add(sh);
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

        public void percentDone()
        {
            if(previousWaypoint == 0)
            {
                percentageAlongTrack = Track.tracks[0].lens[0] *
                                        Splines.getPercentageProgress(prevRandomWaypoint, newRandomWaypoint, position) / Track.tracks[0].totalLen;
                return;
            }
            float curDist = Track.tracks[0].lens[previousWaypoint] *
                            Splines.getPercentageProgress(prevRandomWaypoint, newRandomWaypoint, position) / 100;
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

        public void updateTargetWaypoints(int waypointRadius)
        {
            dists = Splines.getClosestPoints(position, previousWaypoint, currentWaypoint, allWaypoints);

            if (dists[1]+waypointRadius > dists[2])
            {
                previousWaypoint = currentWaypoint;
                currentWaypoint = (currentWaypoint + 1) % allWaypoints.Count;
            }
            if (dists[0] < dists[1])
            {
                currentWaypoint = previousWaypoint;
                previousWaypoint--;
                if (previousWaypoint < 0)
                {
                    previousWaypoint = allWaypoints.Count - 1;
                }

            }
            randomDrivingRadius = rand.Next(waypointRadius / 10);
            randAngle = (float)(rand.NextDouble() * 2) * (float)Math.PI;
            newRandomWaypoint = new Vector2((float)(allWaypoints[currentWaypoint].X + Math.Cos(randAngle) * randomDrivingRadius),
                                            (float)(allWaypoints[currentWaypoint].Y + Math.Sin(randAngle) * randomDrivingRadius));;
            prevRandomWaypoint = allWaypoints[previousWaypoint];
        }

        public void updateInput(float dt)
        {

            updateTargetWaypoints(0);

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

            if (Engine.GetKeyDown(Key.Space))
            {
                if (itemHeld > 0)
                {
                    useItem();
                }
            }

            //percentDone();
        }

        public void updateInputAI(float dt)
        {
            //itemHeld = 3;
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

            //updateTargetWaypoints(rand.Next(450, 500));

            braking = false;
            throttle = Math.Min(1, throttle + tInputScale * dt);    

            angleToWaypoint = (float)Math.Atan2(newRandomWaypoint.Y - position.Y,
                                                    newRandomWaypoint.X - position.X);
            angleToWaypoint %= 2 * (float)Math.PI;

            angleToPlayer = (float)Math.Atan2(PhysicsEngine.player.position.Y - position.Y,
                                                PhysicsEngine.player.position.X - position.X);
            angleToPlayer %= 2 * (float)Math.PI;

            distanceToPlayer = Splines.distanceToPoint(PhysicsEngine.player.position, position);

            if (itemHeld == 3)
            {
                useItem();
            }
            else if (distanceToPlayer < 2000 && itemHeld > 0 && itemHeld != 3)
            {
                useItem();
            }


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

            //percentDone();
        }

        public void update(float dt)
        {
            prevPosition = new Vector2(position.X, position.Y);

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
                stunned = true;
                stunDrag = stunMultiplier;
                throttle = 0;
                steer = 0;
            }
            else
            {
                stunned = false;
                stunDrag = 1f;
            }

            Tuple<float, float, float> terrainConst = PhysicsEngine.terrainConsts[PhysicsEngine.GetPhysicsID(position)];

            // when boosting

            if (boostTime < speedBoostConst)
            {
                throttle *= boostMultiplier;
                terrainConst = PhysicsEngine.terrainConsts[0];
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

            // this code is copy pasted probably bad but eh

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
                wallCollide(0);
            }

            // base.update(dt);

            percentDone();
        }

        public void wallCollide(float wallAngle)
        {
            velocity.X = -velocity.X * 0.75f;
            throttle /= 2;
        }

        public override void collide(Kart kart)
        {
            Vector2 adjust = (radius + kart.radius - (kart.position - position).Length())*(kart.position-position).Normalized();
            kart.position += adjust;
            position -= adjust;
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