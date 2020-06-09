using System;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System.Collections;
using System.Collections.Generic;

namespace Grafika3D
{
    class Program
    {
        public static uint Width = 1000;
        public static uint Height = 1000;
        public static RenderWindow window = new RenderWindow(new VideoMode(Width, Height), "Grafika3D");
        public static Engine silnik = Engine.Instance;
        public static Time deltaTime;

        static void OnClose(object sender, EventArgs e)
        {
            // Close the window when OnClose event is received
            RenderWindow window = (RenderWindow)sender;
            window.Close();
        }

        static void Main(string[] args)
        {
            Clock deltaClock = new Clock();
            window.Closed += new EventHandler(OnClose);
            deltaTime = deltaClock.Restart();
            Vector3f vCamera = new Vector3f(0,0,0);
            float fTheta = 0;

            Vector3f v00 = new Vector3f(0, 0, 0);
            Vector3f v01 = new Vector3f(0, 0, 1);
            Vector3f v02 = new Vector3f(0, 1, 0);
            Vector3f v03 = new Vector3f(0, 1, 1);
            Vector3f v04 = new Vector3f(1, 0, 0);
            Vector3f v05 = new Vector3f(1, 0, 1);
            Vector3f v06 = new Vector3f(1, 1, 0);
            Vector3f v07 = new Vector3f(1, 1, 1);

            
            Mesh meshCube = new Mesh()
            {
                // SOUTH
                new Triangle( v00, v02, v06 ),
                new Triangle( v00, v06, v04 ),

		        // EAST                                                      
		        new Triangle( v04, v06, v07 ),
                new Triangle( v04, v07, v05 ),

		        // NORTH                                                     
		        new Triangle( v05, v07, v03 ),
                new Triangle( v05, v03, v01 ),

		        // WEST                                                      
		        new Triangle( v01, v03, v02 ),
                new Triangle( v01, v02, v00 ),

		        // TOP                                                       
		        new Triangle( v02, v03, v07 ),
                new Triangle( v02, v07, v06 ),
                                          
		        // BOTTOM                                                    
		        new Triangle( v05, v01, v00 ),
                new Triangle( v05, v00, v04 ),
            };

            // Projection Matrix
            float fNear = 0.1f;
            float fFar = 1000.0f;
            float fFov = 90.0f;
            float fAspectRatio = Width / Height;
            float fFovRad = 1.0f / (float)Math.Tan(fFov * 0.5f / 180.0f * 3.14159f);
            

            mat4x4 matProj = new mat4x4();

            matProj.m[0,0] = fAspectRatio * fFovRad;
            matProj.m[1,1] = fFovRad;
            matProj.m[2,2] = fFar / (fFar - fNear);
            matProj.m[3,2] = (-fFar * fNear) / (fFar - fNear);
            matProj.m[2,3] = 1.0f;
            matProj.m[3,3] = 0.0f;

            //making triangle throught drawing lines
            VertexArray trojkat(Triangle troj)
            {
                VertexArray wynik = new VertexArray(PrimitiveType.Lines, 6);
                wynik[0] = new Vertex(new Vector2f(troj.v0.X, troj.v0.Y), Color.White);
                wynik[1] = new Vertex(new Vector2f(troj.v1.X, troj.v1.Y), Color.White);
                wynik[2] = new Vertex(new Vector2f(troj.v1.X, troj.v1.Y), Color.White);
                wynik[3] = new Vertex(new Vector2f(troj.v2.X, troj.v2.Y), Color.White);
                wynik[4] = new Vertex(new Vector2f(troj.v2.X, troj.v2.Y), Color.White);
                wynik[5] = new Vertex(new Vector2f(troj.v0.X, troj.v0.Y), Color.White);
                //Console.WriteLine(troj.v0.X + " " + troj.v0.Y);
                //Console.WriteLine(troj.v1.X + " " + troj.v1.Y);
                //Console.WriteLine(troj.v2.X + " " + troj.v2.Y);
                //Console.WriteLine();
                return wynik;
            }

            VertexArray wypelnionyTrojkat(Triangle troj)
            {
                VertexArray wynik = new VertexArray(PrimitiveType.Triangles, 3);
                wynik[0] = new Vertex(new Vector2f(troj.v0.X, troj.v0.Y), Color.White);
                wynik[1] = new Vertex(new Vector2f(troj.v1.X, troj.v1.Y), Color.White);
                wynik[2] = new Vertex(new Vector2f(troj.v2.X, troj.v2.Y), Color.White);

                return wynik;
            }

            // Start the game loop
            while (window.IsOpen)
            {
                // Process events
                window.DispatchEvents();

                // Clear screen
                window.Clear();

                fTheta += 0.5f*(float)Math.PI*deltaTime.AsSeconds();

                mat4x4 matRotZ = new mat4x4();
                // Rotation Z
                matRotZ.m[0, 0] = (float)Math.Cos(fTheta);
                matRotZ.m[0, 1] = (float)Math.Sin(fTheta);
                matRotZ.m[1, 0] = (float)(-Math.Sin(fTheta));
                matRotZ.m[1, 1] = (float)Math.Cos(fTheta);
                matRotZ.m[2, 2] = 1;
                matRotZ.m[3, 3] = 1;

                mat4x4 matRotX = new mat4x4();

                // Rotation X
                matRotX.m[0, 0] = 1;
                matRotX.m[1, 1] = (float)Math.Cos(fTheta * 0.5f);
                matRotX.m[1, 2] = (float)Math.Sin(fTheta * 0.5f);
                matRotX.m[2, 1] = (float)(-Math.Sin(fTheta * 0.5f));
                matRotX.m[2, 2] = (float)Math.Cos(fTheta * 0.5f);
                matRotX.m[3, 3] = 1;

                foreach (var tri in meshCube)
                {
                    Triangle triProjected, triTranslated, triRotatedZ, triRotatedZX;

                    //Rotate in Z-Axis
                    triRotatedZ = tri;
                    triRotatedZ.v0 = matProj.MultiplyVecMatr(triRotatedZ.v0, matRotZ);
                    triRotatedZ.v1 = matProj.MultiplyVecMatr(triRotatedZ.v1, matRotZ);
                    triRotatedZ.v2 = matProj.MultiplyVecMatr(triRotatedZ.v2, matRotZ);

                    //Rotate in X-Axis
                    triRotatedZX.v0 = matProj.MultiplyVecMatr(triRotatedZ.v0, matRotX);
                    triRotatedZX.v1 = matProj.MultiplyVecMatr(triRotatedZ.v1, matRotX);
                    triRotatedZX.v2 = matProj.MultiplyVecMatr(triRotatedZ.v2, matRotX);

                    //Offset into the screen
                    triTranslated = triRotatedZX;
                    triTranslated.v0.Z = triRotatedZX.v0.Z + 3.0f;
                    triTranslated.v1.Z = triRotatedZX.v1.Z + 3.0f;
                    triTranslated.v2.Z = triRotatedZX.v2.Z + 3.0f;

                    Vector3f normal, line1, line2;
                    line1.X = triTranslated.v1.X - triTranslated.v0.X;
                    line1.Y = triTranslated.v1.Y - triTranslated.v0.Y;
                    line1.Z = triTranslated.v1.Z - triTranslated.v0.Z;

                    line2.X = triTranslated.v2.X - triTranslated.v0.X;
                    line2.Y = triTranslated.v2.Y - triTranslated.v0.Y;
                    line2.Z = triTranslated.v2.Z - triTranslated.v0.Z;

                    normal.X = line1.Y * line2.Z - line1.Z * line2.Y;
                    normal.Y = line1.Z * line2.X - line1.X * line2.Z;
                    normal.Z = line1.X * line2.Y - line1.Y * line2.X;

                    // It's normally normal to normalise the normal
                    double l = Math.Sqrt(normal.X * normal.X + normal.Y * normal.Y + normal.Z * normal.Z);
                    normal.X /= (float)l; normal.Y /= (float)l; normal.Z /= (float)l;

                    //if (normal.Z < 0)
                    if(normal.X * (triTranslated.v0.X - vCamera.X) +
                       normal.Y * (triTranslated.v0.Y - vCamera.Y) +
                       normal.Z * (triTranslated.v0.Z - vCamera.Z) < 0.0)
                    {
                        //Illumination
                        Vector3f light_direction = new Vector3f(0.0f, 0.0f, -1.0f);
                        double i = Math.Sqrt(light_direction.X * light_direction.X + light_direction.Y * light_direction.Y + light_direction.Z * light_direction.Z);
                        light_direction.X /= (float)i; light_direction.Y /= (float)i; light_direction.Z /= (float)i;

                        // How similar is normal to light direction
                        float dp = normal.X * light_direction.X + normal.Y * light_direction.Y + normal.Z * light_direction.Z;

                        //Project triangles from 3D -> 2D
                        triProjected.v0 = matProj.MultiplyVecMatr(triTranslated.v0, matProj);
                        triProjected.v1 = matProj.MultiplyVecMatr(triTranslated.v1, matProj);
                        triProjected.v2 = matProj.MultiplyVecMatr(triTranslated.v2, matProj);

                        triProjected.v0.X += 1f; triProjected.v0.Y += 1f;
                        triProjected.v1.X += 1f; triProjected.v1.Y += 1f;
                        triProjected.v2.X += 1f; triProjected.v2.Y += 1f;

                        triProjected.v0.X *= 0.5f * Width;
                        triProjected.v0.Y *= 0.5f * Height;
                        triProjected.v1.X *= 0.5f * Width;
                        triProjected.v1.Y *= 0.5f * Height;
                        triProjected.v2.X *= 0.5f * Width;
                        triProjected.v2.Y *= 0.5f * Height;

                        var trojk = wypelnionyTrojkat(triProjected);

                        window.Draw(trojk);
                    }
                }
                // Update the window
                window.Display();
                deltaTime = deltaClock.Restart();
            }
        }
    }
}
