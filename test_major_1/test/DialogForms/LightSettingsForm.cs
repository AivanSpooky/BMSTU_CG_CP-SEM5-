using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace test.DialogForms
{
    public partial class LightSettingsForm : Form
    {
        public Vector3 Pos { get; private set; }
        public float Intensity { get; private set; }
        public LightSettingsForm()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (!float.TryParse(tbX.Text.Replace(',', '.'), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float x))
            {
                MessageBox.Show("Некорректное значение для позиции X.", "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbX.Focus();
                return;
            }
            if (!float.TryParse(tbY.Text.Replace(',', '.'), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float y))
            {
                MessageBox.Show("Некорректное значение для позиции Y.", "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbY.Focus();
                return;
            }
            if (!float.TryParse(tbZ.Text.Replace(',', '.'), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float z))
            {
                MessageBox.Show("Некорректное значение для позиции Z.", "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbZ.Focus();
                return;
            }
            if (!float.TryParse(tbI.Text.Replace(',', '.'), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float intensity))
            {
                MessageBox.Show("Некорректное значение для интенсивности.", "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbI.Focus();
                return;
            }
            if (intensity < 0.0f || intensity > 1.0f)
            {
                MessageBox.Show("Интенсивность должна быть в диапазоне от 0 до 1.0.", "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbI.Focus();
                return;
            }

            Pos = new Vector3(x, y, z);
            Intensity = intensity;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
