using SFML.System;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grafika3D
{
    public struct Triangle
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

        public Triangle Slide(float x, float y)
        {
            v0.X += x;
            v1.X += x;
            v2.X += x;
            v0.Y += y;
            v1.Y += y;
            v2.Y += y;

            return this;
        }

        public Triangle Zooming(float d)
        {
            v0.Z += d;
            v1.Z += d;
            v2.Z += d;

            return this;
        }
    }
}
