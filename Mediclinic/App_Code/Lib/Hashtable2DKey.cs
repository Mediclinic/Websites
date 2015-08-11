using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hashtable2D
{
    public struct Key
    {
        public static Key New(int p1, int p2)
        {
            return new Key(p1, p2);
        }

        public readonly int Dimension1;
        public readonly int Dimension2;
        public Key(int p1, int p2)
        {
            Dimension1 = p1;
            Dimension2 = p2;
        }
        // Equals and GetHashCode ommitted
    }
}