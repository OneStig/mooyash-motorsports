using System;
using Mooyash.Modules;

namespace Mooyash.Services
{
    public static class PhysicsEngine
    {
        public static Kart playerKart;

        public static void init()
        {
            playerKart = new Kart();
        }
    }
}

