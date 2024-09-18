using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace src
{
    public class AlgoZbuffer
    {
        private Bitmap img;
        private Bitmap imgFromSun;
        private int[][] Zbuf;
        private int[][] ZbufFromSun;
        private LightSource sun;
        private Size size;
        private double tettax, tettay, tettaz;

        private static readonly int zBackground = -10000;

        public AlgoZbuffer(Size size, LightSource sun)
        {
            this.size = size;
            this.sun = sun;

            img = new Bitmap(size.Width, size.Height);
            imgFromSun = new Bitmap(size.Width, size.Height);

            InitBuf(ref Zbuf, size.Width, size.Height, zBackground);
            InitBuf(ref ZbufFromSun, size.Width, size.Height, zBackground);

            InitTeta();

            // Process all shapes in the scene
            foreach (var m in Scene.Shapes)
            {
                ProcessModel(Zbuf, img, m);
                ProcessModelForSun(ZbufFromSun, imgFromSun, m.GetTurnedShape(tettax, tettay, tettaz));
            }
        }

        private void InitTeta()
        {
            tettax = sun.tetax;
            tettay = sun.tetay;
            tettaz = sun.tetaz;
        }

        private void InitBuf(ref int[][] buf, int w, int h, int value)
        {
            buf = new int[h][];
            for (int i = 0; i < h; i++)
            {
                buf[i] = new int[w];
                for (int j = 0; j < w; j++)
                    buf[i][j] = value;
            }
        }

        public void RenderShape(Graphics g, List<Point> vertices, List<Polygon> polygons)
        {
            // Initialize Z-buffer
            float[,] zBuffer = new float[size.Width, size.Height];
            for (int i = 0; i < size.Width; i++)
            {
                for (int j = 0; j < size.Height; j++)
                {
                    zBuffer[i, j] = float.MaxValue;
                }
            }

            // Render polygons using Z-buffer algorithm
            foreach (var polygon in polygons)
            {
                Point[] points = polygon.VertexIndices.Select(i => vertices[i]).ToArray();
                if (points.Length >= 3)
                {
                    // Implement your Z-buffer rendering logic here
                    // For now, we'll just draw the polygon's outline
                    g.DrawPolygon(Pens.Black, points);

                    // Update Z-buffer here (this requires implementing the actual Z-buffer algorithm)
                }
            }

            // Draw the final image
            g.DrawImage(img, 0, 0);
        }

        public Bitmap AddShadows()
        {
            Bitmap hm = new Bitmap(size.Width, size.Height);

            for (int i = 0; i < size.Width; i++)
            {
                for (int j = 0; j < size.Height; j++)
                {
                    int z = GetZ(i, j);
                    if (z != zBackground)
                    {
                        Point3D newCoord = Transformation.Transform(i, j, z, tettax, tettay, tettaz);

                        Color curPixColor = img.GetPixel(i, j);
                        if (newCoord.X < 0 || newCoord.Y < 0 || newCoord.X >= size.Width || newCoord.Y >= size.Height)
                        {
                            hm.SetPixel(i, j, curPixColor);
                            continue;
                        }

                        if (ZbufFromSun[newCoord.Y][newCoord.X] > newCoord.Z + 5)
                        {
                            hm.SetPixel(i, j, Colors.Mix(Color.Black, curPixColor, 0.4f));
                        }
                        else
                        {
                            hm.SetPixel(i, j, curPixColor);
                        }
                    }
                }
            }

            return hm;
        }

        public Bitmap AddShadowsParallel()
        {
            Color[][] res = new Color[size.Width][];
            for (int i = 0; i < size.Width; i++)
                res[i] = new Color[size.Height];

            Parallel.For(0, size.Width, i =>
            {
                Color[] curRow = res[i];

                for (int j = 0; j < size.Height; j++)
                {
                    int z = GetZ(i, j);
                    if (z != zBackground)
                    {
                        Point3D newCoord = Transformation.Transform(i, j, z, tettax, tettay, tettaz);

                        if (newCoord.X < 0 || newCoord.Y < 0 || newCoord.X >= size.Width || newCoord.Y >= size.Height)
                            continue;

                        Color curPixColor = img.GetPixel(i, j);

                        if (ZbufFromSun[newCoord.Y][newCoord.X] > newCoord.Z + 2)
                        {
                            curRow[j] = Colors.Mix(Color.Black, curPixColor, 0.4f);
                        }
                        else
                        {
                            curRow[j] = curPixColor;
                        }
                    }
                }
            });

            return ConnectBitmap(res);
        }

        private Bitmap ConnectBitmap(Color[][] splited)
        {
            Bitmap b = new Bitmap(splited[0].Length, splited.Length);
            for (int i = 0; i < b.Width; i++)
            {
                for (int j = 0; j < b.Height; j++)
                {
                    b.SetPixel(i, j, splited[i][j]);
                }
            }
            return b;
        }

        #region Get Data Externally

        public Bitmap GetImage()
        {
            return img;
        }

        public Bitmap GetSunImage()
        {
            return imgFromSun;
        }

        public int[][] GetZbuf()
        {
            return Zbuf;
        }

        public int GetZ(int x, int y)
        {
            return Zbuf[y][x];
        }

        public int GetZ(Point p)
        {
            return Zbuf[p.Y][p.X];
        }

        #endregion

        private void ProcessModel(int[][] buffer, Bitmap image, Shape3D m)
        {
            Color draw;
            foreach (Polygon polygon in m.Polygons)
            {
                polygon.CalculatePointsInside(m.Vertices, size.Width, size.Height);
                draw = polygon.GetColor(sun);
                foreach (Point3D point in polygon.PointsInside)
                {
                    ProcessPoint(buffer, image, point, draw);
                }
            }
        }

        private void ProcessModelForSun(int[][] buffer, Bitmap image, Shape3D m)
        {
            Color draw;
            foreach (Polygon polygon in m.Polygons)
            {
                if (polygon.Ignore)
                    continue;
                polygon.CalculatePointsInside(m.Vertices, size.Width, size.Height);
                draw = polygon.GetColor(sun);
                foreach (Point3D point in polygon.PointsInside)
                {
                    ProcessPoint(buffer, image, point, draw);
                }
            }
        }

        private void ProcessPoint(int[][] buffer, Bitmap image, Point3D point, Color color)
        {
            if (!(point.X < 0 || point.X >= size.Width || point.Y < 0 || point.Y >= size.Height))
            {
                if (point.Z > buffer[point.Y][point.X])
                {
                    buffer[point.Y][point.X] = point.Z;
                    image.SetPixel(point.X, point.Y, color);
                }
            }
        }
    }
}
