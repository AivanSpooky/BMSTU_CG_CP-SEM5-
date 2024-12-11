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
    public partial class SimulationSettingsForm : Form
    {
        public float FallSpeed { get; private set; }
        public SimulationSettingsForm()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            string input = tbFS.Text.Replace(',', '.').Trim();
            int decimalIndex = input.IndexOf('.');
            if (decimalIndex >= 0)
            {
                int decimalDigits = input.Length - decimalIndex - 1;
                if (decimalDigits > 3)
                {
                    MessageBox.Show("Скорость падения должна иметь не более 3 знаков после запятой.", "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tbFS.Focus();
                    return;
                }
            }
            if (!float.TryParse(input, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float fallsp))
            {
                MessageBox.Show("Некорректное значение для скорости падения.", "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbFS.Focus();
                return;
            }
            if (fallsp < 0.0f)
            {
                MessageBox.Show("Скорость падения должна быть больше или равна 0.", "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbFS.Focus();
                return;
            }
            FallSpeed = fallsp;

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
