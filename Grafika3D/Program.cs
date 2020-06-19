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
        public static Time deltaTime;
        static float zoom = 5.0f;
        static float x = 0.0f;
        static float y = 0.0f;
        static float XTheta = 0.1f * (float)Math.PI;
        static float YTheta = 0.1f * (float)Math.PI;
        static float ZTheta = 0.1f * (float)Math.PI;
        static Vector3f vCamera = new Vector3f(0, 0, 0);
        static int siatka = 1;

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
            window.KeyPressed += Window_KeyPressed;
            window.KeyReleased += Window_KeyReleased;
            deltaTime = deltaClock.Restart();


            Vector3f v00 = new Vector3f(0, 0, 0);
            Vector3f v01 = new Vector3f(0, 0, 1);
            Vector3f v02 = new Vector3f(0, 1, 0);
            Vector3f v03 = new Vector3f(0, 1, 1);
            Vector3f v04 = new Vector3f(1, 0, 0);
            Vector3f v05 = new Vector3f(1, 0, 1);
            Vector3f v06 = new Vector3f(1, 1, 0);
            Vector3f v07 = new Vector3f(1, 1, 1);

            Vector3f v08 = new Vector3f(0.5f, 2, 0.5f);

            //sześcian
            Mesh meshCube = new Mesh()
            {
                // SOUTH
                new Triangle( v00, v02, v06, Color.Blue ),
                new Triangle( v00, v06, v04, Color.Blue ),

		        // EAST                                                      
		        new Triangle( v04, v06, v07, Color.Blue ),
                new Triangle( v04, v07, v05, Color.Blue ),

		        // NORTH                                                     
		        new Triangle( v05, v07, v03, Color.Blue ),
                new Triangle( v05, v03, v01, Color.Blue ),

		        // WEST                                                      
		        new Triangle( v01, v03, v02, Color.Blue ),
                new Triangle( v01, v02, v00, Color.Blue ),

		        // TOP                                                       
		        new Triangle( v02, v03, v07, Color.Blue ),
                new Triangle( v02, v07, v06, Color.Blue ),
                                          
		        // BOTTOM                                                    
		        new Triangle( v05, v01, v00, Color.Blue ),
                new Triangle( v05, v00, v04, Color.Blue ),
            };

            //ostrosłup
            Mesh newObject3d = new Mesh()
            {
                // SOUTH
                new Triangle( v00, v08, v04, Color.Blue ),

		        // EAST                                                      
		        new Triangle( v04, v08, v05, Color.Cyan ),

		        // NORTH                                                     
		        new Triangle( v05, v08, v01, Color.Green ),

		        // WEST                                                      
		        new Triangle( v01, v08, v00, Color.Magenta ),
                                          
		        // BOTTOM                                                    
		        new Triangle( v05, v01, v00, Color.Yellow ),
                new Triangle( v05, v00, v04, Color.Yellow ),
            };

            //sfera
            Mesh sphere = new Mesh();
            sphere.LoadFromObjectFile("sphere.obj");

            sphere.depth = 5.0f;
            meshCube.depth = 7.0f;
            newObject3d.depth = 10.0f;

            // Projection Matrix
            float fNear = 0.1f;
            float fFar = 1000.0f;
            float fFov = 90.0f;
            float fAspectRatio = Width / Height;
            float fFovRad = 1.0f / (float)Math.Tan(fFov * 0.5f / 180.0f * 3.14159f);
            
            mat4x4 matProj = new mat4x4();
            matProj.Matrix_MakeProjection(fFov, fAspectRatio, fNear, fFar);

            //camera Matrix
            Vector3f vLookDir = new Vector3f(0, 0, 1);
            Vector3f vUp = new Vector3f(0, 1, 0);
            Vector3f vTarget = vCamera + vLookDir;

            mat4x4 matCamera = Matrix_PointAt(vCamera, vTarget, vUp);

            mat4x4 matView = Matrix_QuickInverse(matCamera);

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

            List<Mesh> listameshy = new List<Mesh>();
            listameshy.Add(meshCube);
            listameshy.Add(sphere);
            listameshy.Add(newObject3d);
            listameshy.Sort();

            // Start the game loop
            while (window.IsOpen)
            {
                // Process events
                window.DispatchEvents();

                Keys();

                // Clear screen
                window.Clear();

                for (int i = 0; i < listameshy.Count; i++)
                {
                    foreach (var tri in listameshy[i])
                    {
                        Triangle triProjected, triTranslated, triRotatedZ, triRotatedX, triRotatedY, triViewed;

                        //Rotate in Z-Axis
                        triRotatedZ = mat4x4.ZRotation(ZTheta, tri);

                        //Rotate in X-Axis
                        triRotatedX = mat4x4.XRotation(XTheta, triRotatedZ);

                        //Rotate in Y-Axis
                        triRotatedY = mat4x4.YRotation(YTheta, triRotatedX);

                        //Offset into the screen
                        triTranslated = triRotatedY.Zooming(zoom + listameshy[i].depth);

                        triTranslated = triTranslated.Slide(x, y);

                        Vector3f normal, line1, line2;

                        line1 = triTranslated.v1 - triTranslated.v0;
                        line2 = triTranslated.v2 - triTranslated.v0;

                        //Take cross product of lines to get normal to triangle surface
                        normal = Croos(line1, line2);

                        normal = Vector_Normalise(normal);

                        //Get Ray from triangle to camera
                        Vector3f vCameraRay = triTranslated.v0 - vCamera;

                        //if ray is aligned with normal, then triangle is visible
                        if (Dot(normal, vCameraRay) < 0.0)
                        {
                            //Illumination
                            Vector3f light_direction = new Vector3f(1.0f, 0.0f, -1.0f);
                            light_direction = Vector_Normalise(light_direction);

                            // How similar is normal to light direction
                            float dp = Math.Max(0.1f, Dot(light_direction, normal));

                            //Here i should choose colors as required
                            Color kolor = GetColor(dp);
                            triTranslated.color = kolor;

                            // Convert World Space --> View Space
                            triViewed.v0 = triTranslated.v0 * matView;
                            triViewed.v1 = triTranslated.v1 * matView;
                            triViewed.v2 = triTranslated.v2 * matView;
                            triViewed.color = triTranslated.color;

                            //Project triangles from 3D -> 2D
                            triProjected.v0 = triTranslated.v0 * matProj;
                            triProjected.v1 = triTranslated.v1 * matProj;
                            triProjected.v2 = triTranslated.v2 * matProj;
                            triProjected.color = triTranslated.color;

                            // Offset verts into visible normalised space
                            Vector3f vOffsetView = new Vector3f(1.0f, 1.0f, 0);
                            triProjected.v0 += vOffsetView;
                            triProjected.v1 += vOffsetView;
                            triProjected.v2 += vOffsetView;

                            triProjected.v0.X *= 0.5f * Width;
                            triProjected.v0.Y *= 0.5f * Height;
                            triProjected.v1.X *= 0.5f * Width;
                            triProjected.v1.Y *= 0.5f * Height;
                            triProjected.v2.X *= 0.5f * Width;
                            triProjected.v2.Y *= 0.5f * Height;

                            var trojk = wypelnionyTrojkat(triProjected);
                            var linie = trojkat(triProjected);

                            window.Draw(trojk);
                            if (siatka == 1)
                                window.Draw(linie);
                        }
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
                    case Keyboard.Key.E:
                        zoom+=0.05f;
                        break;
                    case Keyboard.Key.Q:
                        zoom-=0.05f;
                        break;
                    case Keyboard.Key.U:
                        ZTheta += 0.01f * (float)Math.PI;
                        break;
                    case Keyboard.Key.I:
                        ZTheta -= 0.01f * (float)Math.PI;
                        break;
                    case Keyboard.Key.J:
                        XTheta += 0.01f * (float)Math.PI;
                        break;
                    case Keyboard.Key.K:
                        XTheta -= 0.01f * (float)Math.PI;
                        break;
                    case Keyboard.Key.N:
                        YTheta += 0.01f * (float)Math.PI;
                        break;
                    case Keyboard.Key.M:
                        YTheta -= 0.01f * (float)Math.PI;
                        break;
                    case Keyboard.Key.W:
                        y -= 0.05f;
                        //vCamera.Y += 1.0f;
                        break;
                    case Keyboard.Key.S:
                        y += 0.05f;
                        //vCamera.Y -= 1.0f;
                        break;
                    case Keyboard.Key.D:
                        x += 0.05f;
                        //vCamera.X += 1.0f;
                        break;
                    case Keyboard.Key.A:
                        x -= 0.05f;
                        //vCamera.X -= 1.0f;
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
                    pressedKeys.Clear();
                    break;
                case Keyboard.Key.Tab:
                    if (siatka == 1)
                        siatka = 0;
                    else siatka = 1;
                    break;
                case Keyboard.Key.Tilde:
                    zoom = 5.0f;
                    x = 0.0f;
                    y = 0.0f;
                    ZTheta = 0.1f * (float)Math.PI;
                    XTheta = 0.1f * (float)Math.PI;
                    YTheta = 0.1f * (float)Math.PI;
                    pressedKeys.Clear();
                    break;
                default:
                    pressedKeys.Add(e.Code);
                    break;
            }
        }

        //Vector_CrossProduct
        public static Vector3f Croos(Vector3f vec1, Vector3f vec2)
        {
            Vector3f v = new Vector3f();
            v.X = vec1.Y * vec2.Z - vec1.Z * vec2.Y;
            v.Y = vec1.Z * vec2.X - vec1.X * vec2.Z;
            v.Z = vec1.X * vec2.Y - vec1.Y * vec2.X;
            return v;
        }

        //Vector_DotProduct
        public static float Dot(Vector3f vec1, Vector3f vec2)
        {
            return vec1.X * vec2.X + vec1.Y * vec2.Y + vec1.Z * vec2.Z;
        }

        public static double Vector_Length(Vector3f vec)
        {
            double l = Math.Sqrt(Dot(vec,vec));
            return l;
        }

        public static Vector3f Vector_Normalise(Vector3f vec)
        {
            float l = (float)Vector_Length(vec);
            return vec / l;
        }

        public static Color GetColor(float lum)
        {
            int pixel_bw = (int)(10.0f * lum);
            Color kolor = new Color(0,0,0);
            switch (pixel_bw)
            {
                case 0:
                    kolor.B = 25;
                    break;
                case 1:
                    kolor.B = 50;
                    break;
                case 2:
                    kolor.B = 75;
                    break;
                case 3:
                    kolor.B = 100;
                    break;
                case 4:
                    kolor.B = 125;
                    break;
                case 5:
                    kolor.B = 150;
                    break;
                case 6:
                    kolor.B = 175;
                    break;
                case 7:
                    kolor.B = 200;
                    break;
                case 8:
                    kolor.B = 225;
                    break;
                case 9:
                    kolor.B = 250;
                    break;
                default:
                    kolor.B = 0;
                    break;
            }

            return kolor;
        }

        public static mat4x4 Matrix_PointAt(Vector3f pos, Vector3f target, Vector3f up)
        {
            //calculate new forward direction
            Vector3f newForward = target - pos;
            newForward = Vector_Normalise(newForward);

            // Calculate new Up direction
            Vector3f a = newForward * Dot(up, newForward);
            Vector3f newUp = up - a;
            newUp = Vector_Normalise(newUp);

            Vector3f newRight = Croos(newUp, newForward);

            // Construct Dimensioning and Translation Matrix	
            mat4x4 matrix = new mat4x4();
            matrix.m[0,0] = newRight.X; matrix.m[0,1] = newRight.Y; matrix.m[0,2] = newRight.Z; matrix.m[0,3] = 0.0f;
            matrix.m[1,0] = newUp.X; matrix.m[1,1] = newUp.Y; matrix.m[1,2] = newUp.Z; matrix.m[1,3] = 0.0f;
            matrix.m[2,0] = newForward.X; matrix.m[2,1] = newForward.Y; matrix.m[2,2] = newForward.Z; matrix.m[2,3] = 0.0f;
            matrix.m[3,0] = pos.X; matrix.m[3,1] = pos.Y; matrix.m[3,2] = pos.Z; matrix.m[3,3] = 1.0f;
            return matrix;
        }

        public static mat4x4 Matrix_QuickInverse(mat4x4 m) // Only for Rotation/Translation Matrices
        {
            mat4x4 matrix = new mat4x4();
            matrix.m[0,0] = m.m[0,0]; matrix.m[0,1] = m.m[1,0]; matrix.m[0,2] = m.m[2,0]; matrix.m[0,3] = 0.0f;
            matrix.m[1,0] = m.m[0,1]; matrix.m[1,1] = m.m[1,1]; matrix.m[1,2] = m.m[2,1]; matrix.m[1,3] = 0.0f;
            matrix.m[2,0] = m.m[0,2]; matrix.m[2,1] = m.m[1,2]; matrix.m[2,2] = m.m[2,2]; matrix.m[2,3] = 0.0f;
            matrix.m[3,0] = -(m.m[3,0] * matrix.m[0,0] + m.m[3,1] * matrix.m[1,0] + m.m[3,2] * matrix.m[2,0]);
            matrix.m[3,1] = -(m.m[3,0] * matrix.m[0,1] + m.m[3,1] * matrix.m[1,1] + m.m[3,2] * matrix.m[2,1]);
            matrix.m[3,2] = -(m.m[3,0] * matrix.m[0,2] + m.m[3,1] * matrix.m[1,2] + m.m[3,2] * matrix.m[2,2]);
            matrix.m[3,3] = 1.0f;
            return matrix;
        }

        public static List<Mesh> sortowanko(List<Mesh> t)
        {
            List<Mesh> wynik = new List<Mesh>();


            return wynik;
        }
    }
}
