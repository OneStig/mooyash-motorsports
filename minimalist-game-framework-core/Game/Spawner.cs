using System;
using Mooyash.Services;

namespace Mooyash.Modules
{
    public class Spawner
    {
        // distance to be away for item respawn
        public static readonly float spawnDist = 3500;

        GameObject gameObject;

        public Spawner(GameObject gameObject)
        {
            this.gameObject = gameObject;
        }

        public void update(Kart k)
        {
            if (dist(k.position, gameObject.position) > spawnDist)
            {
                PhysicsEngine.gameObjects.Add(gameObject);
                PhysicsEngine.spawners.Remove(this);
            }
        }

        private float dist(Vector2 a, Vector2 b)
        {
            return (float)Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));
        }
    }
}

