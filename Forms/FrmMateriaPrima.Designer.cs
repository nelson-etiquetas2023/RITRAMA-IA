namespace Ritrama2025.Forms
{
    partial class FrmMateriaPrima
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMateriaPrima));
            toolStrip1 = new ToolStrip();
            btn_primero = new ToolStripButton();
            btn_anterior = new ToolStripButton();
            btn_siguiente = new ToolStripButton();
            btn_ultimo = new ToolStripButton();
            btn_create = new ToolStripButton();
            btn_cancel = new ToolStripButton();
            btn_save = new ToolStripButton();
            btn_CloseDoc = new ToolStripButton();
            btn_AnularDoc = new ToolStripButton();
            btn_SearchDoc = new ToolStripButton();
            btn_printDoc = new ToolStripButton();
            btn_ExportDoc = new ToolStripButton();
            panel1 = new Panel();
            label13 = new Label();
            panelContador = new Panel();
            label_counter_rows = new Label();
            panelDatos = new Panel();
            label1 = new Label();
            txt_numeroOrden = new TextBox();
            btn_OrdenBuscar = new Button();
            label5 = new Label();
            txt_OrdenCompra = new TextBox();
            label3 = new Label();
            txt_prov_Id = new TextBox();
            txt_nombre_prov = new TextBox();
            btn_ProvBuscar = new Button();
            label2 = new Label();
            txt_fecha_recepcion = new DateTimePicker();
            label4 = new Label();
            txt_transport_id = new TextBox();
            txt_transport_name = new TextBox();
            btn_TransportBuscar = new Button();
            label12 = new Label();
            txt_fecha_produccion = new DateTimePicker();
            label6 = new Label();
            txt_person_id = new TextBox();
            txt_person_name = new TextBox();
            btn_RecepBuscar = new Button();
            label7 = new Label();
            txt_guia = new TextBox();
            label10 = new Label();
            txt_lote = new TextBox();
            label11 = new Label();
            txt_embarque = new TextBox();
            panelDetalle = new Panel();
            btn_LoadRows = new Button();
            btn_template = new Button();
            btn_deleteRows = new Button();
            btn_addRows = new Button();
            GridItems = new DataGridView();
            panelNotas = new Panel();
            label9 = new Label();
            txt_total_cantidad = new TextBox();
            label16 = new Label();
            chk_anulado = new CheckBox();
            Pic_Document = new PictureBox();
            chk_DocumentClose = new CheckBox();
            label8 = new Label();
            txt_notas = new RichTextBox();
            toolStrip1.SuspendLayout();
            panel1.SuspendLayout();
            panelContador.SuspendLayout();
            panelDatos.SuspendLayout();
            panelDetalle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)GridItems).BeginInit();
            panelNotas.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)Pic_Document).BeginInit();
            SuspendLayout();
            //
            // toolStrip1
            //
            toolStrip1.AutoSize = false;
            toolStrip1.BackColor = Color.FromArgb(225, 225, 225);
            toolStrip1.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            toolStrip1.ForeColor = Color.FromArgb(80, 80, 80);
            toolStrip1.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip1.Items.AddRange(new ToolStripItem[] { btn_primero, btn_anterior, btn_siguiente, btn_ultimo, btn_create, btn_cancel, btn_save, btn_CloseDoc, btn_AnularDoc, btn_SearchDoc, btn_printDoc, btn_ExportDoc });
            toolStrip1.Location = new Point(0, 160);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Padding = new Padding(4, 0, 0, 0);
            toolStrip1.RenderMode = ToolStripRenderMode.Professional;
            toolStrip1.Size = new Size(1264, 40);
            toolStrip1.TabIndex = 1;
            toolStrip1.Text = "toolStrip1";
            //
            // btn_primero
            //
            btn_primero.AutoSize = false;
            btn_primero.BackColor = Color.Transparent;
            btn_primero.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            btn_primero.ForeColor = Color.FromArgb(80, 80, 80);
            btn_primero.Image = (Image)resources.GetObject("btn_primero.Image");
            btn_primero.ImageAlign = ContentAlignment.MiddleLeft;
            btn_primero.ImageScaling = ToolStripItemImageScaling.None;
            btn_primero.ImageTransparentColor = Color.Magenta;
            btn_primero.Margin = new Padding(4, 1, 0, 2);
            btn_primero.Name = "btn_primero";
            btn_primero.Size = new Size(92, 36);
            btn_primero.Text = "Primero";
            btn_primero.ToolTipText = "Primera orden de la lista";
            btn_primero.Click += Btn_primero_Click;
            //
            // btn_anterior
            //
            btn_anterior.AutoSize = false;
            btn_anterior.BackColor = Color.Transparent;
            btn_anterior.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            btn_anterior.ForeColor = Color.FromArgb(80, 80, 80);
            btn_anterior.Image = (Image)resources.GetObject("btn_anterior.Image");
            btn_anterior.ImageAlign = ContentAlignment.MiddleLeft;
            btn_anterior.ImageScaling = ToolStripItemImageScaling.None;
            btn_anterior.ImageTransparentColor = Color.Magenta;
            btn_anterior.Margin = new Padding(4, 1, 0, 2);
            btn_anterior.Name = "btn_anterior";
            btn_anterior.Size = new Size(92, 36);
            btn_anterior.Text = "Anterior";
            btn_anterior.ToolTipText = "Orden anterior";
            btn_anterior.Click += Btn_anterior_Click;
            //
            // btn_siguiente
            //
            btn_siguiente.AutoSize = false;
            btn_siguiente.BackColor = Color.Transparent;
            btn_siguiente.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            btn_siguiente.ForeColor = Color.FromArgb(80, 80, 80);
            btn_siguiente.Image = (Image)resources.GetObject("btn_siguiente.Image");
            btn_siguiente.ImageAlign = ContentAlignment.MiddleLeft;
            btn_siguiente.ImageScaling = ToolStripItemImageScaling.None;
            btn_siguiente.ImageTransparentColor = Color.Magenta;
            btn_siguiente.Margin = new Padding(4, 1, 0, 2);
            btn_siguiente.Name = "btn_siguiente";
            btn_siguiente.Size = new Size(92, 36);
            btn_siguiente.Text = "Siguiente";
            btn_siguiente.ToolTipText = "Orden siguiente";
            btn_siguiente.Click += Btn_siguiente_Click;
            //
            // btn_ultimo
            //
            btn_ultimo.AutoSize = false;
            btn_ultimo.BackColor = Color.Transparent;
            btn_ultimo.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            btn_ultimo.ForeColor = Color.FromArgb(80, 80, 80);
            btn_ultimo.Image = (Image)resources.GetObject("btn_ultimo.Image");
            btn_ultimo.ImageAlign = ContentAlignment.MiddleLeft;
            btn_ultimo.ImageScaling = ToolStripItemImageScaling.None;
            btn_ultimo.ImageTransparentColor = Color.Magenta;
            btn_ultimo.Margin = new Padding(4, 1, 0, 2);
            btn_ultimo.Name = "btn_ultimo";
            btn_ultimo.Size = new Size(92, 36);
            btn_ultimo.Text = "Ultimo";
            btn_ultimo.ToolTipText = "Ultima orden de la lista";
            btn_ultimo.Click += Btn_ultimo_Click;
            //
            // btn_create
            //
            btn_create.AutoSize = false;
            btn_create.BackColor = Color.Transparent;
            btn_create.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            btn_create.ForeColor = Color.FromArgb(80, 80, 80);
            btn_create.Image = (Image)resources.GetObject("btn_create.Image");
            btn_create.ImageAlign = ContentAlignment.MiddleLeft;
            btn_create.ImageScaling = ToolStripItemImageScaling.None;
            btn_create.ImageTransparentColor = Color.Magenta;
            btn_create.Margin = new Padding(4, 1, 0, 2);
            btn_create.Name = "btn_create";
            btn_create.Size = new Size(92, 36);
            btn_create.Text = "Nuevo";
            btn_create.ToolTipText = "Crear una orden de compra nueva";
            btn_create.Click += Btn_create_Click;
            //
            // btn_cancel
            //
            btn_cancel.AutoSize = false;
            btn_cancel.BackColor = Color.Transparent;
            btn_cancel.Enabled = false;
            btn_cancel.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            btn_cancel.ForeColor = Color.FromArgb(80, 80, 80);
            btn_cancel.Image = (Image)resources.GetObject("btn_cancel.Image");
            btn_cancel.ImageAlign = ContentAlignment.MiddleLeft;
            btn_cancel.ImageScaling = ToolStripItemImageScaling.None;
            btn_cancel.ImageTransparentColor = Color.Magenta;
            btn_cancel.Margin = new Padding(4, 1, 0, 2);
            btn_cancel.Name = "btn_cancel";
            btn_cancel.Size = new Size(92, 36);
            btn_cancel.Text = "Cancelar";
            btn_cancel.ToolTipText = "Descartar la orden en edicion";
            btn_cancel.Click += Btn_cancel_Click;
            //
            // btn_save
            //
            btn_save.AutoSize = false;
            btn_save.BackColor = Color.Transparent;
            btn_save.Enabled = false;
            btn_save.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            btn_save.ForeColor = Color.FromArgb(80, 80, 80);
            btn_save.Image = (Image)resources.GetObject("btn_save.Image");
            btn_save.ImageAlign = ContentAlignment.MiddleLeft;
            btn_save.ImageScaling = ToolStripItemImageScaling.None;
            btn_save.ImageTransparentColor = Color.Magenta;
            btn_save.Margin = new Padding(4, 1, 0, 2);
            btn_save.Name = "btn_save";
            btn_save.Size = new Size(92, 36);
            btn_save.Text = "Guardar";
            btn_save.ToolTipText = "Grabar la orden de compra";
            btn_save.Click += Btn_save_Click;
            //
            // btn_CloseDoc
            //
            btn_CloseDoc.AutoSize = false;
            btn_CloseDoc.BackColor = Color.Transparent;
            btn_CloseDoc.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            btn_CloseDoc.ForeColor = Color.FromArgb(80, 80, 80);
            btn_CloseDoc.Image = (Image)resources.GetObject("btn_CloseDoc.Image");
            btn_CloseDoc.ImageAlign = ContentAlignment.MiddleLeft;
            btn_CloseDoc.ImageScaling = ToolStripItemImageScaling.None;
            btn_CloseDoc.ImageTransparentColor = Color.Magenta;
            btn_CloseDoc.Margin = new Padding(4, 1, 0, 2);
            btn_CloseDoc.Name = "btn_CloseDoc";
            btn_CloseDoc.Size = new Size(92, 36);
            btn_CloseDoc.Text = "Cerrar";
            btn_CloseDoc.ToolTipText = "Cerrar la orden de compra";
            btn_CloseDoc.Click += Btn_CloseDoc_Click;
            //
            // btn_AnularDoc
            //
            btn_AnularDoc.AutoSize = false;
            btn_AnularDoc.BackColor = Color.Transparent;
            btn_AnularDoc.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            btn_AnularDoc.ForeColor = Color.FromArgb(80, 80, 80);
            btn_AnularDoc.Image = (Image)resources.GetObject("btn_AnularDoc.Image");
            btn_AnularDoc.ImageAlign = ContentAlignment.MiddleLeft;
            btn_AnularDoc.ImageScaling = ToolStripItemImageScaling.None;
            btn_AnularDoc.ImageTransparentColor = Color.Magenta;
            btn_AnularDoc.Margin = new Padding(4, 1, 0, 2);
            btn_AnularDoc.Name = "btn_AnularDoc";
            btn_AnularDoc.Size = new Size(92, 36);
            btn_AnularDoc.Text = "Anular";
            btn_AnularDoc.ToolTipText = "Anular la orden de compra";
            btn_AnularDoc.Click += Btn_AnularDoc_Click;
            //
            // btn_SearchDoc
            //
            btn_SearchDoc.AutoSize = false;
            btn_SearchDoc.BackColor = Color.Transparent;
            btn_SearchDoc.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            btn_SearchDoc.ForeColor = Color.FromArgb(80, 80, 80);
            btn_SearchDoc.Image = (Image)resources.GetObject("btn_SearchDoc.Image");
            btn_SearchDoc.ImageAlign = ContentAlignment.MiddleLeft;
            btn_SearchDoc.ImageScaling = ToolStripItemImageScaling.None;
            btn_SearchDoc.ImageTransparentColor = Color.Magenta;
            btn_SearchDoc.Margin = new Padding(4, 1, 0, 2);
            btn_SearchDoc.Name = "btn_SearchDoc";
            btn_SearchDoc.Size = new Size(92, 36);
            btn_SearchDoc.Text = "Buscar";
            btn_SearchDoc.ToolTipText = "Buscar una orden de compra";
            btn_SearchDoc.Click += Btn_SearchDoc_Click;
            //
            // btn_printDoc
            //
            btn_printDoc.AutoSize = false;
            btn_printDoc.BackColor = Color.Transparent;
            btn_printDoc.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            btn_printDoc.ForeColor = Color.FromArgb(80, 80, 80);
            btn_printDoc.Image = (Image)resources.GetObject("btn_printDoc.Image");
            btn_printDoc.ImageAlign = ContentAlignment.MiddleLeft;
            btn_printDoc.ImageScaling = ToolStripItemImageScaling.None;
            btn_printDoc.ImageTransparentColor = Color.Magenta;
            btn_printDoc.Margin = new Padding(4, 1, 0, 2);
            btn_printDoc.Name = "btn_printDoc";
            btn_printDoc.Size = new Size(92, 36);
            btn_printDoc.Text = "Imprimir";
            btn_printDoc.ToolTipText = "Imprimir la orden de compra";
            btn_printDoc.Click += Btn_printDoc_Click;
            //
            // btn_ExportDoc
            //
            btn_ExportDoc.AutoSize = false;
            btn_ExportDoc.BackColor = Color.Transparent;
            btn_ExportDoc.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            btn_ExportDoc.ForeColor = Color.FromArgb(80, 80, 80);
            btn_ExportDoc.Image = (Image)resources.GetObject("btn_ExportDoc.Image");
            btn_ExportDoc.ImageAlign = ContentAlignment.MiddleLeft;
            btn_ExportDoc.ImageScaling = ToolStripItemImageScaling.None;
            btn_ExportDoc.ImageTransparentColor = Color.Magenta;
            btn_ExportDoc.Margin = new Padding(4, 1, 0, 2);
            btn_ExportDoc.Name = "btn_ExportDoc";
            btn_ExportDoc.Size = new Size(92, 36);
            btn_ExportDoc.Text = "Exportar";
            btn_ExportDoc.ToolTipText = "Exportar los productos a Excel";
            btn_ExportDoc.Click += Btn_ExportDoc_Click;
            //
            // panel1
            //
            panel1.BackColor = Color.FromArgb(190, 225, 150);
            panel1.Controls.Add(label13);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 35);
            panel1.Name = "panel1";
            panel1.Size = new Size(1264, 60);
            panel1.TabIndex = 0;
            //
            // label13
            //
            label13.Dock = DockStyle.Fill;
            label13.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            label13.ForeColor = Color.FromArgb(70, 140, 25);
            label13.Location = new Point(0, 0);
            label13.Name = "label13";
            label13.Size = new Size(1264, 60);
            label13.TabIndex = 0;
            label13.Text = "RECEPCION DE MATERIA PRIMA";
            label13.TextAlign = ContentAlignment.MiddleCenter;
            //
            // panelContador
            //
            panelContador.BackColor = Color.FromArgb(100, 170, 80);
            panelContador.Controls.Add(label_counter_rows);
            panelContador.Dock = DockStyle.Top;
            panelContador.Location = new Point(0, 95);
            panelContador.Name = "panelContador";
            panelContador.Size = new Size(1264, 32);
            panelContador.TabIndex = 2;
            //
            // label_counter_rows
            //
            label_counter_rows.BackColor = Color.FromArgb(100, 170, 80);
            label_counter_rows.Dock = DockStyle.Fill;
            label_counter_rows.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label_counter_rows.ForeColor = Color.White;
            label_counter_rows.Location = new Point(0, 0);
            label_counter_rows.Name = "label_counter_rows";
            label_counter_rows.Padding = new Padding(12, 4, 0, 0);
            label_counter_rows.Size = new Size(1264, 32);
            label_counter_rows.TabIndex = 0;
            label_counter_rows.Text = "Registros:";
            label_counter_rows.TextAlign = ContentAlignment.MiddleLeft;
            //
            // panelDatos
            //
            panelDatos.BackColor = Color.FromArgb(240, 250, 230);
            panelDatos.Controls.Add(btn_ProvBuscar);
            panelDatos.Controls.Add(txt_nombre_prov);
            panelDatos.Controls.Add(txt_prov_Id);
            panelDatos.Controls.Add(label3);
            panelDatos.Controls.Add(btn_RecepBuscar);
            panelDatos.Controls.Add(txt_person_name);
            panelDatos.Controls.Add(txt_person_id);
            panelDatos.Controls.Add(label6);
            panelDatos.Controls.Add(txt_embarque);
            panelDatos.Controls.Add(label11);
            panelDatos.Controls.Add(txt_lote);
            panelDatos.Controls.Add(label10);
            panelDatos.Controls.Add(txt_guia);
            panelDatos.Controls.Add(label7);
            panelDatos.Controls.Add(btn_TransportBuscar);
            panelDatos.Controls.Add(txt_transport_name);
            panelDatos.Controls.Add(txt_transport_id);
            panelDatos.Controls.Add(label4);
            panelDatos.Controls.Add(txt_fecha_produccion);
            panelDatos.Controls.Add(label12);
            panelDatos.Controls.Add(txt_fecha_recepcion);
            panelDatos.Controls.Add(label2);
            panelDatos.Controls.Add(btn_OrdenBuscar);
            panelDatos.Controls.Add(txt_numeroOrden);
            panelDatos.Controls.Add(label1);
            panelDatos.Controls.Add(txt_OrdenCompra);
            panelDatos.Controls.Add(label5);
            panelDatos.Dock = DockStyle.Top;
            panelDatos.Location = new Point(0, 127);
            panelDatos.Name = "panelDatos";
            panelDatos.Size = new Size(1264, 173);
            panelDatos.TabIndex = 3;
            //
            // label1
            //
            label1.AutoSize = false;
            label1.ForeColor = Color.FromArgb(70, 140, 25);
            label1.Location = new Point(12, 8);
            label1.Name = "label1";
            label1.Size = new Size(250, 18);
            label1.TabIndex = 1;
            label1.Text = "Numero Orden :";
            label1.TextAlign = ContentAlignment.MiddleLeft;
            //
            // txt_numeroOrden
            //
            txt_numeroOrden.Location = new Point(12, 30);
            txt_numeroOrden.Margin = new Padding(3, 4, 3, 4);
            txt_numeroOrden.Name = "txt_numeroOrden";
            txt_numeroOrden.ReadOnly = true;
            txt_numeroOrden.Size = new Size(230, 25);
            txt_numeroOrden.TabIndex = 0;
            //
            // btn_OrdenBuscar
            //
            btn_OrdenBuscar.FlatAppearance.BorderColor = Color.FromArgb(100, 170, 80);
            btn_OrdenBuscar.FlatStyle = FlatStyle.Flat;
            btn_OrdenBuscar.Location = new Point(248, 29);
            btn_OrdenBuscar.Margin = new Padding(3, 4, 3, 4);
            btn_OrdenBuscar.Name = "btn_OrdenBuscar";
            btn_OrdenBuscar.Size = new Size(44, 28);
            btn_OrdenBuscar.TabIndex = 1;
            btn_OrdenBuscar.Text = "...";
                        btn_OrdenBuscar.UseVisualStyleBackColor = true;
            btn_OrdenBuscar.Click += Btn_OrdenBuscar_Click;
            //
            // label5
            //
            label5.AutoSize = false;
            label5.ForeColor = Color.FromArgb(70, 140, 25);
            label5.Location = new Point(304, 8);
            label5.Name = "label5";
            label5.Size = new Size(250, 18);
            label5.TabIndex = 3;
            label5.Text = "Orden de Compra :";
            label5.TextAlign = ContentAlignment.MiddleLeft;
            //
            // txt_OrdenCompra
            //
            txt_OrdenCompra.Location = new Point(304, 30);
            txt_OrdenCompra.Margin = new Padding(3, 4, 3, 4);
            txt_OrdenCompra.Name = "txt_OrdenCompra";
            txt_OrdenCompra.ReadOnly = true;
            txt_OrdenCompra.Size = new Size(230, 25);
            txt_OrdenCompra.TabIndex = 2;
            //
            // label3
            //
            label3.AutoSize = false;
            label3.ForeColor = Color.FromArgb(70, 140, 25);
            label3.Location = new Point(566, 8);
            label3.Name = "label3";
            label3.Size = new Size(625, 18);
            label3.TabIndex = 5;
            label3.Text = "Datos del Proveedor :";
            label3.TextAlign = ContentAlignment.MiddleLeft;
            //
            // txt_prov_Id
            //
            txt_prov_Id.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            txt_prov_Id.Enabled = false;
            txt_prov_Id.Location = new Point(566, 30);
            txt_prov_Id.Margin = new Padding(3, 4, 3, 4);
            txt_prov_Id.Name = "txt_prov_Id";
            txt_prov_Id.ReadOnly = true;
            txt_prov_Id.Size = new Size(100, 25);
            txt_prov_Id.TabIndex = 6;
            //
            // txt_nombre_prov
            //
            txt_nombre_prov.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            txt_nombre_prov.Location = new Point(672, 30);
            txt_nombre_prov.Margin = new Padding(3, 4, 3, 4);
            txt_nombre_prov.Name = "txt_nombre_prov";
            txt_nombre_prov.ReadOnly = true;
            txt_nombre_prov.Size = new Size(519, 25);
            txt_nombre_prov.TabIndex = 7;
            //
            // btn_ProvBuscar
            //
            btn_ProvBuscar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn_ProvBuscar.Enabled = false;
            btn_ProvBuscar.FlatAppearance.BorderColor = Color.FromArgb(100, 170, 80);
            btn_ProvBuscar.FlatStyle = FlatStyle.Flat;
            btn_ProvBuscar.Location = new Point(1197, 29);
            btn_ProvBuscar.Margin = new Padding(3, 4, 3, 4);
            btn_ProvBuscar.Name = "btn_ProvBuscar";
            btn_ProvBuscar.Size = new Size(44, 28);
            btn_ProvBuscar.TabIndex = 8;
            btn_ProvBuscar.Text = "...";
                        btn_ProvBuscar.UseVisualStyleBackColor = true;
            btn_ProvBuscar.Click += Btn_ProvBuscar_Click;
            //
            // label2
            //
            label2.AutoSize = false;
            label2.ForeColor = Color.FromArgb(70, 140, 25);
            label2.Location = new Point(12, 62);
            label2.Name = "label2";
            label2.Size = new Size(250, 18);
            label2.TabIndex = 10;
            label2.Text = "Fecha Recepcion :";
            label2.TextAlign = ContentAlignment.MiddleLeft;
            //
            // txt_fecha_recepcion
            //
            txt_fecha_recepcion.Enabled = false;
            txt_fecha_recepcion.Location = new Point(12, 84);
            txt_fecha_recepcion.Margin = new Padding(3, 4, 3, 4);
            txt_fecha_recepcion.Name = "txt_fecha_recepcion";
            txt_fecha_recepcion.Size = new Size(230, 25);
            txt_fecha_recepcion.TabIndex = 9;
            //
            // label4
            //
            label4.AutoSize = false;
            label4.ForeColor = Color.FromArgb(70, 140, 25);
            label4.Location = new Point(566, 62);
            label4.Name = "label4";
            label4.Size = new Size(625, 18);
            label4.TabIndex = 12;
            label4.Text = "Transportista :";
            label4.TextAlign = ContentAlignment.MiddleLeft;
            //
            // txt_transport_id
            //
            txt_transport_id.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            txt_transport_id.Enabled = false;
            txt_transport_id.Location = new Point(566, 84);
            txt_transport_id.Margin = new Padding(3, 4, 3, 4);
            txt_transport_id.Name = "txt_transport_id";
            txt_transport_id.ReadOnly = true;
            txt_transport_id.Size = new Size(100, 25);
            txt_transport_id.TabIndex = 13;
            //
            // txt_transport_name
            //
            txt_transport_name.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            txt_transport_name.Location = new Point(672, 84);
            txt_transport_name.Margin = new Padding(3, 4, 3, 4);
            txt_transport_name.Name = "txt_transport_name";
            txt_transport_name.ReadOnly = true;
            txt_transport_name.Size = new Size(519, 25);
            txt_transport_name.TabIndex = 14;
            //
            // btn_TransportBuscar
            //
            btn_TransportBuscar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn_TransportBuscar.Enabled = false;
            btn_TransportBuscar.FlatAppearance.BorderColor = Color.FromArgb(100, 170, 80);
            btn_TransportBuscar.FlatStyle = FlatStyle.Flat;
            btn_TransportBuscar.Location = new Point(1197, 83);
            btn_TransportBuscar.Margin = new Padding(3, 4, 3, 4);
            btn_TransportBuscar.Name = "btn_TransportBuscar";
            btn_TransportBuscar.Size = new Size(44, 28);
            btn_TransportBuscar.TabIndex = 15;
            btn_TransportBuscar.Text = "...";
                        btn_TransportBuscar.UseVisualStyleBackColor = true;
            btn_TransportBuscar.Click += Btn_TransportBuscar_Click;
            //
            // label12
            //
            label12.AutoSize = false;
            label12.ForeColor = Color.FromArgb(70, 140, 25);
            label12.Location = new Point(304, 62);
            label12.Name = "label12";
            label12.Size = new Size(250, 18);
            label12.TabIndex = 17;
            label12.Text = "Fecha Produccion :";
            label12.TextAlign = ContentAlignment.MiddleLeft;
            //
            // txt_fecha_produccion
            //
            txt_fecha_produccion.Enabled = false;
            txt_fecha_produccion.Location = new Point(304, 84);
            txt_fecha_produccion.Margin = new Padding(3, 4, 3, 4);
            txt_fecha_produccion.Name = "txt_fecha_produccion";
            txt_fecha_produccion.Size = new Size(230, 25);
            txt_fecha_produccion.TabIndex = 16;
            //
            // label6
            //
            label6.AutoSize = false;
            label6.ForeColor = Color.FromArgb(70, 140, 25);
            label6.Location = new Point(566, 116);
            label6.Name = "label6";
            label6.Size = new Size(625, 18);
            label6.TabIndex = 19;
            label6.Text = "Persona Recepcionista :";
            label6.TextAlign = ContentAlignment.MiddleLeft;
            //
            // txt_person_id
            //
            txt_person_id.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            txt_person_id.Enabled = false;
            txt_person_id.Location = new Point(566, 138);
            txt_person_id.Margin = new Padding(3, 4, 3, 4);
            txt_person_id.Name = "txt_person_id";
            txt_person_id.ReadOnly = true;
            txt_person_id.Size = new Size(100, 25);
            txt_person_id.TabIndex = 20;
            //
            // txt_person_name
            //
            txt_person_name.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            txt_person_name.Location = new Point(672, 138);
            txt_person_name.Margin = new Padding(3, 4, 3, 4);
            txt_person_name.Name = "txt_person_name";
            txt_person_name.ReadOnly = true;
            txt_person_name.Size = new Size(519, 25);
            txt_person_name.TabIndex = 21;
            //
            // btn_RecepBuscar
            //
            btn_RecepBuscar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn_RecepBuscar.Enabled = false;
            btn_RecepBuscar.FlatAppearance.BorderColor = Color.FromArgb(100, 170, 80);
            btn_RecepBuscar.FlatStyle = FlatStyle.Flat;
            btn_RecepBuscar.Location = new Point(1197, 137);
            btn_RecepBuscar.Margin = new Padding(3, 4, 3, 4);
            btn_RecepBuscar.Name = "btn_RecepBuscar";
            btn_RecepBuscar.Size = new Size(44, 28);
            btn_RecepBuscar.TabIndex = 22;
            btn_RecepBuscar.Text = "...";
                        btn_RecepBuscar.UseVisualStyleBackColor = true;
            btn_RecepBuscar.Click += Btn_RecepBuscar_Click;
            //
            // label7
            //
            label7.AutoSize = false;
            label7.ForeColor = Color.FromArgb(70, 140, 25);
            label7.Location = new Point(12, 116);
            label7.Name = "label7";
            label7.Size = new Size(136, 18);
            label7.TabIndex = 24;
            label7.Text = "Guia de Importacion :";
            label7.TextAlign = ContentAlignment.MiddleLeft;
            //
            // txt_guia
            //
            txt_guia.Location = new Point(12, 138);
            txt_guia.Margin = new Padding(3, 4, 3, 4);
            txt_guia.Name = "txt_guia";
            txt_guia.ReadOnly = true;
            txt_guia.Size = new Size(136, 25);
            txt_guia.TabIndex = 23;
            //
            // label10
            //
            label10.AutoSize = false;
            label10.ForeColor = Color.FromArgb(70, 140, 25);
            label10.Location = new Point(160, 116);
            label10.Name = "label10";
            label10.Size = new Size(136, 18);
            label10.TabIndex = 26;
            label10.Text = "Numero Lote :";
            label10.TextAlign = ContentAlignment.MiddleLeft;
            //
            // txt_lote
            //
            txt_lote.Location = new Point(160, 138);
            txt_lote.Margin = new Padding(3, 4, 3, 4);
            txt_lote.Name = "txt_lote";
            txt_lote.ReadOnly = true;
            txt_lote.Size = new Size(136, 25);
            txt_lote.TabIndex = 25;
            //
            // label11
            //
            label11.AutoSize = false;
            label11.ForeColor = Color.FromArgb(70, 140, 25);
            label11.Location = new Point(304, 116);
            label11.Name = "label11";
            label11.Size = new Size(250, 18);
            label11.TabIndex = 28;
            label11.Text = "Numero Embarque :";
            label11.TextAlign = ContentAlignment.MiddleLeft;
            //
            // txt_embarque
            //
            txt_embarque.Location = new Point(304, 138);
            txt_embarque.Name = "txt_embarque";
            txt_embarque.ReadOnly = true;
            txt_embarque.Size = new Size(230, 25);
            txt_embarque.TabIndex = 27;
            //
            // panelDetalle
            //
            panelDetalle.BackColor = Color.White;
            panelDetalle.Controls.Add(btn_LoadRows);
            panelDetalle.Controls.Add(btn_template);
            panelDetalle.Controls.Add(btn_deleteRows);
            panelDetalle.Controls.Add(btn_addRows);
            panelDetalle.Controls.Add(GridItems);
            panelDetalle.Dock = DockStyle.Fill;
            panelDetalle.Location = new Point(0, 333);
            panelDetalle.Name = "panelDetalle";
            panelDetalle.Size = new Size(1264, 292);
            panelDetalle.TabIndex = 4;
            //
            // GridItems
            //
            GridItems.AllowUserToAddRows = false;
            GridItems.AllowUserToDeleteRows = false;
            GridItems.AllowUserToResizeRows = false;
            GridItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            GridItems.BackgroundColor = Color.White;
            GridItems.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            GridItems.Dock = DockStyle.Fill;
            GridItems.Location = new Point(0, 0);
            GridItems.MultiSelect = false;
            GridItems.Name = "GridItems";
            GridItems.ReadOnly = true;
            GridItems.RowHeadersVisible = false;
            GridItems.Size = new Size(1264, 292);
            GridItems.TabIndex = 0;
            //
            // btn_addRows
            //
            btn_addRows.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn_addRows.Enabled = false;
            btn_addRows.FlatAppearance.BorderColor = Color.FromArgb(100, 170, 80);
            btn_addRows.FlatStyle = FlatStyle.Flat;
            btn_addRows.ForeColor = Color.FromArgb(70, 140, 25);
            btn_addRows.Image = (Image)resources.GetObject("btn_addRows.Image");
            btn_addRows.Location = new Point(1212, 8);
            btn_addRows.Margin = new Padding(3, 4, 3, 4);
            btn_addRows.Name = "btn_addRows";
            btn_addRows.Size = new Size(44, 44);
            btn_addRows.TabIndex = 1;
            btn_addRows.Text = "+";
                        btn_addRows.UseVisualStyleBackColor = true;
            btn_addRows.Click += Btn_addRows_Click;
            //
            // btn_deleteRows
            //
            btn_deleteRows.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn_deleteRows.Enabled = false;
            btn_deleteRows.FlatAppearance.BorderColor = Color.FromArgb(100, 170, 80);
            btn_deleteRows.FlatStyle = FlatStyle.Flat;
            btn_deleteRows.ForeColor = Color.FromArgb(70, 140, 25);
            btn_deleteRows.Image = (Image)resources.GetObject("btn_deleteRows.Image");
            btn_deleteRows.Location = new Point(1212, 58);
            btn_deleteRows.Margin = new Padding(3, 4, 3, 4);
            btn_deleteRows.Name = "btn_deleteRows";
            btn_deleteRows.Size = new Size(44, 44);
            btn_deleteRows.TabIndex = 2;
            btn_deleteRows.Text = "-";
                        btn_deleteRows.UseVisualStyleBackColor = true;
            btn_deleteRows.Click += Btn_deleteRows_Click;
            //
            // btn_template
            //
            btn_template.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn_template.Enabled = false;
            btn_template.FlatAppearance.BorderColor = Color.FromArgb(100, 170, 80);
            btn_template.FlatStyle = FlatStyle.Flat;
            btn_template.ForeColor = Color.FromArgb(70, 140, 25);
            btn_template.Image = (Image)resources.GetObject("btn_template.Image");
            btn_template.Location = new Point(1212, 108);
            btn_template.Margin = new Padding(3, 4, 3, 4);
            btn_template.Name = "btn_template";
            btn_template.Size = new Size(44, 44);
            btn_template.TabIndex = 3;
            btn_template.Text = "?";
                        btn_template.UseVisualStyleBackColor = true;
            btn_template.Click += Btn_template_Click;
            //
            // btn_LoadRows
            //
            btn_LoadRows.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn_LoadRows.Enabled = false;
            btn_LoadRows.FlatAppearance.BorderColor = Color.FromArgb(100, 170, 80);
            btn_LoadRows.FlatStyle = FlatStyle.Flat;
            btn_LoadRows.ForeColor = Color.FromArgb(70, 140, 25);
            btn_LoadRows.Image = (Image)resources.GetObject("btn_LoadRows.Image");
            btn_LoadRows.Location = new Point(1212, 158);
            btn_LoadRows.Margin = new Padding(3, 4, 3, 4);
            btn_LoadRows.Name = "btn_LoadRows";
            btn_LoadRows.Size = new Size(44, 44);
            btn_LoadRows.TabIndex = 4;
            btn_LoadRows.Text = "XLS";
                        btn_LoadRows.UseVisualStyleBackColor = true;
            btn_LoadRows.Click += Btn_LoadRows_Click;
            //
            // panelNotas
            //
            panelNotas.BackColor = Color.FromArgb(240, 250, 230);
            panelNotas.Controls.Add(chk_DocumentClose);
            panelNotas.Controls.Add(Pic_Document);
            panelNotas.Controls.Add(chk_anulado);
            panelNotas.Controls.Add(label16);
            panelNotas.Controls.Add(txt_total_cantidad);
            panelNotas.Controls.Add(label9);
            panelNotas.Controls.Add(txt_notas);
            panelNotas.Controls.Add(label8);
            panelNotas.Dock = DockStyle.Bottom;
            panelNotas.Location = new Point(0, 625);
            panelNotas.Name = "panelNotas";
            panelNotas.Size = new Size(1264, 190);
            panelNotas.TabIndex = 5;
            //
            // label8
            //
            label8.AutoSize = false;
            label8.ForeColor = Color.FromArgb(70, 140, 25);
            label8.Location = new Point(12, 8);
            label8.Name = "label8";
            label8.Size = new Size(150, 18);
            label8.TabIndex = 0;
            label8.Text = "Notas del Documento :";
            label8.TextAlign = ContentAlignment.MiddleLeft;
            //
            // txt_notas
            //
            txt_notas.Location = new Point(12, 30);
            txt_notas.Margin = new Padding(3, 4, 3, 4);
            txt_notas.Name = "txt_notas";
            txt_notas.ReadOnly = true;
            txt_notas.Size = new Size(920, 146);
            txt_notas.TabIndex = 1;
            txt_notas.Text = "";
            //
            // label9
            //
            label9.AutoSize = false;
            label9.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label9.ForeColor = Color.FromArgb(70, 140, 25);
            label9.Location = new Point(944, 34);
            label9.Name = "label9";
            label9.Size = new Size(120, 18);
            label9.TabIndex = 2;
            label9.Text = "Total Cantidad :";
            label9.TextAlign = ContentAlignment.MiddleRight;
            //
            // txt_total_cantidad
            //
            txt_total_cantidad.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            txt_total_cantidad.Location = new Point(1070, 30);
            txt_total_cantidad.Margin = new Padding(3, 4, 3, 4);
            txt_total_cantidad.Name = "txt_total_cantidad";
            txt_total_cantidad.ReadOnly = true;
            txt_total_cantidad.Size = new Size(134, 25);
            txt_total_cantidad.TabIndex = 3;
            //
            // label16
            //
            label16.AutoSize = false;
            label16.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label16.ForeColor = Color.FromArgb(70, 140, 25);
            label16.Location = new Point(944, 68);
            label16.Name = "label16";
            label16.Size = new Size(260, 18);
            label16.TabIndex = 4;
            label16.Text = "Status Orden :";
            label16.TextAlign = ContentAlignment.MiddleLeft;
            //
            // chk_anulado
            //
            chk_anulado.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            chk_anulado.AutoSize = true;
            chk_anulado.ForeColor = Color.FromArgb(70, 140, 25);
            chk_anulado.Location = new Point(944, 92);
            chk_anulado.Margin = new Padding(3, 4, 3, 4);
            chk_anulado.Name = "chk_anulado";
            chk_anulado.Size = new Size(153, 23);
            chk_anulado.TabIndex = 5;
            chk_anulado.Text = "Documento Anulado";
            chk_anulado.Click += Chk_anulado_Click;
            chk_anulado.KeyDown += Chk_anulado_KeyDown;
            //
            // Pic_Document
            //
            Pic_Document.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Pic_Document.Image = (Image)resources.GetObject("Pic_Document.Image");
            Pic_Document.Location = new Point(1158, 62);
            Pic_Document.Name = "Pic_Document";
            Pic_Document.Size = new Size(50, 50);
            Pic_Document.SizeMode = PictureBoxSizeMode.AutoSize;
            Pic_Document.TabIndex = 6;
            Pic_Document.TabStop = false;
            //
            // chk_DocumentClose
            //
            chk_DocumentClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            chk_DocumentClose.AutoSize = true;
            chk_DocumentClose.ForeColor = Color.FromArgb(70, 140, 25);
            chk_DocumentClose.Location = new Point(944, 122);
            chk_DocumentClose.Name = "chk_DocumentClose";
            chk_DocumentClose.Size = new Size(155, 23);
            chk_DocumentClose.TabIndex = 7;
            chk_DocumentClose.Text = "Documento Cerrado";
            chk_DocumentClose.Click += Chk_DocumentClose_Click;
            chk_DocumentClose.KeyDown += Chk_DocumentClose_KeyDown;
            //
            // FrmMateriaPrima
            //
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.White;
            ClientSize = new Size(1264, 815);
            Controls.Add(panelDetalle);
            Controls.Add(panelNotas);
            Controls.Add(panelDatos);
            Controls.Add(toolStrip1);
            Controls.Add(panelContador);
            Controls.Add(panel1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(1150, 660);
            Name = "FrmMateriaPrima";
            Text = "RECEPCION MATERIA PRIMA";
            Load += FrmMateriaPrima_Load;
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            panel1.ResumeLayout(false);
            panelContador.ResumeLayout(false);
            panelDatos.ResumeLayout(false);
            panelDatos.PerformLayout();
            panelDetalle.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)GridItems).EndInit();
            panelNotas.ResumeLayout(false);
            panelNotas.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)Pic_Document).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private ToolStrip toolStrip1;
        private ToolStripButton btn_primero;
        private ToolStripButton btn_anterior;
        private ToolStripButton btn_siguiente;
        private ToolStripButton btn_ultimo;
        private ToolStripButton btn_create;
        private ToolStripButton btn_cancel;
        private ToolStripButton btn_save;
        private ToolStripButton btn_CloseDoc;
        private ToolStripButton btn_AnularDoc;
        private ToolStripButton btn_SearchDoc;
        private ToolStripButton btn_printDoc;
        private ToolStripButton btn_ExportDoc;
        private Panel panel1;
        private Label label13;
        private Panel panelContador;
        private Label label_counter_rows;
        private Panel panelDatos;
        private Label label1;
        private TextBox txt_numeroOrden;
        private Button btn_OrdenBuscar;
        private Label label5;
        private TextBox txt_OrdenCompra;
        private Label label3;
        private TextBox txt_prov_Id;
        private TextBox txt_nombre_prov;
        private Button btn_ProvBuscar;
        private Label label2;
        private DateTimePicker txt_fecha_recepcion;
        private Label label4;
        private TextBox txt_transport_id;
        private TextBox txt_transport_name;
        private Button btn_TransportBuscar;
        private Label label12;
        private DateTimePicker txt_fecha_produccion;
        private Label label6;
        private TextBox txt_person_id;
        private TextBox txt_person_name;
        private Button btn_RecepBuscar;
        private Label label7;
        private TextBox txt_guia;
        private Label label10;
        private TextBox txt_lote;
        private Label label11;
        private TextBox txt_embarque;
        private Panel panelDetalle;
        private DataGridView GridItems;
        private Button btn_addRows;
        private Button btn_deleteRows;
        private Button btn_template;
        private Button btn_LoadRows;
        private Panel panelNotas;
        private Label label8;
        private RichTextBox txt_notas;
        private Label label9;
        private TextBox txt_total_cantidad;
        private Label label16;
        private CheckBox chk_anulado;
        private PictureBox Pic_Document;
        private CheckBox chk_DocumentClose;
    }
}