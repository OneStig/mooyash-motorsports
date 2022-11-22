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

        public Track(List<Polygon> interactable, List<Polygon> visual)
        {
            this.interactable = interactable;
            this.visual = visual;
        }
    }
}

