using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace test
{
    public partial class Mesh
    {
        public string Name { get; set; }
        public List<Vertex> Vertices { get; private set; }
        public List<Face> Faces { get; private set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Color Color { get; set; }

        public int GridWidth { get; private set; }
        public int GridDepth { get; private set; }

        public Mesh(string name, Color color)
        {
            Name = name;
            Color = color;
            Vertices = new List<Vertex>();
            Faces = new List<Face>();
            Position = Vector3.Zero;
            Rotation = Quaternion.Identity;
            GridWidth = 0;
            GridDepth = 0;
        }

        public static Mesh CreateCube(Vector3 position, float size, Color color)
        {
            Mesh mesh = new Mesh("Cube", color);
            mesh.Position = position;

            float half = size / 2;

            // Вершины и нормали для каждой грани
            Vector3[] positions = new Vector3[]
            {
        // Передняя грань (Z+)
        new Vector3(-half, -half, half), // 0
        new Vector3(half, -half, half),  // 1
        new Vector3(half, half, half),   // 2
        new Vector3(-half, half, half),  // 3

        // Задняя грань (Z-)
        new Vector3(half, -half, -half), // 4
        new Vector3(-half, -half, -half),// 5
        new Vector3(-half, half, -half), // 6
        new Vector3(half, half, -half),  // 7

        // Левая грань (X-)
        new Vector3(-half, -half, -half),// 8
        new Vector3(-half, -half, half), // 9
        new Vector3(-half, half, half),  //10
        new Vector3(-half, half, -half), //11

        // Правая грань (X+)
        new Vector3(half, -half, half),  //12
        new Vector3(half, -half, -half), //13
        new Vector3(half, half, -half),  //14
        new Vector3(half, half, half),   //15

        // Верхняя грань (Y+)
        new Vector3(-half, half, half),  //16
        new Vector3(half, half, half),   //17
        new Vector3(half, half, -half),  //18
        new Vector3(-half, half, -half), //19

        // Нижняя грань (Y-)
        new Vector3(-half, -half, -half),//20
        new Vector3(half, -half, -half), //21
        new Vector3(half, -half, half),  //22
        new Vector3(-half, -half, half), //23
            };

            Vector3[] normals = new Vector3[]
            {
        // Нормали передней грани
        Vector3.UnitZ, Vector3.UnitZ, Vector3.UnitZ, Vector3.UnitZ,
        // Нормали задней грани
        -Vector3.UnitZ, -Vector3.UnitZ, -Vector3.UnitZ, -Vector3.UnitZ,
        // Нормали левой грани
        -Vector3.UnitX, -Vector3.UnitX, -Vector3.UnitX, -Vector3.UnitX,
        // Нормали правой грани
        Vector3.UnitX, Vector3.UnitX, Vector3.UnitX, Vector3.UnitX,
        // Нормали верхней грани
        Vector3.UnitY, Vector3.UnitY, Vector3.UnitY, Vector3.UnitY,
        // Нормали нижней грани
        -Vector3.UnitY, -Vector3.UnitY, -Vector3.UnitY, -Vector3.UnitY,
            };

            int[] indices = new int[]
            {
        // Передняя грань
        0, 1, 2, 0, 2, 3,
        // Задняя грань
        4, 5, 6, 4, 6, 7,
        // Левая грань
        8, 9, 10, 8, 10, 11,
        // Правая грань
        12, 13, 14, 12, 14, 15,
        // Верхняя грань
        16, 17, 18, 16, 18, 19,
        // Нижняя грань
        20, 21, 22, 20, 22, 23,
            };

            // Добавляем вершины с соответствующими нормалями
            for (int i = 0; i < positions.Length; i++)
            {
                mesh.Vertices.Add(new Vertex(positions[i], normals[i]));
            }

            // Добавляем грани
            for (int i = 0; i < indices.Length; i += 3)
            {
                mesh.Faces.Add(new Face(indices[i], indices[i + 1], indices[i + 2]));
            }

            // Нормали уже заданы, вычислять не нужно
            return mesh;
        }

        public static Mesh CreateGridPlane(Vector3 position, int gridWidth, int gridDepth, float cellSize, Color color)
        {
            Mesh mesh = new Mesh("GridPlane", color);
            mesh.Position = position;

            // Создание массива вершин
            Vertex[,] vertices = new Vertex[gridWidth + 1, gridDepth + 1];

            // Создание вершин
            for (int i = 0; i <= gridWidth; i++)
            {
                for (int j = 0; j <= gridDepth; j++)
                {
                    float x = i * cellSize;
                    float z = j * cellSize;
                    float y = 0f;

                    Vector3 normal = Vector3.UnitY; // Нормаль направлена вверх

                    Vector3 vertexPos = new Vector3(x, y, z);
                    vertices[i, j] = new Vertex(vertexPos, normal);
                }
            }

            // Добавление вершин в список
            foreach (var vertex in vertices)
            {
                mesh.Vertices.Add(vertex);
            }

            // Создание граней
            for (int i = 0; i < gridWidth; i++)
            {
                for (int j = 0; j < gridDepth; j++)
                {
                    int topLeft = i * (gridDepth + 1) + j;
                    int topRight = (i + 1) * (gridDepth + 1) + j;
                    int bottomLeft = i * (gridDepth + 1) + (j + 1);
                    int bottomRight = (i + 1) * (gridDepth + 1) + (j + 1);

                    // Обеспечиваем правильный порядок вершин для нормалей
                    mesh.Faces.Add(new Face(topLeft, bottomLeft, topRight));
                    mesh.Faces.Add(new Face(bottomLeft, bottomRight, topRight));
                }
            }

            // Вычисляем нормали (не обязательно, так как нормали уже заданы)
            // mesh.ComputeNormals();

            return mesh;
        }

        public static Mesh CreateSphere(Vector3 position, float radius, int latitudeSegments, int longitudeSegments, Color color, float minTheta = 0f, float maxTheta = (float)Math.PI)
        {
            Mesh mesh = new Mesh("Sphere", color);
            mesh.Position = position;

            for (int lat = 0; lat <= latitudeSegments; lat++)
            {
                float theta = minTheta + (maxTheta - minTheta) * lat / latitudeSegments;
                float sinTheta = (float)Math.Sin(theta);
                float cosTheta = (float)Math.Cos(theta);

                for (int lon = 0; lon <= longitudeSegments; lon++)
                {
                    float phi = lon * 2 * (float)Math.PI / longitudeSegments;
                    float sinPhi = (float)Math.Sin(phi);
                    float cosPhi = (float)Math.Cos(phi);

                    float x = cosPhi * sinTheta;
                    float y = cosTheta;
                    float z = sinPhi * sinTheta;

                    Vector3 normal = new Vector3(x, y, z);
                    Vector3 positionVertex = normal * radius;

                    mesh.Vertices.Add(new Vertex(positionVertex, normal));
                }
            }

            for (int lat = 0; lat < latitudeSegments; lat++)
            {
                for (int lon = 0; lon < longitudeSegments; lon++)
                {
                    int first = (lat * (longitudeSegments + 1)) + lon;
                    int second = first + longitudeSegments + 1;

                    mesh.Faces.Add(new Face(first, second, first + 1));
                    mesh.Faces.Add(new Face(second, second + 1, first + 1));
                }
            }

            // Нормали уже вычислены
            return mesh;
        }

        public void ComputeNormals()
        {
            // Initialize vertex normals
            foreach (var vertex in Vertices)
            {
                vertex.Normal = Vector3.Zero;
            }

            // Accumulate face normals for each vertex
            foreach (var face in Faces)
            {
                Vector3 v0 = Vertices[face.A].Position;
                Vector3 v1 = Vertices[face.B].Position;
                Vector3 v2 = Vertices[face.C].Position;

                Vector3 edge1 = v1 - v0;
                Vector3 edge2 = v2 - v0;

                Vector3 faceNormal = Vector3.Cross(edge1, edge2);
                if (faceNormal.LengthSquared() > 0)
                {
                    faceNormal = Vector3.Normalize(faceNormal);
                    Vertices[face.A].Normal += faceNormal;
                    Vertices[face.B].Normal += faceNormal;
                    Vertices[face.C].Normal += faceNormal;
                }
            }

            // Normalize vertex normals
            foreach (var vertex in Vertices)
            {
                if (vertex.Normal.LengthSquared() > 0)
                {
                    vertex.Normal = Vector3.Normalize(vertex.Normal);
                }
                else
                {
                    vertex.Normal = Vector3.UnitY; // Default normal
                }
            }
        }
    }
}
