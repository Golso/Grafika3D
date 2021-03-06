﻿using SFML.System;
using SFML.Graphics;
using System;

namespace Grafika3D
{
    public struct Triangle :IComparable<Triangle>
    {
        public Vector3f v0;
        public Vector3f v1;
        public Vector3f v2;
        public Color color;

        public Triangle(Vector3f v0, Vector3f v1, Vector3f v2, Color color)
        {
            this.v0 = v0;
            this.v1 = v1;
            this.v2 = v2;
            this.color = color;
        }

        public int CompareTo(Triangle other)
        {
            float depth = (v0.Z + v1.Z + v2.Z) / 3;
            float depthOfOther = (other.v0.Z + other.v1.Z + other.v2.Z) / 3;
            if (depth > depthOfOther)
                return -1;
            else return 1;
        }

        public Triangle SlideAndZoom(float x, float y, float z)
        {
            v0.X += x;
            v1.X += x;
            v2.X += x;

            v0.Y += y;
            v1.Y += y;
            v2.Y += y;

            v0.Z += z;
            v1.Z += z;
            v2.Z += z;

            return this;
        }
    }
}
