using System;

namespace Mooyash.Modules
{
    public class GameObject
    {
        public Vector2 position;
        public double angle; //radians counter-clockwise to positive x-axis
        public Polygon hitbox;
        public Texture[] textures;
    }

    public class Kart : GameObject
    {
        public Vector2 velocity;
        public Vector2 acceleration;
        public double throttle;
        public double turnAngle;
        public bool stunned;
    }

    public class Item : GameObject
    {
        public String name;
        public Item()
        {

        }

    }
}