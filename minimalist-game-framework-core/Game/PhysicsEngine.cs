using System;
using System.Collections.Generic;
using Mooyash.Modules;

namespace Mooyash.Services
{
    public static class PhysicsEngine
    {
        public static Dictionary<string, GameObject> gameObjects;
        public static Kart player;

        public static void init()
        {
            player = new Kart();
            gameObjects = new Dictionary<string, GameObject>();
            gameObjects.Add("player", player);
        }

        public static void update(float dt)
        {
            player.updateInput(dt);
            player.update(dt);

            RenderEngine.camera.followKart(player);
        }
    }
}

