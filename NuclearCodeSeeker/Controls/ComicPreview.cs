using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace NuclearCodeSeeker
{
    public partial class ComicPreview : UserControl
    {
        private Utils utils = new Utils();

        public ComicPreview()
        {
            InitializeComponent();
            lblNombre.BackColor = Color.FromArgb(100, Color.Black);
        }

        private void lblNombre_DoubleClick(object sender, EventArgs e)
        {
            var p = new Process();
            p.StartInfo = new ProcessStartInfo(this.Tag.ToString()) { UseShellExecute = true };
            p.Start();
        }

        private void lblNombre_MouseEnter(object sender, EventArgs e)
        {
            lblNombre.BackColor = Color.FromArgb(100, Color.Black);
            lblNombre.ForeColor = Color.White;
            //
            lblNombre.Font = new Font("Segoe UI", 13, FontStyle.Bold, GraphicsUnit.Pixel);
        }

        private void lblNombre_MouseLeave(object sender, EventArgs e)
        {
            //lblNombre.BackColor = Color.Transparent;
            lblNombre.BackColor = Color.FromArgb(100, Color.Black);
            lblNombre.ForeColor = Color.Black;
            //
            lblNombre.Font = new Font("Segoe UI", 13, FontStyle.Bold, GraphicsUnit.Pixel);
        }

        public void CDispose()
        {
            pboxMain.Dispose();
            lblNombre.Dispose();
            this.Dispose(true);
        }
    }
}