using System;
using System.ComponentModel;
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
        public float height; // above ground

        public GameObject()
        {
            position = new Vector2();
            angle = 0;
            curTex = 0;
            // need to add hitbox and textures later
        }

        public GameObject(Vector2 position, Vector2 size, Texture texture, int curTex, int numTex, float height)
        {
            this.position = position;
            this.curTex = curTex;
            this.texture = texture;
            this.size = size;
            this.resolution = new Vector2(texture.Size.X / numTex, texture.Size.Y);
            this.numTex = numTex;
            this.height = height;
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
            angle = (float) ((2 * wallAngle - angle + 2*Math.PI) % (2*Math.PI));
        }
    }

    public class Kart : GameObject
    {
        public float acceleration;
        public Vector2 velocity;
        //throttle could be signed or unsigned, it doesn't matter that much
        public float throttle;
        public float steer;
        //physicsID for the player's current position
        public int id;
        public int coins;

        public float finTime;

        public Vector2 prevPosition;

        public bool stunned;
        public bool braking;
        public bool isAI;
        
        //drifting
        public bool drifting;
        public float driftTime;
        public bool camFlipped;

        public int place;

        public int itemHeld;
        public Color iconColor;

        public float stunTime = float.MaxValue / 2; // time passed since last stun
        public float boostTime = float.MaxValue / 2; // time passed since last speed boost
        public float rollItemTime = float.MaxValue / 2; // time passed since rolled item
        public float largeTime = float.MaxValue / 2; // time passed since last enlargement
        public float dBoostTime = float.MaxValue / 2; // timer for drift boosting

        private float stunDrag = 1f;

        // Constants to determine effect intensity
        private readonly float boostMultiplier = 1.8f;
        private readonly float largeMultiplier = 2f;
        private readonly float stunMultiplier = 6f;
        private float dBoostMultiplier = 1f;

        // Constants to determine how long certain effects will last (in seconds)
        private readonly float stunConst = 1.8f;
        public readonly float speedBoostConst = 2f;
        private readonly float largeConst = 6f;
        private readonly float rollConst = 2f;
        //for lap completion
        public int lapCount;
        public int lapDisplay;
        public float dBoostConst = 1f;

        //Waypoint variables for ai driving
        public int currentWaypoint;
        public int previousWaypoint;
        public List<Vector2> allWaypoints;
        public List<Vector2> playerWaypoints;

        public int prevProgressInd;
        public int curProgressInd;
        public Vector2 prevProgressPoint;
        public Vector2 curProgressPoint;

        public float distanceTraveled;
        public float prevPercent;
        public float curPercent;
        public float percentageAlongTrack;

        //Distance from kart to closet waypoints updated every frame
        public float[] dists;

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

        //for sounds
        public float prevThrottle;
        public SoundInstance rev;
        public SoundInstance terrain;
        public SoundInstance driftSound;
        public float collideTimer;

        // score
        public int score;

        // particle textures
        public static Texture smoke;
        
        public Kart(float throttleConst, bool isAI, string kartName, Color color) : base()
        {
            iconColor = color;
            selfId = kartName;

            texture = Engine.LoadTexture(kartName + "_sheet.png");
            smoke = Engine.LoadTexture("smoke2.png");
            numTex = 15;
            size = new Vector2(62.5f, 62.5f);
            resolution = new Vector2(32, 32);

            velocity = new Vector2(0, 0);
            position = new Vector2(4500, 0);
            radius = 24f;
            coins = 0;

            //Waypoint initialiazation
            this.allWaypoints = Track.tracks[0].splines;
            this.playerWaypoints = Track.tracks[0].playerSplines;
            currentWaypoint = 0;
            previousWaypoint = 0;

            finTime = 0;

            prevProgressInd = 0;
            curProgressInd = 1;
            prevProgressPoint = playerWaypoints[0];
            curProgressPoint = playerWaypoints[0];

            newRandomWaypoint = allWaypoints[0];
            prevRandomWaypoint = allWaypoints[0];
            minDistanceToReachWaypoint = 500;

            //Kart-dependent lap
            lapCount = 0;
            lapDisplay = 1;

            itemHeld = 0;
            score = 0;
            camFlipped = false;

            place = 1;
            
            this.isAI = isAI;
            this.throttleConst = throttleConst;
        }

        //do not call itemHeld unless the item is greater than 0
        private void useItem()
        {
            if (itemHeld != 0)
            {
                Engine.PlaySound(Sounds.sounds["useItem"]);
            }
            // "nothing", "banana", "green shell", "mushroom", "bread"
            if (itemHeld == 1) // banana
            {
                float sin = (float)Math.Sin(angle);
                float cos = (float)Math.Cos(angle);

                Vector2 spawnPosition = position - new Vector2(cos, sin) * 100;

                PhysicsEngine.gameObjects.Add(new Banana(spawnPosition));
            }
            else if (itemHeld == 2) // green shell
            {
                float shellAngle = angle;

                if (camFlipped)
                {
                    shellAngle = (angle + (float)Math.PI) % (2 * (float)Math.PI);
                }

                float sin = (float)Math.Sin(shellAngle);
                float cos = (float)Math.Cos(shellAngle);

                Vector2 spawnPosition = position + new Vector2(cos, sin) * 100;

                Shell sh = new Shell(spawnPosition, shellAngle);

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
            } else if (itemHeld == 4) // bread
            {
                if(largeTime < largeConst)
                {
                    largeTime -= largeConst;
                } else
                {
                    largeTime = 0;
                }

            }
            
            itemHeld = 0;
        }

        public void percentDone()
        {
            if (prevProgressInd == 0)
            {
                percentageAlongTrack = Track.tracks[0].pLens[0] *
                                        Splines.getPercentageProgress(prevProgressPoint, curProgressPoint, position) /
                                        Track.tracks[0].pTotalLen;
                return;
            }
            float curDist = Track.tracks[0].pLens[prevProgressInd] *
                            Splines.getPercentageProgress(prevProgressPoint, curProgressPoint, position) / 100;
            float prevDist = Track.tracks[0].pLensToPoint[prevProgressInd - 1];
            percentageAlongTrack = (curDist + prevDist) / Track.tracks[0].pTotalLen * 100;
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

        public void updateTargetWaypoints()
        {
            dists = Splines.getClosestPoints(position, prevProgressInd, curProgressInd, playerWaypoints);

            if (dists[1] > dists[2])
            {
                prevProgressInd = curProgressInd;
                curProgressInd = (curProgressInd + 1) % playerWaypoints.Count;
            }
            if (dists[0] < dists[1])
            {
                curProgressInd = prevProgressInd;
                prevProgressInd--;
                if (prevProgressInd < 0)
                {
                    prevProgressInd = playerWaypoints.Count - 1;
                }

            }
            curProgressPoint = playerWaypoints[curProgressInd];
            prevProgressPoint = playerWaypoints[prevProgressInd];
        }

        public void updateInput(float dt)
        {
            updateTargetWaypoints();

            braking = false;
            prevThrottle = throttle;

            if (Engine.GetKeyHeld(Key.W) || Engine.GetKeyHeld(Key.Up))
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
            else if (Engine.GetKeyHeld(Key.S) || Engine.GetKeyHeld(Key.Down))
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

            if ((Engine.GetKeyHeld(Key.LeftShift) || Engine.GetKeyHeld(Key.RightShift)) && Math.Abs(steer) > 0.2f && velocity.X > 0)
            {
                if (drifting == false)
                {
                    driftTime = 0;
                    driftSound = Engine.PlaySound(Sounds.sounds["drift"], repeat: true);
                }
                drifting = true;
                driftTime += dt;
                steer = Math.Sign(steer) * Math.Min(Math.Abs(steer)+sInputScale*dt, 1.2f);
            }
            else
            {
                if(drifting == true)
                {
                    driftBoost();
                    drifting = false;
                    Engine.StopSound(driftSound, fadeTime: 0.2f);
                }
                if (Engine.GetKeyHeld(Key.A) || Engine.GetKeyHeld(Key.Left))
                {
                    steer = Math.Max(-1, steer - sInputScale * dt);
                }
                else if (Engine.GetKeyHeld(Key.D) || Engine.GetKeyHeld(Key.Right))
                {
                    steer = Math.Min(1, steer + sInputScale * dt);
                }
                else
                {
                    steer = decay(steer, steerDecay, dt);
                }
            }

            if (Engine.GetKeyDown(Key.Space))
            {
                if (itemHeld > 0)
                {
                    useItem();
                }
            }

            if (Engine.GetKeyHeld(Key.K))
            {
                camFlipped = true;
            }
            else
            {
                camFlipped = false;
            }

        }

        public void updateInputAI(float dt)
        {
            updateTargetWaypoints();
            angle %= 2*(float)Math.PI;
            //target is current waypoint

            Vector2 distToWaypoint = new Vector2(allWaypoints[currentWaypoint].X - position.X, allWaypoints[currentWaypoint].Y - position.Y);
            if (Math.Sqrt(distToWaypoint.X * distToWaypoint.X + distToWaypoint.Y * distToWaypoint.Y) < minDistanceToReachWaypoint)
            {
                minDistanceToReachWaypoint = rand.Next(400, 500);
                previousWaypoint = currentWaypoint;
                currentWaypoint = (currentWaypoint + 1) % allWaypoints.Count;

                randomDrivingRadius = rand.Next(0, 100);
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

            angleToPlayer = (float)Math.Atan2(PhysicsEngine.player.position.Y - position.Y,
                                                PhysicsEngine.player.position.X - position.X);
            angleToPlayer %= 2 * (float)Math.PI;

            distanceToPlayer = Splines.distanceToPoint(PhysicsEngine.player.position, position);

            if (itemHeld == 4)
            {
                useItem();
            }
            else if (itemHeld == 3)
            {
                if (Math.Sqrt(distToWaypoint.X * distToWaypoint.X + distToWaypoint.Y * distToWaypoint.Y) < 700)
                {
                    useItem();
                }
            }
            else if (distanceToPlayer < 700 && itemHeld > 0)
            {
                if ((Math.Abs(angleToPlayer - angle) < .1) && (angleToPlayer - angle) < 0 && itemHeld == 1)
                {
                    useItem();
                }
                else if (itemHeld == 2 && Math.Abs(angleToPlayer - angle) < .1) //shell
                {
                    useItem();
                }
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

            if (rand.Next(0, 100) < 7)
            {
                randomDrivingRadius = rand.Next(0, 100);
                randAngle = (float)(rand.NextDouble() * 2) * (float)Math.PI;
                prevRandomWaypoint = newRandomWaypoint;
                newRandomWaypoint = new Vector2((float)(allWaypoints[currentWaypoint].X + Math.Cos(randAngle) * randomDrivingRadius),
                                                (float)(allWaypoints[currentWaypoint].Y + Math.Sin(randAngle) * randomDrivingRadius));
            }
        }

        public void update(float dt)
        {
            prevPosition = new Vector2(position.X, position.Y);
            float prevVelocity = velocity.X;
            int prevId = id;

            // update various timers
            stunTime += dt;
            boostTime += dt;
            largeTime += dt;
            rollItemTime += dt;
            if(collideTimer > 0)
            {
                collideTimer -= dt;
            }
            dBoostTime += dt;

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

            id = PhysicsEngine.GetPhysicsID(position);
            Tuple<float, float, float> terrainConst = PhysicsEngine.terrainConsts[id];

            // when boosting

            if (boostTime < speedBoostConst)
            {
                throttle *= boostMultiplier;
                terrainConst = PhysicsEngine.terrainConsts[0];
            }

            if (largeTime < largeConst)
            {
                size = new Vector2(62.5f * largeMultiplier, 62.5f * largeMultiplier);
                terrainConst = PhysicsEngine.terrainConsts[0];
                radius = 48f;
            }
            else
            {
                size = new Vector2(62.5f, 62.5f);
                radius = 24f;
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
                tempA += throttle * throttleConst * (dBoostTime <= dBoostConst ? dBoostMultiplier : 1) * (1+coins/50);
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

            bool collided = false;
            if (minCollision != 1)
            {
                position = finalPos;
                wallCollide(0);
                collided = true;
            }

            // base.update(dt);

            //handle sounds
            if (!isAI && !collided)
            {
                if (id != prevId)
                {
                    Engine.StopSound(terrain);
                }
                else if (velocity.X == 0 && prevVelocity != 0)
                {
                    Engine.StopSound(terrain, fadeTime: 0.2f);
                }
                if (id != prevId || (prevVelocity == 0 && velocity.X != 0))
                {
                    terrain = Engine.PlaySound(Sounds.sounds["terrain" + id], repeat: true);
                }
                if (throttle == 0 && prevThrottle != 0)
                {
                    Engine.StopSound(rev);
                    rev = Engine.PlaySound(Sounds.sounds["zeroRev"], repeat:true);
                }
                else if (Math.Abs(throttle) >= 0.75f && Math.Abs(prevThrottle) < 0.75f)
                {
                    Engine.StopSound(rev);
                    rev = Engine.PlaySound(Sounds.sounds["highRev"], repeat:true);
                }
                else if ((0 != throttle && Math.Abs(throttle) < 0.75f) && (0 == prevThrottle || Math.Abs(prevThrottle) >= 0.75f))
                {
                    Engine.StopSound(rev);
                    rev = Engine.PlaySound(Sounds.sounds["lowRev"], repeat:true);
                }
            }

            percentDone();
        }

        public void wallCollide(float wallAngle)
        {
            if (!isAI && collideTimer <= 0)
            {
                Engine.PlaySound(Sounds.sounds["collide"]);
                collideTimer = 0.5f;
            }
            velocity.X = -velocity.X * 0.75f;
            throttle /= 2;
        }

        public override void collide(Kart kart)
        {
            if (!isAI && collideTimer <= 0)
            {
                Engine.PlaySound(Sounds.sounds["collide"]);
                collideTimer = 0.5f;
            }

            if (kart.largeTime < kart.largeConst)
            {
                stunTime = 0;
            }
            else if (largeTime < largeConst)
            {
                kart.stunTime = 0;
            }

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
            
            if (camFlipped)
            {
                if (curTex == 0)
                {
                    curTex = 14;
                }
                else
                {
                    curTex = -1 * Math.Sign(curTex) * (14 - Math.Abs(curTex));
                }
            }

            if (drifting)
            {
                curTex += Math.Sign(angularVelo) * (driftTime > 0.07f ? 2 : 1);
            }
        }

        public void driftBoost()
        {
            dBoostTime = 0;

            if (driftTime > 1.1f)
            {
                dBoostMultiplier = 1.8f;
            }
            else if (driftTime > 0.3f) {
                dBoostMultiplier = 1.3f;
            }

            dBoostConst = driftTime * 3;
        }

        public void hit()
        {
            stunTime = 0;
            if(!isAI)
            {
                Engine.PlaySound(Sounds.sounds["hit"]);
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