using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace test
{
    public partial class Form1 : Form
    {
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
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
