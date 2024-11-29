namespace test
{
    partial class Form1
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btn_add_ident = new System.Windows.Forms.Button();
            this.btn_add_fig = new System.Windows.Forms.Button();
            this.btn_simulate = new System.Windows.Forms.Button();
            this.lbl_form = new System.Windows.Forms.Label();
            this.focus_panel = new System.Windows.Forms.Panel();
            this.btnGridSettings = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(500, 500);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnGridSettings);
            this.groupBox1.Controls.Add(this.btn_add_ident);
            this.groupBox1.Controls.Add(this.btn_add_fig);
            this.groupBox1.Location = new System.Drawing.Point(518, 40);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(214, 126);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            // 
            // btn_add_ident
            // 
            this.btn_add_ident.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_add_ident.Location = new System.Drawing.Point(6, 47);
            this.btn_add_ident.Name = "btn_add_ident";
            this.btn_add_ident.Size = new System.Drawing.Size(202, 32);
            this.btn_add_ident.TabIndex = 5;
            this.btn_add_ident.TabStop = false;
            this.btn_add_ident.Text = "Добавить лунку";
            this.btn_add_ident.UseVisualStyleBackColor = true;
            this.btn_add_ident.Click += new System.EventHandler(this.btn_add_ident_Click);
            // 
            // btn_add_fig
            // 
            this.btn_add_fig.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_add_fig.Location = new System.Drawing.Point(6, 9);
            this.btn_add_fig.Name = "btn_add_fig";
            this.btn_add_fig.Size = new System.Drawing.Size(202, 32);
            this.btn_add_fig.TabIndex = 4;
            this.btn_add_fig.TabStop = false;
            this.btn_add_fig.Text = "Добавить фигуру";
            this.btn_add_fig.UseVisualStyleBackColor = true;
            this.btn_add_fig.Click += new System.EventHandler(this.btn_add_fig_Click);
            // 
            // btn_simulate
            // 
            this.btn_simulate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_simulate.Location = new System.Drawing.Point(524, 172);
            this.btn_simulate.Name = "btn_simulate";
            this.btn_simulate.Size = new System.Drawing.Size(202, 32);
            this.btn_simulate.TabIndex = 2;
            this.btn_simulate.TabStop = false;
            this.btn_simulate.Text = "Запустить симуляцию";
            this.btn_simulate.UseVisualStyleBackColor = true;
            this.btn_simulate.Click += new System.EventHandler(this.btn_simulate_Click);
            // 
            // lbl_form
            // 
            this.lbl_form.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbl_form.Location = new System.Drawing.Point(518, 12);
            this.lbl_form.Name = "lbl_form";
            this.lbl_form.Size = new System.Drawing.Size(211, 34);
            this.lbl_form.TabIndex = 2;
            this.lbl_form.Text = "Действия";
            // 
            // focus_panel
            // 
            this.focus_panel.Location = new System.Drawing.Point(12, 518);
            this.focus_panel.Name = "focus_panel";
            this.focus_panel.Size = new System.Drawing.Size(200, 100);
            this.focus_panel.TabIndex = 3;
            this.focus_panel.Visible = false;
            // 
            // btnGridSettings
            // 
            this.btnGridSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnGridSettings.Location = new System.Drawing.Point(6, 85);
            this.btnGridSettings.Name = "btnGridSettings";
            this.btnGridSettings.Size = new System.Drawing.Size(202, 32);
            this.btnGridSettings.TabIndex = 6;
            this.btnGridSettings.TabStop = false;
            this.btnGridSettings.Text = "Изменить площадку";
            this.btnGridSettings.UseVisualStyleBackColor = true;
            this.btnGridSettings.Click += new System.EventHandler(this.btnGridSettings_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(744, 740);
            this.Controls.Add(this.focus_panel);
            this.Controls.Add(this.lbl_form);
            this.Controls.Add(this.btn_simulate);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pictureBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btn_simulate;
        private System.Windows.Forms.Label lbl_form;
        private System.Windows.Forms.Panel focus_panel;
        private System.Windows.Forms.Button btn_add_ident;
        private System.Windows.Forms.Button btn_add_fig;
        private System.Windows.Forms.Button btnGridSettings;
    }
}

