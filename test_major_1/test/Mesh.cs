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
        public FigureType Type { get; set; }

        public int GridWidth { get; private set; }
        public int GridDepth { get; private set; }
        public float SizeInCells { get; set; }   // For cubes
        public float HeightInCells { get; set; } // For cylinders and prisms
        public float RadiusInCells { get; set; } // For spheres and cylinders

        public Mesh(string name, Color color)
        {
            Type = FigureType.Default;
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
            mesh.Type = FigureType.Cube;
            mesh.Position = position;
            mesh.SizeInCells = size / GPO.cellSize;

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

        public static Mesh CreateSphere(Vector3 position, float radius, int latitudeSegments, int longitudeSegments, Color color, float minTheta = 0f, float maxTheta = (float)Math.PI)
        {
            Mesh mesh = new Mesh("Sphere", color);
            mesh.Type = FigureType.Sphere;
            mesh.Position = position;
            mesh.RadiusInCells = radius / GPO.cellSize;

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

        public static Mesh CreateHexPrism(Vector3 position, float size, float height, Color color)
        {
            Mesh mesh = new Mesh("HexPrism", color);
            mesh.Type = FigureType.HexPrism;
            mesh.Position = position;
            mesh.RadiusInCells = size / GPO.cellSize;
            mesh.HeightInCells = height / GPO.cellSize;

            // Calculate the vertices for the top and bottom hexagons
            List<Vertex> vertices = new List<Vertex>();
            float angleIncrement = (float)(Math.PI / 3); // 60 degrees

            // Top and bottom vertices
            for (int i = 0; i < 6; i++)
            {
                float angle = i * angleIncrement;
                float x = size * (float)Math.Cos(angle);
                float z = size * (float)Math.Sin(angle);

                // Top vertex
                vertices.Add(new Vertex(new Vector3(x, height / 2, z), Vector3.Zero));
                // Bottom vertex
                vertices.Add(new Vertex(new Vector3(x, -height / 2, z), Vector3.Zero));
            }

            // Add vertices to mesh
            mesh.Vertices.AddRange(vertices);

            // Add side faces with corrected winding order
            for (int i = 0; i < 6; i++)
            {
                int next = (i + 1) % 6;

                int topCurrent = i * 2;
                int bottomCurrent = i * 2 + 1;
                int topNext = next * 2;
                int bottomNext = next * 2 + 1;

                // Side face (two triangles) with corrected vertex order
                mesh.Faces.Add(new Face(topCurrent, topNext, bottomCurrent));     // Triangle 1
                mesh.Faces.Add(new Face(topNext, bottomNext, bottomCurrent));     // Triangle 2
            }

            // Add top and bottom faces
            // Top face
            int centerTopIndex = mesh.Vertices.Count;
            mesh.Vertices.Add(new Vertex(new Vector3(0, height / 2, 0), Vector3.Zero));

            for (int i = 0; i < 6; i++)
            {
                int next = (i + 1) % 6;
                // Corrected winding order for the top face
                mesh.Faces.Add(new Face(centerTopIndex, next * 2, i * 2));
            }

            // Bottom face
            int centerBottomIndex = mesh.Vertices.Count;
            mesh.Vertices.Add(new Vertex(new Vector3(0, -height / 2, 0), Vector3.Zero));

            for (int i = 0; i < 6; i++)
            {
                int next = (i + 1) % 6;
                // Corrected winding order for the bottom face
                mesh.Faces.Add(new Face(centerBottomIndex, next * 2 + 1, i * 2 + 1));
            }

            // Compute normals
            mesh.ComputeNormals();

            return mesh;
        }

        public static Mesh CreateCylinder(Vector3 position, float radius, float height, int segments, Color color)
        {
            Mesh mesh = new Mesh("Cylinder", color);
            mesh.Type = FigureType.Cylinder;
            mesh.Position = position;
            mesh.RadiusInCells = radius / GPO.cellSize;
            mesh.HeightInCells = height / GPO.cellSize;

            List<Vertex> vertices = new List<Vertex>();
            float angleIncrement = (float)(2 * Math.PI / segments);

            // Top and bottom center vertices
            Vertex topCenter = new Vertex(new Vector3(0, height / 2, 0), Vector3.UnitY);
            Vertex bottomCenter = new Vertex(new Vector3(0, -height / 2, 0), -Vector3.UnitY);
            vertices.Add(topCenter);    // Index 0
            vertices.Add(bottomCenter); // Index 1

            // Side vertices
            for (int i = 0; i <= segments; i++)
            {
                float angle = i * angleIncrement;
                float x = radius * (float)Math.Cos(angle);
                float z = radius * (float)Math.Sin(angle);

                // Top vertex
                vertices.Add(new Vertex(new Vector3(x, height / 2, z), Vector3.Zero));
                // Bottom vertex
                vertices.Add(new Vertex(new Vector3(x, -height / 2, z), Vector3.Zero));
            }

            // Add vertices to mesh
            mesh.Vertices.AddRange(vertices);

            int offset = 2; // Offset due to top and bottom center vertices

            // Add faces
            for (int i = 0; i < segments; i++)
            {
                int topVertexIndex = offset + i * 2;
                int topNextVertexIndex = offset + ((i + 1) % segments) * 2;

                int bottomVertexIndex = offset + i * 2 + 1;
                int bottomNextVertexIndex = offset + ((i + 1) % segments) * 2 + 1;

                // Top face (winding order to make normals point up)
                mesh.Faces.Add(new Face(0, topVertexIndex, topNextVertexIndex));

                // Bottom face (winding order to make normals point down)
                mesh.Faces.Add(new Face(1, bottomNextVertexIndex, bottomVertexIndex));

                // Side faces
                // First triangle
                mesh.Faces.Add(new Face(topVertexIndex, topNextVertexIndex, bottomVertexIndex));
                // Second triangle
                mesh.Faces.Add(new Face(topNextVertexIndex, bottomNextVertexIndex, bottomVertexIndex));
            }

            // Compute normals
            mesh.ComputeNormals();

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
