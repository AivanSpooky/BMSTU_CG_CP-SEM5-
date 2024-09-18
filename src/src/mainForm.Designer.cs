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
            this.main_pb = new System.Windows.Forms.PictureBox();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.main_pb)).BeginInit();
            this.SuspendLayout();
            // 
            // main_pb
            // 
            this.main_pb.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.main_pb.Location = new System.Drawing.Point(12, 12);
            this.main_pb.Name = "main_pb";
            this.main_pb.Size = new System.Drawing.Size(1557, 1017);
            this.main_pb.TabIndex = 1;
            this.main_pb.TabStop = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1635, 87);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // mainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1904, 1041);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.main_pb);
            this.Name = "mainForm";
            this.Text = "Курсовая работа";
            ((System.ComponentModel.ISupportInitialize)(this.main_pb)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private PictureBox main_pb;
        private Button button1;
    }
}
