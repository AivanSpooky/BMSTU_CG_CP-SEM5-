using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Windows.Forms;

namespace src
{
    public class Camera
    {
        public Vector3 Position { get; set; }
        public Vector3 Target { get; set; }
        public Vector3 Up { get; private set; }
        public float FieldOfView { get; set; }
        public float AspectRatio { get; set; }
        public float NearClip { get; set; }
        public float FarClip { get; set; }

        public Camera(Vector3 position, Vector3 target, Vector3 up, float fieldOfView, float aspectRatio, float nearClip, float farClip)
        {
            Position = position;
            Target = target;
            Up = up;
            FieldOfView = fieldOfView;
            AspectRatio = aspectRatio;
            NearClip = nearClip;
            FarClip = farClip;
        }

        public Matrix4x4 GetViewMatrix()
        {
            return Matrix4x4.CreateLookAt(Position, Target, Up);
        }

        public Matrix4x4 GetProjectionMatrix()
        {
            return Matrix4x4.CreatePerspectiveFieldOfView(FieldOfView, AspectRatio, NearClip, FarClip);
        }
    }

    public class Renderer
    {
        private Matrix4x4 cameraViewMatrix;
        private Matrix4x4 projectionMatrix;
        private Camera camera;
        private float zoom = 100.0f; // Initial zoom level

        private PictureBox main_pb;
        private RenderAlgorithm renderAlgorithm;

        public delegate void RenderAlgorithm(Graphics g, List<Point> projectedVertices, int[][] faces);

        public Renderer(Form mainform, PictureBox pictureBox)
        {
            main_pb = pictureBox;
            // Define initial camera parameters
            Vector3 initialPosition = new Vector3(0, 10, -zoom);
            Vector3 initialTarget = Vector3.UnitY;
            Vector3 initialUp = Vector3.UnitZ;
            float initialFOV = (float)Math.PI / 2; // 45 degrees
            float aspectRatio = (float)main_pb.Width / main_pb.Height;
            float nearClip = 0.1f;
            float farClip = 1000f;

            // Initialize camera with new parameters
            camera = new Camera(
                initialPosition,
                initialTarget,
                initialUp,
                initialFOV,
                aspectRatio,
                nearClip,
                farClip
            );

            renderAlgorithm = ZBufferRendering;

            // Set initial camera and projection
            UpdateCameraAndProjection();

            mainform.KeyDown += new KeyEventHandler(OnKeyDown);
            main_pb.Paint += new PaintEventHandler(OnPaint);
        }

        public void UpdateCameraAndProjection()
        {
            // Update view matrix
            cameraViewMatrix = camera.GetViewMatrix();

            // Update projection matrix
            projectionMatrix = camera.GetProjectionMatrix();

            foreach (var shape in Scene.Shapes)
            {
                shape.TransformedVertices = new List<Vector3>(shape.Vertices.Count);

                for (int i = 0; i < shape.Vertices.Count; i++)
                {
                    // Apply view and projection matrices
                    var transformed = Vector3.Transform(shape.Vertices[i], cameraViewMatrix);
                    transformed = Vector3.Transform(transformed, projectionMatrix);
                    shape.TransformedVertices.Add(transformed);
                }
            }
        }

        private List<Point> ProjectVertices(List<Vector3> vertices)
        {
            List<Point> projected = new List<Point>();

            foreach (var vertex in vertices)
            {
                var transformed = Vector3.Transform(vertex, cameraViewMatrix);
                transformed = Vector3.Transform(transformed, projectionMatrix);

                if (transformed.Z != 0)
                {
                    float x = (transformed.X / transformed.Z) * (main_pb.Width / 2) + (main_pb.Width / 2);
                    float y = (transformed.Y / transformed.Z) * (main_pb.Height / 2) + (main_pb.Height / 2);

                    int screenX = (int)Clamp(x, 0, main_pb.Width - 1);
                    int screenY = (int)Clamp(y, 0, main_pb.Height - 1);

                    projected.Add(new Point(screenX, screenY));
                }
                else
                {
                    Console.WriteLine($"Vertex {vertex} has Z=0, skipping projection.");
                    projected.Add(new Point(0, 0)); // or some default value
                }
            }
            return projected;
        }

        private float Clamp(float value, float min, float max)
        {
            return Math.Max(min, Math.Min(value, max));
        }

        private float[,] zBuffer;

        private void InitializeZBuffer(int width, int height)
        {
            zBuffer = new float[width, height];

            // Initialize Z-buffer with very large values so that every point is beyond the screen
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    zBuffer[x, y] = float.MinValue;
                }
            }
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.White);

            InitializeZBuffer(main_pb.Width, main_pb.Height);

            foreach (var shape in Scene.Shapes)
            {
                List<Point> projectedVertices = ProjectVertices(shape.TransformedVertices);

                foreach (Polygon polygon in shape.Polygons)
                {
                    try
                    {
                        Point[] points = polygon.VertexIndices.Select(i => projectedVertices[i]).ToArray();
                        if (points.Length >= 3)
                        {
                            // Break polygon into triangles
                            List<int[]> triangles = new List<int[]>();
                            for (int i = 1; i < polygon.VertexIndices.Length - 1; i++)
                            {
                                triangles.Add(new int[] { polygon.VertexIndices[0], polygon.VertexIndices[i], polygon.VertexIndices[i + 1] });
                            }

                            // Process each triangle
                            foreach (var triangle in triangles)
                            {
                                // Extract vertices by indices from triangle
                                var triangleVertices = new List<Point>
                            {
                                projectedVertices[triangle[0]],
                                projectedVertices[triangle[1]],
                                projectedVertices[triangle[2]]
                            };

                                // Fill triangle with Z-buffer consideration
                                FillTriangleWithZBuffer(triangleVertices, polygon, shape.TransformedVertices, shape.ShapeColor, g);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error rendering polygon: {ex.Message}");
                    }
                }
            }
        }

        // Function to fill triangle with Z-buffer
        private void FillTriangleWithZBuffer(List<Point> triangleVertices, Polygon polygon, List<Vector3> originalVertices, Color shapeColor, Graphics g)
        {
            // Use bounding box to limit the search area
            Point min = new Point(triangleVertices.Min(p => p.X), triangleVertices.Min(p => p.Y));
            Point max = new Point(triangleVertices.Max(p => p.X), triangleVertices.Max(p => p.Y));

            // Iterate over all points inside bounding box
            for (int x = min.X; x <= max.X; x++)
            {
                for (int y = min.Y; y <= max.Y; y++)
                {
                    Point currentPoint = new Point(x, y);

                    if (IsPointInTriangle(currentPoint, triangleVertices)) // Check if point is inside the triangle
                    {
                        // Compute barycentric coordinates to determine Z
                        var barycentricCoords = ComputeBarycentricCoordinates(currentPoint, triangleVertices);

                        if (barycentricCoords.Item1 >= 0 && barycentricCoords.Item2 >= 0 && barycentricCoords.Item3 >= 0)
                        {
                            // Interpolate Z coordinate from original vertices
                            float z = barycentricCoords.Item1 * originalVertices[polygon.VertexIndices[0]].Z +
                                      barycentricCoords.Item2 * originalVertices[polygon.VertexIndices[1]].Z +
                                      barycentricCoords.Item3 * originalVertices[polygon.VertexIndices[2]].Z;

                            // Check Z-buffer
                            if (z > zBuffer[x, y])
                            {
                                zBuffer[x, y] = z; // Update Z-buffer
                                                   // Draw pixel considering the shape color
                                g.FillRectangle(new SolidBrush(shapeColor), x, y, 1, 1);
                            }
                        }
                    }
                }
            }
        }

        private bool IsPointInTriangle(Point p, List<Point> triangle)
        {
            var (lambda1, lambda2, lambda3) = ComputeBarycentricCoordinates(p, triangle);

            // Точка внутри треугольника, если все барицентрические координаты в пределах от 0 до 1
            return lambda1 >= 0 && lambda2 >= 0 && lambda3 >= 0 && lambda1 <= 1 && lambda2 <= 1 && lambda3 <= 1;
        }

        private (float, float, float) ComputeBarycentricCoordinates(Point p, List<Point> triangle)
        {
            // Вычисление знаменателя барицентрических координат
            float denom = (float)((triangle[1].Y - triangle[2].Y) * (triangle[0].X - triangle[2].X) +
                                   (triangle[2].X - triangle[1].X) * (triangle[0].Y - triangle[2].Y));

            // Если знаменатель очень мал, треугольник вырожден
            if (Math.Abs(denom) < 1e-5)
            {
                return (-1, -1, -1); // Означает, что точка вне треугольника
            }

            // Вычисление барицентрических координат
            float lambda1 = ((triangle[1].Y - triangle[2].Y) * (p.X - triangle[2].X) +
                             (triangle[2].X - triangle[1].X) * (p.Y - triangle[2].Y)) / denom;

            float lambda2 = ((triangle[2].Y - triangle[0].Y) * (p.X - triangle[2].X) +
                             (triangle[0].X - triangle[2].X) * (p.Y - triangle[2].Y)) / denom;

            float lambda3 = 1.0f - lambda1 - lambda2;

            return (lambda1, lambda2, lambda3);
        }

        public void Render(AlgoZbuffer zbuf)
        {
            // Clear previous render
            main_pb.Image = new Bitmap(main_pb.Width, main_pb.Height);
            using (Graphics g = Graphics.FromImage(main_pb.Image))
            {
                // Render each shape
                foreach (var shape in Scene.Shapes)
                {
                    // Project vertices of the shape
                    List<Point> projectedVertices = ProjectVertices(shape.Vertices);

                    // Draw the shape using Z-buffer algorithm
                    zbuf.RenderShape(g, projectedVertices, shape.Polygons);
                }

                // Optional: Draw additional elements like UI overlays, etc.
            }
        }

        private void ZBufferRendering(Graphics g, List<Point> projectedVertices, int[][] faces)
        {
            foreach (var face in faces)
            {
                Point[] points = face.Select(i => projectedVertices[i]).ToArray();
                if (points.Length >= 3)
                {
                    g.DrawPolygon(Pens.Black, points);
                }
            }
        }
        public void OnKeyDown(object sender, KeyEventArgs e)
        {
            float rotationSpeed = 0.05f;
            float movementSpeed = 0.5f;

            if (e.KeyCode == Keys.A) // Rotate left
            {
                float angle = -rotationSpeed;
                Vector3 direction = camera.Position - camera.Target;
                Matrix4x4 rotation = Matrix4x4.CreateRotationY(angle);
                camera.Position = Vector3.Transform(direction, rotation) + camera.Target;
            }
            if (e.KeyCode == Keys.D) // Rotate right
            {
                float angle = rotationSpeed;
                Vector3 direction = camera.Position - camera.Target;
                Matrix4x4 rotation = Matrix4x4.CreateRotationY(angle);
                camera.Position = Vector3.Transform(direction, rotation) + camera.Target;
            }
            if (e.KeyCode == Keys.W) // Move target up (increase Y)
            {
                camera.Target = new Vector3(camera.Target.X, camera.Target.Y + movementSpeed, camera.Target.Z);
            }
            if (e.KeyCode == Keys.S) // Move target down (decrease Y)
            {
                camera.Target = new Vector3(camera.Target.X, camera.Target.Y - movementSpeed, camera.Target.Z);
            }
            if (e.KeyCode == Keys.Oemplus) // Zoom in
            {
                camera.FieldOfView = Math.Max(camera.FieldOfView - 0.1f, 0.1f);
            }
            if (e.KeyCode == Keys.OemMinus) // Zoom out
            {
                camera.FieldOfView = Math.Min(camera.FieldOfView + 0.1f, (float)Math.PI / 2);
            }

            UpdateCameraAndProjection();
            main_pb.Invalidate();
        }
    }
}
