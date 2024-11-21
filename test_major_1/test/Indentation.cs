using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace test
{
    public class Indentation
    {
        public int GridX { get; set; } // координата по X в клетках
        public int GridZ { get; set; } // координата по Z в клетках
        public int Width { get; set; } // ширина лунки в клетках
        public int Depth { get; set; } // глубина лунки в клетках (по размеру, не по оси Y)
        public IndentationType Type { get; set; }

        public Indentation(int gridX, int gridZ, int width, int depth, IndentationType type)
        {
            GridX = gridX;
            GridZ = gridZ;
            Width = width;
            Depth = depth;
            Type = type;
        }
    }
}
