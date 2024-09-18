using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
        public static void TransformShapes(double tetax, double tetay, double tetaz)
        {
            // Create rotation matrices for each axis
            Matrix4x4 rotationX = Matrix4x4.CreateRotationX((float)tetax);
            Matrix4x4 rotationY = Matrix4x4.CreateRotationY((float)tetay);
            Matrix4x4 rotationZ = Matrix4x4.CreateRotationZ((float)tetaz);

            // Combine the rotations
            Matrix4x4 rotationMatrix = rotationX * rotationY * rotationZ;

            // Apply the rotation matrix to each shape
            foreach (var shape in Shapes)
            {
                shape.Transform(rotationMatrix);
            }
        }
    }
}
