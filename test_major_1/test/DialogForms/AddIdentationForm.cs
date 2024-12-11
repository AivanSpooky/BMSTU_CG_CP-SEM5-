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
    public partial class AddIdentationForm : Form
    {
        public Func<int, int, int, int, IndentationType, bool> ValidateIndentation { get; set; }
        public IndentationType SelectedIndentationType { get; private set; }
        public int GridX { get; private set; }
        public int Y { get; private set; }
        public int GridZ { get; private set; }
        public int IdSize { get; private set; }
        public int IdHeight { get; private set; }

        public AddIdentationForm()
        {
            InitializeComponent();

            comboBoxIndentationType.DataSource = Enum.GetValues(typeof(IndentationType))
                .Cast<IndentationType>()
                .Select(it => new { Value = it, Text = GetIndentationTypeName(it) })
                .ToList();
            comboBoxIndentationType.DisplayMember = "Text";
            comboBoxIndentationType.ValueMember = "Value";

            numericUpDownSize.Value = 1;
        }

        private string GetIndentationTypeName(IndentationType indentationType)
        {
            switch (indentationType)
            {
                case IndentationType.Cube:
                    return "Кубическая";
                case IndentationType.Sphere:
                    return "Сферическая";
                case IndentationType.HexPrism:
                    return "Для шестигранной призмы";
                case IndentationType.Cylinder:
                    return "Цилиндрическая";
                default:
                    return "Unknown";
            }
        }

        private void btnOk_Click_1(object sender, EventArgs e)
        {
            SelectedIndentationType = (IndentationType)((dynamic)comboBoxIndentationType.SelectedItem).Value;
            GridX = (int)numericUpDownX.Value;
            GridZ = (int)numericUpDownZ.Value;
            IdSize = (int)numericUpDownSize.Value;
            if (SelectedIndentationType == IndentationType.HexPrism || SelectedIndentationType == IndentationType.Cylinder)
                IdHeight = (int)numericUpDownHeight.Value;

            // ПРОВЕРКА НА ВОЗМОЖНОСТЬ ДОБАВЛЕНИЯ ЛУНКИ
            if (ValidateIndentation != null)
            {
                bool canAdd = ValidateIndentation(GridX, GridZ, IdSize, IdHeight, SelectedIndentationType);
                if (!canAdd)
                {
                    MessageBox.Show("Cannot add indentation at the specified location.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            /*if (ValidateIndentation != null)
            {
                bool canAdd = ValidateIndentation(GridX, GridZ, IdSize, SelectedIndentationType);
                if (!canAdd)
                {
                    MessageBox.Show("Невозможно добавить лунку в текущее место! Пожалуйста, перепроверьте размер и координаты!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }*/

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void comboBoxIndentationType_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedType = (IndentationType)((dynamic)comboBoxIndentationType.SelectedItem).Value;
            bool requiresHeight = selectedType == IndentationType.Cylinder || selectedType == IndentationType.HexPrism;
            numericUpDownHeight.Visible = requiresHeight;
            groupBox2.Visible = requiresHeight;
        }

        private void AddIdentationForm_Load(object sender, EventArgs e)
        {
            comboBoxIndentationType_SelectedIndexChanged(null, null);
        }
    }
}
