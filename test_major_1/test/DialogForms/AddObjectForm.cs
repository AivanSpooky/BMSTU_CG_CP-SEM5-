using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace test.DialogForms
{
    public partial class AddObjectForm : Form
    {
        public FigureType SelectedFigureType { get; private set; }
        public Color SelectedColor { get; private set; }
        public Vector3 Position { get; private set; }
        public float ObjSize { get; private set; }
        public string ObjectName { get; private set; }
        public int HeightInCells { get; private set; }

        public AddObjectForm()
        {
            InitializeComponent();

            // Populate object types
            comboBoxObjectType.DataSource = Enum.GetValues(typeof(FigureType))
                .Cast<FigureType>()
                .Where(ft => ft != FigureType.Default)
                .Select(ft => new { Value = ft, Text = GetFigureTypeName(ft) })
                .ToList();
            comboBoxObjectType.DisplayMember = "Text";
            comboBoxObjectType.ValueMember = "Value";

            // Populate colors
            var colors = new List<KeyValuePair<string, Color>>()
            {
                new KeyValuePair<string, Color>("Красный", Color.Red),
                new KeyValuePair<string, Color>("Зеленый", Color.Green),
                new KeyValuePair<string, Color>("Синий", Color.Blue),
                new KeyValuePair<string, Color>("Желтый", Color.Yellow),
                new KeyValuePair<string, Color>("Фиолетовый", Color.Magenta),
                new KeyValuePair<string, Color>("Бирюзовый", Color.Cyan)
            };
            comboBoxColor.DataSource = colors;
            comboBoxColor.DisplayMember = "Key";
            comboBoxColor.ValueMember = "Value";

            // Set default values
            numericUpDownSize.Value = 1;
        }

        private string GetFigureTypeName(FigureType figureType)
        {
            switch (figureType)
            {
                case FigureType.Cube:
                    return "Куб";
                case FigureType.Sphere:
                    return "Сфера";
                case FigureType.HexPrism:
                    return "Шестиугольная призма";
                case FigureType.Cylinder:
                    return "Цилиндр";
                default:
                    return "Unknown";
            }
        }

        private void btnOk_Click_1(object sender, EventArgs e)
        {
            SelectedFigureType = (FigureType)((dynamic)comboBoxObjectType.SelectedItem).Value;
            SelectedColor = ((KeyValuePair<string, Color>)comboBoxColor.SelectedItem).Value;
            float x = (float)numericUpDownX.Value * GPO.cellSize;
            float y = (float)numericUpDownY.Value * GPO.cellSize;
            float z = (float)numericUpDownZ.Value * GPO.cellSize;
            Position = new Vector3(x, y, z);
            ObjSize = (float)numericUpDownSize.Value * GPO.cellSize;
            HeightInCells = (int)numericUpDownHeight.Value;
            ObjectName = textBoxName.Text;

            if (ObjSize <= 0)
            {
                MessageBox.Show("Размер должен быть больше 0!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (SelectedFigureType == FigureType.Cylinder || SelectedFigureType == FigureType.HexPrism)
                if (HeightInCells <= 0)
                {
                    MessageBox.Show("Высота должна быть больше 0!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void comboBoxObjectType_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedType = (FigureType)((dynamic)comboBoxObjectType.SelectedItem).Value;
            bool requiresHeight = selectedType == FigureType.Cylinder || selectedType == FigureType.HexPrism;
            numericUpDownHeight.Visible = requiresHeight;
            groupBox10.Visible = requiresHeight;
        }

        private void AddObjectForm_Load(object sender, EventArgs e)
        {
            comboBoxObjectType_SelectedIndexChanged(null, null);
        }
    }
}
