using Sunny.UI;
using Ritrama2025.Helpers;
namespace Ritrama2025
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            panel1 = new Panel();
            OPC_MENU_LABELS = new Button();
            panel_DATA = new Panel();
            panel2 = new Panel();
            label4 = new Label();
            LAB_MODE_RUN = new Label();
            pictureBox1 = new PictureBox();
            panel_version_software = new Panel();
            label3 = new Label();
            label2 = new Label();
            label1 = new Label();
            lbl_user_name = new Label();
            button4 = new Button();
            button3 = new Button();
            button2 = new Button();
            button1 = new Button();
            bot_products = new Button();
            bot_recepciones = new Button();
            bot_inventario = new Button();
            bot_despacho = new Button();
            bot_ordencorte = new Button();
            bot_pedidos = new Button();
            tabContent = new UITabControl();
            panelSpacer = new Panel();
            panel1.SuspendLayout();
            panel_DATA.SuspendLayout();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            panel_version_software.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(OPC_MENU_LABELS);
            panel1.Controls.Add(panel_DATA);
            panel1.Controls.Add(button4);
            panel1.Controls.Add(button3);
            panel1.Controls.Add(button2);
            panel1.Controls.Add(button1);
            panel1.Controls.Add(bot_products);
            panel1.Controls.Add(bot_recepciones);
            panel1.Controls.Add(bot_inventario);
            panel1.Controls.Add(bot_despacho);
            panel1.Controls.Add(bot_ordencorte);
            panel1.Controls.Add(bot_pedidos);
            panel1.Dock = DockStyle.Left;
            panel1.Location = new Point(0, 35);
            panel1.Name = "panel1";
            panel1.Size = new Size(210, 1022);
            panel1.TabIndex = 0;
            // 
            // OPC_MENU_LABELS
            // 
            OPC_MENU_LABELS.Dock = DockStyle.Top;
            OPC_MENU_LABELS.FlatAppearance.BorderSize = 0;
            OPC_MENU_LABELS.FlatStyle = FlatStyle.Flat;
            OPC_MENU_LABELS.Image = (Image)resources.GetObject("OPC_MENU_LABELS.Image");
            OPC_MENU_LABELS.Location = new Point(0, 630);
            OPC_MENU_LABELS.Name = "OPC_MENU_LABELS";
            OPC_MENU_LABELS.Size = new Size(210, 70);
            OPC_MENU_LABELS.TabIndex = 12;
            OPC_MENU_LABELS.Text = "Etiquetas";
            OPC_MENU_LABELS.TextImageRelation = TextImageRelation.ImageBeforeText;
            OPC_MENU_LABELS.Click += OPC_MENU_LABELS_Click;
            // 
            // panel_DATA
            // 
            panel_DATA.Controls.Add(panel2);
            panel_DATA.Controls.Add(pictureBox1);
            panel_DATA.Controls.Add(panel_version_software);
            panel_DATA.Controls.Add(lbl_user_name);
            panel_DATA.Location = new Point(0, 706);
            panel_DATA.Name = "panel_DATA";
            panel_DATA.Size = new Size(210, 328);
            panel_DATA.TabIndex = 10;
            // 
            // panel2
            // 
            panel2.Anchor = AnchorStyles.None;
            panel2.Controls.Add(label4);
            panel2.Controls.Add(LAB_MODE_RUN);
            panel2.Location = new Point(0, 3);
            panel2.Name = "panel2";
            panel2.Size = new Size(207, 79);
            panel2.TabIndex = 1;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Teko SemiBold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label4.Location = new Point(5, 9);
            label4.Name = "label4";
            label4.Size = new Size(108, 23);
            label4.TabIndex = 3;
            label4.Text = "Modo Ejecucion :";
            // 
            // LAB_MODE_RUN
            // 
            LAB_MODE_RUN.AutoSize = true;
            LAB_MODE_RUN.Font = new Font("Teko SemiBold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            LAB_MODE_RUN.Location = new Point(63, 38);
            LAB_MODE_RUN.Name = "LAB_MODE_RUN";
            LAB_MODE_RUN.Size = new Size(47, 23);
            LAB_MODE_RUN.TabIndex = 11;
            LAB_MODE_RUN.Text = "MODE.";
            // 
            // pictureBox1
            // 
            pictureBox1.Dock = DockStyle.Bottom;
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(0, 81);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(210, 124);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // panel_version_software
            // 
            panel_version_software.Controls.Add(label3);
            panel_version_software.Controls.Add(label2);
            panel_version_software.Controls.Add(label1);
            panel_version_software.Dock = DockStyle.Bottom;
            panel_version_software.Location = new Point(0, 205);
            panel_version_software.Name = "panel_version_software";
            panel_version_software.Size = new Size(210, 105);
            panel_version_software.TabIndex = 1;
            // 
            // label3
            // 
            label3.Location = new Point(13, 58);
            label3.Name = "label3";
            label3.Size = new Size(97, 31);
            label3.TabIndex = 2;
            label3.Text = "Compilacion: 12-11-2025 10:42";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(3, 22);
            label2.Name = "label2";
            label2.Size = new Size(39, 16);
            label2.TabIndex = 1;
            label2.Text = "1.02";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(3, 7);
            label1.Name = "label1";
            label1.Size = new Size(143, 16);
            label1.TabIndex = 0;
            label1.Text = "Version Software:";
            // 
            // lbl_user_name
            // 
            lbl_user_name.Dock = DockStyle.Bottom;
            lbl_user_name.Location = new Point(0, 310);
            lbl_user_name.Name = "lbl_user_name";
            lbl_user_name.Size = new Size(210, 18);
            lbl_user_name.TabIndex = 11;
            lbl_user_name.Text = "Usuario : Nelson Pino";
            lbl_user_name.TextAlign = ContentAlignment.TopCenter;
            // 
            // button4
            // 
            button4.Dock = DockStyle.Top;
            button4.FlatAppearance.BorderSize = 0;
            button4.FlatStyle = FlatStyle.Flat;
            button4.Image = (Image)resources.GetObject("button4.Image");
            button4.Location = new Point(0, 560);
            button4.Name = "button4";
            button4.Size = new Size(210, 70);
            button4.TabIndex = 9;
            button4.Text = "Reportes";
            button4.TextImageRelation = TextImageRelation.ImageBeforeText;
            // 
            // button3
            // 
            button3.Dock = DockStyle.Top;
            button3.FlatAppearance.BorderSize = 0;
            button3.FlatStyle = FlatStyle.Flat;
            button3.Image = (Image)resources.GetObject("button3.Image");
            button3.Location = new Point(0, 490);
            button3.Name = "button3";
            button3.Size = new Size(210, 70);
            button3.TabIndex = 8;
            button3.Text = "Proveedores";
            button3.TextImageRelation = TextImageRelation.ImageBeforeText;
            // 
            // button2
            // 
            button2.Dock = DockStyle.Top;
            button2.FlatAppearance.BorderSize = 0;
            button2.FlatStyle = FlatStyle.Flat;
            button2.Image = (Image)resources.GetObject("button2.Image");
            button2.Location = new Point(0, 420);
            button2.Name = "button2";
            button2.Size = new Size(210, 70);
            button2.TabIndex = 7;
            button2.Text = "Usuarios";
            button2.TextImageRelation = TextImageRelation.ImageBeforeText;
            button2.Click += Bot_usuarios_Click;
            // 
            // button1
            // 
            button1.Dock = DockStyle.Top;
            button1.FlatAppearance.BorderSize = 0;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Image = (Image)resources.GetObject("button1.Image");
            button1.Location = new Point(0, 350);
            button1.Name = "button1";
            button1.Size = new Size(210, 70);
            button1.TabIndex = 6;
            button1.Text = "Clientes";
            button1.TextImageRelation = TextImageRelation.ImageBeforeText;
            button1.Click += Bot_clientes_Click;
            // 
            // bot_products
            // 
            bot_products.Dock = DockStyle.Top;
            bot_products.FlatAppearance.BorderSize = 0;
            bot_products.FlatStyle = FlatStyle.Flat;
            bot_products.Image = (Image)resources.GetObject("bot_products.Image");
            bot_products.Location = new Point(0, 280);
            bot_products.Name = "bot_products";
            bot_products.Size = new Size(210, 70);
            bot_products.TabIndex = 5;
            bot_products.Text = "Productos";
            bot_products.TextImageRelation = TextImageRelation.ImageBeforeText;
            bot_products.Click += Bot_products_Click;
            // 
            // bot_recepciones
            // 
            bot_recepciones.Dock = DockStyle.Top;
            bot_recepciones.FlatAppearance.BorderSize = 0;
            bot_recepciones.FlatStyle = FlatStyle.Flat;
            bot_recepciones.Image = Properties.Resources.add_to_clipboard_48px;
            bot_recepciones.Location = new Point(0, 210);
            bot_recepciones.Name = "bot_recepciones";
            bot_recepciones.Size = new Size(210, 70);
            bot_recepciones.TabIndex = 4;
            bot_recepciones.Text = "Recepciones";
            bot_recepciones.TextImageRelation = TextImageRelation.ImageBeforeText;
            bot_recepciones.Click += Bot_recepciones_Click;
            // 
            // bot_inventario
            // 
            bot_inventario.Dock = DockStyle.Top;
            bot_inventario.FlatAppearance.BorderSize = 0;
            bot_inventario.FlatStyle = FlatStyle.Flat;
            bot_inventario.Image = Properties.Resources.procurement_48px;
            bot_inventario.Location = new Point(0, 140);
            bot_inventario.Name = "bot_inventario";
            bot_inventario.Size = new Size(210, 70);
            bot_inventario.TabIndex = 3;
            bot_inventario.Text = "Inventario";
            bot_inventario.TextImageRelation = TextImageRelation.ImageBeforeText;
            bot_inventario.Click += Bot_inventario_Click;
            // 
            // bot_despacho
            // 
            bot_despacho.Dock = DockStyle.Top;
            bot_despacho.FlatAppearance.BorderSize = 0;
            bot_despacho.FlatStyle = FlatStyle.Flat;
            bot_despacho.Image = Properties.Resources.product_48px;
            bot_despacho.Location = new Point(0, 70);
            bot_despacho.Name = "bot_despacho";
            bot_despacho.Size = new Size(210, 70);
            bot_despacho.TabIndex = 2;
            bot_despacho.Text = "Despacho";
            bot_despacho.TextImageRelation = TextImageRelation.ImageBeforeText;
            bot_despacho.Click += Bot_despacho_Click;
            // 
            // bot_ordencorte
            // 
            bot_ordencorte.Dock = DockStyle.Top;
            bot_ordencorte.FlatAppearance.BorderSize = 0;
            bot_ordencorte.FlatStyle = FlatStyle.Flat;
            bot_ordencorte.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            bot_ordencorte.Image = (Image)resources.GetObject("bot_ordencorte.Image");
            bot_ordencorte.Location = new Point(0, 0);
            bot_ordencorte.Margin = new Padding(2);
            bot_ordencorte.Name = "bot_ordencorte";
            bot_ordencorte.Size = new Size(210, 70);
            bot_ordencorte.TabIndex = 1;
            bot_ordencorte.Text = "Orden Corte";
            bot_ordencorte.TextImageRelation = TextImageRelation.ImageBeforeText;
            bot_ordencorte.Click += Bot_ordencorte_Click;
            // 
            // bot_pedidos
            // 
            bot_pedidos.Dock = DockStyle.Top;
            bot_pedidos.FlatAppearance.BorderSize = 0;
            bot_pedidos.FlatStyle = FlatStyle.Flat;
            bot_pedidos.Image = Properties.Resources.add_file_32px;
            bot_pedidos.Location = new Point(0, 420);
            bot_pedidos.Name = "bot_pedidos";
            bot_pedidos.Size = new Size(210, 70);
            bot_pedidos.TabIndex = 13;
            bot_pedidos.Text = "Pedidos";
            bot_pedidos.TextImageRelation = TextImageRelation.ImageBeforeText;
            bot_pedidos.Click += Bot_pedidos_Click;
            // 
            // tabContent
            // 
            tabContent.Dock = DockStyle.Fill;
            tabContent.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabContent.Font = new Font("Segoe UI", 11F, FontStyle.Regular);
            tabContent.Location = new Point(210, 35);
            tabContent.MainPage = "";
            tabContent.MenuStyle = Sunny.UI.UIMenuStyle.Custom;
            tabContent.Name = "tabContent";
            tabContent.SelectedIndex = 0;
            tabContent.Size = new Size(1069, 1022);
            tabContent.TabIndex = 1;
            tabContent.TabSelectedForeColor = Color.White;
            tabContent.TabSelectedHighColor = Color.FromArgb(110, 190, 40);
            tabContent.TabSelectedHighColorSize = 3;
            tabContent.TabUnSelectedForeColor = Color.FromArgb(180, 180, 180);
            tabContent.TipsFont = new Font("Microsoft Sans Serif", 9F);
            tabContent.Visible = false;
            
            // 
            // panelSpacer
            // 
            panelSpacer.BackColor = TemaOscuroHelper.Fondo;
            panelSpacer.Dock = DockStyle.Top;
            panelSpacer.Height = 10;
            panelSpacer.Name = "panelSpacer";

            // 
            // Main
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(1279, 1057);
            Controls.Add(panelSpacer);
            Controls.Add(tabContent);
            Controls.Add(panel1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Main";
            Text = "Sistema de Produccion e Inventario";
            WindowState = FormWindowState.Maximized;
            ZoomScaleRect = new Rectangle(15, 15, 1279, 1057);
            Load += Main_Load;
            panel1.ResumeLayout(false);
            panel_DATA.ResumeLayout(false);
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            panel_version_software.ResumeLayout(false);
            panel_version_software.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Sunny.UI.UITabControl tabContent;
        private Panel panelSpacer;
        private Panel panel1;
        private Button bot_ordencorte;
        private Button bot_products;
        private Button bot_recepciones;
        private Button bot_inventario;
        private Button bot_despacho;
        private Button button2;
        private Button button1;
        private Button button3;
        private Button button4;
        private Panel panel_DATA;
        private PictureBox pictureBox1;
        private Label lbl_user_name;
        private Button OPC_MENU_LABELS;
        private Button bot_pedidos;
        private Panel panel_version_software;
        private Label label2;
        private Label label1;
        private Label label3;
        private Panel panel2;
        private Label LAB_MODE_RUN;
        private Label label4;
    }
}










