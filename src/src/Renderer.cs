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
        public Vector3 Up { get; set; }
        public Vector3 LookAt { get; set; }

        public Camera(Vector3 position, Vector3 up, Vector3 lookAt)
        {
            Position = position;
            Up = up;
            LookAt = lookAt;
        }

        public Matrix4x4 GetViewMatrix()
        {
            return Matrix4x4.CreateLookAt(Position, LookAt, Up);
        }
    }

    public class Renderer : Control
    {
        private List<Shape3D> shapes;
        private Matrix4x4 cameraViewMatrix;
        private Matrix4x4 projectionMatrix;
        private Camera camera;
        private float zoom = 10.0f; // Initial zoom level

        public delegate void RenderAlgorithm(Graphics g, List<Point> projectedVertices, int[][] faces);
        private RenderAlgorithm renderAlgorithm;

        public Renderer()
        {
            // Initialize camera
            camera = new Camera(new Vector3(0, 0, -zoom), Vector3.UnitY, Vector3.Zero);

            shapes = new List<Shape3D>
            {
                new Cube3D(2.0f, new Vector3(0, 0, 0)),
                new Sphere3D(16, 16, new Vector3(0, 0, 10)),
                new RectangularPrism3D(20, 2, 20, new Vector3(0, -1, 0))
            };
            renderAlgorithm = ZBufferRendering;

            // Set initial camera and projection
            UpdateCameraAndProjection();

            // Subscribe to key events
            KeyDown += new KeyEventHandler(OnKeyDown);
            DoubleBuffered = true; // Improve rendering performance
        }

        private void UpdateCameraAndProjection()
        {
            // Update view matrix
            cameraViewMatrix = camera.GetViewMatrix();

            // Create projection matrix
            projectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView((float)Math.PI / 4, (float)Width / Height, 0.1f, 1000.0f);
        }

        private List<Point> ProjectVertices(List<Vector3> vertices)
        {
            List<Point> projected = new List<Point>();
            foreach (var vertex in vertices)
            {
                // Apply transformations to the vertex
                var transformed = Vector3.Transform(vertex, cameraViewMatrix);
                transformed = Vector3.Transform(transformed, projectionMatrix);

                // Handle perspective division
                float x = (transformed.X / transformed.Z) * (Width / 2) + (Width / 2);
                float y = (transformed.Y / transformed.Z) * (Height / 2) + (Height / 2);

                // Clamp values to screen bounds
                int screenX = (int)Clamp(x, 0, Width - 1);
                int screenY = (int)Clamp(y, 0, Height - 1);

                projected.Add(new Point(screenX, screenY));
            }
            return projected;
        }

        private float Clamp(float value, float min, float max)
        {
            return Math.Max(min, Math.Min(value, max));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.Clear(Color.White);

            foreach (var shape in shapes)
            {
                List<Point> projectedVertices = ProjectVertices(shape.Vertices);
                foreach (var polygon in shape.Polygons)
                {
                    try
                    {
                        Point[] points = polygon.VertexIndices.Select(i => projectedVertices[i]).ToArray();
                        if (points.Length >= 3)
                        {
                            g.DrawPolygon(Pens.Black, points);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error rendering polygon: {ex.Message}");
                    }
                }
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

        private float angleY = 0; // Angle for YZ plane rotation
        private float angleX = 0; // Angle for XZ plane rotation

        private const float zoomSpeed = 1.0f; // Zoom speed (adjusted for noticeable effect)
        private const float angleStep = 0.1f; // Angle increment

        private const float TopmostBound = 20.0f;  // Adjust these values as needed
        private const float BottommostBound = -20.0f;

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A)
            {
                // Rotate around Y-axis
                angleY -= angleStep;
            }
            if (e.KeyCode == Keys.D)
            {
                // Rotate around Y-axis
                angleY += angleStep;
            }

            if (e.KeyCode == Keys.W)
            {
                // Rotate around X-axis
                angleX -= angleStep;
                // Clamp camera position within bounds
                camera.Position = new Vector3(
                    camera.Position.X,
                    Clamp(camera.Position.Y, BottommostBound, TopmostBound),
                    camera.Position.Z
                );
            }
            if (e.KeyCode == Keys.S)
            {
                // Rotate around X-axis
                angleX += angleStep;
                // Clamp camera position within bounds
                camera.Position = new Vector3(
                    camera.Position.X,
                    Clamp(camera.Position.Y, BottommostBound, TopmostBound),
                    camera.Position.Z
                );
            }

            if (e.KeyCode == Keys.Oemplus) // `+` key
            {
                zoom = Math.Max(1.0f, zoom - zoomSpeed); // Zoom in (adjusted minimum zoom)
                camera.Position = new Vector3(zoom * (float)Math.Cos(angleY), zoom * (float)Math.Sin(angleX), -zoom); // Adjust camera position based on zoom and angles
            }
            if (e.KeyCode == Keys.OemMinus) // `-` key
            {
                zoom += zoomSpeed; // Zoom out
                camera.Position = new Vector3(zoom * (float)Math.Cos(angleY), zoom * (float)Math.Sin(angleX), -zoom); // Adjust camera position based on zoom and angles
            }

            // Update camera position based on angles
            camera.Position = new Vector3(
                zoom * (float)Math.Cos(angleY) * (float)Math.Cos(angleX),
                Clamp(zoom * (float)Math.Sin(angleX), BottommostBound, TopmostBound), // Apply clamping here
                -zoom * (float)Math.Sin(angleY) * (float)Math.Cos(angleX)
            );

            // Ensure camera always looks at (0,0,0)
            camera.LookAt = Vector3.Zero;

            // Update camera and projection matrices
            UpdateCameraAndProjection();

            // Redraw the scene
            Invalidate();
        }
    }
}
