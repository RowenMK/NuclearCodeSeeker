namespace NuclearCodeSeeker
{
    partial class ComicPreview
    {
        /// <summary> 
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de componentes

        /// <summary> 
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lblNombre = new System.Windows.Forms.Label();
            this.pboxMain = new System.Windows.Forms.PictureBox();
            this.cmsComicPreview = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.btnAgregarAFavoritos = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.pboxMain)).BeginInit();
            this.cmsComicPreview.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblNombre
            // 
            this.lblNombre.BackColor = System.Drawing.Color.Transparent;
            this.lblNombre.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblNombre.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.lblNombre.ForeColor = System.Drawing.Color.Silver;
            this.lblNombre.Location = new System.Drawing.Point(0, 105);
            this.lblNombre.Name = "lblNombre";
            this.lblNombre.Padding = new System.Windows.Forms.Padding(2);
            this.lblNombre.Size = new System.Drawing.Size(275, 120);
            this.lblNombre.TabIndex = 0;
            this.lblNombre.Text = "<Nombre>";
            this.lblNombre.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.lblNombre.DoubleClick += new System.EventHandler(this.lblNombre_DoubleClick);
            this.lblNombre.MouseEnter += new System.EventHandler(this.lblNombre_MouseEnter);
            this.lblNombre.MouseLeave += new System.EventHandler(this.lblNombre_MouseLeave);
            // 
            // pboxMain
            // 
            this.pboxMain.ContextMenuStrip = this.cmsComicPreview;
            this.pboxMain.Location = new System.Drawing.Point(0, 0);
            this.pboxMain.Name = "pboxMain";
            this.pboxMain.Size = new System.Drawing.Size(186, 129);
            this.pboxMain.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pboxMain.TabIndex = 1;
            this.pboxMain.TabStop = false;
            this.pboxMain.DoubleClick += new System.EventHandler(this.lblNombre_DoubleClick);
            this.pboxMain.MouseEnter += new System.EventHandler(this.lblNombre_MouseEnter);
            this.pboxMain.MouseLeave += new System.EventHandler(this.lblNombre_MouseLeave);
            // 
            // cmsComicPreview
            // 
            this.cmsComicPreview.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAgregarAFavoritos});
            this.cmsComicPreview.Name = "cmsComicPreview";
            this.cmsComicPreview.Size = new System.Drawing.Size(205, 50);
            // 
            // btnAgregarAFavoritos
            // 
            this.btnAgregarAFavoritos.Font = new System.Drawing.Font("Segoe UI", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point);
            this.btnAgregarAFavoritos.Name = "btnAgregarAFavoritos";
            this.btnAgregarAFavoritos.Size = new System.Drawing.Size(204, 24);
            this.btnAgregarAFavoritos.Text = "Agregar a Favoritos";
            this.btnAgregarAFavoritos.Click += new System.EventHandler(this.btnAgregarAFavoritos_Click);
            // 
            // ComicPreview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Controls.Add(this.pboxMain);
            this.Controls.Add(this.lblNombre);
            this.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Name = "ComicPreview";
            this.Size = new System.Drawing.Size(275, 225);
            this.Load += new System.EventHandler(this.ComicPreview_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pboxMain)).EndInit();
            this.cmsComicPreview.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Label lblNombre;
        public System.Windows.Forms.PictureBox pboxMain;
        private System.Windows.Forms.ContextMenuStrip cmsComicPreview;
        private System.Windows.Forms.ToolStripMenuItem btnAgregarAFavoritos;
    }
}
