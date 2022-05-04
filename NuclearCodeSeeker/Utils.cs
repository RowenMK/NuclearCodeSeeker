using HeapStack.Core.CBR;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace NuclearCodeSeeker
{
    public class Utils
    {
        Sizes PreviewSize;

        public string replaceInvalid(string pCheckString)
        {
            try
            {
                /*
                string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());

                foreach (char c in invalid)
                {
                    pCheckString = pCheckString.Replace(c.ToString(), "");
                }

                return pCheckString;
                */

                string vReturnStuff = string.Join("-", WebUtility.HtmlDecode(pCheckString).Split(Path.GetInvalidFileNameChars()));

                return vReturnStuff;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetPageHtml(string link, WebProxy proxy = null)
        {
            using (WebClient client = new WebClient() { Encoding = Encoding.UTF8 })
            {
                //client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                client.Headers["User-Agent"] = "MOZILLA/5.0 (WINDOWS NT 6.1; WOW64) APPLEWEBKIT/537.1 (KHTML, LIKE GECKO) CHROME/21.0.1180.75 SAFARI/537.1";
                if (proxy != null)
                {
                    client.Proxy = proxy;
                }

                try
                {
                    return client.DownloadString(link);
                }
                catch (Exception ex)
                {
                    using (var myWebClient = new WebClient())
                    {
                        myWebClient.Headers["User-Agent"] = "MOZILLA/5.0 (WINDOWS NT 6.1; WOW64) APPLEWEBKIT/537.1 (KHTML, LIKE GECKO) CHROME/21.0.1180.75 SAFARI/537.1";

                        string page = myWebClient.DownloadString(link);

                        HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                        doc.LoadHtml(page);

                        return doc.Text;
                    }
                }
            }
        }

        public string checkShrinkFileName(string pFileName)
        {
            if (pFileName.Length > 200 && pFileName.Contains("("))
            {
                pFileName = Regex.Replace(pFileName, @"\([^)]*\)", "");
                pFileName = Regex.Replace(pFileName, @"\s{2,}", " ").Trim();
            }

            if (pFileName.Length > 200 && pFileName.Contains("["))
            {
                pFileName = Regex.Replace(pFileName, @"\[[^\]]*\]", "");
                pFileName = Regex.Replace(pFileName, @"\s{2,}", " ").Trim();
            }

            if (pFileName.Length > 200)
            {
                pFileName = pFileName.Substring(0, 200);
            }

            return pFileName;
        } 

        public bool validaUrlWeb(string pUrl)
        {
            try
            {
                Uri uriResult;
                return Uri.TryCreate(pUrl, UriKind.Absolute, out uriResult)
                    && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string readUserData(string pValName)
        {
            try
            {
                using (StreamReader vFileReader = new StreamReader("userdata.txt"))
                {
                    string vLine;
                    //
                    while ((vLine = vFileReader.ReadLine()) != null)
                    {
                        if (vLine.Split("$")[0].Trim().Equals(pValName))
                            return vLine.Split("$")[1].Trim();
                    }

                    return string.Empty;
                }
            }
            catch (FileNotFoundException)
            {
                return string.Empty;
            }
            catch (Exception ex2)
            {
                throw ex2;
            }
        }

        public void OpenURL(string pURL)
        {
            try
            {
                pURL = string.Concat(@"", pURL);

                if (validaUrlWeb(pURL))
                {
                    Process.Start(pURL);
                }
                else
                {
                    if (Directory.Exists(pURL) || File.Exists(pURL))
                    {
                        /*
                        ProcessStartInfo startInfo = new ProcessStartInfo();

                        startInfo.FileName = "explorer.exe";
                        startInfo.Arguments = "\"" + pURL + "\"";
                        startInfo.UseShellExecute = true;

                        Process.Start(startInfo);
                        */

                        var p = new Process();
                        p.StartInfo = new ProcessStartInfo(pURL) { UseShellExecute = true };
                        p.Start();
                    }
                    //Process.Start("explorer.exe", pURL);
                }
            }
            catch (Exception ex)
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    pURL = pURL.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {pURL}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", pURL);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", pURL);
                }
                else
                {
                    throw ex;
                }
            }
        }

        public List<Cola> SortData(List<Cola> list, string column, bool ascending)
        {
            return ascending ?
                list.OrderBy(_ => _.GetType().GetProperty(column).GetValue(_)).ToList() :
                list.OrderByDescending(_ => _.GetType().GetProperty(column).GetValue(_)).ToList();
        }

        public void ajustarColumnasDGV(DataGridView pDGV)
        {
            for (int i = 0; i <= pDGV.Columns.Count - 1; i++)
            {
                pDGV.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                int colw = pDGV.Columns[i].Width;
                pDGV.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                pDGV.Columns[i].Width = colw;
            }

            foreach (DataGridViewColumn column in pDGV.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.Automatic;
            }
        }

        public void addQueue_nHentai(int pCode, string pSite, List<Cola> pDercargas)
        {
            try
            {
                Cola vColaReg = new Cola() { Sitio = pSite, Done = false, Archivo = string.Empty };

                HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                HtmlAgilityPack.HtmlDocument tmpDoc = new HtmlAgilityPack.HtmlDocument();
                //
                vColaReg.URL = string.Format(@"https://nhentai.net/g/{0}", pCode);

                if (pDercargas.FirstOrDefault(d => d.URL == vColaReg.URL) != null)
                    return;
                //
                var content = GetPageHtml(vColaReg.URL);//new WebClient().DownloadString(vColaReg.URL);
                //
                htmlDoc.LoadHtml(content);
                //
                vColaReg.Nombre = checkShrinkFileName(WebUtility.HtmlDecode(htmlDoc.DocumentNode.SelectNodes("//div[contains(@id, 'info')]//h1")[0].InnerText));
                //
                foreach (HtmlNode node in htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'tag-container field-name')]"))
                {
                    tmpDoc.LoadHtml(node.InnerHtml);

                    if (string.Concat(tmpDoc.DocumentNode.InnerText.Split('\n')[1].Trim()).Equals("Tags:"))
                    {
                        try
                        {
                            foreach (HtmlNode innerNode in tmpDoc.DocumentNode.SelectNodes("//span[contains(@class, 'name')]"))
                            {
                                if (!innerNode.InnerText.Trim().Length.Equals(0))
                                {
                                    vColaReg.Tags += string.Concat(innerNode.InnerText.Trim(), ", ");
                                }
                            }

                            if (vColaReg.Tags.Length > 1)
                            {
                                vColaReg.Tags = vColaReg.Tags.Substring(0, vColaReg.Tags.Length - 2);
                            }
                            else
                            {
                                vColaReg.Tags = "no info";
                            }
                        }
                        catch { }
                    }
                    else if (string.Concat(tmpDoc.DocumentNode.InnerText.Split('\n')[1].Trim()).Equals("Pages:"))
                    {
                        foreach (HtmlNode innerNode in tmpDoc.DocumentNode.SelectNodes("//span[contains(@class, 'name')]"))
                        {
                            if (!innerNode.InnerText.Trim().Length.Equals(0))
                            {
                                vColaReg.Cantidad_Pags = int.Parse(innerNode.InnerText.Trim());
                            }
                        }
                    }
                }

                pDercargas.Add(vColaReg);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void writeUserData(string pValName, string pValData)
        {
            try
            {
                List<string> vLines = new List<string>();

                if (File.Exists("userdata.txt"))
                    vLines = File.ReadAllLines("userdata.txt").ToList();

                int idx = vLines.IndexOf(vLines.Find(d => d.StartsWith(pValName)));

                if (idx == -1)
                    vLines.Add(string.Concat(pValName, "$", pValData));
                else
                    vLines[idx] = string.Concat(pValName, "$", pValData);

                using (StreamWriter vFileWriter = new StreamWriter("userdata.txt"))
                {
                    foreach (string line in vLines)
                    {
                        vFileWriter.WriteLine(string.Concat(line));
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void addQueue_eHentai(string vUrl_eHentai, string pSite, List<Cola> pDercargas)
        {
            try
            {
                Cola vColaReg = new Cola() { URL = vUrl_eHentai, Sitio = pSite, Done = false, Archivo = string.Empty };
                //
                if (pDercargas.FirstOrDefault(d => d.URL == vColaReg.URL) != null)
                    return;

                HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                HtmlAgilityPack.HtmlDocument tmpDoc = new HtmlAgilityPack.HtmlDocument();
                var content = GetPageHtml(vColaReg.URL);//new WebClient().DownloadString(vColaReg.URL);
                //
                htmlDoc.LoadHtml(content);
                //
                vColaReg.Nombre = checkShrinkFileName(WebUtility.HtmlDecode(htmlDoc.GetElementbyId("gn").InnerText));

                vColaReg.Cantidad_Pags = int.Parse(htmlDoc.DocumentNode.SelectNodes("//td[contains(@class, 'gdt2')]")[5].InnerText.Split(' ')[0]);
                //
                foreach (HtmlNode node in htmlDoc.DocumentNode.SelectNodes("//div[contains(@id, 'taglist')]//table//tr"))
                {
                    tmpDoc.LoadHtml(node.InnerHtml);

                    if (tmpDoc.DocumentNode.SelectSingleNode("//td[contains(@class, 'tc')]").InnerText.Equals("female:"))
                    {
                        try
                        {
                            foreach (HtmlNode innerNode in tmpDoc.DocumentNode.SelectNodes("//div"))
                            {
                                if (!innerNode.InnerText.Trim().Length.Equals(0))
                                {
                                    vColaReg.Tags += string.Concat(innerNode.InnerText.Trim(), ", ");
                                }
                            }

                            if (vColaReg.Tags.Length > 1)
                            {
                                vColaReg.Tags = vColaReg.Tags.Substring(0, vColaReg.Tags.Length - 2);
                            }
                            else
                            {
                                vColaReg.Tags = "no info";
                            }
                        }
                        catch { }
                    }
                }

                pDercargas.Add(vColaReg);
            }
            catch (Exception)
            {
                return;
            }
        }

        public string get177013()
        {
            var choices = new[] { "No entre ahi soldado!!",
                                  "No se haga esto",
                                  "Por amor a Saki, salga de ahi!!",
                                  "Hay un lugar en el infierno para los que ahorcan el ganso con esto...",
                                  "Sea un puerco decente",
                                  "No vale la pena soldado",
                                  "Escape ahora, aun no es tarde..."
            };

            return choices[new Random().Next(0, choices.Length)];
        }

        public void GenerateCover(string pArchivoOrigen, string pArchivoDestino)
        {
            var reader = new ComicReader();
            var book = reader.Read(pArchivoOrigen);
            int SizeRef = 350;

            if (book.Pages.Count.Equals(0))
            {
                Console.WriteLine(String.Concat("gotcha!"));
            }
            else
            {
                using (var stream = new MemoryStream(book.Pages.FirstOrDefault(d => d.Data.Length != 0).Data))
                {
                    Image img = null;

                    try
                    {
                        img = Image.FromStream(stream);
                    }
                    catch (Exception ex)
                    {
                        //throw ex;
                        img = Properties.Resources.NotGen;
                    }

                    //
                    var tHeight = img.Height;
                    var tWidth = img.Width;
                    var vRatio = (decimal)tHeight / (decimal)tWidth;

                    if (tHeight > tWidth)
                    {
                        tHeight = SizeRef;
                        tWidth = (int)(SizeRef / vRatio);
                    }
                    else
                    {
                        tHeight = (int)(SizeRef * vRatio);
                        tWidth = SizeRef;
                    }

                    var thumbnail = img.GetThumbnailImage(tWidth, tHeight, () => false, IntPtr.Zero);

                    using (var thumbStream = new MemoryStream())
                    {
                        thumbnail.Save(thumbStream, ImageFormat.Jpeg);
                        thumbnail.Save(pArchivoDestino, ImageFormat.Jpeg);  // Or Png
                    }
                }
            }
        }

        public string generarNombre(string pThumsDir)
        {
            int length = 7;

            // creating a StringBuilder object()
            StringBuilder str_build = new StringBuilder();
            Random random = new Random();

            char letter;

            for (int i = 0; i < length; i++)
            {
                double flt = random.NextDouble();
                int shift = Convert.ToInt32(Math.Floor(25 * flt));
                letter = Convert.ToChar(shift + 65);
                str_build.Append(letter);
            }

            if (File.Exists(Path.Combine(pThumsDir, string.Concat(str_build.ToString(), ".jpg"))))
                return generarNombre(pThumsDir);
            else
                return str_build.ToString();
        }

        public void showFrontPreview(string pDoujinUrl, string pSitio, Form pOwner)
        {
            try
            {
                if (string.IsNullOrEmpty(pSitio))
                    return;
                //
                HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                var content = GetPageHtml(pDoujinUrl);//new WebClient().DownloadString(pDoujinUrl);
                htmlDoc.LoadHtml(content);
                //
                string image = "";

                using (WebClient client = new WebClient())
                {
                    if (pSitio == "nHentai")
                    {
                        image = htmlDoc.DocumentNode.SelectNodes("//div[contains(@id, 'cover')]//img")[0].Attributes["data-src"].Value;
                    }
                    else if (pSitio == "eHentai")
                    {
                        image = new Regex(@"\(.*?\)").Match(htmlDoc.DocumentNode.SelectNodes("//div[contains(@id, 'gd1')]//div")[0].Attributes["style"].Value).Value.Replace("(", "").Replace(")", "");
                    }
                }
                //
                byte[] imageData = new WebClient().DownloadData(image);
                MemoryStream imgStream = new MemoryStream(imageData);
                Image img = Image.FromStream(imgStream);
                //
                PreviewSize = new Sizes(img.Height, img.Width, pOwner.Size.Height);
                //
                PreviewSize.Shrink(SystemInformation.PrimaryMonitorMaximizedWindowSize.Height);
                //
                using (var vPreviewForm = new Form()
                {
                    Height = PreviewSize.height,
                    Width = PreviewSize.width,
                    StartPosition = FormStartPosition.CenterParent,
                    ShowIcon = false,
                    FormBorderStyle = FormBorderStyle.SizableToolWindow,
                    Text = image
                })
                {
                    var pBox = new PictureBox()
                    {
                        Image = new Bitmap(imgStream),
                        Height = PreviewSize.height,
                        Width = PreviewSize.width,
                        SizeMode = PictureBoxSizeMode.StretchImage,
                        Dock = DockStyle.Fill
                    };

                    vPreviewForm.KeyDown += CloseControlFromKeyDown;
                    vPreviewForm.ResizeEnd += KeepPreviewRatio;

                    vPreviewForm.Controls.Add(pBox);

                    vPreviewForm.ShowDialog(pOwner);
                }
            }
            catch (WebException)
            {
                MessageBox.Show("Url Inválida o no ha buscado ningun comic");
            }
        }

        private void CloseControlFromKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                try
                {
                    ((Control)sender).Parent.Dispose();
                }
                catch
                {
                    ((Form)sender).Dispose();
                }
            }
        }

        private void KeepPreviewRatio(object sender, EventArgs e)
        {
            var vForm = (Form)sender;

            if (vForm.Height != PreviewSize.height)
            {
                vForm.Width = (int)(vForm.Height / PreviewSize.ratio);                
            }
            else if (vForm.Width != PreviewSize.width)
            {
                vForm.Height = (int)(vForm.Width * PreviewSize.ratio);
            }

            PreviewSize.width = vForm.Width;
            PreviewSize.height = vForm.Height;
        }
    }
}