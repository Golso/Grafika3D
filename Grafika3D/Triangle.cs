using SFML.System;
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

        public Triangle(Vector3f v0, Vector3f v1, Vector3f v2)
        {
            this.v0 = v0;
            this.v1 = v1;
            this.v2 = v2;
        }
    }
}
