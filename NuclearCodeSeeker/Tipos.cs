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
}