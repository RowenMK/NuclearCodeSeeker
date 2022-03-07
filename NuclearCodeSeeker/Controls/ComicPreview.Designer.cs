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
            this.lblNombre = new System.Windows.Forms.Label();
            this.pboxMain = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pboxMain)).BeginInit();
            this.SuspendLayout();
            // 
            // lblNombre
            // 
            this.lblNombre.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblNombre.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.lblNombre.ForeColor = System.Drawing.Color.Black;
            this.lblNombre.Location = new System.Drawing.Point(150, 0);
            this.lblNombre.Name = "lblNombre";
            this.lblNombre.Padding = new System.Windows.Forms.Padding(2);
            this.lblNombre.Size = new System.Drawing.Size(125, 225);
            this.lblNombre.TabIndex = 0;
            this.lblNombre.Text = "<Nombre>";
            this.lblNombre.DoubleClick += new System.EventHandler(this.lblNombre_DoubleClick);
            this.lblNombre.MouseEnter += new System.EventHandler(this.lblNombre_MouseEnter);
            this.lblNombre.MouseLeave += new System.EventHandler(this.lblNombre_MouseLeave);
            // 
            // pboxMain
            // 
            this.pboxMain.Dock = System.Windows.Forms.DockStyle.Left;
            this.pboxMain.Location = new System.Drawing.Point(0, 0);
            this.pboxMain.Name = "pboxMain";
            this.pboxMain.Size = new System.Drawing.Size(150, 225);
            this.pboxMain.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pboxMain.TabIndex = 1;
            this.pboxMain.TabStop = false;
            this.pboxMain.DoubleClick += new System.EventHandler(this.lblNombre_DoubleClick);
            this.pboxMain.MouseEnter += new System.EventHandler(this.lblNombre_MouseEnter);
            this.pboxMain.MouseLeave += new System.EventHandler(this.lblNombre_MouseLeave);
            // 
            // ComicPreview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Controls.Add(this.lblNombre);
            this.Controls.Add(this.pboxMain);
            this.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Name = "ComicPreview";
            this.Size = new System.Drawing.Size(275, 225);
            ((System.ComponentModel.ISupportInitialize)(this.pboxMain)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Label lblNombre;
        public System.Windows.Forms.PictureBox pboxMain;
    }
}
