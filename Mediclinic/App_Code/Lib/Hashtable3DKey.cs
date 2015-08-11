using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hashtable3D
{
    public struct Key
    {
        public static Key New(int p1, int p2, int p3)
        {
            return new Key(p1, p2, p3);
        }

        public readonly int Dimension1;
        public readonly int Dimension2;
        public readonly int Dimension3;
        public Key(int p1, int p2, int p3)
        {
            Dimension1 = p1;
            Dimension2 = p2;
            Dimension3 = p3;
        }
        // Equals and GetHashCode ommitted
    }

    public struct KeyWithDate
    {
        public static KeyWithDate New(int p1, int p2, DateTime p3)
        {
            return new KeyWithDate(p1, p2, p3);
        }

        public readonly int Dimension1;
        public readonly int Dimension2;
        public readonly DateTime Dimension3;
        public KeyWithDate(int p1, int p2, DateTime p3)
        {
            Dimension1 = p1;
            Dimension2 = p2;
            Dimension3 = p3;
        }
        // Equals and GetHashCode ommitted
    }

}