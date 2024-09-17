using System;
using System.Windows.Forms;

namespace src
{
    public partial class mainForm : Form
    {
        private Renderer renderer;  // Создаем объект рендерера

        public mainForm()
        {
            InitializeComponent();
            renderer = new Renderer();  // Инициализируем рендерер

            // Добавляем рендерер как контрол в форму
            renderer.Dock = DockStyle.Fill;  // Устанавливаем, чтобы рендерер занимал все пространство формы
            Controls.Add(renderer);     // Добавляем рендерер в контролы формы
        }
    }
}
