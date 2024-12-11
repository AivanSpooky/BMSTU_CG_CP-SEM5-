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
            this.btnGridSettings = new System.Windows.Forms.Button();
            this.btn_simulate = new System.Windows.Forms.Button();
            this.lbl_form = new System.Windows.Forms.Label();
            this.focus_panel = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnSimSettings = new System.Windows.Forms.Button();
            this.btnLightSettings = new System.Windows.Forms.Button();
            this.lblCamHints = new System.Windows.Forms.Label();
            this.btnCamBack = new System.Windows.Forms.Button();
            this.lblSimInfo = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox5.SuspendLayout();
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
            this.groupBox1.BackColor = System.Drawing.SystemColors.Info;
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
            this.btn_add_fig.Text = "Добавить тело";
            this.btn_add_fig.UseVisualStyleBackColor = true;
            this.btn_add_fig.Click += new System.EventHandler(this.btn_add_fig_Click);
            // 
            // btnGridSettings
            // 
            this.btnGridSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnGridSettings.Location = new System.Drawing.Point(6, 9);
            this.btnGridSettings.Name = "btnGridSettings";
            this.btnGridSettings.Size = new System.Drawing.Size(202, 32);
            this.btnGridSettings.TabIndex = 6;
            this.btnGridSettings.TabStop = false;
            this.btnGridSettings.Text = "Изменить площадку";
            this.btnGridSettings.UseVisualStyleBackColor = true;
            this.btnGridSettings.Click += new System.EventHandler(this.btnGridSettings_Click);
            // 
            // btn_simulate
            // 
            this.btn_simulate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_simulate.Location = new System.Drawing.Point(6, 21);
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
            this.lbl_form.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.lbl_form.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbl_form.Location = new System.Drawing.Point(518, 12);
            this.lbl_form.Name = "lbl_form";
            this.lbl_form.Size = new System.Drawing.Size(214, 34);
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
            // label1
            // 
            this.label1.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(518, 169);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(214, 34);
            this.label1.TabIndex = 4;
            this.label1.Text = "Параметры";
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.SystemColors.Info;
            this.groupBox2.Controls.Add(this.btnSimSettings);
            this.groupBox2.Controls.Add(this.btnLightSettings);
            this.groupBox2.Controls.Add(this.btnGridSettings);
            this.groupBox2.Location = new System.Drawing.Point(518, 206);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(214, 126);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            // 
            // btnSimSettings
            // 
            this.btnSimSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnSimSettings.Location = new System.Drawing.Point(6, 85);
            this.btnSimSettings.Name = "btnSimSettings";
            this.btnSimSettings.Size = new System.Drawing.Size(202, 32);
            this.btnSimSettings.TabIndex = 11;
            this.btnSimSettings.TabStop = false;
            this.btnSimSettings.Text = "Изменить симуляцию";
            this.btnSimSettings.UseVisualStyleBackColor = true;
            this.btnSimSettings.Click += new System.EventHandler(this.btnSimSettings_Click);
            // 
            // btnLightSettings
            // 
            this.btnLightSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnLightSettings.Location = new System.Drawing.Point(6, 47);
            this.btnLightSettings.Name = "btnLightSettings";
            this.btnLightSettings.Size = new System.Drawing.Size(202, 32);
            this.btnLightSettings.TabIndex = 8;
            this.btnLightSettings.TabStop = false;
            this.btnLightSettings.Text = "Изменить источник";
            this.btnLightSettings.UseVisualStyleBackColor = true;
            this.btnLightSettings.Click += new System.EventHandler(this.btnLightSettings_Click);
            // 
            // lblCamHints
            // 
            this.lblCamHints.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblCamHints.Location = new System.Drawing.Point(518, 586);
            this.lblCamHints.Name = "lblCamHints";
            this.lblCamHints.Size = new System.Drawing.Size(214, 75);
            this.lblCamHints.TabIndex = 9;
            this.lblCamHints.Text = "Управление камерой:\r\n- Перемещение: W, A, S, D\r\n- Вращение: ←, →, ↑, ↓";
            // 
            // btnCamBack
            // 
            this.btnCamBack.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnCamBack.Location = new System.Drawing.Point(6, 19);
            this.btnCamBack.Name = "btnCamBack";
            this.btnCamBack.Size = new System.Drawing.Size(202, 32);
            this.btnCamBack.TabIndex = 10;
            this.btnCamBack.TabStop = false;
            this.btnCamBack.Text = "Вернуть камеру";
            this.btnCamBack.UseVisualStyleBackColor = true;
            this.btnCamBack.Click += new System.EventHandler(this.btnCamBack_Click);
            // 
            // lblSimInfo
            // 
            this.lblSimInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblSimInfo.Location = new System.Drawing.Point(12, 609);
            this.lblSimInfo.Name = "lblSimInfo";
            this.lblSimInfo.Size = new System.Drawing.Size(214, 52);
            this.lblSimInfo.TabIndex = 11;
            this.lblSimInfo.Text = "Параметры симуляции:\r\nскорость падения = ";
            this.lblSimInfo.Click += new System.EventHandler(this.label2_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.SystemColors.Info;
            this.groupBox3.Controls.Add(this.btnCamBack);
            this.groupBox3.Location = new System.Drawing.Point(518, 664);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(214, 64);
            this.groupBox3.TabIndex = 12;
            this.groupBox3.TabStop = false;
            // 
            // groupBox5
            // 
            this.groupBox5.BackColor = System.Drawing.SystemColors.Info;
            this.groupBox5.Controls.Add(this.btn_simulate);
            this.groupBox5.Location = new System.Drawing.Point(12, 664);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(214, 64);
            this.groupBox5.TabIndex = 14;
            this.groupBox5.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(744, 740);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.lblSimInfo);
            this.Controls.Add(this.lblCamHints);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.focus_panel);
            this.Controls.Add(this.lbl_form);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pictureBox1);
            this.Name = "Form1";
            this.Text = "Симуляция падения тел в лунки";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
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
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnLightSettings;
        private System.Windows.Forms.Label lblCamHints;
        private System.Windows.Forms.Button btnCamBack;
        private System.Windows.Forms.Button btnSimSettings;
        private System.Windows.Forms.Label lblSimInfo;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox5;
    }
}

