using SFML.System;
using System;

namespace Grafika3D
{
    public class mat4x4
    {
        public float[,] m = new float[4, 4];
       
        public static Vector3f operator *(Vector3f i, mat4x4 m)
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

        public static mat4x4 operator *(mat4x4 m1, mat4x4 m2)
        {
            mat4x4 matrix = new mat4x4();
            for (int c = 0; c < 4; c++)
                for (int r = 0; r < 4; r++)
                    matrix.m[r, c] = m1.m[r, 0] * m2.m[0, c] + m1.m[r, 1] * m2.m[1, c] + m1.m[r, 2] * m2.m[2, c] + m1.m[r, 3] * m2.m[3, c];
            return matrix;
        }
       
        public static Triangle XRotation(float fTheta, Triangle tri)
        {
            mat4x4 matRotX = new mat4x4();

            matRotX.m[0, 0] = 1;
            matRotX.m[1, 1] = (float)Math.Cos(fTheta * 0.5f);
            matRotX.m[1, 2] = (float)Math.Sin(fTheta * 0.5f);
            matRotX.m[2, 1] = (float)(-Math.Sin(fTheta * 0.5f));
            matRotX.m[2, 2] = (float)Math.Cos(fTheta * 0.5f);
            matRotX.m[3, 3] = 1;

            Triangle wynik = tri;
            wynik.v0 = tri.v0 * matRotX;
            wynik.v1 = tri.v1 * matRotX;
            wynik.v2 = tri.v2 * matRotX;

            return wynik;
        }

        public static Triangle YRotation(float fTheta, Triangle tri)
        {
            mat4x4 matRotY = new mat4x4();

            matRotY.m[0, 0] = (float)Math.Cos(fTheta);
            matRotY.m[0, 2] = (float)(-Math.Sin(fTheta));
            matRotY.m[2, 0] = (float)(Math.Sin(fTheta));
            matRotY.m[1, 1] = 1;
            matRotY.m[2, 2] = (float)Math.Cos(fTheta);
            matRotY.m[3, 3] = 1;

            Triangle wynik = tri;
            wynik.v0 = tri.v0 * matRotY;
            wynik.v1 = tri.v1 * matRotY;
            wynik.v2 = tri.v2 * matRotY;

            return wynik;
        }
      
        public static Triangle ZRotation(float fTheta, Triangle tri)
        {
            mat4x4 matRotZ = new mat4x4();

            matRotZ.m[0, 0] = (float)Math.Cos(fTheta);
            matRotZ.m[0, 1] = (float)Math.Sin(fTheta);
            matRotZ.m[1, 0] = (float)(-Math.Sin(fTheta));
            matRotZ.m[1, 1] = (float)Math.Cos(fTheta);
            matRotZ.m[2, 2] = 1;
            matRotZ.m[3, 3] = 1;

            Triangle wynik = tri;
            wynik.v0 = tri.v0 * matRotZ;
            wynik.v1 = tri.v1 * matRotZ;
            wynik.v2 = tri.v2 * matRotZ;

            return wynik;
        }
        
        public mat4x4 Matrix_MakeProjection(float fFovDegrees, float fAspectRatio, float fNear, float fFar)
        {
            float fFovRad = 1.0f / (float)Math.Tan(fFovDegrees * 0.5f / 180.0f * 3.14159f);
            m[0, 0] = fAspectRatio * fFovRad;
            m[1, 1] = fFovRad;
            m[2, 2] = fFar / (fFar - fNear);
            m[3, 2] = (-fFar * fNear) / (fFar - fNear);
            m[2, 3] = 1.0f;
            m[3, 3] = 0.0f;

            return this;
        }
    }
}
