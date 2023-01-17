﻿using System;
using Mooyash.Services;

namespace Mooyash.Modules
{
    public class Manure : GameObject
    {
        public Manure(Vector2 position) : base()
        {
            this.position = position;

            texture = Engine.LoadTexture("manure.png");
            size = new Vector2(62.5f, 62.5f);
            resolution = new Vector2(32, 32);

            radius = 30f;
        }

        public override void collide(Kart k)
        {
            if(k.score < 2)
            {
                k.score = 0;
            } else
            {
                k.score -= 2;
            }

            k.stunTime = 0;

            PhysicsEngine.gameObjects.Remove(this);
        }
    }
}
