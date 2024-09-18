using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace src
{
    public class Polygon
    {
        public int[] VertexIndices { get; private set; }
        public Color BasicColor { get; private set; } = Color.Gray; // Цвет по умолчанию
        public List<Point3D> PointsInside { get; private set; } // Внутренние точки полигона
        public Vector3 Normal { get; private set; } // Нормаль
        public bool Ignore { get; private set; } // Игнорирование полигона, например, от солнца

        public Polygon(int[] vertexIndices, bool special = false)
        {
            VertexIndices = vertexIndices;
            PointsInside = new List<Point3D>();
            Ignore = special;
        }

        public Polygon(int[] vertexIndices, Color color, bool special = false)
        {
            VertexIndices = vertexIndices;
            BasicColor = color;
            PointsInside = new List<Point3D>();
            Ignore = special;
        }

        // Метод для получения нормали полигона
        public Vector3 GetNormal(List<Vector3> vertices)
        {
            var v0 = vertices[VertexIndices[0]];
            var v1 = vertices[VertexIndices[1]];
            var v2 = vertices[VertexIndices[2]];

            // Вычисляем нормаль через векторное произведение
            var normal = Vector3.Cross(v1 - v0, v2 - v0);
            normal = Vector3.Normalize(normal);
            Normal = normal;
            return normal;
        }

        // Нахождение точек внутри многоугольника
        public void CalculatePointsInside(List<Vector3> vertices, int maxX, int maxY)
        {
            PointsInside = new List<Point3D>();

            if (VertexIndices.Length > 4)
            {
                // TODO: Разбить на треугольники
            }
            else if (VertexIndices.Length == 4)
            {
                // Обрабатываем четырехугольник как два треугольника
                var triangle1 = new int[] { VertexIndices[0], VertexIndices[2], VertexIndices[1] };
                CalculatePointsInsideTriangle(triangle1, vertices, maxX, maxY);

                var triangle2 = new int[] { VertexIndices[0], VertexIndices[2], VertexIndices[3] };
                CalculatePointsInsideTriangle(triangle2, vertices, maxX, maxY);
            }
            else if (VertexIndices.Length == 3)
            {
                CalculatePointsInsideTriangle(VertexIndices, vertices, maxX, maxY);
            }
        }

        // Метод для расчета внутренних точек для треугольника
        private void CalculatePointsInsideTriangle(int[] triangleIndices, List<Vector3> vertices, int maxX, int maxY, int firstXPossible = 0, int firstYPossible = 0)
        {
            // Примерно как в вашем исходном коде
            int[] x = new int[3], y = new int[3];

            for (int i = 0; i < 3; ++i)
            {
                x[i] = (int)vertices[triangleIndices[i]].X;
                y[i] = (int)vertices[triangleIndices[i]].Y;
            }

            int yMax = y.Max(), yMin = y.Min();
            yMin = Math.Max(yMin, firstYPossible);
            yMax = Math.Min(yMax, maxY);

            for (int yDot = yMin; yDot <= yMax; yDot++)
            {
                int x1 = 0, x2 = 0;
                double z1 = 0, z2 = 0;
                bool first = true;

                for (int n = 0; n < 3; ++n)
                {
                    int n1 = (n == 2) ? 0 : n + 1;
                    if (yDot >= Math.Max(y[n], y[n1]) || yDot < Math.Min(y[n], y[n1])) continue;

                    double m = (double)(y[n] - yDot) / (y[n] - y[n1]);
                    if (first)
                    {
                        x1 = x[n] + (int)(m * (x[n1] - x[n]));
                        z1 = vertices[triangleIndices[n]].Z + m * (vertices[triangleIndices[n1]].Z - vertices[triangleIndices[n]].Z);
                        first = false;
                    }
                    else
                    {
                        x2 = x[n] + (int)(m * (x[n1] - x[n]));
                        z2 = vertices[triangleIndices[n]].Z + m * (vertices[triangleIndices[n1]].Z - vertices[triangleIndices[n]].Z);
                    }
                }

                if (x2 < x1)
                {
                    Swap(ref x1, ref x2);
                    Swap(ref z1, ref z2);
                }

                for (int xDot = x1; xDot <= x2; xDot++)
                {
                    double m = (double)(x1 - xDot) / (x1 - x2);
                    double zDot = z1 + m * (z2 - z1);
                    PointsInside.Add(new Point3D(xDot, yDot, (int)zDot));
                }
            }
        }

        // Метод для получения цвета полигона с учетом источника света
        public Color GetColor(LightSource light)
        {
            double cos = Vector3.Dot(light.direction, Normal) / (light.direction.Length() * Normal.Length());

            if (cos <= 0)
                return Colors.Mix(BasicColor, Color.Black, 0.2f);
            return Colors.Mix(BasicColor, Color.Black, (float)cos);
        }

        static void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }
    }

    // Класс для точки в 3D пространстве
    public class Point3D
    {
        public int X, Y, Z;
        public Point3D(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}
