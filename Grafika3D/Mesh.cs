using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Grafika3D
{
    public class Mesh : IEnumerable<Triangle>
    {
        public List<Triangle> Triangles { get; set; } = new List<Triangle>();

        public void Add(Triangle t)
        {
            Triangles.Add(t);
        }

        public IEnumerator<Triangle> GetEnumerator()
        {
            return ((IEnumerable<Triangle>)Triangles).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<Triangle>)Triangles).GetEnumerator();
        }
    }
}
