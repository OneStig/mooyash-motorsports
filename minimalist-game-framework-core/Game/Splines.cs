using System;
using System.Collections.Generic;
using System.Linq;
namespace Mooyash.Services
{
    public class Splines
    {
        //stores all the points in the spline for a specific track
        public Vector2[] points;
        public Splines(Vector2[] points)
        {
            this.points = points;
        }

        //gets two closest waypoints to the player
        public static float[] getClosestPoints(Vector2 position, int prevWaypoint, int curWaypoint, List<Vector2> waypoints)
        {
            float distToPrev = distanceToPoint(position, waypoints[prevWaypoint]);
            float distToCur = distanceToPoint(position, waypoints[curWaypoint]);
            float distToNext = distanceToPoint(position, waypoints[(curWaypoint + 1) % waypoints.Count]);
            return new float[] { distToPrev, distToCur, distToNext };
        }

        //returns the distance from one point to another
        private static float distanceToPoint(Vector2 point1, Vector2 point2)
        {
            double a = point2.X - point1.X;
            double b = point2.Y - point1.Y;

            return (float)Math.Sqrt(a * a + b * b);
        }

        //where p is the cartLocation, finds the distance to the closest point on a line from a to b to the cart
        public static float getPercentageProgress(Vector2 a, Vector2 b, Vector2 cartLocation) 
        {
            Vector2 aToCart = cartLocation - a;
            Vector2 aToB = b - a;

            float atbMagnitude = aToB.Length();
            float dotProduct = Vector2.Dot(aToCart, aToB);

            if (dotProduct == 0)
            {
                return 0;
            }

            float percent = dotProduct/ (atbMagnitude * atbMagnitude);

            return percent*100;
        }
    }
}

