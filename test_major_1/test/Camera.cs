using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace test
{
    public class Camera
    {
        public Vector3 Position;
        public Vector3 Target;
        public Vector3 Up;

        public Camera(Vector3 position, Vector3 target, Vector3 up)
        {
            Position = position;
            Target = target;
            Up = up;
        }
    }
}
