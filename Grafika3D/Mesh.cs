using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SFML.System;
using SFML.Graphics;

namespace Grafika3D
{
    public class Mesh : IEnumerable<Triangle>, IComparable<Mesh>
    {
        public List<Triangle> Triangles { get; set; } = new List<Triangle>();

        public float depth = 0;

        public void Add(Triangle t)
        {
            Triangles.Add(t);
        }

        public void LoadFromObjectFile(string sFilename)
        {
            List<Vector3f> listawektorowa = new List<Vector3f>();
            try
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader(sFilename))
                {
                    // Read a text file line by line.  
                    string[] lines = File.ReadAllLines(sFilename);

                    foreach (string line in lines)
                    {
                        if (line[0] == 'v')
                        {
                            string[] test = line.Split(' ');
                            Vector3f v = new Vector3f();
                            v.X = float.Parse(test[1], System.Globalization.CultureInfo.InvariantCulture);
                            v.Y = float.Parse(test[2], System.Globalization.CultureInfo.InvariantCulture);
                            v.Z = float.Parse(test[3], System.Globalization.CultureInfo.InvariantCulture);
                            listawektorowa.Add(v);
                        }

                        if (line[0] == 'f')
                        {
                            string[] test = line.Split(' ');
                            Triangles.Add(new Triangle(listawektorowa[Int32.Parse(test[1])-1], listawektorowa[Int32.Parse(test[2])-1], listawektorowa[Int32.Parse(test[3])-1],new Color (0,0,255)));
                        }
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }

        public IEnumerator<Triangle> GetEnumerator()
        {
            return ((IEnumerable<Triangle>)Triangles).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<Triangle>)Triangles).GetEnumerator();
        }

        public int CompareTo(Mesh other)
        {
            if (depth > other.depth)
                return -1;
            else return 1;
        }
    }
}
