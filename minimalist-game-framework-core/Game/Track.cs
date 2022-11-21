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
new Polygon(new float[] {90, 96, 1123, 1094},
new float[] {91, 839, 831, 89}, new Color(31, 109, 4, 255)),
new Polygon(new float[] {286, 146, 147, 290, 969, 1065, 1030, 902},
new float[] {141, 253, 720, 809, 798, 719, 233, 153}, new Color(155, 118, 83, 255)),
new Polygon(new float[] {282, 255, 238, 221, 208, 202, 286},
new float[] {202, 209, 219, 235, 259, 285, 283}, new Color(128, 128, 128, 255)),
new Polygon(new float[] {282, 907, 915, 284},
new float[] {206, 201, 288, 284}, new Color(128, 128, 128, 255)),
new Polygon(new float[] {907, 925, 945, 959, 980, 987, 992, 903},
new float[] {204, 205, 212, 222, 241, 259, 291, 291}, new Color(128, 128, 128, 255)),
new Polygon(new float[] {288, 299, 205, 204},
new float[] {280, 689, 692, 287}, new Color(128, 128, 128, 255)),
new Polygon(new float[] {208, 217, 249, 283, 298, 299},
new float[] {695, 723, 745, 762, 765, 690}, new Color(128, 128, 128, 255)),
new Polygon(new float[] {289, 299, 293, 212, 207},
new float[] {681, 694, 706, 703, 691}, new Color(128, 128, 128, 255)),
new Polygon(new float[] {297, 940, 935, 290},
new float[] {767, 759, 684, 698}, new Color(128, 128, 128, 255)),
new Polygon(new float[] {939, 967, 995, 1006, 1009, 931},
new float[] {761, 754, 734, 714, 679, 688}, new Color(128, 128, 128, 255)),
new Polygon(new float[] {1010, 992, 902, 930},
new float[] {683, 292, 285, 696}, new Color(128, 128, 128, 255)),
new Polygon(new float[] {911, 863, 917},
new float[] {320, 285, 275}, new Color(128, 128, 128, 255)),
new Polygon(new float[] {937, 894, 945},
new float[] {655, 697, 698}, new Color(128, 128, 128, 255)),
new Polygon(new float[] {330, 293, 293},
new float[] {706, 670, 715}, new Color(128, 128, 128, 255)),
new Polygon(new float[] {281, 321, 278},
new float[] {312, 277, 271}, new Color(128, 128, 128, 255)),
},
 new List<Polygon>() {
new Polygon(new float[] {205, 292, 294, 205},
new float[] {373, 376, 390, 388}, new Color(255, 255, 255, 255)),
new Polygon(new float[] {308, 312, 327, 320},
new float[] {457, 526, 525, 461}, new Color(255, 0, 0, 255)),
new Polygon(new float[] {327, 313, 314, 330},
new float[] {576, 580, 644, 643}, new Color(255, 0, 0, 255)),
new Polygon(new float[] {373, 449, 451, 376},
new float[] {679, 678, 693, 694}, new Color(255, 0, 0, 255)),
new Polygon(new float[] {524, 603, 603, 531},
new float[] {680, 679, 692, 695}, new Color(255, 0, 0, 255)),
new Polygon(new float[] {682, 684, 763, 762},
new float[] {680, 692, 690, 678}, new Color(255, 0, 0, 255)),
new Polygon(new float[] {926, 902, 900, 917},
new float[] {646, 647, 579, 580}, new Color(255, 0, 0, 255)),
new Polygon(new float[] {916, 897, 891, 912},
new float[] {505, 506, 441, 441}, new Color(255, 0, 0, 255)),
new Polygon(new float[] {402, 404, 494, 493},
new float[] {291, 307, 305, 291}, new Color(0, 0, 255, 255)),
new Polygon(new float[] {589, 588, 674, 675},
new float[] {292, 309, 310, 291}, new Color(0, 0, 255, 255)),
new Polygon(new float[] {738, 740, 817, 816},
new float[] {294, 311, 309, 293}, new Color(0, 0, 255, 255)),
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
    }
}

