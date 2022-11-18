using System;
using Mooyash.Modules;

namespace Mooyash.Services
{
    public class Camera
    {

    }

    public static class RenderEngine
    {
        static Camera camera;

        public static void drawPerTrack(Track t)
        {
            foreach (Polygon p in t.interactable) {
                Engine.DrawConvexPolygon(p);
            }

            foreach (Polygon p in t.visual) {
                Engine.DrawConvexPolygon(p);
            }
        }
    }
}

