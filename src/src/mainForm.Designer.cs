using System.Windows.Forms;

namespace src
{
    partial class mainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.renderer = new src.Renderer();
            this.SuspendLayout();
            // 
            // renderer
            // 
            this.renderer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.renderer.Location = new System.Drawing.Point(0, 0);
            this.renderer.Name = "renderer";
            this.renderer.Size = new System.Drawing.Size(1584, 961);
            this.renderer.TabIndex = 0;
            // 
            // mainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1584, 961);
            this.Controls.Add(this.renderer);
            this.Name = "mainForm";
            this.Text = "Курсовая работа";
            this.ResumeLayout(false);

        }

        #endregion
    }
}
