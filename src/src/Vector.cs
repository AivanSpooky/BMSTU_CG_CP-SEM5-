using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace src
{
    public class Vector
    {
        public double x, y, z;
        public double length;

        public Vector(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            FindLength();
        }

        public Vector(Point3D a, Point3D b)
        {
            x = b.X - a.X;
            y = b.Y - a.Y;
            z = b.Z - a.Z;
            FindLength();
        }

        public static Vector operator -(Vector a, Vector b)
        {
            return new Vector(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Vector operator +(Vector v1, Vector v2)
        {
            return new Vector(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
        }

        // Метод для преобразования вектора с использованием матрицы преобразования
        public static Vector Transform(Vector vector, Matrix4x4 matrix)
        {
            double x = vector.x;
            double y = vector.y;
            double z = vector.z;
            float w = 1.0f; // Вектор считается однородным, поэтому w = 1

            double newX = x * matrix.M11 + y * matrix.M21 + z * matrix.M31 + w * matrix.M41;
            double newY = x * matrix.M12 + y * matrix.M22 + z * matrix.M32 + w * matrix.M42;
            double newZ = x * matrix.M13 + y * matrix.M23 + z * matrix.M33 + w * matrix.M43;

            return new Vector(newX, newY, newZ);
        }

        private void FindLength()
        {
            length = Math.Sqrt(x * x + y * y + z * z);
        }

        public double GetLength()
        {
            return length;
        }

        public void RotateVectorX(double tetax)
        {
            tetax = tetax * Math.PI / 180;
            double buf = y;
            y = Math.Cos(tetax) * y - Math.Sin(tetax) * z;
            z = Math.Cos(tetax) * z + Math.Sin(tetax) * buf;
        }

        public void RotateVectorY(double tetay)
        {
            tetay = tetay * Math.PI / 180;
            double buf = x;
            x = Math.Cos(tetay) * x - Math.Sin(tetay) * z;
            z = Math.Cos(tetay) * z + Math.Sin(tetay) * buf;
        }

        public static Vector VectorMultiplication(Vector a, Vector b)
        {
            Vector res = new Vector(0, 0, 0);
            res.x = a.y * b.z - a.z * b.y;
            res.y = a.z * b.x - a.x * b.z;
            res.z = a.x * b.y - a.y * b.x;
            res.FindLength();
            return res;
        }

        public static double ScalarMultiplication(Vector a, Vector b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }

        public Vector Normalize()
        {
            if (length == 0)
                throw new InvalidOperationException("Cannot normalize a zero-length vector.");

            double invLength = 1.0 / length;
            return new Vector(x * invLength, y * invLength, z * invLength);
        }
    }
}
