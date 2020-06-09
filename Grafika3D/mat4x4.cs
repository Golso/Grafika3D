using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grafika3D
{
    public class mat4x4
    {
        public float[,] m = new float[4, 4];

        public Vector3f MultiplyVecMatr(Vector3f i, mat4x4 m)
        {
            Vector3f o = new Vector3f();
            o.X = i.X * m.m[0, 0] + i.Y * m.m[1, 0] + i.Z * m.m[2, 0] + m.m[3, 0];
            o.Y = i.X * m.m[0, 1] + i.Y * m.m[1, 1] + i.Z * m.m[2, 1] + m.m[3, 1];
            o.Z = i.X * m.m[0, 2] + i.Y * m.m[1, 2] + i.Z * m.m[2, 2] + m.m[3, 2];
            float w = i.X * m.m[0, 3] + i.Y * m.m[1, 3] + i.Z * m.m[2, 3] + m.m[3, 3];

            if (w != 0.0f)
            {
                o.X /= w; o.Y /= w; o.Z /= w;
            }

            return o;
        }
    }
}
