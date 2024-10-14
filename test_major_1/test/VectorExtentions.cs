using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace test
{

    public static class VectorExtensions
    {
        public static Vector3 XYZ(this Vector4 v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }
    }
}
