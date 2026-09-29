namespace Ritrama2025.Forms.Controls
{
    partial class ProductCard
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
            if (disposing && Region != null)
            {
                Region.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            pnlContent = new Panel();
            lblIdChip = new Label();
            lblCategoriaChip = new Label();
            lblNombre = new Label();
            lblExistencia = new Label();
            pnlContent.SuspendLayout();
            SuspendLayout();
            // 
            // pnlContent
            // 
            pnlContent.BackColor = Color.FromArgb(214, 245, 214);
            pnlContent.Controls.Add(lblIdChip);
            pnlContent.Controls.Add(lblCategoriaChip);
            pnlContent.Controls.Add(lblNombre);
            pnlContent.Controls.Add(lblExistencia);
            pnlContent.Dock = DockStyle.Fill;
            pnlContent.Location = new Point(0, 0);
            pnlContent.Name = "pnlContent";
            pnlContent.Size = new Size(352, 84);
            pnlContent.TabIndex = 0;
            // 
            // lblIdChip
            // 
            lblIdChip.BackColor = Color.FromArgb(240, 240, 240);
            lblIdChip.Font = new Font("JetBrains Mono", 7F, FontStyle.Regular, GraphicsUnit.Point);
            lblIdChip.ForeColor = Color.FromArgb(80, 80, 80);
            lblIdChip.Location = new Point(8, 8);
            lblIdChip.Name = "lblIdChip";
            lblIdChip.Size = new Size(76, 18);
            lblIdChip.TabIndex = 0;
            lblIdChip.Text = "PROD-001";
            lblIdChip.TextAlign = ContentAlignment.MiddleCenter;
            lblIdChip.AutoEllipsis = true;
            // 
            // lblCategoriaChip
            // 
            lblCategoriaChip.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblCategoriaChip.BackColor = Color.FromArgb(110, 190, 40);
            lblCategoriaChip.Font = new Font("JetBrains Mono", 7F, FontStyle.Bold, GraphicsUnit.Point);
            lblCategoriaChip.ForeColor = Color.White;
            lblCategoriaChip.Location = new Point(248, 8);
            lblCategoriaChip.Name = "lblCategoriaChip";
            lblCategoriaChip.Size = new Size(96, 18);
            lblCategoriaChip.TabIndex = 1;
            lblCategoriaChip.Text = "";
            lblCategoriaChip.TextAlign = ContentAlignment.MiddleCenter;
            lblCategoriaChip.AutoEllipsis = true;
            // 
            // lblNombre
            // 
            lblNombre.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblNombre.Font = new Font("JetBrains Mono", 9F, FontStyle.Bold, GraphicsUnit.Point);
            lblNombre.ForeColor = Color.FromArgb(40, 40, 40);
            lblNombre.Location = new Point(8, 34);
            lblNombre.Name = "lblNombre";
            lblNombre.Size = new Size(336, 18);
            lblNombre.TabIndex = 2;
            lblNombre.Text = "Product Name";
            lblNombre.TextAlign = ContentAlignment.MiddleLeft;
            lblNombre.AutoEllipsis = true;
            // 
            // lblExistencia
            // 
            lblExistencia.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblExistencia.Font = new Font("JetBrains Mono", 7.5F, FontStyle.Regular, GraphicsUnit.Point);
            lblExistencia.ForeColor = Color.FromArgb(90, 120, 90);
            lblExistencia.Location = new Point(8, 58);
            lblExistencia.Name = "lblExistencia";
            lblExistencia.Size = new Size(336, 14);
            lblExistencia.TabIndex = 3;
            lblExistencia.Text = "Exist: --";
            lblExistencia.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // ProductCard
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(214, 245, 214);
            Controls.Add(pnlContent);
            Margin = new Padding(4, 4, 4, 4);
            Name = "ProductCard";
            Size = new Size(352, 84);
            pnlContent.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlContent;
        private Label lblIdChip;
        private Label lblCategoriaChip;
        private Label lblNombre;
        private Label lblExistencia;
    }
}
