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
            new KeyValuePair<string, Color>("Red", Color.Red),
            new KeyValuePair<string, Color>("Green", Color.Green),
            new KeyValuePair<string, Color>("Blue", Color.Blue),
            new KeyValuePair<string, Color>("Yellow", Color.Yellow),
            new KeyValuePair<string, Color>("Magenta", Color.Magenta),
            new KeyValuePair<string, Color>("Cyan", Color.Cyan)
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
                    return "Cube";
                case FigureType.Sphere:
                    return "Sphere";
                case FigureType.HexPrism:
                    return "Hexagonal Prism";
                case FigureType.Tetrahedron:
                    return "Tetrahedron";
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
            ObjectName = textBoxName.Text;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
