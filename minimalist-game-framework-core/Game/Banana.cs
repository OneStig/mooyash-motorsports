using System;

namespace Mooyash.Modules
{
    public class Banana : GameObject // Equivalent of mario kart mystery box
    {
        bool exists;
        float radius;

        public Banana(Vector2 position) : base()
        {
            this.position = position;

            exists = true;
            radius = 0.5f;
        }

        public void collide(Kart k)
        {
            exists = false;
            k.stunTime = 0;
        }
    }
}

