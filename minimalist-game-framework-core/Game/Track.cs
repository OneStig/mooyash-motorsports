using System;
using System.Collections.Generic;

namespace Mooyash.Modules
{
    public class Track
    {
        public List<Polygon> interactable;
        public List<Polygon> visual;

        public static readonly Color dirt = new Color(155, 118, 83);
        public static readonly Color grass = new Color(31, 109, 4);

        public static readonly Track templateTrack = new Track(
            new List<Polygon>() // interactable polys
            {
                new Polygon(
                    new int[] { // x values
                        300,
                        200,
                        175,
                        300,
                        400
                    },
                    new int[] { // y values
                        200,
                        300,
                        400,
                        400,
                        300
                    }, Track.dirt
                ),

                new Polygon(
                    new int[] { // x values
                        175,
                        175,
                        300,
                        300
                    },
                    new int[] { // y values
                        400,
                        500,
                        500,
                        400
                    }, Track.dirt
                ),

                new Polygon(
                    new int[] { // x values
                        300,
                        175,
                        200,
                        300,
                        400,
                        400
                    },
                    new int[] { // y values
                        500,
                        500,
                        600,
                        700,
                        725,
                        600,
                    }, Track.dirt
                )
            },
            new List<Polygon>() // visual polys
            {
                new Polygon(
                    new int[] { // x values
                        185,
                        185,
                        285,
                        285
                    },
                    new int[] { // y values
                        405,
                        505,
                        505,
                        405
                    }, new Color(255, 0, 0)
                )
            }
        );

        public Track(List<Polygon> interactable, List<Polygon> visual)
        {
            this.interactable = interactable;
            this.visual = visual;
        }
    }
}

