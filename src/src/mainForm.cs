using System;
using System.Drawing;
using System.Numerics;
using System.Windows.Forms;

namespace src
{
    public partial class mainForm : Form
    {
        private Renderer renderer;  // Создаем объект рендерера
        private LightSource sun1, sun2, sun3, sun4, sun5, currentSun;

        private void button1_Click(object sender, EventArgs e)
        {
            Scene.RotateShapesInXZPlane(10 * (float)Math.PI/180);
            main_pb.Focus();
            renderer.UpdateCameraAndProjection();
            main_pb.Invalidate();
        }

        private AlgoZbuffer zbuf;
        private double tetax, tetay, tetaz;

        public mainForm()
        {
            InitializeComponent();
            this.KeyPreview = true;  // Разрешаем обработку событий клавиатуры для всей формы
            this.KeyDown += new KeyEventHandler(OnKeyDownHandler);

            Scene.AddShape(new Cube3D(8.0f, new Vector3(0, 0, 0), Color.Red));
            Scene.AddShape(new Cube3D(4.0f, new Vector3(5, 0, 0), Color.Brown));
            Scene.AddShape(new Sphere3D(16, 16, new Vector3(0, 0, 10), Color.Blue));
            Scene.AddShape(new Sphere3D(16, 16, new Vector3(0, 0, -10), Color.Green));
            Scene.AddShape(new RectangularPrism3D(20, 2, 20, new Vector3(0, 4, 0), Color.Gray));
            renderer = new Renderer(this, main_pb);  // Инициализируем рендерер
            SetSun(); // Устанавливаем освещение
            HandleSceneChange(); // Обрабатываем изменение сцены
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            // Проверяем, если фокус на PictureBox или любой контрол
            if (main_pb.Focused)
            {
                renderer.OnKeyDown(sender, e);
            }
        }

        private void SetSun()
        {
            sun1 = new LightSource(Color.White, -90, new Vector3(1, 0, 0));
            sun2 = new LightSource(Color.White, -110, new Vector3(0.4f, -0.5f, 0));
            sun3 = new LightSource(Color.White, 180, new Vector3(0, -1, 0));
            sun4 = new LightSource(Color.White, 110, new Vector3(-0.4f, -0.5f, 0));
            sun5 = new LightSource(Color.White, 90, new Vector3(-1, 0, 0));
            currentSun = sun3;
        }

        private void HandleSceneChange()
        {
            // Применяем трансформацию и обновляем буфер
            Scene.TransformShapes(tetax, tetay, tetaz);
            zbuf = new AlgoZbuffer(main_pb.Size, currentSun);
            renderer.Render(zbuf);
        }
    }
}
