namespace Ritrama2025.Forms.Otros
{
    partial class Frm_ImportProductos
    {
        /// <summary>Required designer variable.</summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>Clean up any resources being used.</summary>
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
            this.lblTitulo = new Sunny.UI.UILabel();
            this.lblInstrucciones = new Sunny.UI.UILabel();
            this.txtPathFile = new Sunny.UI.UITextBox();
            this.btnBuscar = new Sunny.UI.UIButton();
            this.btnPlantilla = new Sunny.UI.UIButton();
            this.gridPreview = new System.Windows.Forms.DataGridView();
            this.lblContador = new Sunny.UI.UILabel();
            this.btnImportar = new Sunny.UI.UIButton();
            this.btnCancelar = new Sunny.UI.UIButton();
            ((System.ComponentModel.ISupportInitialize)(this.gridPreview)).BeginInit();
            this.SuspendLayout();
            //
            // lblTitulo
            //
            this.lblTitulo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTitulo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = System.Drawing.Color.FromArgb(60, 110, 20);
            this.lblTitulo.Location = new System.Drawing.Point(0, 0);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Padding = new System.Windows.Forms.Padding(8, 6, 0, 6);
            this.lblTitulo.Size = new System.Drawing.Size(884, 36);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "IMPORTAR PRODUCTOS DESDE EXCEL";
            this.lblTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblInstrucciones
            //
            // Sin DockStyle.Top: los dos rotulos anclados en el mismo borde se reparten el
            // espacio segun el orden de la coleccion y el texto acababa montado uno sobre
            // otro. Con posicion explicita el titulo siempre queda encima.
            this.lblInstrucciones.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblInstrucciones.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.lblInstrucciones.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
            this.lblInstrucciones.Location = new System.Drawing.Point(0, 36);
            this.lblInstrucciones.Name = "lblInstrucciones";
            this.lblInstrucciones.Padding = new System.Windows.Forms.Padding(8, 2, 4, 2);
            this.lblInstrucciones.Size = new System.Drawing.Size(884, 54);
            this.lblInstrucciones.TabIndex = 1;
            this.lblInstrucciones.Text = "Columnas: product_id (el código Ritrama que teclea usted), product_name y categoria.\r\n" +
                "Categorías válidas: Master, Rollo Cortado, Resma o Graphics. Pulse «Descargar plantilla» para obtenerla en blanco.\r\n" +
                "El código consecutivo (Product_ID) y el resto de valores los pone el sistema.";
            this.lblInstrucciones.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // txtPathFile
            //
            this.txtPathFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPathFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.txtPathFile.Location = new System.Drawing.Point(12, 104);
            this.txtPathFile.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtPathFile.Name = "txtPathFile";
            this.txtPathFile.Padding = new System.Windows.Forms.Padding(5);
            this.txtPathFile.ReadOnly = true;
            this.txtPathFile.ShowText = false;
            this.txtPathFile.Size = new System.Drawing.Size(770, 28);
            this.txtPathFile.TabIndex = 2;
            this.txtPathFile.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtPathFile.Watermark = "";
            //
            // btnBuscar
            //
            this.btnBuscar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBuscar.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.btnBuscar.Location = new System.Drawing.Point(788, 102);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(84, 32);
            this.btnBuscar.TabIndex = 3;
            this.btnBuscar.Text = "Buscar...";
            this.btnBuscar.Click += new System.EventHandler(this.BtnBuscar_Click);
            //
            // gridPreview
            //
            this.gridPreview.AllowUserToAddRows = false;
            this.gridPreview.AllowUserToDeleteRows = false;
            this.gridPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridPreview.BackgroundColor = System.Drawing.Color.White;
            this.gridPreview.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridPreview.Location = new System.Drawing.Point(12, 146);
            this.gridPreview.Name = "gridPreview";
            this.gridPreview.ReadOnly = true;
            this.gridPreview.RowHeadersVisible = false;
            this.gridPreview.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridPreview.Size = new System.Drawing.Size(860, 276);
            this.gridPreview.TabIndex = 4;
            //
            // lblContador
            //
            this.lblContador.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblContador.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblContador.ForeColor = System.Drawing.Color.FromArgb(60, 110, 20);
            this.lblContador.Location = new System.Drawing.Point(12, 426);
            this.lblContador.Name = "lblContador";
            this.lblContador.Size = new System.Drawing.Size(640, 24);
            this.lblContador.TabIndex = 5;
            this.lblContador.Text = "Seleccione un archivo Excel para comenzar.";
            this.lblContador.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnImportar
            //
            this.btnImportar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnImportar.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.btnImportar.Location = new System.Drawing.Point(668, 458);
            this.btnImportar.Name = "btnImportar";
            this.btnImportar.Size = new System.Drawing.Size(100, 34);
            this.btnImportar.TabIndex = 6;
            this.btnImportar.Text = "Importar";
            this.btnImportar.Click += new System.EventHandler(this.BtnImportar_Click);
            //
            // btnCancelar
            //
            this.btnCancelar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelar.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.btnCancelar.Location = new System.Drawing.Point(774, 458);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(98, 34);
            this.btnCancelar.TabIndex = 7;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.Click += new System.EventHandler(this.BtnCancelar_Click);
            //
            // btnPlantilla
            //
            this.btnPlantilla.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPlantilla.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.btnPlantilla.Location = new System.Drawing.Point(12, 458);
            this.btnPlantilla.Name = "btnPlantilla";
            this.btnPlantilla.Size = new System.Drawing.Size(180, 34);
            this.btnPlantilla.TabIndex = 8;
            this.btnPlantilla.Text = "Descargar plantilla";
            this.btnPlantilla.Click += new System.EventHandler(this.BtnPlantilla_Click);
            //
            // Frm_ImportProductos
            //
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(884, 500);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnImportar);
            this.Controls.Add(this.btnPlantilla);
            this.Controls.Add(this.lblContador);
            this.Controls.Add(this.gridPreview);
            this.Controls.Add(this.btnBuscar);
            this.Controls.Add(this.txtPathFile);
            this.Controls.Add(this.lblInstrucciones);
            this.Controls.Add(this.lblTitulo);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Frm_ImportProductos";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Importar Productos";
            this.ZoomScaleRect = new System.Drawing.Rectangle(15, 15, 884, 500);
            ((System.ComponentModel.ISupportInitialize)(this.gridPreview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Sunny.UI.UILabel lblTitulo;
        private Sunny.UI.UILabel lblInstrucciones;
        private Sunny.UI.UITextBox txtPathFile;
        private Sunny.UI.UIButton btnBuscar;
        private Sunny.UI.UIButton btnPlantilla;
        private System.Windows.Forms.DataGridView gridPreview;
        private Sunny.UI.UILabel lblContador;
        private Sunny.UI.UIButton btnImportar;
        private Sunny.UI.UIButton btnCancelar;
    }
}
