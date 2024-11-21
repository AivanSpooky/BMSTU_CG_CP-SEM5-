using System;
using System.Numerics;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using test.DialogForms;

namespace test
{
    public static class GPO // Grid Plane Options
    {
        public static int gridWidth = 20;     // количество клеток по X
        public static int gridDepth = 20;     // количество клеток по Z
        public static float cellSize = 0.25f; // размер каждой клетки
        public static int SPCPA = 10;         // Spheric Polygon Count Per Axis - количество полигонов на ось для сферических объектов
    }
    public partial class Form1 : Form
    {
        Bitmap bitmap;
        float[,] zBuffer;
        Scene scene;
        Camera camera;
        Light light;
        Timer timer;
        float angle = 0;
        private bool focus = true;
        private bool isMessageBoxShown = false;

        // Переменные для управления камерой
        private bool movingForward = false;
        private bool movingBackward = false;
        private bool movingLeft = false;
        private bool movingRight = false;
        private bool rotatingUp = false;
        private bool rotatingDown = false;
        private bool rotatingLeft = false;
        private bool rotatingRight = false;
        private float cameraSpeed = 1f;
        private float rotationSpeed = 0.15f;
        // === DEBUG TOOLS ===
        private bool debug_mode = true;
        private bool debug_identation = false;
        private bool useGouraudShading = true;

        // === SIMULATION ===
        // Флаг для управления симуляцией
        private bool isSimulationRunning = false;
        // Скорость падения фигур
        private float fallSpeed = 0.15f;

        // Карта занятых клеток
        bool[,] gridOccupied;
        // Список лунок
        List<Indentation> indentations;
        private List<(Vector3 Start, Vector3 End)> indentationEdges = new List<(Vector3 Start, Vector3 End)>();

        public Form1()
        {
            InitializeComponent();

            // Инициализация графики
            bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            zBuffer = new float[pictureBox1.Width, pictureBox1.Height];

            // Initialize the scene
            scene = new Scene();

            // Инициализация карты занятых клеток
            gridOccupied = new bool[GPO.gridWidth, GPO.gridDepth];

            // Инициализация списка лунок
            indentations = new List<Indentation>();

            // Добавление лунок
            AddIndentation(8, 8, (int)(1/GPO.cellSize), (int)(1/GPO.cellSize), IndentationType.Sphere);

            // Add a cubic indentation at grid position (10, 14) with size 4x4 (for example)
            AddIndentation(2, 2, (int)(1/GPO.cellSize), (int)(1/GPO.cellSize), IndentationType.Cube);

            // Создание площадки с лунками
            Mesh ground = Mesh.CreateGridPlane(
                new Vector3(0, 0, 0),
                GPO.gridWidth,
                GPO.gridDepth,
                GPO.cellSize,
                Color.Green,
                indentations, // Pass the list of indentations
                indentationEdges // Pass the list to store edges
            );
            ground.Name = "Ground";
            scene.AddObject(ground);

            #region "meshes"
            // Добавление красного куба new Vector3(1f-0.1f, -0.5f, 1f-0.1f)
            Mesh cube = Mesh.CreateCube(new Vector3(1f-0.1f, 10f, 1f-0.1f), 1, Color.Red);
            cube.Name = "«Красный куб»";
            scene.AddObject(cube);

            Mesh cube1 = Mesh.CreateCube(new Vector3(4f - 0.1f, 6f, 4f - 0.1f), 1, Color.Blue);
            cube1.Name = "«Синий куб»";
            scene.AddObject(cube1);

            Mesh sphere = Mesh.CreateSphere(new Vector3(2.5f, 2f, 2.5f), 0.5f, 10, 10, Color.Yellow);
            sphere.Name = "«Желтая сфера»";
            scene.AddObject(sphere);

            Mesh h = Mesh.CreateHexPrism(new Vector3(4f, 2, 4f), 1f, 1f, Color.Red);
            h.Name = "«Красный hexagon»";
            scene.AddObject(h);

            /*sphere = Mesh.CreateSphere(new Vector3(6, 0.5f, 8.7f), 0.4f, 10, 10, Color.AliceBlue);
            sphere.Name = "Sphere";
            scene.AddObject(sphere);*/

            /*sphere = Mesh.CreateSphere(new Vector3(5, 0.5f, 1f), 2f, 10, 10, Color.Magenta);
            sphere.Name = "Sphere";
            scene.AddObject(sphere);*/
            #endregion
            #region "light"
            light = new Light(new Vector3(1, 1f, 2f));
            if (debug_mode)
            {
                Mesh lightSphere = Mesh.CreateSphere(light.Position, 0.5f, 10, 10, Color.Purple);
                lightSphere.Name = "LightSphere";
                scene.AddObject(lightSphere);
            }
            #endregion
            #region "camera"
            camera = new Camera(
                new Vector3(0, 9f, 15),   // Camera position
                new Vector3(1, 4f, 1),  // Camera target
                new Vector3(0, 1, 0)   // Up vector
            );
            #endregion
            #region "timer"
            // Таймер для анимации
            timer = new Timer();
            timer.Interval = 30;
            timer.Tick += Timer_Tick;
            timer.Start();
            #endregion
            #region "picturebox keyboard set"
            focusForm();
            #endregion
        }
        private void focusForm()
        {
            focus_panel.PreviewKeyDown += FocusPanel_PreviewKeyDown;
            this.KeyPreview = true;
            focus_panel.Focus();
        }
        #region Луночка методы
        private void AddIndentation(int gridX, int gridZ, int width, int depth, IndentationType type)
        {
            // Check if cells are occupied
            for (int x = gridX; x < gridX + width; x++)
            {
                for (int z = gridZ; z < gridZ + depth; z++)
                {
                    if (x < 0 || x >= GPO.gridWidth || z < 0 || z >= GPO.gridDepth || gridOccupied[x, z])
                    {
                        // Cell is occupied or out of bounds
                        Console.WriteLine("Cannot add indentation at ({0}, {1}): cell is occupied or out of bounds", x, z);
                        return;
                    }
                }
            }

            // Mark cells as occupied
            for (int x = gridX; x < gridX + width; x++)
            {
                for (int z = gridZ; z < gridZ + depth; z++)
                {
                    gridOccupied[x, z] = true;
                }
            }

            // Add indentation
            Indentation indentation = new Indentation(gridX, gridZ, width, depth, type);
            indentations.Add(indentation);
        }
        private void AddIndentation(int gridX, int gridZ, int width, int depth, int height, IndentationType type)
        {
            for (int x = gridX; x < gridX + width; x++)
            {
                for (int z = gridZ; z < gridZ + depth; z++)
                {
                    if (x < 0 || x >= GPO.gridWidth || z < 0 || z >= GPO.gridDepth || gridOccupied[x, z])
                    {
                        // Cell is occupied or out of bounds
                        Console.WriteLine("Cannot add indentation at ({0}, {1}): cell is occupied or out of bounds", x, z);
                        return;
                    }
                }
            }

            for (int x = gridX; x < gridX + width; x++)
            {
                for (int z = gridZ; z < gridZ + depth; z++)
                {
                    gridOccupied[x, z] = true;
                }
            }

            Indentation indentation = new Indentation(gridX, gridZ, width, depth, height, type);
            indentations.Add(indentation);
        }
        private void FreeOccupiedCells(Indentation indentation)
        {
            for (int x = indentation.GridX; x < indentation.GridX + indentation.Width; x++)
            {
                for (int z = indentation.GridZ; z < indentation.GridZ + indentation.Depth; z++)
                {
                    if (x >= 0 && x < GPO.gridWidth && z >= 0 && z < GPO.gridDepth)
                    {
                        gridOccupied[x, z] = false;
                    }
                }
            }
        }
        private bool CanAddIndentation(int gridX, int gridZ, int size, int height, IndentationType type)
        {
            int width = size;
            int depth = size;

            for (int x = gridX; x < gridX + width; x++)
                for (int z = gridZ; z < gridZ + depth; z++)
                    if (x < 0 || x >= GPO.gridWidth || z < 0 || z >= GPO.gridDepth || gridOccupied[x, z])
                        return false;

            return true;
        }
        #endregion
        private void Timer_Tick(object sender, EventArgs e)
        {
            /*light.Position.Z += 0.4f;*/
            angle += 0.01f;

            if (isSimulationRunning)
            {
                List<Mesh> meshesToRemove = new List<Mesh>();
                List<Indentation> indentationsToRemove = new List<Indentation>();

                // Используем копию списка Meshes для итерации
                var meshesCopy = scene.Meshes.ToList();

                foreach (var mesh in meshesCopy)
                {
                    if (mesh.Type != FigureType.Default)
                    {
                        // Обновляем позицию фигуры
                        mesh.Position -= new Vector3(0, fallSpeed, 0);

                        // Проверяем столкновение с площадкой
                        if (mesh.Position.Y <= 0)
                        {
                            // Проверяем попадание в лунку
                            if (CheckFigureCollision(mesh, meshesToRemove, indentationsToRemove))
                            {
                                // Если фигура обработана, удаляем её из списка на дальнейшую обработку
                                meshesToRemove.Add(mesh);
                            }
                        }
                    }
                }

                // Удаляем фигуры из сцены
                foreach (var mesh in meshesToRemove)
                {
                    scene.RemoveObjectByName(mesh.Name);
                }

                // Удаляем лунки и обновляем площадку
                if (indentationsToRemove.Any())
                {
                    foreach (var indentation in indentationsToRemove)
                    {
                        indentations.Remove(indentation);
                        // Free up the occupied cells
                        FreeOccupiedCells(indentation);
                    }

                    // Rebuild the ground mesh
                    scene.RemoveObjectByName("Ground");
                    Mesh ground = Mesh.CreateGridPlane(
                        new Vector3(0, 0, 0),
                        GPO.gridWidth,
                        GPO.gridDepth,
                        GPO.cellSize,
                        Color.Green,
                        indentations,
                        indentationEdges
                    );
                    ground.Name = "Ground";
                    scene.AddObject(ground);
                }

                // Останавливаем симуляцию, если все фигуры удалены
                if (!scene.Meshes.Any(m => m.Type != FigureType.Default))
                {
                    isSimulationRunning = false;
                    MessageBox.Show("Симуляция завершена.");
                    focusForm();
                }
            }

            // Обновление положения камеры
            UpdateCamera();

            Render();
        }

        private void UpdateCamera()
        {
            Vector3 direction = Vector3.Normalize(camera.Target - camera.Position);
            Vector3 right = Vector3.Normalize(Vector3.Cross(direction, camera.Up));

            // Movement
            if (movingForward)
            {
                camera.Position += direction * cameraSpeed;
                camera.Target += direction * cameraSpeed;
                movingForward = false;
            }
            if (movingBackward)
            {
                camera.Position -= direction * cameraSpeed;
                camera.Target -= direction * cameraSpeed;
                movingBackward = false;
            }
            if (movingLeft)
            {
                camera.Position -= right * cameraSpeed;
                camera.Target -= right * cameraSpeed;
                movingLeft = false;
            }
            if (movingRight)
            {
                camera.Position += right * cameraSpeed;
                camera.Target += right * cameraSpeed;
                movingRight = false;
            }

            // Rotation around the Y axis (yaw)
            if (rotatingLeft)
            {
                Matrix4x4 rotationMatrix = Matrix4x4.CreateFromAxisAngle(camera.Up, rotationSpeed);
                direction = Vector3.TransformNormal(direction, rotationMatrix);
                camera.Target = camera.Position + direction;
                rotatingLeft = false;
            }
            if (rotatingRight)
            {
                Matrix4x4 rotationMatrix = Matrix4x4.CreateFromAxisAngle(camera.Up, -rotationSpeed);
                direction = Vector3.TransformNormal(direction, rotationMatrix);
                camera.Target = camera.Position + direction;
                rotatingRight = false;
            }

            // Rotation around the right vector (pitch)
            if (rotatingUp)
            {
                Matrix4x4 rotationMatrix = Matrix4x4.CreateFromAxisAngle(right, rotationSpeed);
                direction = Vector3.TransformNormal(direction, rotationMatrix);
                camera.Up = Vector3.Normalize(Vector3.TransformNormal(camera.Up, rotationMatrix));
                camera.Target = camera.Position + direction;
                rotatingUp = false;
            }
            if (rotatingDown)
            {
                Matrix4x4 rotationMatrix = Matrix4x4.CreateFromAxisAngle(right, -rotationSpeed);
                direction = Vector3.TransformNormal(direction, rotationMatrix);
                camera.Up = Vector3.Normalize(Vector3.TransformNormal(camera.Up, rotationMatrix));
                camera.Target = camera.Position + direction;
                rotatingDown = false;
            }
        }

        private void Render()
        {
            // Clear buffers
            Graphics g = Graphics.FromImage(bitmap);
            g.Clear(Color.Black);
            for (int i = 0; i < zBuffer.GetLength(0); i++)
                for (int j = 0; j < zBuffer.GetLength(1); j++)
                    zBuffer[i, j] = float.MaxValue;

            // Transformation matrices
            Matrix4x4 viewMatrix = Matrix4x4.CreateLookAt(camera.Position, camera.Target, camera.Up);
            float aspectRatio = (float)bitmap.Width / bitmap.Height;
            Matrix4x4 projectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(
                (float)Math.PI / 2, aspectRatio, 0.1f, 100f
            );

            Matrix4x4 viewportMatrix = new Matrix4x4(
                bitmap.Width / 2f, 0, 0, bitmap.Width / 2f,
                0, -bitmap.Height / 2f, 0, bitmap.Height / 2f,
                0, 0, 1, 0,
                0, 0, 0, 1
            );

            if (debug_mode)
                foreach (var mesh in scene.Meshes)
                {
                    Matrix4x4 worldMatrix = Matrix4x4.CreateFromQuaternion(mesh.Rotation) *
                                            Matrix4x4.CreateTranslation(mesh.Position);

                    foreach (var face in mesh.Faces)
                    {
                        Vertex v1 = mesh.Vertices[face.A];
                        Vertex v2 = mesh.Vertices[face.B];
                        Vertex v3 = mesh.Vertices[face.C];

                        // Transform vertices to world coordinates
                        Vector3 worldV1 = Vector3.Transform(v1.Position, worldMatrix);
                        Vector3 worldV2 = Vector3.Transform(v2.Position, worldMatrix);
                        Vector3 worldV3 = Vector3.Transform(v3.Position, worldMatrix);

                        // Project vertices
                        Vector4 projV1 = Vector4.Transform(new Vector4(worldV1, 1), viewMatrix * projectionMatrix);
                        Vector4 projV2 = Vector4.Transform(new Vector4(worldV2, 1), viewMatrix * projectionMatrix);
                        Vector4 projV3 = Vector4.Transform(new Vector4(worldV3, 1), viewMatrix * projectionMatrix);

                        if (projV1.W <= 0 || projV2.W <= 0 || projV3.W <= 0)
                            continue;

                        projV1 /= projV1.W;
                        projV2 /= projV2.W;
                        projV3 /= projV3.W;

                        // Convert to screen coordinates
                        Vector3 screenV1 = Vector3.Transform(projV1.XYZ(), viewportMatrix);
                        Vector3 screenV2 = Vector3.Transform(projV2.XYZ(), viewportMatrix);
                        Vector3 screenV3 = Vector3.Transform(projV3.XYZ(), viewportMatrix);

                        // Draw lines (pseudo-code, replace with actual drawing logic)
                        g.DrawLine(Pens.White, screenV1.X, screenV1.Y, screenV2.X, screenV2.Y);
                        g.DrawLine(Pens.White, screenV2.X, screenV2.Y, screenV3.X, screenV3.Y);
                        g.DrawLine(Pens.White, screenV3.X, screenV3.Y, screenV1.X, screenV1.Y);
                    }
                }
            else
                foreach (var mesh in scene.Meshes)
                {
                    // Use the mesh's color
                    Color objectColor = mesh.Color;

                    // Special color for the light sphere if needed
                    if (mesh.Name == "LightSphere")
                        objectColor = mesh.Color;

                    Matrix4x4 worldMatrix = Matrix4x4.CreateFromQuaternion(mesh.Rotation) *
                                            Matrix4x4.CreateTranslation(mesh.Position);

                    foreach (var face in mesh.Faces)
                    {
                        // Получаем вершины треугольника
                        Vertex v1 = mesh.Vertices[face.A];
                        Vertex v2 = mesh.Vertices[face.B];
                        Vertex v3 = mesh.Vertices[face.C];

                        // Преобразуем вершины в мировые координаты
                        Vector3 worldV1 = Vector3.Transform(v1.Position, worldMatrix);
                        Vector3 worldV2 = Vector3.Transform(v2.Position, worldMatrix);
                        Vector3 worldV3 = Vector3.Transform(v3.Position, worldMatrix);

                        // Вычисляем нормаль треугольника
                        Vector3 edge1 = worldV2 - worldV1;
                        Vector3 edge2 = worldV3 - worldV1;
                        Vector3 faceNormal = Vector3.Normalize(Vector3.Cross(edge1, edge2));

                        // Вычисляем освещённость для треугольника или вершин
                        float intensity = 0;
                        float i1 = 0, i2 = 0, i3 = 0;

                        if (useGouraudShading)
                        {
                            // Освещённость для каждой вершины (Gouraud Shading)
                            i1 = ComputeLighting(worldV1, Vector3.Normalize(Vector3.TransformNormal(v1.Normal, worldMatrix)));
                            i2 = ComputeLighting(worldV2, Vector3.Normalize(Vector3.TransformNormal(v2.Normal, worldMatrix)));
                            i3 = ComputeLighting(worldV3, Vector3.Normalize(Vector3.TransformNormal(v3.Normal, worldMatrix)));
                        }
                        else
                        {
                            // Плоское освещение (Flat Shading)
                            intensity = ComputeLighting(worldV1, faceNormal);
                        }

                        // Проецируем вершины
                        Vector4 projV1 = Vector4.Transform(new Vector4(worldV1, 1), viewMatrix * projectionMatrix);
                        Vector4 projV2 = Vector4.Transform(new Vector4(worldV2, 1), viewMatrix * projectionMatrix);
                        Vector4 projV3 = Vector4.Transform(new Vector4(worldV3, 1), viewMatrix * projectionMatrix);

                        if (projV1.W <= 0 || projV2.W <= 0 || projV3.W <= 0)
                            continue;

                        projV1 /= projV1.W;
                        projV2 /= projV2.W;
                        projV3 /= projV3.W;

                        // Преобразуем в экранные координаты
                        Vector3 screenV1 = Vector3.Transform(projV1.XYZ(), viewportMatrix);
                        Vector3 screenV2 = Vector3.Transform(projV2.XYZ(), viewportMatrix);
                        Vector3 screenV3 = Vector3.Transform(projV3.XYZ(), viewportMatrix);

                        if (useGouraudShading)
                        {
                            // Закрашиваем треугольник с использованием Гуро освещения
                            DrawTriangle(screenV1, screenV2, screenV3, i1, i2, i3, objectColor);
                        }
                    }
                }

            // Draw indentation wireframes if debug flag is true
            if (debug_identation)
            {
                DrawIndentationWireframes(g, viewportMatrix, viewMatrix, projectionMatrix);
            }

            // Update PictureBox
            pictureBox1.Image = bitmap;
        }

        private void DrawIndentationWireframes(Graphics g, Matrix4x4 viewportMatrix, Matrix4x4 viewMatrix, Matrix4x4 projectionMatrix)
        {
            using (Pen wireframePen = new Pen(Color.Black, 1))
            {
                foreach (var edge in indentationEdges)
                {
                    // Transform start and end points to screen coordinates
                    Vector4 projStart = Vector4.Transform(new Vector4(edge.Start, 1), viewMatrix * projectionMatrix);
                    Vector4 projEnd = Vector4.Transform(new Vector4(edge.End, 1), viewMatrix * projectionMatrix);

                    if (projStart.W <= 0 || projEnd.W <= 0)
                        continue;

                    projStart /= projStart.W;
                    projEnd /= projEnd.W;

                    // Convert to screen coordinates
                    Vector3 screenStart = Vector3.Transform(projStart.XYZ(), viewportMatrix);
                    Vector3 screenEnd = Vector3.Transform(projEnd.XYZ(), viewportMatrix);

                    // Draw the line in black
                    g.DrawLine(wireframePen, screenStart.X, screenStart.Y, screenEnd.X, screenEnd.Y);
                }
            }
        }

        private float ComputeLighting(Vector3 position, Vector3 normal)
        {
            float intensity = 0;

            // Фоновое освещение
            float ambient = 0.2f;
            intensity += ambient;

            // Направление от точки к источнику света
            Vector3 lightDir = Vector3.Normalize(light.Position - position);

            // Косинус угла между нормалью и направлением света
            float nDotL = Vector3.Dot(normal, lightDir);

            if (nDotL > 0)
            {
                if (IsInShadow(position, light.Position))
                {
                    // Точка в тени, оставляем только фоновое освещение
                    intensity = ambient;
                }
                else
                {
                    // Добавляем диффузное освещение
                    intensity += nDotL;
                }
            }

            return Clamp(intensity, 0, 1);
        }

        private bool IsInShadow(Vector3 point, Vector3 lightPos)
        {
            Vector3 dir = Vector3.Normalize(lightPos - point);
            float distanceToLight = Vector3.Distance(lightPos, point);

            // Если расстояние очень маленькое или нулевое, то считаем, что точка не в тени
            if (distanceToLight < 0.001f)
                return false;

            // Apply a small bias to prevent self-shadowing
            float bias = 0.001f;
            Vector3 shadowOrigin = point + dir * bias;

            foreach (var mesh in scene.Meshes)
            {
                if (mesh.Name == "LightSphere")
                    continue;

                Matrix4x4 worldMatrix = Matrix4x4.CreateFromQuaternion(mesh.Rotation) * Matrix4x4.CreateTranslation(mesh.Position);

                foreach (var face in mesh.Faces)
                {
                    Vertex v1 = mesh.Vertices[face.A];
                    Vertex v2 = mesh.Vertices[face.B];
                    Vertex v3 = mesh.Vertices[face.C];

                    Vector3 worldV1 = Vector3.Transform(v1.Position, worldMatrix);
                    Vector3 worldV2 = Vector3.Transform(v2.Position, worldMatrix);
                    Vector3 worldV3 = Vector3.Transform(v3.Position, worldMatrix);

                    if (IntersectTriangle(shadowOrigin, dir, worldV1, worldV2, worldV3, out float t))
                    {
                        if (t > 0 && t < distanceToLight)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private bool IntersectTriangle(Vector3 orig, Vector3 dir, Vector3 v0, Vector3 v1, Vector3 v2, out float t)
        {
            t = 0;
            const float EPSILON = 0.0000001f;
            Vector3 edge1 = v1 - v0;
            Vector3 edge2 = v2 - v0;
            Vector3 h = Vector3.Cross(dir, edge2);
            float a = Vector3.Dot(edge1, h);
            if (a > -EPSILON && a < EPSILON)
                return false;    // Луч параллелен треугольнику
            float f = 1.0f / a;
            Vector3 s = orig - v0;
            float u = f * Vector3.Dot(s, h);
            if (u < 0.0 || u > 1.0)
                return false;
            Vector3 q = Vector3.Cross(s, edge1);
            float v = f * Vector3.Dot(dir, q);
            if (v < 0.0 || u + v > 1.0)
                return false;
            t = f * Vector3.Dot(edge2, q);
            if (t > EPSILON)
                return true;
            else
                return false;
        }

        // ГУРО
        private void DrawTriangle(Vector3 v1, Vector3 v2, Vector3 v3, float i1, float i2, float i3, Color objectColor)
        {
            // Сортировка вершин по Y
            if (v1.Y > v2.Y)
            {
                Swap(ref v1, ref v2);
                Swap(ref i1, ref i2);
            }
            if (v2.Y > v3.Y)
            {
                Swap(ref v2, ref v3);
                Swap(ref i2, ref i3);
            }
            if (v1.Y > v2.Y)
            {
                Swap(ref v1, ref v2);
                Swap(ref i1, ref i2);
            }

            int yStart = (int)Math.Max(0, Math.Ceiling(v1.Y));
            int yEnd = (int)Math.Min(bitmap.Height - 1, Math.Floor(v3.Y));

            for (int y = yStart; y <= yEnd; y++)
            {
                bool secondHalf = y > v2.Y || v2.Y == v1.Y;
                float segmentHeight = secondHalf ? v3.Y - v2.Y : v2.Y - v1.Y;
                if (segmentHeight == 0) segmentHeight = 1;
                float alpha = (y - v1.Y) / (v3.Y - v1.Y);
                float beta = (y - (secondHalf ? v2.Y : v1.Y)) / segmentHeight;

                Vector3 A = v1 + (v3 - v1) * alpha;
                Vector3 B = secondHalf ? v2 + (v3 - v2) * beta : v1 + (v2 - v1) * beta;

                float iA = i1 + (i3 - i1) * alpha;
                float iB = secondHalf ? i2 + (i3 - i2) * beta : i1 + (i2 - i1) * beta;

                if (A.X > B.X)
                {
                    Swap(ref A, ref B);
                    Swap(ref iA, ref iB);
                }

                int xStart = (int)Math.Max(0, Math.Ceiling(A.X));
                int xEnd = (int)Math.Min(bitmap.Width - 1, Math.Floor(B.X));

                for (int x = xStart; x <= xEnd; x++)
                {
                    float phi = (B.X == A.X) ? 1.0f : (x - A.X) / (B.X - A.X);
                    Vector3 P = A + (B - A) * phi;
                    float iP = iA + (iB - iA) * phi;

                    int zIndex = x;
                    int yIndex = y;
                    if (zIndex < 0 || zIndex >= bitmap.Width || yIndex < 0 || yIndex >= bitmap.Height)
                        continue;

                    if (P.Z < zBuffer[zIndex, yIndex])
                    {
                        zBuffer[zIndex, yIndex] = P.Z;

                        // Применяем интенсивность к цвету объекта
                        int r = (int)(objectColor.R * Clamp(iP, 0, 1));
                        int g = (int)(objectColor.G * Clamp(iP, 0, 1));
                        int b = (int)(objectColor.B * Clamp(iP, 0, 1));

                        bitmap.SetPixel(zIndex, yIndex, Color.FromArgb(r, g, b));
                    }
                }
            }
        }

        #region Кнопки
        private void btn_simulate_Click(object sender, EventArgs e)
        {
            isSimulationRunning = true;
            focus_panel.Focus();
        }
        private void btn_add_fig_Click(object sender, EventArgs e)
        {
            using (AddObjectForm addObjectForm = new AddObjectForm())
            {
                if (addObjectForm.ShowDialog() == DialogResult.OK)
                {
                    FigureType type = addObjectForm.SelectedFigureType;
                    Color color = addObjectForm.SelectedColor;
                    Vector3 position = addObjectForm.Position;
                    float size = addObjectForm.ObjSize;
                    int heightInCells = addObjectForm.HeightInCells;
                    float height = heightInCells * GPO.cellSize;
                    string name = addObjectForm.ObjectName;

                    Mesh mesh = null;
                    switch (type)
                    {
                        case FigureType.Cube:
                            mesh = Mesh.CreateCube(position, size, color);
                            break;
                        case FigureType.Sphere:
                            mesh = Mesh.CreateSphere(position, size / 2, GPO.SPCPA, GPO.SPCPA, color);
                            break;
                        case FigureType.HexPrism:
                            mesh = Mesh.CreateHexPrism(position, size / 2, height, color);
                            mesh.HeightInCells = heightInCells;
                            break;
                        case FigureType.Cylinder:
                            mesh = Mesh.CreateCylinder(position, size / 2, height, GPO.SPCPA, color);
                            mesh.HeightInCells = heightInCells;
                            break;
                    }

                    if (mesh != null)
                    {
                        mesh.Name = name;
                        scene.AddObject(mesh);
                    }
                }
            }
        }
        private void btn_add_ident_Click(object sender, EventArgs e)
        {
            using (AddIdentationForm addIndentationForm = new AddIdentationForm())
            {
                addIndentationForm.ValidateIndentation = CanAddIndentation;
                if (addIndentationForm.ShowDialog() == DialogResult.OK)
                {
                    IndentationType type = addIndentationForm.SelectedIndentationType;
                    int gridX = addIndentationForm.GridX;
                    int gridZ = addIndentationForm.GridZ;
                    int size = addIndentationForm.IdSize;
                    int height = addIndentationForm.IdHeight;

                    if (type == IndentationType.Cylinder || type == IndentationType.HexPrism)
                        AddIndentation(gridX, gridZ, size, size, height, type);
                    else
                        AddIndentation(gridX, gridZ, size, size, type);

                    // Rebuild the ground mesh
                    scene.RemoveObjectByName("Ground");
                    Mesh ground = Mesh.CreateGridPlane(
                        new Vector3(0, 0, 0),
                        GPO.gridWidth,
                        GPO.gridDepth,
                        GPO.cellSize,
                        Color.Green,
                        indentations,
                        indentationEdges
                    );
                    ground.Name = "Ground";
                    scene.AddObject(ground);
                }
            }
        }
        #endregion
    }
}
