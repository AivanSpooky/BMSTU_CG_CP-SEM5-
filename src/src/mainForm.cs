using System;
using System.Numerics;
using System.Windows.Forms;

namespace src
{
    public partial class mainForm : Form
    {
        private Renderer renderer;  // Создаем объект рендерера

        public mainForm()
        {
            InitializeComponent();
                
            Scene.AddShape(new Cube3D(2.0f, new Vector3(0, 0, 0)));
            Scene.AddShape(new Sphere3D(16, 16, new Vector3(0, 0, 10)));
            Scene.AddShape(new RectangularPrism3D(20, 2, 20, new Vector3(0, -1, 0)));
            Renderer renderer = new Renderer(this, main_pb);  // Инициализируем рендерер
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void renderer_Click(object sender, EventArgs e)
        {

        }
    }
}
