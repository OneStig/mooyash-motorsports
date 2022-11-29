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

        public static readonly Track genTrack = new Track(
        new List<Polygon>() {
new Polygon(new float[] {900, 960, 11230, 10940},
new float[] {910, 8390, 8310, 890}, new Color(31, 109, 4, 255)),
new Polygon(new float[] {2860, 1460, 1470, 2900, 9690, 10650, 10300, 9020},
new float[] {1410, 2530, 7200, 8090, 7980, 7190, 2330, 1530}, new Color(155, 118, 83, 255)),
new Polygon(new float[] {2820, 2550, 2380, 2210, 2080, 2020, 2860},
new float[] {2020, 2090, 2190, 2350, 2590, 2850, 2830}, new Color(128, 128, 128, 255)),
new Polygon(new float[] {2820, 9070, 9150, 2840},
new float[] {2060, 2010, 2880, 2840}, new Color(128, 128, 128, 255)),
new Polygon(new float[] {9070, 9250, 9450, 9590, 9800, 9870, 9920, 9030},
new float[] {2040, 2050, 2120, 2220, 2410, 2590, 2910, 2910}, new Color(128, 128, 128, 255)),
new Polygon(new float[] {2880, 2990, 2050, 2040},
new float[] {2800, 6890, 6920, 2870}, new Color(128, 128, 128, 255)),
new Polygon(new float[] {2080, 2170, 2490, 2830, 2980, 2990},
new float[] {6950, 7230, 7450, 7620, 7650, 6900}, new Color(128, 128, 128, 255)),
new Polygon(new float[] {2890, 2990, 2930, 2120, 2070},
new float[] {6810, 6940, 7060, 7030, 6910}, new Color(128, 128, 128, 255)),
new Polygon(new float[] {2970, 9400, 9350, 2900},
new float[] {7670, 7590, 6840, 6980}, new Color(128, 128, 128, 255)),
new Polygon(new float[] {9390, 9670, 9950, 10060, 10090, 9310},
new float[] {7610, 7540, 7340, 7140, 6790, 6880}, new Color(128, 128, 128, 255)),
new Polygon(new float[] {10100, 9920, 9020, 9300},
new float[] {6830, 2920, 2850, 6960}, new Color(128, 128, 128, 255)),
new Polygon(new float[] {9110, 8630, 9170},
new float[] {3200, 2850, 2750}, new Color(128, 128, 128, 255)),
new Polygon(new float[] {9370, 8940, 9450},
new float[] {6550, 6970, 6980}, new Color(128, 128, 128, 255)),
new Polygon(new float[] {3300, 2930, 2930},
new float[] {7060, 6700, 7150}, new Color(128, 128, 128, 255)),
new Polygon(new float[] {2810, 3210, 2780},
new float[] {3120, 2770, 2710}, new Color(128, 128, 128, 255)),
        },
         new List<Polygon>() {
new Polygon(new float[] {2050, 2920, 2940, 2050},
new float[] {3730, 3760, 3900, 3880}, new Color(255, 255, 255, 255)),
new Polygon(new float[] {3080, 3120, 3270, 3200},
new float[] {4570, 5260, 5250, 4610}, new Color(255, 0, 0, 255)),
new Polygon(new float[] {3270, 3130, 3140, 3300},
new float[] {5760, 5800, 6440, 6430}, new Color(255, 0, 0, 255)),
new Polygon(new float[] {3730, 4490, 4510, 3760},
new float[] {6790, 6780, 6930, 6940}, new Color(255, 0, 0, 255)),
new Polygon(new float[] {5240, 6030, 6030, 5310},
new float[] {6800, 6790, 6920, 6950}, new Color(255, 0, 0, 255)),
new Polygon(new float[] {6820, 6840, 7630, 7620},
new float[] {6800, 6920, 6900, 6780}, new Color(255, 0, 0, 255)),
new Polygon(new float[] {9260, 9020, 9000, 9170},
new float[] {6460, 6470, 5790, 5800}, new Color(255, 0, 0, 255)),
new Polygon(new float[] {9160, 8970, 8910, 9120},
new float[] {5050, 5060, 4410, 4410}, new Color(255, 0, 0, 255)),
new Polygon(new float[] {4020, 4040, 4940, 4930},
new float[] {2910, 3070, 3050, 2910}, new Color(0, 0, 255, 255)),
new Polygon(new float[] {5890, 5880, 6740, 6750},
new float[] {2920, 3090, 3100, 2910}, new Color(0, 0, 255, 255)),
new Polygon(new float[] {7380, 7400, 8170, 8160},
new float[] {2940, 3110, 3090, 2930}, new Color(0, 0, 255, 255)),
        });

        public static readonly Track testTrack = new Track(
            new List<Polygon>()
            {
                new Polygon(
                    new float[]
                    {
                        50, 100, 100, 50
                    },
                    new float[]
                    {
                        50, 50, 100, 100
                    }, new Color(255, 255, 255)
                    )
            },

            new List<Polygon>()
            );

        public Track(List<Polygon> interactable, List<Polygon> visual)
        {
            this.interactable = interactable;
            this.visual = visual;
        }

        public bool isConvex()
        {
            foreach (Polygon p in interactable)
            {
                if (!p.isConvex()) { return false; }
            }
            foreach (Polygon p in visual)
            {
                if (!p.isConvex()) { return false; }
            }
            return true;
        }
    }
}

