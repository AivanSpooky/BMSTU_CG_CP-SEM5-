using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace test
{
    public partial class Mesh
    {
        public static Mesh CreateGridPlane(Vector3 position, int gridWidth, int gridDepth, float cellSize, Color color, List<Indentation> indentations, List<(Vector3 Start, Vector3 End)> indentationEdges)
        {
            Mesh mesh = new Mesh("GridPlane", color);
            mesh.Position = position;

            // Create a 2D array to store vertices
            Vertex[,] vertices = new Vertex[gridWidth + 1, gridDepth + 1];

            // Create vertices with indentations
            for (int i = 0; i <= gridWidth; i++)
            {
                for (int j = 0; j <= gridDepth; j++)
                {
                    float x = i * cellSize;
                    float z = j * cellSize;
                    float y = 0f; // Base height

                    Vector3 normal = Vector3.UnitY; // Default normal

                    // Check if the vertex is within any indentation
                    foreach (var indentation in indentations)
                    {
                        if (IsVertexInIndentation(i, j, indentation, cellSize))
                        {
                            switch (indentation.Type)
                            {
                                case IndentationType.Sphere:
                                    y = CalculateSphericalIndentationY(i, j, indentation, cellSize, out normal);
                                    break;
                                case IndentationType.Cube:
                                case IndentationType.HexPrism:
                                case IndentationType.Cylinder:
                                    y = -indentation.Height * cellSize;
                                    normal = Vector3.UnitY;
                                    break;
                                default:
                                    break;
                            }
                            break; // Выход из цикла после первого совпадения
                        }
                    }

                    Vector3 vertexPos = new Vector3(x, y, z);
                    vertices[i, j] = new Vertex(vertexPos, normal);
                }
            }

            // Add vertices to the mesh
            foreach (var vertex in vertices)
            {
                mesh.Vertices.Add(vertex);
            }

            // Create faces (two triangles per grid cell)
            for (int i = 0; i < gridWidth; i++)
            {
                for (int j = 0; j < gridDepth; j++)
                {
                    int topLeft = i * (gridDepth + 1) + j;
                    int topRight = (i + 1) * (gridDepth + 1) + j;
                    int bottomLeft = i * (gridDepth + 1) + (j + 1);
                    int bottomRight = (i + 1) * (gridDepth + 1) + (j + 1);

                    // First triangle
                    mesh.Faces.Add(new Face(topLeft, bottomLeft, topRight));
                    // Second triangle
                    mesh.Faces.Add(new Face(bottomLeft, bottomRight, topRight));
                }
            }

            // Add walls for all indentations to create depth
            foreach (var indentation in indentations)
            {
                switch (indentation.Type)
                {
                    case IndentationType.Cube:
                        AddCubeIndentationWalls(mesh, vertices, indentation, cellSize, indentationEdges);
                        break;
                    case IndentationType.Sphere:
                        AddSphereIndentationWalls(mesh, vertices, indentation, cellSize, indentationEdges);
                        break;
                    case IndentationType.HexPrism:
                        AddHexPrismIndentationWalls(mesh, vertices, indentation, cellSize, indentationEdges, Form1.light.Position);
                        break;
                    case IndentationType.Cylinder:
                        AddCylinderIndentationWalls(mesh, vertices, indentation, cellSize, indentationEdges, Form1.light.Position);
                        break;
                    default:
                        break;
                }
            }

            // Recompute normals for smooth shading
            mesh.ComputeNormals();

            return mesh;
        }

        #region Куб лунка
        private static void AddCubeIndentationWalls(Mesh mesh, Vertex[,] vertices, Indentation indentation, float cellSize, List<(Vector3 Start, Vector3 End)> indentationEdges)
        {
            int startX = indentation.GridX;
            int endX = indentation.GridX + indentation.Width;
            int startZ = indentation.GridZ;
            int endZ = indentation.GridZ + indentation.Depth;

            float indentationY = -indentation.Depth * cellSize;

            // Iterate around the perimeter of the indentation
            for (int i = startX; i < endX; i++)
            {
                for (int j = startZ; j < endZ; j++)
                {
                    // Left wall
                    if (i == startX)
                    {
                        AddVerticalFace(mesh, vertices[i, j], vertices[i, j + 1], indentationY, indentationEdges);
                    }
                    // Right wall
                    if (i == endX - 1)
                    {
                        AddVerticalFace(mesh, vertices[i + 1, j + 1], vertices[i + 1, j], indentationY, indentationEdges);
                    }
                    // Back wall
                    if (j == startZ)
                    {
                        AddVerticalFace(mesh, vertices[i + 1, j], vertices[i, j], indentationY, indentationEdges);
                    }
                    // Front wall
                    if (j == endZ - 1)
                    {
                        AddVerticalFace(mesh, vertices[i, j + 1], vertices[i + 1, j + 1], indentationY, indentationEdges);
                    }
                }
            }
        }

        private static void AddVerticalFace(Mesh mesh, Vertex v1, Vertex v2, float indentationY, List<(Vector3 Start, Vector3 End)> indentationEdges)
        {
            // Define the lower vertices based on indentation depth
            Vector3 v1Lower = new Vector3(v1.Position.X, indentationY, v1.Position.Z);
            Vector3 v2Lower = new Vector3(v2.Position.X, indentationY, v2.Position.Z);

            // Calculate the normal for the vertical face
            Vector3 edge1 = v2.Position - v1.Position;
            Vector3 edge2 = v1Lower - v1.Position;
            Vector3 normal = Vector3.Normalize(Vector3.Cross(edge1, edge2));

            // Add new vertices for the wall
            int index = mesh.Vertices.Count;
            mesh.Vertices.Add(new Vertex(v1.Position, normal));
            mesh.Vertices.Add(new Vertex(v2.Position, normal));
            mesh.Vertices.Add(new Vertex(v2Lower, normal));
            mesh.Vertices.Add(new Vertex(v1Lower, normal));

            // Add two triangles to form the wall face
            mesh.Faces.Add(new Face(index, index + 1, index + 2));
            mesh.Faces.Add(new Face(index, index + 2, index + 3));

            // Record the edges for debugging
            indentationEdges.Add((v1.Position, v2.Position));
            indentationEdges.Add((v2.Position, v2Lower));
            indentationEdges.Add((v2Lower, v1Lower));
            indentationEdges.Add((v1Lower, v1.Position));
        }
#endregion
        #region Общие методы
        // Helper method to get neighboring grid positions
        private static List<(int i, int j)> GetNeighbors(int i, int j)
        {
            return new List<(int i, int j)>
            {
                (i - 1, j),
                (i + 1, j),
                (i, j - 1),
                (i, j + 1)
            };
        }

        // Helper method to sort perimeter vertices clockwise
        private static List<(int i, int j)> SortPerimeterClockwise(List<(int i, int j)> perimeterVertices, float centerX, float centerZ)
        {
            // Calculate angles for each perimeter vertex relative to the center
            var verticesWithAngles = perimeterVertices.Select(v =>
            {
                float dx = v.i - centerX;
                float dz = v.j - centerZ;
                float angle = (float)Math.Atan2(dz, dx);
                return new { Vertex = v, Angle = angle };
            })
            .OrderBy(v => v.Angle)
            .Select(v => v.Vertex)
            .ToList();

            return verticesWithAngles;
        }

        // Helper method to calculate wall normal facing outward
        private static Vector3 CalculateWallNormal(Vector3 v1, Vector3 v2, float indentationDepth)
        {
            // Calculate the vector along the perimeter edge
            Vector3 edge = v2 - v1;

            // Calculate a vector pointing outward (perpendicular to the edge)
            Vector3 outward = new Vector3(-edge.Z, 0, edge.X);
            outward = Vector3.Normalize(outward);

            return outward;
        }
        #endregion
        #region Сфера лунка
        private static void AddSphereIndentationWalls(Mesh mesh, Vertex[,] vertices, Indentation indentation, float cellSize, List<(Vector3 Start, Vector3 End)> indentationEdges)
        {
            // Ищем корды лунки и радиус
            float centerX = indentation.GridX + indentation.Width / 2f;         // В КОНСТРУКТОРСКОЙ ЧАСТИ ЭТО X_0
            float centerZ = indentation.GridZ + indentation.Depth / 2f;         // В КОНСТРУКТОРСКОЙ ЧАСТИ ЭТО Z_0
            float radius = Math.Min(indentation.Width, indentation.Depth) / 2f; // В КОНСТРУКТОРСКОЙ ЧАСТИ ЭТО R

            // Identify the perimeter of the spherical indentation
            // We'll iterate over the grid and find vertices that are on the boundary of the sphere
            List<(int i, int j)> perimeterVertices = new List<(int i, int j)>();
            
            for (int i = indentation.GridX; i <= indentation.GridX + indentation.Width; i++)
            {
                for (int j = indentation.GridZ; j <= indentation.GridZ + indentation.Depth; j++)
                {
                    // СОСТАВЛЯЕМ УРАВНЕНИЕ СФЕРЫ
                    float dx = i - centerX;
                    float dz = j - centerZ;
                    float distanceSquared = dx * dx + dz * dz;

                    if (distanceSquared <= radius * radius)
                    {
                        // Check if this vertex is on the boundary
                        // A vertex is on the boundary if at least one of its neighboring vertices is outside the sphere
                        bool isBoundary = false;
                        foreach (var neighbor in GetNeighbors(i, j))
                        {
                            int ni = neighbor.i;
                            int nj = neighbor.j;
                            if (ni < 0 || ni > mesh.GridWidth || nj < 0 || nj > mesh.GridDepth)
                                continue;

                            float ndx = ni - centerX;
                            float ndz = nj - centerZ;
                            float ndistanceSquared = ndx * ndx + ndz * ndz;

                            if (ndistanceSquared > radius * radius)
                            {
                                isBoundary = true;
                                break;
                            }
                        }

                        if (isBoundary)
                        {
                            perimeterVertices.Add((i, j));
                        }
                    }
                }
            }

            // To avoid duplicates and ensure correct ordering, sort the perimeter vertices clockwise
            perimeterVertices = SortPerimeterClockwise(perimeterVertices, centerX, centerZ);

            // Create vertical walls along the perimeter
            for (int idx = 0; idx < perimeterVertices.Count; idx++)
            {
                var current = perimeterVertices[idx];
                var next = perimeterVertices[(idx + 1) % perimeterVertices.Count];

                // Get the corresponding vertices
                Vertex vCurrent = vertices[current.i, current.j];
                Vertex vNext = vertices[next.i, next.j];

                // Define the lower vertices based on indentation depth
                float indentationY = -indentation.Depth * cellSize;

                Vector3 vCurrentLower = new Vector3(vCurrent.Position.X, indentationY, vCurrent.Position.Z);
                Vector3 vNextLower = new Vector3(vNext.Position.X, indentationY, vNext.Position.Z);

                // Calculate the normal for the vertical wall (facing outward)
                Vector3 wallNormal = CalculateWallNormal(vCurrent.Position, vNext.Position, indentation.Depth * cellSize);

                // Add new vertices for the wall
                int index = mesh.Vertices.Count;
                mesh.Vertices.Add(new Vertex(vCurrent.Position, wallNormal));
                mesh.Vertices.Add(new Vertex(vNext.Position, wallNormal));
                mesh.Vertices.Add(new Vertex(vNextLower, wallNormal));
                mesh.Vertices.Add(new Vertex(vCurrentLower, wallNormal));

                // Add two triangles to form the wall face
                mesh.Faces.Add(new Face(index, index + 1, index + 2));
                mesh.Faces.Add(new Face(index, index + 2, index + 3));

                // Record the edges for debugging
                indentationEdges.Add((vCurrent.Position, vNext.Position));
                indentationEdges.Add((vNext.Position, vNextLower));
                indentationEdges.Add((vNextLower, vCurrentLower));
                indentationEdges.Add((vCurrentLower, vCurrent.Position));
            }
        }

        private static bool IsVertexInIndentation(int i, int j, Indentation indentation, float cellSize)
        {
            return i >= indentation.GridX && i < indentation.GridX + indentation.Width &&
                   j >= indentation.GridZ && j < indentation.GridZ + indentation.Depth;
        }

        private static float CalculateSphericalIndentationY(int i, int j, Indentation indentation, float cellSize, out Vector3 normal)
        {
            // Ищем корды лунки и радиус
            float centerX = indentation.GridX + indentation.Width / 2f; // В КОНСТРУКТОРСКОЙ ЧАСТИ ЭТО X_0
            float centerZ = indentation.GridZ + indentation.Depth / 2f; // В КОНСТРУКТОРСКОЙ ЧАСТИ ЭТО Z_0
            float radius = Math.Min(indentation.Width, indentation.Depth) / 2f; // В КОНСТРУКТОРСКОЙ ЧАСТИ ЭТО R

            // СОСТАВЛЯЕМ УРАВНЕНИЕ СФЕРЫ
            float dx = i - centerX;
            float dz = j - centerZ;
            float distanceSquared = dx * dx + dz * dz;

            if (distanceSquared <= radius * radius)
            {
                // Sphere equation: x^2 + y^2 + z^2 = r^2 => y = -sqrt(r^2 - x^2 - z^2)
                float y = -(float)Math.Sqrt(radius * radius - distanceSquared);

                // Normal is the normalized vector from the sphere center to the point
                normal = Vector3.Normalize(new Vector3(dx, y, dz));

                return y * cellSize; // Scale Y-coordinate
            }
            else
            {
                normal = Vector3.UnitY;
                return 0f;
            }
        }
        #endregion
        #region Шестиугольная призма лунка
        private static void AddHexPrismIndentationWalls(
    Mesh mesh,
    Vertex[,] vertices,
    Indentation indentation,
    float cellSize,
    List<(Vector3 Start, Vector3 End)> indentationEdges,
    Vector3 lightPosition // Положение источника света
)
        {
            float centerX = (indentation.GridX + indentation.Width / 2f) * cellSize;
            float centerZ = (indentation.GridZ + indentation.Depth / 2f) * cellSize;
            float radius = (indentation.Width / 2f) * cellSize;

            List<Vector2> hexVertices = new List<Vector2>();
            for (int i = 0; i < 6; i++)
            {
                float angle = (float)(Math.PI / 3 * i);
                float x = centerX + radius * (float)Math.Cos(angle);
                float z = centerZ + radius * (float)Math.Sin(angle);
                hexVertices.Add(new Vector2(x, z));
            }

            float indentationY = -indentation.Height * cellSize;

            // Опускаем вершины внутри шестиугольника
            for (int i = 0; i < vertices.GetLength(0); i++)
            {
                for (int j = 0; j < vertices.GetLength(1); j++)
                {
                    Vector3 vertexPos = vertices[i, j].Position;
                    Vector2 point = new Vector2(vertexPos.X, vertexPos.Z);

                    if (IsPointInPolygon(point, hexVertices))
                    {
                        vertices[i, j].Position = new Vector3(vertexPos.X, indentationY, vertexPos.Z);
                    }
                }
            }

            // Создание стенок вдоль рёбер шестиугольника
            for (int i = 0; i < hexVertices.Count; i++)
            {
                int next = (i + 1) % hexVertices.Count;

                // Верхние и нижние вершины
                Vector3 topCurrent = new Vector3(hexVertices[i].X, 0, hexVertices[i].Y);
                Vector3 topNext = new Vector3(hexVertices[next].X, 0, hexVertices[next].Y);
                Vector3 bottomCurrent = new Vector3(hexVertices[i].X, indentationY, hexVertices[i].Y);
                Vector3 bottomNext = new Vector3(hexVertices[next].X, indentationY, hexVertices[next].Y);

                // Проверка положения света относительно стенки
                Vector3 wallCenter = (topCurrent + bottomCurrent) / 2;
                Vector3 lightDirection = lightPosition - wallCenter;

                // Нормаль стены (направление зависит от положения света)
                Vector3 edge = topNext - topCurrent;
                Vector3 normal = Vector3.Normalize(Vector3.Cross(edge, Vector3.UnitY));
                if (Vector3.Dot(normal, lightDirection) < 0)
                {
                    // Если нормаль направлена "не туда", инвертируем её
                    normal = -normal;
                }

                // Добавление вершин и граней для стены
                int index = mesh.Vertices.Count;
                mesh.Vertices.Add(new Vertex(topCurrent, normal));
                mesh.Vertices.Add(new Vertex(topNext, normal));
                mesh.Vertices.Add(new Vertex(bottomNext, normal));
                mesh.Vertices.Add(new Vertex(bottomCurrent, normal));

                mesh.Faces.Add(new Face(index, index + 1, index + 2));
                mesh.Faces.Add(new Face(index, index + 2, index + 3));

                // Запись рёбер для отладки
                indentationEdges.Add((topCurrent, topNext));
                indentationEdges.Add((topNext, bottomNext));
                indentationEdges.Add((bottomNext, bottomCurrent));
                indentationEdges.Add((bottomCurrent, topCurrent));
            }
        }

        private static bool IsPointInPolygon(Vector2 point, List<Vector2> polygon)
        {
            int crossings = 0;
            int count = polygon.Count;

            for (int i = 0; i < count; i++)
            {
                Vector2 a = polygon[i];
                Vector2 b = polygon[(i + 1) % count];

                if (((a.Y > point.Y) != (b.Y > point.Y)) &&
                    (point.X < (b.X - a.X) * (point.Y - a.Y) / (b.Y - a.Y + float.Epsilon) + a.X))
                {
                    crossings++;
                }
            }

            return (crossings % 2 != 0);
        }
        #endregion
        #region Цилиндр лунка
        private static void AddCylinderIndentationWalls(
    Mesh mesh,
    Vertex[,] vertices,
    Indentation indentation,
    float cellSize,
    List<(Vector3 Start, Vector3 End)> indentationEdges,
    Vector3 lightPosition // Положение источника света
)
        {
            float centerX = (indentation.GridX + indentation.Width / 2f) * cellSize;
            float centerZ = (indentation.GridZ + indentation.Depth / 2f) * cellSize;
            float radius = (indentation.Width / 2f) * cellSize;
            float indentationY = -indentation.Height * cellSize;

            int gridWidth = vertices.GetLength(0) - 1;
            int gridDepth = vertices.GetLength(1) - 1;

            // Опускаем вершины внутри круга
            for (int i = 0; i <= gridWidth; i++)
            {
                for (int j = 0; j <= gridDepth; j++)
                {
                    Vector3 vertexPos = vertices[i, j].Position;
                    float dx = vertexPos.X - centerX;
                    float dz = vertexPos.Z - centerZ;

                    if ((dx * dx + dz * dz) <= radius * radius)
                    {
                        vertices[i, j].Position = new Vector3(vertexPos.X, indentationY, vertexPos.Z);
                    }
                }
            }

            // Вычисление вершин окружности (периметр) для создания стенок
            int segments = 36;
            List<Vector3> perimeterVertices = new List<Vector3>();
            for (int i = 0; i < segments; i++)
            {
                float angle = (float)(2 * Math.PI * i / segments);
                float x = centerX + radius * (float)Math.Cos(angle);
                float z = centerZ + radius * (float)Math.Sin(angle);
                perimeterVertices.Add(new Vector3(x, 0, z));
            }

            // Создание стенок вдоль периметра
            for (int idx = 0; idx < perimeterVertices.Count; idx++)
            {
                Vector3 topCurrent = perimeterVertices[idx];
                Vector3 topNext = perimeterVertices[(idx + 1) % perimeterVertices.Count];
                Vector3 bottomCurrent = new Vector3(topCurrent.X, indentationY, topCurrent.Z);
                Vector3 bottomNext = new Vector3(topNext.X, indentationY, topNext.Z);

                Vector3 wallCenter = (topCurrent + bottomCurrent) / 2;
                Vector3 lightDirection = lightPosition - wallCenter;

                Vector3 edge = topNext - topCurrent;
                Vector3 normal = Vector3.Normalize(Vector3.Cross(edge, Vector3.UnitY));
                if (Vector3.Dot(normal, lightDirection) < 0)
                {
                    normal = -normal;
                }

                int index = mesh.Vertices.Count;
                mesh.Vertices.Add(new Vertex(topCurrent, normal));
                mesh.Vertices.Add(new Vertex(topNext, normal));
                mesh.Vertices.Add(new Vertex(bottomNext, normal));
                mesh.Vertices.Add(new Vertex(bottomCurrent, normal));

                mesh.Faces.Add(new Face(index, index + 1, index + 2));
                mesh.Faces.Add(new Face(index, index + 2, index + 3));

                indentationEdges.Add((topCurrent, topNext));
                indentationEdges.Add((topNext, bottomNext));
                indentationEdges.Add((bottomNext, bottomCurrent));
                indentationEdges.Add((bottomCurrent, topCurrent));
            }
        }

        // Метод проверки, находится ли точка внутри окружности
        private static bool IsPointInCircle(float x, float z, float centerX, float centerZ, float radius)
        {
            float dx = x - centerX;
            float dz = z - centerZ;
            return (dx * dx + dz * dz) <= radius * radius;
        }
        #endregion
    }
}
