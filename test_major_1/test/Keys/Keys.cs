using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace test
{
    public static class GPO                   // Grid Plane Options
    {
        public static int gridWidth = 20;     // количество клеток по X
        public static int gridDepth = 20;     // количество клеток по Z
        public static float cellSize = 0.25f; // размер каждой клетки
        public static int SPCPA = 20;         // Spheric Polygon Count Per Axis - количество полигонов на ось для сферических объектов
    }
    public partial class Form1 : Form
    {
        private bool KP = true;
        private void FocusPanel_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down ||
                e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
            {
                e.IsInputKey = true;
            }
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            KP = true;
            switch (keyData)
            {
                case Keys.W:
                    movingForward = true;
                    break;
                case Keys.S:
                    movingBackward = true;
                    break;
                case Keys.A:
                    movingLeft = true;
                    break;
                case Keys.D:
                    movingRight = true;
                    break;
                case Keys.Up:
                    rotatingUp = true;
                    break;
                case Keys.Down:
                    rotatingDown = true;
                    break;
                case Keys.Left:
                    rotatingLeft = true;
                    break;
                case Keys.Right:
                    rotatingRight = true;
                    break;
                default:
                    KP = false;
                    break;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
