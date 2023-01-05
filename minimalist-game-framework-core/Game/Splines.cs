using System;
using System.Collections.Generic;
using System.Linq;
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

        //gets two closest waypoints to the player
        public static Vector2[] getClosestPoints(Vector2 cartLocation, List<Vector2> waypoints)
        {
            return waypoints.Select(point => new
            {
                Point = point,
                Distance = distanceToPoint(point, cartLocation)


            })
                .OrderBy(p => p.Distance)
                .Take(2)
                .Select(p => p.Point)
                .ToArray();
        }

        //returns the distance from one point to another
        private static double distanceToPoint(Vector2 point1, Vector2 point2)
        {
            double a = point2.X - point1.X;
            double b = point2.Y - point1.Y;

            return Math.Sqrt(a * a + b * b);
        }

        //where p is the cartLocation, finds the distance to the closest point on a line from a to b to the cart
        public static double getClosestDistance(Vector2 a, Vector2 b, Vector2 cartLocation) 
        {
            Vector2 aToCart = cartLocation - a;
            Vector2 aToB = b - a;

            double atbMagnitude = aToB.Length();
            double dotProduct = Vector2.Dot(aToCart, aToB);

            double percent = dotProduct/ (atbMagnitude * atbMagnitude);

            Vector2 closestPoint = new Vector2((float) (a.X + a.X * percent), (float) (a.Y + a.Y * percent));

            if (percent < 0)
            {
                return distanceToPoint(cartLocation, a);
            }
            else if (percent > 1)
            {
                return distanceToPoint(cartLocation, b);
            }
            else 
            {
                return distanceToPoint(cartLocation, closestPoint);
            }
        }
    }
}

