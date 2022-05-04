using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;

namespace NuclearCodeSeeker
{
    public partial class ComicPreview : UserControl
    {
        public bool Favorito = false;
        private Main Parent;

        PictureBox pFavIcon = new PictureBox()
        {
            Image = Properties.Resources.favStar,
            Width = 20,
            Height = 20,
            SizeMode = PictureBoxSizeMode.Zoom,
            Location = new Point(5, 5),
            BackColor = Color.Transparent
        };
        private void ComicPreview_Load(object sender, EventArgs e)
        { }

        public ComicPreview(Main pOwner)
        {
            InitializeComponent();
            lblNombre.BackColor = Color.FromArgb(100, Color.Black);
            Parent = pOwner;

            
        }

        public ComicPreview(bool pEsFavorito)
        {
            InitializeComponent();
            lblNombre.BackColor = Color.FromArgb(100, Color.Black);

            setAsFavorite(pEsFavorito);
        }

        public void checkFavorite()
        {
            setAsFavorite(Parent.vListaFavoritos.FirstOrDefault(a => a == lblNombre.Text) != null);
        }

        public void setAsFavorite(bool pSetFavorite)
        {
            Favorito = pSetFavorite;
            btnAgregarAFavoritos.Text = Favorito ? "Remover de Favoritos" : "Agregar a Favoritos";

            if (pSetFavorite)
            {
                pboxMain.Controls.Add(pFavIcon);
            }
            else
            {
                pboxMain.Controls.Remove(pFavIcon);
            }
        }

        private void lblNombre_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var p = new Process();
                p.StartInfo = new ProcessStartInfo(this.Tag.ToString()) { UseShellExecute = true };
                p.Start();
            }
            catch (Exception)
            { }
        }

        private void lblNombre_MouseEnter(object sender, EventArgs e)
        {
            lblNombre.BackColor = Color.FromArgb(100, Color.Black);
            lblNombre.ForeColor = Color.White;
            //
            lblNombre.Font = new Font("Segoe UI", 12, FontStyle.Bold, GraphicsUnit.Pixel);
        }

        private void lblNombre_MouseLeave(object sender, EventArgs e)
        {
            //lblNombre.BackColor = Color.Transparent;
            lblNombre.BackColor = Color.FromArgb(100, Color.Black);
            lblNombre.ForeColor = Color.Silver;
            //
            lblNombre.Font = new Font("Segoe UI", 12, FontStyle.Bold, GraphicsUnit.Pixel);
        }

        public void CDispose()
        {
            pboxMain.Dispose();
            lblNombre.Dispose();
            this.Dispose(true);
        }

        private void btnAgregarAFavoritos_Click(object sender, EventArgs e)
        {
            setAsFavorite(!Favorito);

            if (Favorito)
            {
                Parent.vListaFavoritos.Add(lblNombre.Text);
            }
            else
            {
                Parent.vListaFavoritos.Remove(Parent.vListaFavoritos.FirstOrDefault(d => d == lblNombre.Text));
            }

            Parent.guardarListaFavoritos();
        }
    }
}