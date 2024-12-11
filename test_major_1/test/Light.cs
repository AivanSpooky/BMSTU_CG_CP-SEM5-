using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace test
{
    public class Light
    {
        public Vector3 Position;
        public float Intensity;

        public Light(Vector3 position, float intensity = 1f)
        {
            Position = position;
            Intensity = intensity;
        }
    }
}
