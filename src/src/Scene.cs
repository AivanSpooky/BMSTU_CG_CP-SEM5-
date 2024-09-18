using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src
{
    public static class Scene
    {
        public static List<Shape3D> Shapes { get; private set; } = new List<Shape3D>();

        public static void AddShape(Shape3D shape)
        {
            Shapes.Add(shape);
        }

        public static void RemoveShape(Shape3D shape)
        {
            Shapes.Remove(shape);
        }

        public static void ClearScene()
        {
            Shapes.Clear();
        }
    }
}
