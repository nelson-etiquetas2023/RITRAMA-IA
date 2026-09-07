namespace Ritrama2025.Forms.Otros
{
    partial class Frm_AprobarOC
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_AprobarOC));
            panel1 = new Panel();
            pictureBox1 = new PictureBox();
            label1 = new Label();
            label2 = new Label();
            txt_person = new TextBox();
            txt_OrdenTrabajo = new TextBox();
            label3 = new Label();
            txt_OrdenServicio = new TextBox();
            label4 = new Label();
            txt_comentarios = new RichTextBox();
            label5 = new Label();
            chk_closeOrden = new CheckBox();
            bot_documentCheck = new Button();
            bot_cancel = new Button();
            pictureBox2 = new PictureBox();
            pictureBox3 = new PictureBox();
            pictureBox4 = new PictureBox();
            label_datetime = new Label();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(pictureBox1);
            panel1.Controls.Add(label1);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Margin = new Padding(3, 4, 3, 4);
            panel1.Name = "panel1";
            panel1.Size = new Size(485, 89);
            panel1.TabIndex = 0;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.data_quality_50px;
            pictureBox1.Location = new Point(27, 18);
            pictureBox1.Margin = new Padding(3, 4, 3, 4);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(57, 60);
            pictureBox1.TabIndex = 1;
            pictureBox1.TabStop = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(91, 34);
            label1.Name = "label1";
            label1.Size = new Size(312, 33);
            label1.TabIndex = 0;
            label1.Text = "Aprobación Documento :";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(15, 101);
            label2.Name = "label2";
            label2.Size = new Size(114, 18);
            label2.TabIndex = 1;
            label2.Text = "Persona Autoriza:";
            // 
            // txt_person
            // 
            txt_person.Location = new Point(14, 123);
            txt_person.Margin = new Padding(3, 4, 3, 4);
            txt_person.Name = "txt_person";
            txt_person.Size = new Size(311, 25);
            txt_person.TabIndex = 2;
            // 
            // txt_OrdenTrabajo
            // 
            txt_OrdenTrabajo.Location = new Point(13, 179);
            txt_OrdenTrabajo.Margin = new Padding(3, 4, 3, 4);
            txt_OrdenTrabajo.Name = "txt_OrdenTrabajo";
            txt_OrdenTrabajo.Size = new Size(311, 25);
            txt_OrdenTrabajo.TabIndex = 4;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(14, 157);
            label3.Name = "label3";
            label3.Size = new Size(113, 18);
            label3.TabIndex = 3;
            label3.Text = "Order de Trabajo:";
            // 
            // txt_OrdenServicio
            // 
            txt_OrdenServicio.Location = new Point(15, 234);
            txt_OrdenServicio.Margin = new Padding(3, 4, 3, 4);
            txt_OrdenServicio.Name = "txt_OrdenServicio";
            txt_OrdenServicio.Size = new Size(311, 25);
            txt_OrdenServicio.TabIndex = 6;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(16, 213);
            label4.Name = "label4";
            label4.Size = new Size(114, 18);
            label4.TabIndex = 5;
            label4.Text = "Order de Servicio:";
            // 
            // txt_comentarios
            // 
            txt_comentarios.Location = new Point(13, 311);
            txt_comentarios.Margin = new Padding(3, 4, 3, 4);
            txt_comentarios.Name = "txt_comentarios";
            txt_comentarios.Size = new Size(457, 150);
            txt_comentarios.TabIndex = 7;
            txt_comentarios.Text = "";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(13, 290);
            label5.Name = "label5";
            label5.Size = new Size(187, 18);
            label5.TabIndex = 8;
            label5.Text = "Observaciones de Esta Orden:";
            // 
            // chk_closeOrden
            // 
            chk_closeOrden.AutoSize = true;
            chk_closeOrden.Location = new Point(329, 469);
            chk_closeOrden.Margin = new Padding(3, 4, 3, 4);
            chk_closeOrden.Name = "chk_closeOrden";
            chk_closeOrden.Size = new Size(144, 21);
            chk_closeOrden.TabIndex = 9;
            chk_closeOrden.Text = "Deseo Cerrar Orden";
            // 
            // bot_documentCheck
            // 
            bot_documentCheck.Image = (Image)resources.GetObject("bot_documentCheck.Image");
            bot_documentCheck.Location = new Point(11, 469);
            bot_documentCheck.Margin = new Padding(3, 4, 3, 4);
            bot_documentCheck.Name = "bot_documentCheck";
            bot_documentCheck.Size = new Size(114, 42);
            bot_documentCheck.TabIndex = 10;
            bot_documentCheck.Text = "Aprobar";
            bot_documentCheck.TextImageRelation = TextImageRelation.ImageBeforeText;
            bot_documentCheck.Click += Bot_documentCheck_Click;
            // 
            // bot_cancel
            // 
            bot_cancel.Image = Properties.Resources.icons8_cancel_16;
            bot_cancel.Location = new Point(132, 469);
            bot_cancel.Margin = new Padding(3, 4, 3, 4);
            bot_cancel.Name = "bot_cancel";
            bot_cancel.Size = new Size(114, 42);
            bot_cancel.TabIndex = 11;
            bot_cancel.Text = "Cancelar";
            bot_cancel.TextImageRelation = TextImageRelation.ImageBeforeText;
            // 
            // pictureBox2
            // 
            pictureBox2.Image = (Image)resources.GetObject("pictureBox2.Image");
            pictureBox2.Location = new Point(331, 125);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(26, 23);
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.TabIndex = 12;
            pictureBox2.TabStop = false;
            // 
            // pictureBox3
            // 
            pictureBox3.Image = Properties.Resources.orden_trabajo;
            pictureBox3.Location = new Point(330, 181);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(26, 23);
            pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox3.TabIndex = 13;
            pictureBox3.TabStop = false;
            // 
            // pictureBox4
            // 
            pictureBox4.Image = Properties.Resources.orden_servicio_32;
            pictureBox4.Location = new Point(332, 234);
            pictureBox4.Name = "pictureBox4";
            pictureBox4.Size = new Size(26, 23);
            pictureBox4.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox4.TabIndex = 14;
            pictureBox4.TabStop = false;
            // 
            // label_datetime
            // 
            label_datetime.AutoSize = true;
            label_datetime.Location = new Point(14, 538);
            label_datetime.Name = "label_datetime";
            label_datetime.Size = new Size(47, 18);
            label_datetime.TabIndex = 15;
            label_datetime.Text = "label6";
            // 
            // Frm_AprobarOC
            // 
            AutoScaleDimensions = new SizeF(8F, 18F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(485, 575);
            Controls.Add(label_datetime);
            Controls.Add(pictureBox4);
            Controls.Add(pictureBox3);
            Controls.Add(pictureBox2);
            Controls.Add(bot_cancel);
            Controls.Add(bot_documentCheck);
            Controls.Add(chk_closeOrden);
            Controls.Add(label5);
            Controls.Add(txt_comentarios);
            Controls.Add(txt_OrdenServicio);
            Controls.Add(label4);
            Controls.Add(txt_OrdenTrabajo);
            Controls.Add(label3);
            Controls.Add(txt_person);
            Controls.Add(label2);
            Controls.Add(panel1);
            Font = new Font("Noto Sans", 9.75F);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(3, 4, 3, 4);
            Name = "Frm_AprobarOC";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Aprobar Documento Orden de Corte";
            Load += Frm_AprobarOC_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panel1;
        private Label label1;
        private Label label2;
        private TextBox txt_person;
        private TextBox txt_OrdenTrabajo;
        private Label label3;
        private TextBox txt_OrdenServicio;
        private Label label4;
        private RichTextBox txt_comentarios;
        private Label label5;
        private CheckBox chk_closeOrden;
        private Button bot_documentCheck;
        private Button bot_cancel;
        private PictureBox pictureBox1;
        private PictureBox pictureBox2;
        private PictureBox pictureBox3;
        private PictureBox pictureBox4;
        private Label label_datetime;
    }
}


