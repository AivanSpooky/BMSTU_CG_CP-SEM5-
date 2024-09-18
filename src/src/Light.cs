using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace src
{
    public class LightSource
    {
        public Color color;
        public double tetax = 90, tetay = 0, tetaz = 0;
        public Vector3 direction;


        public LightSource(Color color, double tetay, Vector3 direction)
        {
            this.direction = direction;
            this.color = color;
            this.tetay = tetay;
        }
    }
}
