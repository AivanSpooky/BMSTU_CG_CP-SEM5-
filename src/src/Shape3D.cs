using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace src
{
    /*public class Polygon
    {
        public int[] VertexIndices { get; private set; }

        public Polygon(int[] vertexIndices)
        {
            VertexIndices = vertexIndices;
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

            return normal;
        }
    }*/

    public abstract class Shape3D
    {
        public List<Vector3> Vertices { get; protected set; }
        public List<Polygon> Polygons { get; protected set; }
        public Vector3 Position { get; set; }

        public Shape3D(Vector3 position)
        {
            Position = position;
            Vertices = new List<Vector3>();
            Polygons = new List<Polygon>();
        }

        // Метод для трансформации объекта (перемещения, вращения, масштабирования)
        public void Transform(Matrix4x4 transformMatrix)
        {
            for (int i = 0; i < Vertices.Count; i++)
            {
                Vertices[i] = Vector3.Transform(Vertices[i], transformMatrix);
            }
        }

        // Метод для применения позиции к вершинам
        public void ApplyPosition()
        {
            for (int i = 0; i < Vertices.Count; i++)
            {
                Vertices[i] += Position;
            }
        }

        public void TransformShape(double tetax, double tetay, double tetaz)
        {
            // Преобразование углов из градусов в радианы
            tetax = tetax * Math.PI / 180;
            tetay = tetay * Math.PI / 180;
            tetaz = tetaz * Math.PI / 180;

            // Вычисление косинусов и синусов для каждой оси
            double cosTetX = Math.Cos(tetax), sinTetX = Math.Sin(tetax);
            double cosTetY = Math.Cos(tetay), sinTetY = Math.Sin(tetay);
            double cosTetZ = Math.Cos(tetaz), sinTetZ = Math.Sin(tetaz);

            // Преобразование каждой вершины
            for (int i = 0; i < Vertices.Count; i++)
            {
                var v = Vertices[i];
                // Выполнение поворотов вокруг каждой оси
                Transformation.Transform(ref v.X, ref v.Y, ref v.Z, cosTetX, sinTetX, cosTetY, sinTetY, cosTetZ, sinTetZ);
                Vertices[i] = v;
            }
        }


        public Shape3D GetTurnedShape(double tetax, double tetay, double tetaz)
        {
            // Создание копии текущей фигуры
            Shape3D turnedShape = (Shape3D)this.MemberwiseClone();

            // Копирование вершин
            turnedShape.Vertices = new List<Vector3>(Vertices);

            // Применение поворота
            turnedShape.TransformShape(tetax, tetay, tetaz);

            // Копирование полигонов
            turnedShape.Polygons = new List<Polygon>(Polygons);

            return turnedShape;
        }

        // Абстрактный метод для генерации вершин и полигонов фигуры
        public abstract void GenerateShape();
    }

    // Класс для куба, наследуется от Shape3D
    public class Cube3D : Shape3D
    {
        public float Size { get; private set; }

        public Cube3D(float size, Vector3 position) : base(position)
        {
            Size = size;
            GenerateShape();
            ApplyPosition();
        }

        public override void GenerateShape()
        {
            // Генерация вершин куба
            Vertices.AddRange(new Vector3[]
            {
                new Vector3(-Size / 2, -Size / 2, -Size / 2),
                new Vector3(Size / 2, -Size / 2, -Size / 2),
                new Vector3(Size / 2, Size / 2, -Size / 2),
                new Vector3(-Size / 2, Size / 2, -Size / 2),
                new Vector3(-Size / 2, -Size / 2, Size / 2),
                new Vector3(Size / 2, -Size / 2, Size / 2),
                new Vector3(Size / 2, Size / 2, Size / 2),
                new Vector3(-Size / 2, Size / 2, Size / 2)
            });

            // Генерация полигонов куба (6 граней)
            Polygons.Add(new Polygon(new int[] { 0, 1, 2, 3 })); // Задняя грань
            Polygons.Add(new Polygon(new int[] { 4, 5, 6, 7 })); // Передняя грань
            Polygons.Add(new Polygon(new int[] { 0, 1, 5, 4 })); // Нижняя грань
            Polygons.Add(new Polygon(new int[] { 2, 3, 7, 6 })); // Верхняя грань
            Polygons.Add(new Polygon(new int[] { 0, 3, 7, 4 })); // Левая грань
            Polygons.Add(new Polygon(new int[] { 1, 2, 6, 5 })); // Правая грань
        }
    }

    public class RectangularPrism3D : Shape3D
    {
        public float Width { get; private set; }
        public float Height { get; private set; }
        public float Depth { get; private set; }

        public RectangularPrism3D(float width, float height, float depth, Vector3 center) : base(center)
        {
            Width = width;
            Height = height;
            Depth = depth;
            GenerateShape();
            ApplyPosition();
        }

        public override void GenerateShape()
        {
            // Calculate half extents
            float halfWidth = Width / 2;
            float halfHeight = Height / 2;
            float halfDepth = Depth / 2;

            // Generate vertices
            Vertices.AddRange(new Vector3[]
            {
                new Vector3(-halfWidth, -halfHeight, -halfDepth),
                new Vector3(halfWidth, -halfHeight, -halfDepth),
                new Vector3(halfWidth, halfHeight, -halfDepth),
                new Vector3(-halfWidth, halfHeight, -halfDepth),
                new Vector3(-halfWidth, -halfHeight, halfDepth),
                new Vector3(halfWidth, -halfHeight, halfDepth),
                new Vector3(halfWidth, halfHeight, halfDepth),
                new Vector3(-halfWidth, halfHeight, halfDepth)
            });

            // Generate polygons (12 triangles, 6 faces)
            Polygons.Add(new Polygon(new int[] { 0, 1, 2, 3 })); // Back face
            Polygons.Add(new Polygon(new int[] { 4, 5, 6, 7 })); // Front face
            Polygons.Add(new Polygon(new int[] { 0, 1, 5, 4 })); // Bottom face
            Polygons.Add(new Polygon(new int[] { 2, 3, 7, 6 })); // Top face
            Polygons.Add(new Polygon(new int[] { 0, 3, 7, 4 })); // Left face
            Polygons.Add(new Polygon(new int[] { 1, 2, 6, 5 })); // Right face
        }
    }

    // Класс для сферы, наследуется от Shape3D
    public class Sphere3D : Shape3D
    {
        private int latitudeSegments;
        private int longitudeSegments;

        public Sphere3D(int latitudeSegments = 16, int longitudeSegments = 16, Vector3 position = default(Vector3))
            : base(position)
        {
            this.latitudeSegments = latitudeSegments;
            this.longitudeSegments = longitudeSegments;
            GenerateShape();
            ApplyPosition();
        }

        public override void GenerateShape()
        {
            List<Vector3> verticesList = new List<Vector3>();
            List<Polygon> facesList = new List<Polygon>();

            for (int lat = 0; lat <= latitudeSegments; lat++)
            {
                double theta = lat * Math.PI / latitudeSegments;
                double sinTheta = Math.Sin(theta);
                double cosTheta = Math.Cos(theta);

                for (int lon = 0; lon <= longitudeSegments; lon++)
                {
                    double phi = lon * 2 * Math.PI / longitudeSegments;
                    double sinPhi = Math.Sin(phi);
                    double cosPhi = Math.Cos(phi);

                    double x = cosPhi * sinTheta;
                    double y = cosTheta;
                    double z = sinPhi * sinTheta;
                    verticesList.Add(new Vector3((float)x, (float)y, (float)z));
                }
            }

            for (int lat = 0; lat < latitudeSegments; lat++)
            {
                for (int lon = 0; lon < longitudeSegments; lon++)
                {
                    int first = lat * (longitudeSegments + 1) + lon;
                    int second = first + longitudeSegments + 1;

                    facesList.Add(new Polygon(new int[] { first, second, first + 1 }));
                    facesList.Add(new Polygon(new int[] { second, second + 1, first + 1 }));
                }
            }

            Vertices = verticesList;
            Polygons = facesList;
        }
    }
}
