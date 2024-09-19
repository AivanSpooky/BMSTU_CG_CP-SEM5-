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
            this.btn_chromatic = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btn_vignette = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.main_pb)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
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
            // btn_chromatic
            // 
            this.btn_chromatic.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_chromatic.Location = new System.Drawing.Point(196, 61);
            this.btn_chromatic.Name = "btn_chromatic";
            this.btn_chromatic.Size = new System.Drawing.Size(97, 33);
            this.btn_chromatic.TabIndex = 3;
            this.btn_chromatic.Text = "Применить";
            this.btn_chromatic.UseVisualStyleBackColor = true;
            this.btn_chromatic.Click += new System.EventHandler(this.btn_chromatic_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btn_chromatic);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBox1.Location = new System.Drawing.Point(1593, 209);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(299, 100);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Хроматическая аберрация";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btn_vignette);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBox2.Location = new System.Drawing.Point(1593, 315);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(299, 100);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Виньетка";
            // 
            // btn_vignette
            // 
            this.btn_vignette.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_vignette.Location = new System.Drawing.Point(196, 61);
            this.btn_vignette.Name = "btn_vignette";
            this.btn_vignette.Size = new System.Drawing.Size(97, 33);
            this.btn_vignette.TabIndex = 3;
            this.btn_vignette.Text = "Применить";
            this.btn_vignette.UseVisualStyleBackColor = true;
            this.btn_vignette.Click += new System.EventHandler(this.btn_vignette_Click);
            // 
            // mainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1904, 1041);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.main_pb);
            this.Name = "mainForm";
            this.Text = "Курсовая работа";
            ((System.ComponentModel.ISupportInitialize)(this.main_pb)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private PictureBox main_pb;
        private Button button1;
        private Button btn_chromatic;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private Button btn_vignette;
    }
}
