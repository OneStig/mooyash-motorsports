﻿using System;
namespace Mooyash.Services
{
    public class Splines
    {
        /*
        228, 654
        264, 704
        523, 668
        869, 692
        905, 629
        742, 480
        608, 513
        546, 449
        634, 330
        579, 239
        256, 232
        226, 284
        */

        //stores all the points in the spline for a specific track
        public Vector2[] points;
        public Splines(Vector2[] points)
        {
            this.points = points;
        }

        //returns the 
        public Vector2[] getTurnAngle(Vector2 cartLocation) 
        {
            Vector2[] points = new Vector2[2];
            return null;
        }

        //where p is the cartLocation, finds the closest point on a line from a to b to the cart
        public Vector2 getClosestPoint(Vector2 a, Vector2 b, Vector2 cartLocation) 
        {
            Vector2 aToCart = cartLocation - a;
            Vector2 aToB = b - a;

            double atcMagnitude = aToCart.Length() * aToCart.Length();
            double dotProduct = Vector2.Dot(aToCart, aToB);

            double percent = dotProduct/ atcMagnitude;

            return new Vector2((float) (a.X + a.X * percent), (float) (a.Y + a.Y * percent));

        }
    }
}
