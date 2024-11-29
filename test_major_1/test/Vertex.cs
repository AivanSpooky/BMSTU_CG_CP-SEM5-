using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace test
{
    public class Vertex
    {
        public Vector3 Position;
        public Vector3 Normal;
        public bool IsInIndentation { get; set; }
        public bool IsPerimeter { get; set; }

        public Vertex(Vector3 position, Vector3 normal)
        {
            Position = position;
            Normal = normal;
            IsInIndentation = false;
            IsPerimeter = false;
        }
    }
}
