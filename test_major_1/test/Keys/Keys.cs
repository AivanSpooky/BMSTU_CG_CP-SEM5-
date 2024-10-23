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
        /*private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)
                movingForward = true;
            if (e.KeyCode == Keys.S)
                movingBackward = true;
            if (e.KeyCode == Keys.A)
                movingLeft = true;
            if (e.KeyCode == Keys.D)
                movingRight = true;
            if (e.KeyCode == Keys.Up)
                rotatingUp = true;
            if (e.KeyCode == Keys.Down)
                rotatingDown = true;
            if (e.KeyCode == Keys.Left)
                rotatingLeft = true;
            if (e.KeyCode == Keys.Right)
                rotatingRight = true;
        }
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)
                movingForward = false;
            if (e.KeyCode == Keys.S)
                movingBackward = false;
            if (e.KeyCode == Keys.A)
                movingLeft = false;
            if (e.KeyCode == Keys.D)
                movingRight = false;
            if (e.KeyCode == Keys.Up)
                rotatingUp = false;
            if (e.KeyCode == Keys.Down)
                rotatingDown = false;
            if (e.KeyCode == Keys.Left)
                rotatingLeft = false;
            if (e.KeyCode == Keys.Right)
                rotatingRight = false;
        }*/
    }
}
