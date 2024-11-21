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
                            if (indentation.Type == IndentationType.Sphere)
                            {
                                y = CalculateSphericalIndentationY(i, j, indentation, cellSize, out normal);
                            }
                            else if (indentation.Type == IndentationType.Cube)
                            {
                                y = -indentation.Depth * cellSize;
                                normal = Vector3.UnitY;
                            }
                            else if (indentation.Type == IndentationType.HexPrism)
                            {
                                y = -indentation.Depth * cellSize;
                                normal = Vector3.UnitY;
                            }
                            else if (indentation.Type == IndentationType.Tetrahedron)
                            {
                                y = CalculateTetrahedralIndentationY(i, j, indentation, cellSize, out normal);
                            }
                            break;
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
                if (indentation.Type == IndentationType.Cube)
                    AddCubeIndentationWalls(mesh, vertices, indentation, cellSize, indentationEdges);
                else if (indentation.Type == IndentationType.Sphere)
                    AddSphereIndentationWalls(mesh, vertices, indentation, cellSize, indentationEdges);
                else if (indentation.Type == IndentationType.HexPrism)
                    AddHexPrismIndentationWalls(mesh, vertices, indentation, cellSize, indentationEdges);
                else if (indentation.Type == IndentationType.Tetrahedron)
                    AddTetrahedronIndentationWalls(mesh, vertices, indentation, cellSize, indentationEdges);
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
        private static bool IsPointInPolygon(float x, float z, List<(float x, float z)> polygon)
        {
            int intersections = 0;
            int count = polygon.Count;

            for (int i = 0; i < count; i++)
            {
                var (x1, z1) = polygon[i];
                var (x2, z2) = polygon[(i + 1) % count];

                if (IsIntersecting(x1, z1, x2, z2, x, z, x + 10000, z))
                {
                    intersections++;
                }
            }

            return (intersections % 2 != 0);
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
        /*private static bool IsVertexInIndentation(int i, int j, Indentation indentation, float cellSize)
        {
            if (indentation.Type == IndentationType.Cube || indentation.Type == IndentationType.Sphere)
            {
                return i >= indentation.GridX && i <= indentation.GridX + indentation.Width &&
                       j >= indentation.GridZ && j <= indentation.GridZ + indentation.Depth;
            }
            else if (indentation.Type == IndentationType.HexPrism)
            {
                return IsPointInHexagon(i, j, indentation);
            }
            else if (indentation.Type == IndentationType.Tetrahedron)
            {
                return IsPointInTriangle(i, j, indentation);
            }
            return false;
        }*/
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
        private static void AddHexPrismIndentationWalls(Mesh mesh, Vertex[,] vertices, Indentation indentation, float cellSize, List<(Vector3 Start, Vector3 End)> indentationEdges)
        {
            // Parameters for the hexagon
            float centerX = indentation.GridX + indentation.Width / 2f;
            float centerZ = indentation.GridZ + indentation.Depth / 2f;
            float radius = indentation.Width / 2f; // Assuming width and depth are equal

            // Calculate the hexagon vertices
            List<(float x, float z)> hexVertices = new List<(float x, float z)>();
            for (int i = 0; i < 6; i++)
            {
                float angle = (float)Math.PI / 3f * i; // 60 degrees increments
                float x = centerX + radius * (float)Math.Cos(angle);
                float z = centerZ + radius * (float)Math.Sin(angle);
                hexVertices.Add((x, z));
            }

            // Identify the perimeter vertices within the grid
            List<(int i, int j)> perimeterVertices = new List<(int i, int j)>();

            for (int i = 0; i <= mesh.GridWidth; i++)
            {
                for (int j = 0; j <= mesh.GridDepth; j++)
                {
                    float x = i;
                    float z = j;

                    // Check if the point is inside the hexagon
                    if (IsPointInHexagon(x, z, hexVertices))
                    {
                        // Check if this vertex is on the boundary
                        bool isBoundary = false;
                        foreach (var neighbor in GetNeighbors(i, j))
                        {
                            int ni = neighbor.i;
                            int nj = neighbor.j;
                            if (ni < 0 || ni > mesh.GridWidth || nj < 0 || nj > mesh.GridDepth)
                                continue;

                            float nx = ni;
                            float nz = nj;

                            if (!IsPointInHexagon(nx, nz, hexVertices))
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

            // Sort perimeter vertices clockwise
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

        // Helper method to check if a point is inside a hexagon
        private static bool IsPointInHexagon(float x, float z, List<(float x, float z)> hexVertices)
        {
            int intersections = 0;
            float x0 = x;
            float z0 = z;

            for (int i = 0; i < hexVertices.Count; i++)
            {
                var v1 = hexVertices[i];
                var v2 = hexVertices[(i + 1) % hexVertices.Count];

                if (IsIntersecting(v1.x, v1.z, v2.x, v2.z, x0, z0, x0 + 10000, z0))
                {
                    intersections++;
                }
            }

            return (intersections % 2 != 0);
        }

        // Helper method to check if two lines intersect
        private static bool IsIntersecting(float x1, float z1, float x2, float z2, float x3, float z3, float x4, float z4)
        {
            float denom = (x1 - x2) * (z3 - z4) - (z1 - z2) * (x3 - x4);
            if (denom == 0)
                return false;

            float ua = ((x1 - x3) * (z3 - z4) - (z1 - z3) * (x3 - x4)) / denom;
            float ub = ((x1 - x3) * (z1 - z2) - (z1 - z3) * (x1 - x2)) / denom;

            return (ua >= 0 && ua <= 1 && ub >= 0 && ub <= 1);
        }
        #endregion
        #region Тетраэдр лунка
        private static void AddTetrahedronIndentationWalls(Mesh mesh, Vertex[,] vertices, Indentation indentation, float cellSize, List<(Vector3 Start, Vector3 End)> indentationEdges)
        {
            // Parameters for the tetrahedron
            float centerX = indentation.GridX + indentation.Width / 2f;
            float centerZ = indentation.GridZ + indentation.Depth / 2f;
            float size = indentation.Width; // Assuming width and depth are equal

            // Define the base triangle vertices in grid coordinates
            List<(float x, float z)> triangleVertices = new List<(float x, float z)>
            {
                (centerX, centerZ + size / (2 * (float)Math.Sqrt(3))),
                (centerX - size / 2, centerZ - size / (2 * (float)Math.Sqrt(3))),
                (centerX + size / 2, centerZ - size / (2 * (float)Math.Sqrt(3)))
            };

            // Identify the perimeter vertices within the grid
            List<(int i, int j)> perimeterVertices = new List<(int i, int j)>();

            for (int i = 0; i <= mesh.GridWidth; i++)
            {
                for (int j = 0; j <= mesh.GridDepth; j++)
                {
                    int x = i;
                    int z = j;

                    // Check if the point is inside the triangle
                    if (IsPointInTriangle(x, z, triangleVertices))
                    {
                        // Check if this vertex is on the boundary
                        bool isBoundary = false;
                        foreach (var neighbor in GetNeighbors(i, j))
                        {
                            int ni = neighbor.i;
                            int nj = neighbor.j;
                            if (ni < 0 || ni > mesh.GridWidth || nj < 0 || nj > mesh.GridDepth)
                                continue;

                            int nx = ni;
                            int nz = nj;

                            if (!IsPointInTriangle(nx, nz, triangleVertices))
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

            // Sort perimeter vertices clockwise
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

        // Helper method to check if a point is inside a triangle
        private static bool IsPointInTriangle(float x, float z, List<(float x, float z)> triangle)
        {
            var (x1, z1) = triangle[0];
            var (x2, z2) = triangle[1];
            var (x3, z3) = triangle[2];

            float denominator = ((z2 - z3) * (x1 - x3) + (x3 - x2) * (z1 - z3));
            float a = ((z2 - z3) * (x - x3) + (x3 - x2) * (z - z3)) / denominator;
            float b = ((z3 - z1) * (x - x3) + (x1 - x3) * (z - z3)) / denominator;
            float c = 1 - a - b;

            return 0 <= a && a <= 1 && 0 <= b && b <= 1 && 0 <= c && c <= 1;
        }

        private static float CalculateTetrahedralIndentationY(int i, int j, Indentation indentation, float cellSize, out Vector3 normal)
        {
            // Parameters for the tetrahedron
            float centerX = indentation.GridX + indentation.Width / 2f;
            float centerZ = indentation.GridZ + indentation.Depth / 2f;
            float size = indentation.Width; // Assuming width and depth are equal

            // Height of the tetrahedron
            float height = size * (float)(Math.Sqrt(6) / 3);

            // Convert grid coordinates to world coordinates
            float x = i;
            float z = j;

            float dx = x - centerX;
            float dz = z - centerZ;

            Vector3 v0 = new Vector3(centerX, 0, centerZ + size / (2 * (float)Math.Sqrt(3)));
            Vector3 v1 = new Vector3(centerX - size / 2f, 0, centerZ - size / (2 * (float)Math.Sqrt(3)));
            Vector3 v2 = new Vector3(centerX + size / 2f, 0, centerZ - size / (2 * (float)Math.Sqrt(3)));

            Vector3 vTop = new Vector3(centerX, -height, centerZ);

            Vector3 p = new Vector3(x, 0, z);
            Vector3 barycentric = CalculateBarycentricCoordinates(p, v0, v1, v2);

            float y = -barycentric.X * vTop.Y - barycentric.Y * vTop.Y - barycentric.Z * vTop.Y;

            normal = Vector3.Normalize(Vector3.Cross(v1 - v0, v2 - v0));

            return y * cellSize;
        }
        private static Vector3 CalculateBarycentricCoordinates(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
        {
            Vector3 v0 = b - a;
            Vector3 v1 = c - a;
            Vector3 v2 = p - a;

            float d00 = Vector3.Dot(v0, v0);
            float d01 = Vector3.Dot(v0, v1);
            float d11 = Vector3.Dot(v1, v1);
            float d20 = Vector3.Dot(v2, v0);
            float d21 = Vector3.Dot(v2, v1);
            float denom = d00 * d11 - d01 * d01;

            if (denom == 0)
                return new Vector3(-1, -1, -1);

            float v = (d11 * d20 - d01 * d21) / denom;
            float w = (d00 * d21 - d01 * d20) / denom;
            float u = 1.0f - v - w;

            return new Vector3(u, v, w);
        }
        #endregion
    }
}
