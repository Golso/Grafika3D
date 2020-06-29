using System;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System.Collections.Generic;

namespace Grafika3D
{
    class Program
    {
        public static uint Width = 1000;
        public static uint Height = 1000;
        public static RenderWindow window = new RenderWindow(new VideoMode(Width, Height), "Grafika3D");
        static float z = 5.0f;
        static float x = -2.0f;
        static float y = 0.0f;
        static float XTheta = 0.1f * (float)Math.PI;
        static float YTheta = 0.1f * (float)Math.PI;
        static float ZTheta = 0.1f * (float)Math.PI;
        static Vector3f vCamera = new Vector3f(0, 0, 0);
        static Vector3f light_direction = new Vector3f(1.0f, 0.0f, -1.0f);
        static int net = 1;

        static void OnClose(object sender, EventArgs e)
        {
            // Close the window when OnClose event is received
            RenderWindow window = (RenderWindow)sender;
            window.Close();
        }

        static void Main(string[] args)
        {
            window.Closed += new EventHandler(OnClose);
            window.KeyPressed += Window_KeyPressed;
            window.KeyReleased += Window_KeyReleased;

            Vector3f v00 = new Vector3f(0, 0, 0);
            Vector3f v01 = new Vector3f(0, 0, 1);
            Vector3f v02 = new Vector3f(0, 1, 0);
            Vector3f v03 = new Vector3f(0, 1, 1);
            Vector3f v04 = new Vector3f(1, 0, 0);
            Vector3f v05 = new Vector3f(1, 0, 1);
            Vector3f v06 = new Vector3f(1, 1, 0);
            Vector3f v07 = new Vector3f(1, 1, 1);
           
            Vector3f v08 = new Vector3f(0.5f, 2.5f, 0.5f);

            Color blue = new Color(0, 0, 255);

            //Cube
            Mesh meshCube = new Mesh()
            {
                // SOUTH WALL
                new Triangle( v00, v02, v06, blue ),
                new Triangle( v00, v06, v04, blue ),

		        // EAST WALL                                                      
		        new Triangle( v04, v06, v07, blue ),
                new Triangle( v04, v07, v05, blue ),

		        // NORTH WALL                                                    
		        new Triangle( v05, v07, v03, blue ),
                new Triangle( v05, v03, v01, blue ),

		        // WEST WALL                                                     
		        new Triangle( v01, v03, v02, blue ),
                new Triangle( v01, v02, v00, blue ),

		        // TOP WALL                                                       
		        new Triangle( v02, v03, v07, blue ),
                new Triangle( v02, v07, v06, blue ),
                                          
		        // BOTTOM WALL                                                   
		        new Triangle( v05, v01, v00, blue ),
                new Triangle( v05, v00, v04, blue ),
            };

            //pyramid
            Mesh meshPyramid = new Mesh()
            {
                // SOUTH WALL
                new Triangle( v00, v08, v04, blue ),

		        // EAST WALL                                                     
		        new Triangle( v04, v08, v05, blue ),

		        // NORTH WALL                                                    
		        new Triangle( v05, v08, v01, blue ),

		        // WEST WALL                                                     
		        new Triangle( v01, v08, v00, blue ),
                                          
		        // BOTTOM WALL                                                   
		        new Triangle( v05, v01, v00, blue ),
                new Triangle( v05, v00, v04, blue ),
            };

            //Sphere
            Mesh meshSphere = new Mesh();
            meshSphere.LoadFromObjectFile("sphere.obj");

            List<Triangle> triangleList = new List<Triangle>();

            //Determining the depth and location of figures
            meshSphere.depth = 7.0f;
            for (int i = 0; i < meshSphere.Triangles.Count; i++)
            {
                meshSphere.Triangles[i] = meshSphere.Triangles[i].SlideAndZoom(0, 0, meshSphere.depth);
                triangleList.Add(meshSphere.Triangles[i]);
            }

            meshCube.depth = 4.0f;
            for (int i = 0; i < meshCube.Triangles.Count; i++)
            {
                meshCube.Triangles[i] = meshCube.Triangles[i].SlideAndZoom(0, 0, meshCube.depth);
                triangleList.Add(meshCube.Triangles[i]);
            }

            meshPyramid.depth = 10.0f;
            for (int i = 0; i < meshPyramid.Triangles.Count; i++)
            {
                meshPyramid.Triangles[i] = meshPyramid.Triangles[i].SlideAndZoom(0, 0, meshPyramid.depth);
                triangleList.Add(meshPyramid.Triangles[i]);
            }
            
            // Projection Matrix
            Matrix4x4 projMatrix = new Matrix4x4();
            projMatrix.Matrix_MakeProjection(90.0f, Height / Width, 0.5f, 1200.0f);

            //Making triangle throught drawing lines
            VertexArray drawTriangleNet(Triangle tri)
            {
                VertexArray result = new VertexArray(PrimitiveType.Lines, 6);
                result[0] = new Vertex(new Vector2f(tri.v0.X, tri.v0.Y), Color.White);
                result[1] = new Vertex(new Vector2f(tri.v1.X, tri.v1.Y), Color.White);
                result[2] = new Vertex(new Vector2f(tri.v1.X, tri.v1.Y), Color.White);
                result[3] = new Vertex(new Vector2f(tri.v2.X, tri.v2.Y), Color.White);
                result[4] = new Vertex(new Vector2f(tri.v2.X, tri.v2.Y), Color.White);
                result[5] = new Vertex(new Vector2f(tri.v0.X, tri.v0.Y), Color.White);

                return result;
            }

            VertexArray drawFilledTriangle(Triangle tri)
            {
                VertexArray result = new VertexArray(PrimitiveType.Triangles, 3);
                result[0] = new Vertex(new Vector2f(tri.v0.X, tri.v0.Y), tri.color);
                result[1] = new Vertex(new Vector2f(tri.v1.X, tri.v1.Y), tri.color);
                result[2] = new Vertex(new Vector2f(tri.v2.X, tri.v2.Y), tri.color);

                return result;
            }

            //Sorting triangles for Painter's algorithm
            triangleList.Sort();

            // Start the game loop
            while (window.IsOpen)
            {
                // Process events
                window.DispatchEvents();

                Keys();

                // Clear screen
                window.Clear();

                foreach (var tri in triangleList)
                {
                    Triangle triProjected, triConverted, triRotatedZ, triRotatedX, triRotatedY, triMoved;

                    //Move in scene
                    triMoved = Matrix4x4.Move(x, y, z, tri);

                    //Rotate in Z-Axis
                    triRotatedZ = Matrix4x4.ZRotation(ZTheta, triMoved);

                    //Rotate in X-Axis
                    triRotatedX = Matrix4x4.XRotation(XTheta, triRotatedZ);

                    //Rotate in Y-Axis
                    triRotatedY = Matrix4x4.YRotation(YTheta, triRotatedX);

                    triConverted = triRotatedY;

                    Vector3f normal, line1, line2;

                    line1 = triConverted.v1 - triConverted.v0;
                    line2 = triConverted.v2 - triConverted.v0;

                    //Take cross product of lines to get normal to triangle surface
                    normal = Cross(line1, line2);
                    normal = Vector_Normalise(normal);

                    //Get Ray from triangle to camera
                    Vector3f vCameraRay = triConverted.v0 - vCamera;

                    //If ray is aligned with normal, then triangle is visible thanks to that we aren't drawing triangles which we wouldn't see
                    if (Dot(normal, vCameraRay) < 0.0)
                    {
                        //Illumination
                        light_direction = Vector_Normalise(light_direction);

                        // How similar is normal to light direction
                        float dp = Math.Max(0.1f, Dot(light_direction, normal));

                        triConverted.color.R = Convert.ToByte(triConverted.color.R * dp);
                        triConverted.color.G = Convert.ToByte(triConverted.color.G * dp);
                        triConverted.color.B = Convert.ToByte(triConverted.color.B * dp);

                        //Project triangles from 3D -> 2D
                        triProjected.v0 = triConverted.v0 * projMatrix;
                        triProjected.v1 = triConverted.v1 * projMatrix;
                        triProjected.v2 = triConverted.v2 * projMatrix;
                        triProjected.color = triConverted.color;

                        Vector3f vOffsetView = new Vector3f(1.0f, 1.0f, 0.0f);
                        triProjected.v0 += vOffsetView;
                        triProjected.v1 += vOffsetView;
                        triProjected.v2 += vOffsetView;

                        triProjected.v0.X *= 0.5f * Width;
                        triProjected.v0.Y *= 0.5f * Height;
                        triProjected.v1.X *= 0.5f * Width;
                        triProjected.v1.Y *= 0.5f * Height;
                        triProjected.v2.X *= 0.5f * Width;
                        triProjected.v2.Y *= 0.5f * Height;

                        var drawnTriangle = drawFilledTriangle(triProjected);
                        var lines = drawTriangleNet(triProjected);

                        //Checking if triangle is not behind camera
                        if(triProjected.v0.Z < 0.999 || triProjected.v1.Z < 0.999 || triProjected.v2.Z < 0.999)
                        {
                            window.Draw(drawnTriangle);
                            if (net == 0)
                                window.Draw(lines);
                        }
                    }
                }
                // Update the window
                window.Display();
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
                        z+=0.05f;
                        break;
                    case Keyboard.Key.Q:
                        z-=0.05f;
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
                        break;
                    case Keyboard.Key.S:
                        y += 0.05f;
                        break;
                    case Keyboard.Key.D:
                        x += 0.05f;
                        break;
                    case Keyboard.Key.A:
                        x -= 0.05f;
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
                    if (net == 1)
                        net = 0;
                    else net = 1;
                    break;
                case Keyboard.Key.Tilde:
                    z = 5.0f;
                    x = -2.0f;
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
        public static Vector3f Cross(Vector3f vec1, Vector3f vec2)
        {
            Vector3f v = new Vector3f
            {
                X = vec1.Y * vec2.Z - vec1.Z * vec2.Y,
                Y = vec1.Z * vec2.X - vec1.X * vec2.Z,
                Z = vec1.X * vec2.Y - vec1.Y * vec2.X
            };
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
    }
}
