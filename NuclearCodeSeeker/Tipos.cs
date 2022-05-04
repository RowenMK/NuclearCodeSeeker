namespace NuclearCodeSeeker
{
    public class Cola
    {
        public string URL { get; set; }
        public string Nombre { get; set; }
        public string Tags { get; set; }
        public int Cantidad_Pags { get; set; }
        public string Sitio { get; set; }
        public string Archivo { get; set; }
        public bool Done { get; set; }
    }

    public class ThumbIndex
    {
        public string ComicPath { get; set; }
        public string ThumbPath { get; set; }
    }

    public class NoCargado
    {
        public string Archivo { get; set; }
        public string Error { get; set; }

        public NoCargado(string archivo, string error)
        {
            Archivo = archivo;
            Error = error;
        }
    }

    public class Sizes
    {
        public int height { get; set; }
        public int width { get; set; }
        public decimal ratio { get; set; }

        public Sizes(int Height, int Width)
        {
            height = Height;
            width = Width;

            ratio = (decimal)Height / (decimal)Width;

            if (height < 500)
            {
                height = 500;
                width = (int)(height / ratio);
            }
        }

        public Sizes(int Height, int Width, int ParentHeight)
        {
            height = Height;
            width = Width;

            ratio = (decimal)Height / (decimal)Width;

            if (height < ParentHeight)
            {
                height = ParentHeight;
                width = (int)(height / ratio);
            }
        }

        public void Shrink(int MaxHeight)
        {
            if (height > MaxHeight)
            {
                height = height / 2;
                width = width / 2;
            }

            if (height > MaxHeight)
            {
                Shrink(MaxHeight);
            }
        }
    }
}