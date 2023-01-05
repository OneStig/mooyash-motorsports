using System;
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

        //returns the two closest points to the player, which forms a line segment
        public Vector2[] getClosestSegment(Vector2 cartLocation) 
        {
            return null;
        }

        //where p is the cartLocation, finds the closest point on a line from a to b to the cart
        public double getDistanceToSpline(Vector2 a, Vector2 b, Vector2 cartLocation) 
        {
            Vector2 aToCart = cartLocation - a;
            Vector2 aToB = b - a;

            double atcMagnitude = aToCart.Length() * aToCart.Length();
            double dotProduct = Vector2.Dot(aToCart, aToB);

            double percent = dotProduct/ atcMagnitude;

            Vector2 closestPoint = new Vector2((float) (a.X + a.X * percent), (float) (a.Y + a.Y * percent));

            return (Math.Sqrt(Math.Pow(closestPoint.X - cartLocation.X, 2) + Math.Pow(closestPoint.Y - cartLocation.Y, 2)));

        }
    }
}

