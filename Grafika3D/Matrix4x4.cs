using SFML.System;
using System;

namespace Grafika3D
{
    public class Matrix4x4
    {
        public float[,] m = new float[4, 4];
       
        public static Vector3f operator *(Vector3f i, Matrix4x4 m)
        {
            Vector3f vec = new Vector3f
            {
                X = i.X * m.m[0, 0] + i.Y * m.m[1, 0] + i.Z * m.m[2, 0] + m.m[3, 0],
                Y = i.X * m.m[0, 1] + i.Y * m.m[1, 1] + i.Z * m.m[2, 1] + m.m[3, 1],
                Z = i.X * m.m[0, 2] + i.Y * m.m[1, 2] + i.Z * m.m[2, 2] + m.m[3, 2]
            };
            float w = i.X * m.m[0, 3] + i.Y * m.m[1, 3] + i.Z * m.m[2, 3] + m.m[3, 3];

            if (w != 0.0f)
            {
                vec.X /= w; vec.Y /= w; vec.Z /= w;
            }

            return vec;
        }

        public static Matrix4x4 operator *(Matrix4x4 m1, Matrix4x4 m2)
        {
            Matrix4x4 matrix = new Matrix4x4();
            for (int c = 0; c < 4; c++)
                for (int r = 0; r < 4; r++)
                    matrix.m[r, c] = m1.m[r, 0] * m2.m[0, c] + m1.m[r, 1] * m2.m[1, c] + m1.m[r, 2] * m2.m[2, c] + m1.m[r, 3] * m2.m[3, c];
            return matrix;
        }

        public static Triangle Move(float x, float y, float z, Triangle tri)
        {
            Matrix4x4 matMove = new Matrix4x4();

            matMove.m[0, 0] = 1;
            matMove.m[1, 1] = 1;
            matMove.m[2, 2] = 1;
            matMove.m[3, 3] = 1;
            matMove.m[3, 0] = x;
            matMove.m[3, 1] = y;
            matMove.m[3, 2] = z;

            Triangle result = tri;
            result.v0 = tri.v0 * matMove;
            result.v1 = tri.v1 * matMove;
            result.v2 = tri.v2 * matMove;

            return result;
        }

        public static Triangle XRotation(float fTheta, Triangle tri)
        {
            Matrix4x4 matRotX = new Matrix4x4();

            matRotX.m[0, 0] = 1;
            matRotX.m[1, 1] = (float)Math.Cos(fTheta * 0.5f);
            matRotX.m[1, 2] = (float)Math.Sin(fTheta * 0.5f);
            matRotX.m[2, 1] = (float)(-Math.Sin(fTheta * 0.5f));
            matRotX.m[2, 2] = (float)Math.Cos(fTheta * 0.5f);
            matRotX.m[3, 3] = 1;

            Triangle result = tri;
            result.v0 = tri.v0 * matRotX;
            result.v1 = tri.v1 * matRotX;
            result.v2 = tri.v2 * matRotX;

            return result;
        }

        public static Triangle YRotation(float fTheta, Triangle tri)
        {
            Matrix4x4 matRotY = new Matrix4x4();

            matRotY.m[0, 0] = (float)Math.Cos(fTheta);
            matRotY.m[0, 2] = (float)(-Math.Sin(fTheta));
            matRotY.m[2, 0] = (float)(Math.Sin(fTheta));
            matRotY.m[1, 1] = 1;
            matRotY.m[2, 2] = (float)Math.Cos(fTheta);
            matRotY.m[3, 3] = 1;

            Triangle result = tri;
            result.v0 = tri.v0 * matRotY;
            result.v1 = tri.v1 * matRotY;
            result.v2 = tri.v2 * matRotY;

            return result;
        }
      
        public static Triangle ZRotation(float fTheta, Triangle tri)
        {
            Matrix4x4 matRotZ = new Matrix4x4();

            matRotZ.m[0, 0] = (float)Math.Cos(fTheta);
            matRotZ.m[0, 1] = (float)Math.Sin(fTheta);
            matRotZ.m[1, 0] = (float)(-Math.Sin(fTheta));
            matRotZ.m[1, 1] = (float)Math.Cos(fTheta);
            matRotZ.m[2, 2] = 1;
            matRotZ.m[3, 3] = 1;

            Triangle result = tri;
            result.v0 = tri.v0 * matRotZ;
            result.v1 = tri.v1 * matRotZ;
            result.v2 = tri.v2 * matRotZ;

            return result;
        }

        //fielOfView in degrees
        public Matrix4x4 Matrix_MakeProjection(float fielOfView, float fAspectRatio, float zMin, float zMax)
        {
            float fFovRad = 1.0f / (float)Math.Tan(fielOfView * 0.5f / 180.0f * Math.PI);
            m[0, 0] = fAspectRatio * fFovRad;
            m[1, 1] = fFovRad;
            m[2, 2] = zMax / (zMax - zMin);
            m[3, 2] = (-zMax * zMin) / (zMax - zMin);
            m[2, 3] = 1.0f;
            m[3, 3] = 0.0f;

            return this;
        }
    }
}
