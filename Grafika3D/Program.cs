﻿using System;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System.Collections;
using System.Collections.Generic;

namespace Grafika3D
{
    class Program
    {
        public static uint Width = 800;
        public static uint Height = 800;
        public static RenderWindow window = new RenderWindow(new VideoMode(Width, Height), "Grafika3D");
        public static Time deltaTime;

        static void OnClose(object sender, EventArgs e)
        {
            // Close the window when OnClose event is received
            RenderWindow window = (RenderWindow)sender;
            window.Close();
        }

        static float d = 5.0f;
        static float XTheta = 0;
        static float YTheta = 0;
        static float ZTheta = 0;

        static void Main(string[] args)
        {
            Clock deltaClock = new Clock();
            window.Closed += new EventHandler(OnClose);
            window.KeyPressed += Window_KeyPressed;
            window.KeyReleased += Window_KeyReleased;
            deltaTime = deltaClock.Restart();
            Vector3f vCamera = new Vector3f(0,0,0);

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
                new Triangle( v00, v02, v06, Color.Blue ),
                new Triangle( v00, v06, v04, Color.Blue ),

		        // EAST                                                      
		        new Triangle( v04, v06, v07, Color.Cyan ),
                new Triangle( v04, v07, v05, Color.Cyan ),

		        // NORTH                                                     
		        new Triangle( v05, v07, v03, Color.Green ),
                new Triangle( v05, v03, v01, Color.Green ),

		        // WEST                                                      
		        new Triangle( v01, v03, v02, Color.Magenta ),
                new Triangle( v01, v02, v00, Color.Magenta ),

		        // TOP                                                       
		        new Triangle( v02, v03, v07, Color.Red ),
                new Triangle( v02, v07, v06, Color.Red ),
                                          
		        // BOTTOM                                                    
		        new Triangle( v05, v01, v00, Color.Yellow ),
                new Triangle( v05, v00, v04, Color.Yellow ),
            };

            // Projection Matrix
            float fNear = 0.1f;
            float fFar = 1000.0f;
            float fFov = 90.0f;
            float fAspectRatio = Width / Height;
            float fFovRad = 1.0f / (float)Math.Tan(fFov * 0.5f / 180.0f * 3.14159f);
            

            mat4x4 matProj = new mat4x4();
            matProj.Matrix_MakeProjection(fFov, fAspectRatio, fNear, fFar);

            //making triangle throught drawing lines
            VertexArray trojkat(Triangle troj)
            {
                VertexArray wynik = new VertexArray(PrimitiveType.Lines, 6);
                wynik[0] = new Vertex(new Vector2f(troj.v0.X, troj.v0.Y), Color.Black);
                wynik[1] = new Vertex(new Vector2f(troj.v1.X, troj.v1.Y), Color.Black);
                wynik[2] = new Vertex(new Vector2f(troj.v1.X, troj.v1.Y), Color.Black);
                wynik[3] = new Vertex(new Vector2f(troj.v2.X, troj.v2.Y), Color.Black);
                wynik[4] = new Vertex(new Vector2f(troj.v2.X, troj.v2.Y), Color.Black);
                wynik[5] = new Vertex(new Vector2f(troj.v0.X, troj.v0.Y), Color.Black);

                return wynik;
            }

            VertexArray wypelnionyTrojkat(Triangle troj)
            {
                VertexArray wynik = new VertexArray(PrimitiveType.Triangles, 3);
                wynik[0] = new Vertex(new Vector2f(troj.v0.X, troj.v0.Y), troj.color);
                wynik[1] = new Vertex(new Vector2f(troj.v1.X, troj.v1.Y), troj.color);
                wynik[2] = new Vertex(new Vector2f(troj.v2.X, troj.v2.Y), troj.color);

                return wynik;
            }

            // Start the game loop
            while (window.IsOpen)
            {
                // Process events
                window.DispatchEvents();

                Keys();

                Console.WriteLine(d);

                // Clear screen
                window.Clear();

                foreach (var tri in meshCube)
                {
                    Triangle triProjected, triTranslated, triRotatedZ, triRotatedX, triRotatedY;

                    //Rotate in Z-Axis
                    triRotatedZ = mat4x4.ZRotation(ZTheta, tri);

                    //Rotate in X-Axis
                    triRotatedX = mat4x4.XRotation(XTheta, triRotatedZ);

                    //Rotate in Y-Axis
                    triRotatedY = mat4x4.YRotation(YTheta, triRotatedX);

                    //Offset into the screen
                    triTranslated = triRotatedY.Zooming(d);

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
                        triProjected.v0 = mat4x4.MultiplyVecMatr(triTranslated.v0, matProj);
                        triProjected.v1 = mat4x4.MultiplyVecMatr(triTranslated.v1, matProj);
                        triProjected.v2 = mat4x4.MultiplyVecMatr(triTranslated.v2, matProj);
                        triProjected.color = triTranslated.color; 

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
                        var linie = trojkat(triProjected);

                        window.Draw(trojk);
                        window.Draw(linie);
                    }
                }
                // Update the window
                window.Display();
                deltaTime = deltaClock.Restart();
            }
        }

        public static List<Keyboard.Key> pressedKeys = new List<Keyboard.Key>();

        private static void Keys()
        {
            foreach (var key in pressedKeys)
            {
                switch (key)
                {
                    //Moving
                    case Keyboard.Key.W:
                        d+=0.05f;
                        break;
                    case Keyboard.Key.S:
                        d-=0.05f;
                        break;
                    case Keyboard.Key.A:
                        ZTheta += 0.01f * (float)Math.PI;
                        break;
                    case Keyboard.Key.D:
                        ZTheta -= 0.01f * (float)Math.PI;
                        break;
                    case Keyboard.Key.Q:
                        XTheta += 0.01f * (float)Math.PI;
                        break;
                    case Keyboard.Key.E:
                        XTheta -= 0.01f * (float)Math.PI;
                        break;
                    case Keyboard.Key.Z:
                        YTheta += 0.01f * (float)Math.PI;
                        break;
                    case Keyboard.Key.C:
                        YTheta -= 0.01f * (float)Math.PI;
                        break;
                    case Keyboard.Key.Escape:
                        window.Close();
                        break;
                }
            }
        }

        private static void Window_KeyReleased(object sender, KeyEventArgs e)
        {
            pressedKeys.Remove(e.Code);
        }

        private static void Window_KeyPressed(object sender, KeyEventArgs e)
        {
            switch (e.Code)
            {
                case Keyboard.Key.Space:
                    d = 5.0f;
                    ZTheta = 0.0f;
                    XTheta = 0.0f;
                    YTheta = 0.0f;
                    pressedKeys.Clear();
                    break;
                case Keyboard.Key.F2:
                    break;
                case Keyboard.Key.F3:
                    break;
                default:
                    pressedKeys.Add(e.Code);
                    break;
            }
        }
    }
}
