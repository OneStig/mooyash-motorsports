using System;
using System.Collections.Generic;

namespace Mooyash.Modules
{
    public class Track
    {
        public List<Polygon> interactable;
        public List<Polygon> visual;

        public Track(List<Polygon> interactable, List<Polygon> visual)
        {
            this.interactable = interactable;
            this.visual = visual;
        }
    }
}

