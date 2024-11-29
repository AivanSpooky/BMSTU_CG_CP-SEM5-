using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace test.DialogForms
{
    public partial class FieldSettingsForm : Form
    {
        public int GridWidth { get; private set; }
        public int GridDepth { get; private set; }
        public float CellSize { get; private set; }
        public FieldSettingsForm(int currentWidth, int currentDepth, float currentCellSize)
        {
            InitializeComponent();

            textBoxGridWidth.Text = currentWidth.ToString();
            textBoxGridDepth.Text = currentDepth.ToString();
            textBoxCellSize.Text = currentCellSize.ToString("0.##");
        }

        private bool ValidateInputs()
        {
            // Проверка ширины
            if (!int.TryParse(textBoxGridWidth.Text, out int gridWidth) || gridWidth <= 0)
            {
                MessageBox.Show("Ширина поля должна быть целым числом больше 0.", "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Проверка глубины
            if (!int.TryParse(textBoxGridDepth.Text, out int gridDepth) || gridDepth <= 0)
            {
                MessageBox.Show("Глубина поля должна быть целым числом больше 0.", "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Проверка размера клетки
            if (!float.TryParse(textBoxCellSize.Text, out float cellSize) || cellSize <= 0)
            {
                MessageBox.Show("Размер клетки должен быть числом с плавающей точкой больше 0.", "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Установка значений свойств, если все проверки пройдены
            GridWidth = gridWidth;
            GridDepth = gridDepth;
            CellSize = cellSize;

            return true;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (ValidateInputs())
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
