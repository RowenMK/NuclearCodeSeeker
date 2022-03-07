using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Windows.Forms;

namespace NuclearCodeSeeker
{
    public partial class Main : Form
    {
        private static string vStuffDirectory = string.Empty;
        private static int vTotalFiles = 0;
        public static string vUrl_H = string.Empty;
        private Utils vUtils;

        private int _previousIndex;

        private bool _sortDirection;
        private const string thumbsDir = "thumbs";
        private const string thumbsIndexPath = "thumbs//ThumbIndex.json";

        private int vTotalPages = 0;
        private int vPageActual = 0;
        private const int vPageSize = 40;

        private enum sitio
        { nHentai, eHentai };

        private static sitio vSitioActual = sitio.nHentai;

        private delegate void SafeCallDelegateString(string pString);

        private delegate void SafeCallDelegateInt(int pCount);

        private List<Cola> vColaDescargas = new List<Cola>();

        private BindingSource vDataSource = new BindingSource();

        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            vUtils = new Utils();
            txtDirDownload.Text = vUtils.readUserData("DirDescargas");
            txtUbicacionBib.Text = vUtils.readUserData("DirBiblioteca");
            //
            validateFiles();
            flpPreview.Controls.Clear();
            bwLoadBiblioteca.RunWorkerAsync();
            MainTabControl.TabPages[2].Text = "Cargando Biblioteca...";
            //
            try
            {
                if (File.Exists(@"userDataQueue.json"))
                    vColaDescargas = JsonSerializer.Deserialize<List<Cola>>(File.ReadAllText(@"userDataQueue.json"));
            }
            catch (Exception ex)
            {
                showEx(ex);
            }

            vDataSource.DataSource = vColaDescargas;
            dgvColaDescargas.DataSource = vDataSource;
            vUtils.ajustarColumnasDGV(dgvColaDescargas);
        }

        private void btnBuscaInfo_nHentai_Click(object sender, EventArgs e)
        {
            try
            {
                if (nudNuckCode.Value.Equals(177013))
                {
                    if (MessageBox.Show(vUtils.get177013(), "...", MessageBoxButtons.OKCancel, MessageBoxIcon.Question).Equals(DialogResult.Cancel))
                    {
                        return;
                    }
                }

                if (nudNuckCode.Value > 0)
                {
                    if (bwDownloader.IsBusy)
                    {
                        MessageBox.Show("Descarga en ejecución");
                    }
                    else
                    {
                        vSitioActual = sitio.nHentai;
                        //
                        lBoxInfo.Items.Clear();
                        HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                        HtmlAgilityPack.HtmlDocument tmpDoc = new HtmlAgilityPack.HtmlDocument();
                        //
                        vUrl_H = string.Format(@"https://nhentai.net/g/{0}", nudNuckCode.Value);
                        ll_nh_link.Text = vUrl_H;
                        //
                        var content = new WebClient().DownloadString(vUrl_H);
                        //
                        htmlDoc.LoadHtml(content);
                        //
                        lblCodeName.Text = htmlDoc.DocumentNode.SelectNodes("//div[contains(@id, 'info')]//h1")[0].InnerText;
                        //
                        string vInfoLine;
                        //
                        foreach (HtmlNode node in htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'tag-container field-name')]"))
                        {
                            tmpDoc.LoadHtml(node.InnerHtml);

                            vInfoLine = string.Concat(tmpDoc.DocumentNode.InnerText.Split('\n')[1].Trim(), " ");

                            try
                            {
                                foreach (HtmlNode innerNode in tmpDoc.DocumentNode.SelectNodes("//span[contains(@class, 'name')]"))
                                {
                                    if (!innerNode.InnerText.Trim().Length.Equals(0))
                                    {
                                        if (vInfoLine.Equals(string.Empty))
                                            vInfoLine += string.Concat(innerNode.InnerText.Trim(), " ");
                                        else
                                        {
                                            if (vInfoLine.Equals("Pages: "))
                                            {
                                                vTotalFiles = int.Parse(innerNode.InnerText.Trim());
                                            }

                                            vInfoLine += string.Concat(innerNode.InnerText.Trim(), ", ");
                                        }
                                    }
                                }

                                if (vInfoLine.Length > 1)
                                {
                                    if (vInfoLine.Substring(vInfoLine.Length - 2).Equals(": "))
                                        vInfoLine += "no info";
                                    else
                                        vInfoLine = vInfoLine.Substring(0, vInfoLine.Length - 2);

                                    lBoxInfo.Items.Add(vInfoLine);
                                }
                            }
                            catch { }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Código Inválido");
                }
            }
            catch (Exception ex)
            {
                showEx(ex);
            }
        }

        private void showEx(Exception pEx)
        {
            if (pEx.InnerException != null)
                showEx(pEx.InnerException);
            else
                MessageBox.Show(pEx.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void lblLinkCount(int pLinkCount)
        {
            if (lblLinksCount.InvokeRequired)
            {
                var d = new SafeCallDelegateInt(lblLinkCount);
                lblLinksCount.Invoke(d, new object[] { pLinkCount });
            }
            else
            {
                lblLinksCount.Text = string.Format("{0} / {1} Imagenes Descargadas", pLinkCount, vTotalFiles);
            }
        }

        private void lblDownloadDir(string pDL_dir)
        {
            if (llDownloadDir.InvokeRequired)
            {
                var d = new SafeCallDelegateString(lblDownloadDir);
                llDownloadDir.Invoke(d, new object[] { pDL_dir });
            }
            else
            {
                llDownloadDir.Text = pDL_dir;
            }
        }

        private void lblDescargaActualQueue(string pDL_dir)
        {
            if (lblDescargaActual.InvokeRequired)
            {
                var d = new SafeCallDelegateString(lblDescargaActualQueue);
                lblDescargaActual.Invoke(d, new object[] { pDL_dir });
            }
            else
            {
                lblDescargaActual.Text = pDL_dir;
            }
        }

        private void bwDownloader_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                var content = new WebClient().DownloadString(vUrl_H);
                htmlDoc.LoadHtml(content);

                if (vSitioActual.Equals(sitio.nHentai))
                {
                    int pCurrent = 0;

                    using (WebClient client = new WebClient())
                    {
                        foreach (HtmlNode node in htmlDoc.DocumentNode.SelectNodes("//div[contains(@id, 'thumbnail-container')]//a[contains(@class, 'gallerythumb')]"))
                        {
                            //
                            pCurrent++;

                            using (WebClient picPage = new WebClient())
                            {
                                var picContent = picPage.DownloadString(String.Format(@"https://nhentai.net/{0}", node.Attributes["href"].Value));
                                HtmlAgilityPack.HtmlDocument picHtmlDoc = new HtmlAgilityPack.HtmlDocument();
                                picHtmlDoc.LoadHtml(picContent);

                                HtmlNode picNode = picHtmlDoc.DocumentNode.SelectSingleNode("//section[contains(@id, 'image-container')]//img");

                                string stuff = picNode.Attributes["src"].Value;
                                //
                                string vUrl = Path.Combine(vStuffDirectory,
                                    string.Concat(cboxOverwriteNames.Checked ? pCurrent.ToString().PadLeft(4, '0') : Path.GetFileNameWithoutExtension(new Uri(stuff).AbsolutePath), Path.GetExtension(new Uri(stuff).AbsolutePath)));
                                //
                                if (!File.Exists(vUrl) || cboxOverwriteFiles.Checked)
                                {
                                    client.DownloadFile(new Uri(stuff), vUrl);
                                }
                                //
                                lblLinkCount(pCurrent);
                                bwDownloader.ReportProgress(pCurrent * 100 / vTotalFiles);
                            }
                        }
                    }
                }
                else if (vSitioActual.Equals(sitio.eHentai))
                {
                    get_eHentaiPic(htmlDoc.DocumentNode.SelectSingleNode("//div[@id='gdt']//a[@href]").Attributes["href"].Value, vStuffDirectory, vTotalFiles, 0, false);
                }

                if (rbtnGuardarZip.Checked || rbtnGuardarCbr.Checked)
                {
                    string ZipFileName = string.Concat(vStuffDirectory, ".zip");
                    //
                    if (File.Exists(ZipFileName))
                    {
                        File.Delete(ZipFileName);
                    }
                    //
                    using (var archive = ZipFile.Open(ZipFileName, ZipArchiveMode.Create))
                    {
                        foreach (string vPicLocalDir in Directory.GetFiles(vStuffDirectory))
                        {
                            archive.CreateEntryFromFile(vPicLocalDir, Path.GetFileName(vPicLocalDir));
                        }
                    }

                    if (rbtnGuardarCbr.Checked)
                    {
                        if (File.Exists(ZipFileName.Replace(".zip", ".cbr")))
                        {
                            File.Delete(ZipFileName.Replace(".zip", ".cbr"));
                        }

                        File.Move(ZipFileName, ZipFileName.Replace(".zip", ".cbr"));
                        lblDownloadDir(ZipFileName.Replace(".zip", ".cbr"));
                    }
                    else
                    {
                        lblDownloadDir(ZipFileName);
                    }

                    Directory.Delete(vStuffDirectory, true);
                }
            }
            catch (Exception ex)
            {
                showEx(ex);
            }
        }

        private void bwDownloader_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            try
            {
                pbarDownload.Value = e.ProgressPercentage;
            }
            catch { }
        }

        private void bwDownloader_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            controlEnabled(true);
        }

        private void controlEnabled(bool pEnabled)
        {
            cboxOverwriteNames.Enabled = pEnabled;
            cboxOverwriteFiles.Enabled = pEnabled;
            rbtnNoComprimir.Enabled = pEnabled;
            rbtnGuardarZip.Enabled = pEnabled;
            rbtnGuardarCbr.Enabled = pEnabled;
            tabMain.Enabled = pEnabled;
            //
            btnPasteCodes.Enabled = pEnabled;
            btnBorrarQueue.Enabled = pEnabled;
            btnGuardarCola.Enabled = pEnabled;
            btnLimpiarCola.Enabled = pEnabled;
            btnSelectDirDownload.Enabled = pEnabled;
        }

        private void btnGetStuff_Click(object sender, EventArgs e)
        {
            if (MainTabControl.SelectedTab.Name.Equals("tpQueue"))
            {
                iniciarDescargaCola();
            }
            else
            {
                try
                {
                    if (bwDownloader.IsBusy || bwDownloadQueue.IsBusy)
                    {
                        MessageBox.Show("Descarga en ejecución");
                    }
                    else
                    {
                        if (vSitioActual.Equals(sitio.nHentai) && !vUtils.validaUrlWeb(vUrl_H))
                            return;

                        if (vSitioActual.Equals(sitio.eHentai) && string.IsNullOrEmpty(txtHE_Url.Text))
                            return;

                        if (Directory.Exists(txtDirDownload.Text))
                        {
                            vStuffDirectory = Path.Combine(txtDirDownload.Text, vUtils.replaceInvalid(lblCodeName.Text));

                            if (!Directory.Exists(vStuffDirectory))
                            {
                                Directory.CreateDirectory(vStuffDirectory);
                            }
                            //
                            llDownloadDir.Text = vStuffDirectory;
                            //
                            controlEnabled(false);
                            bwDownloader.RunWorkerAsync();
                            //
                        }
                        else
                        {
                            MessageBox.Show("Debe Seleccionar una ubicación válida para las descargas.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void llDownloadDir_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            vUtils.OpenURL(llDownloadDir.Text);
        }

        private void btnAbrir_Click(object sender, EventArgs e)
        {
            vUtils.OpenURL(dgvColaDescargas.SelectedRows[0].Cells[5].Value.ToString());
        }

        private void ll_nh_link_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            vUtils.OpenURL(vUrl_H);
        }

        // *************************** eHentai ***********************************
        private void btnLook4Stuff_eHentai_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtHE_Url.Text))
                {
                    if (bwDownloader.IsBusy || bwDownloadQueue.IsBusy)
                    {
                        MessageBox.Show("Descarga en ejecución");
                    }
                    else
                    {
                        vSitioActual = sitio.eHentai;
                        //
                        lBoxInfo.Items.Clear();
                        HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                        HtmlAgilityPack.HtmlDocument tmpDoc = new HtmlAgilityPack.HtmlDocument();
                        var content = new WebClient().DownloadString(txtHE_Url.Text);
                        //
                        htmlDoc.LoadHtml(content);
                        //
                        vUrl_H = txtHE_Url.Text;

                        lblCodeName.Text = htmlDoc.GetElementbyId("gn").InnerText;
                        vTotalFiles = int.Parse(htmlDoc.DocumentNode.SelectNodes("//td[contains(@class, 'gdt2')]")[5].InnerText.Split(' ')[0]);

                        string vInfoLine;
                        //
                        foreach (HtmlNode node in htmlDoc.DocumentNode.SelectNodes("//div[contains(@id, 'taglist')]//table//tr"))
                        {
                            tmpDoc.LoadHtml(node.InnerHtml);

                            vInfoLine = string.Concat(tmpDoc.DocumentNode.SelectSingleNode("//td[contains(@class, 'tc')]").InnerText, " ");

                            try
                            {
                                foreach (HtmlNode innerNode in tmpDoc.DocumentNode.SelectNodes("//div"))
                                {
                                    if (!innerNode.InnerText.Trim().Length.Equals(0))
                                    {
                                        if (vInfoLine.Equals(string.Empty))
                                            vInfoLine += string.Concat(innerNode.InnerText.Trim(), " ");
                                        else
                                        {
                                            vInfoLine += string.Concat(innerNode.InnerText.Trim(), ", ");
                                        }
                                    }
                                }

                                if (vInfoLine.Length > 1)
                                {
                                    if (vInfoLine.Substring(vInfoLine.Length - 2).Equals(": "))
                                        vInfoLine += "no info";
                                    else
                                        vInfoLine = vInfoLine.Substring(0, vInfoLine.Length - 2);

                                    lBoxInfo.Items.Add(vInfoLine);
                                }
                            }
                            catch { }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("There's no Stuff!");
                }
            }
            catch (Exception ex)
            {
                showEx(ex);
            }
        }

        private void get_eHentaiPic(string pEH_Url, string pDownloadDir, int pTotal, int pCurrent, bool pIsQueue)
        {
            HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
            var content = new WebClient().DownloadString(pEH_Url);
            htmlDoc.LoadHtml(content);
            //
            var stuff = htmlDoc.GetElementbyId("img").Attributes["src"].Value;
            pCurrent++;

            try
            {
                using (WebClient client = new WebClient())
                {
                    //
                    string vUrl = Path.Combine(pDownloadDir,
                        string.Concat(cboxOverwriteNames.Checked ? pCurrent.ToString().PadLeft(4, '0') : Path.GetFileNameWithoutExtension(new Uri(stuff).AbsolutePath), Path.GetExtension(new Uri(stuff).AbsolutePath)));
                    //
                    if (!File.Exists(vUrl) || cboxOverwriteFiles.Checked)
                    {
                        client.DownloadFile(new Uri(stuff), vUrl);
                    }
                    //
                    lblLinkCount(pCurrent);

                    if (pIsQueue)
                        bwDownloadQueue.ReportProgress(pCurrent * 100 / pTotal);
                    else
                        bwDownloader.ReportProgress(pCurrent * 100 / pTotal);
                }
            }
            catch (Exception ex)
            {
                showEx(ex);
            }

            var next = htmlDoc.GetElementbyId("next").Attributes["href"].Value;

            if (pEH_Url != next)
                get_eHentaiPic(next, pDownloadDir, pTotal, pCurrent, pIsQueue);
        }

        private void btnSelectDirDownload_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult resultDir = fbd.ShowDialog();

                if (Directory.Exists(fbd.SelectedPath))
                {
                    txtDirDownload.Text = fbd.SelectedPath;

                    vUtils.writeUserData("DirDescargas", fbd.SelectedPath);
                }
            }
        }

        private void btnAbrirDirDescargas_Click(object sender, EventArgs e)
        {
            vUtils.OpenURL(txtDirDownload.Text);
        }

        private void btnLimpiarCola_Click(object sender, EventArgs e)
        {
            try
            {
                vColaDescargas.Clear();
                vDataSource.ResetBindings(false);
            }
            catch (Exception ex)
            {
                showEx(ex);
            }
        }

        private void btnAdd2Queue_nHentai_Click(object sender, EventArgs e)
        {
            try
            {
                vUtils.addQueue_nHentai((int)nudNuckCode.Value, sitio.nHentai.ToString(), vColaDescargas);
                vDataSource.ResetBindings(false);
                MainTabControl.SelectedTab = MainTabControl.TabPages["tpQueue"];
            }
            catch (Exception ex)
            {
                showEx(ex);
            }
        }

        private void btnAdd2Queue_eHentai_Click(object sender, EventArgs e)
        {
            try
            {
                vUtils.addQueue_eHentai(txtHE_Url.Text, sitio.eHentai.ToString(), vColaDescargas);
                vDataSource.ResetBindings(false);
                MainTabControl.SelectedTab = MainTabControl.TabPages["tpQueue"];
            }
            catch (Exception ex)
            {
                showEx(ex);
            }
        }

        private void btnPasteCodes_Click(object sender, EventArgs e)
        {
            try
            {
                char[] rowSplitter = { '\r', '\n' };
                //
                IDataObject dataInClipboard = Clipboard.GetDataObject();
                string stringInClipboard = (string)dataInClipboard.GetData(DataFormats.Text);
                int vOut;
                //
                foreach (string vCode in stringInClipboard.Split(rowSplitter, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (int.TryParse(vCode.Replace("#", ""), out vOut))
                    {
                        vUtils.addQueue_nHentai(vOut, sitio.nHentai.ToString(), vColaDescargas);
                        vDataSource.ResetBindings(false);
                    }
                    else
                    {
                        vUtils.addQueue_eHentai(vCode, sitio.eHentai.ToString(), vColaDescargas);
                        vDataSource.ResetBindings(false);
                    }
                };

                vDataSource.ResetBindings(false);
            }
            catch (Exception ex)
            {
                showEx(ex);
            }
        }

        private void btnBorrarQueue_Click(object sender, EventArgs e)
        {
            try
            {
                vColaDescargas.Remove(vColaDescargas.FirstOrDefault(d => d.URL == dgvColaDescargas.SelectedRows[0].Cells[0].Value.ToString()));
                vDataSource.DataSource = vColaDescargas;
                vDataSource.ResetBindings(false);
            }
            catch (Exception ex)
            {
                showEx(ex);
            }
        }

        private void btnGuardarCola_Click(object sender, EventArgs e)
        {
            try
            {
                var vJsonDescargas = JsonSerializer.Serialize(vColaDescargas);
                File.WriteAllText(@"userDataQueue.json", vJsonDescargas);

                MessageBox.Show("Datos Guardados");
            }
            catch (Exception ex)
            {
                showEx(ex);
            }
        }

        private void btnAjustarColumnas_Click(object sender, EventArgs e)
        {
            try
            {
                vUtils.ajustarColumnasDGV(dgvColaDescargas);
            }
            catch (Exception ex)
            {
                showEx(ex);
            }
        }

        private void btnResetDownload_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Desea marcar como pendiente el registro seleccionado para descargarlo de nuevo?", "Marcar como Pendiente", MessageBoxButtons.YesNo, MessageBoxIcon.Question).Equals(DialogResult.Yes))
                {
                    vColaDescargas.FirstOrDefault(d => d.URL == dgvColaDescargas.SelectedRows[0].Cells[0].Value.ToString()).Done = false;
                }
            }
            catch (Exception ex)
            {
                showEx(ex);
            }
        }

        private void iniciarDescargaCola()
        {
            try
            {
                if (bwDownloader.IsBusy || bwDownloadQueue.IsBusy)
                {
                    if (bwDownloadQueue.IsBusy)
                    {
                        if (MessageBox.Show("Desea detener la descarga?", "Detener", MessageBoxButtons.YesNo, MessageBoxIcon.Question).Equals(DialogResult.Yes))
                        {
                            bwDownloadQueue.CancelAsync();
                            MessageBox.Show("El proceso se detendrá al finalizar la descarga actual.");
                        }
                    }
                    else
                        MessageBox.Show("Descarga en ejecución");
                }
                else
                {
                    if (Directory.Exists(txtDirDownload.Text))
                    {
                        btnGetStuff.Text = "Detener Descarga Cola";

                        llDownloadDir.Text = string.Empty;
                        //
                        controlEnabled(false);
                        bwDownloadQueue.RunWorkerAsync();
                        //
                    }
                    else
                    {
                        MessageBox.Show("Debe Seleccionar una ubicación válida para las descargas.");
                    }
                }
            }
            catch (Exception ex)
            {
                showEx(ex);
            }
        }

        private void bwDownloadQueue_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                string vDirectorioArchivo;

                List<Cola> vColaTMP = vColaDescargas;

                foreach (Cola vColaReg in vColaTMP)
                {
                    if (!vColaReg.Done)
                    {
                        vDirectorioArchivo = Path.Combine(txtDirDownload.Text, vUtils.replaceInvalid(vColaReg.Nombre));
                        vTotalFiles = vColaReg.Cantidad_Pags;

                        if (!Directory.Exists(vDirectorioArchivo))
                        {
                            Directory.CreateDirectory(vDirectorioArchivo);
                        }
                        //
                        lblDescargaActualQueue(string.Concat("Descargando: ", vColaReg.Nombre));

                        var content = new WebClient().DownloadString(vColaReg.URL);
                        htmlDoc.LoadHtml(content);

                        if (vColaReg.Sitio.Equals(sitio.nHentai.ToString()))
                        {
                            int pCurrent = 0;

                            using (WebClient client = new WebClient())
                            {
                                foreach (HtmlNode node in htmlDoc.DocumentNode.SelectNodes("//div[contains(@id, 'thumbnail-container')]//a[contains(@class, 'gallerythumb')]"))
                                {
                                    //
                                    pCurrent++;

                                    using (WebClient picPage = new WebClient())
                                    {
                                        var picContent = picPage.DownloadString(String.Format(@"https://nhentai.net/{0}", node.Attributes["href"].Value));
                                        HtmlAgilityPack.HtmlDocument picHtmlDoc = new HtmlAgilityPack.HtmlDocument();
                                        picHtmlDoc.LoadHtml(picContent);

                                        HtmlNode picNode = picHtmlDoc.DocumentNode.SelectSingleNode("//section[contains(@id, 'image-container')]//img");

                                        string stuff = picNode.Attributes["src"].Value;
                                        //
                                        string vUrl = Path.Combine(vDirectorioArchivo,
                                            string.Concat(cboxOverwriteNames.Checked ? pCurrent.ToString().PadLeft(4, '0') : Path.GetFileNameWithoutExtension(new Uri(stuff).AbsolutePath), Path.GetExtension(new Uri(stuff).AbsolutePath)));
                                        //
                                        if (!File.Exists(vUrl) || cboxOverwriteFiles.Checked)
                                        {
                                            client.DownloadFile(new Uri(stuff), vUrl);
                                        }
                                        //
                                        lblLinkCount(pCurrent);
                                        bwDownloader.ReportProgress(pCurrent * 100 / vTotalFiles);

                                        if (bwDownloadQueue.CancellationPending)
                                        {
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                        else if (vColaReg.Sitio.Equals(sitio.eHentai.ToString()))
                        {
                            get_eHentaiPic(htmlDoc.DocumentNode.SelectSingleNode("//div[@id='gdt']//a[@href]").Attributes["href"].Value, vDirectorioArchivo, vColaReg.Cantidad_Pags, 0, true);
                        }

                        if (rbtnGuardarZip.Checked || rbtnGuardarCbr.Checked)
                        {
                            string ZipFileName = string.Concat(vDirectorioArchivo, ".zip");
                            //
                            if (File.Exists(ZipFileName))
                            {
                                File.Delete(ZipFileName);
                            }
                            //
                            using (var archive = ZipFile.Open(ZipFileName, ZipArchiveMode.Create))
                            {
                                foreach (string vPicLocalDir in Directory.GetFiles(vDirectorioArchivo))
                                {
                                    archive.CreateEntryFromFile(vPicLocalDir, Path.GetFileName(vPicLocalDir));
                                }
                            }

                            if (rbtnGuardarCbr.Checked)
                            {
                                if (File.Exists(ZipFileName.Replace(".zip", ".cbr")))
                                {
                                    File.Delete(ZipFileName.Replace(".zip", ".cbr"));
                                }

                                File.Move(ZipFileName, ZipFileName.Replace(".zip", ".cbr"));

                                ZipFileName = ZipFileName.Replace(".zip", ".cbr");
                            }

                            Directory.Delete(vDirectorioArchivo, true);

                            vDirectorioArchivo = ZipFileName;
                        }

                        vColaDescargas.FirstOrDefault(d => d.URL == vColaReg.URL).Archivo = vDirectorioArchivo;

                        vColaDescargas.FirstOrDefault(d => d.URL == vColaReg.URL).Done = true;

                        if (bwDownloadQueue.CancellationPending)
                        {
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                showEx(ex);
            }
        }

        private void bwDownloadQueue_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            try
            {
                pbarDownload.Value = e.ProgressPercentage;
                //
                if (e.ProgressPercentage.Equals(100))
                {
                    vDataSource.ResetBindings(false);
                    //
                    var vJsonDescargas = JsonSerializer.Serialize(vColaDescargas);
                    File.WriteAllText(@"userDataQueue.json", vJsonDescargas);
                }
            }
            catch { }
        }

        private void bwDownloadQueue_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            lblDescargaActual.Text = "Ninguna Descarga Activa";
            btnGetStuff.Text = "Iniciar Descarga";
            //
            var vJsonDescargas = JsonSerializer.Serialize(vColaDescargas);
            File.WriteAllText(@"userDataQueue.json", vJsonDescargas);
            //
            controlEnabled(true);
        }

        private void dgvColaDescargas_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            vUtils.OpenURL(dgvColaDescargas.SelectedRows[0].Cells[5].Value.ToString());
        }

        private void dgvColaDescargas_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == _previousIndex)
                    _sortDirection ^= true; // toggle direction
                                            //
                vColaDescargas = vUtils.SortData((List<Cola>)vDataSource.DataSource, dgvColaDescargas.Columns[e.ColumnIndex].Name, _sortDirection);
                //
                vDataSource.DataSource = vColaDescargas;
                vDataSource.ResetBindings(false);
                _previousIndex = e.ColumnIndex;
            }
            catch (Exception ex)
            {
                showEx(ex);
            }
        }

        private void btnIrA_Url_Click(object sender, EventArgs e)
        {
            vUtils.OpenURL(dgvColaDescargas.SelectedRows[0].Cells[0].Value.ToString());
        }

        private void MainTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            gboxOpciones.Height = 100;
            btnGetStuff.Enabled = true;

            switch (MainTabControl.SelectedTab.Name)
            {
                case "tpQueue":
                    dgvColaDescargas.AutoResizeColumns();
                    dgvColaDescargas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                    break;

                case "tpBiblioteca":
                    gboxOpciones.Height = 0;
                    btnGetStuff.Enabled = false;

                    break;
            }
        }

        private void btnBuscarBib_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult resultDir = fbd.ShowDialog();

                if (Directory.Exists(fbd.SelectedPath))
                {
                    txtUbicacionBib.Text = fbd.SelectedPath;

                    vUtils.writeUserData("DirBiblioteca", fbd.SelectedPath);

                    if (!bwRefreshBiblioteca.IsBusy)
                    {
                        bwRefreshBiblioteca.RunWorkerAsync();
                        btnRefreshBibl.Enabled = false;
                    }
                }
            }
        }

        private void BtnAbrirBib_Click(object sender, EventArgs e)
        {
            vUtils.OpenURL(txtUbicacionBib.Text);
        }

        private void bwRefreshBiblioteca_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (!Directory.Exists(thumbsDir))
                Directory.CreateDirectory(thumbsDir);
            //
            string[] fileEntries = Directory.GetFiles(txtUbicacionBib.Text, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".cbr") || s.EndsWith(".cbz")).ToArray();

            List<ThumbIndex> vThumbs;

            if (File.Exists(thumbsIndexPath))
            {
                vThumbs = JsonSerializer.Deserialize<List<ThumbIndex>>(File.ReadAllText(thumbsIndexPath));
            }
            else
            {
                vThumbs = new List<ThumbIndex>();
            }
                
            int vCnt = 0;
            string vThumbName = string.Empty;
            //
            foreach (string fileName in fileEntries)
            {
                //if (!File.Exists(Path.Combine(thumbsDir, string.Concat(Path.GetFileNameWithoutExtension(fileName), ".jpg"))))
                if (vThumbs.FirstOrDefault(d => d.ComicPath.Equals(fileName)) == null) // || vThumbs.FirstOrDefault(d => d.ComicPath.Equals(fileName)).ThumbPath.Equals(string.Empty))
                {
                    vThumbName = Path.Combine(thumbsDir, string.Concat(vUtils.generarNombre(thumbsDir), ".jpg"));
                    vUtils.GenerateCover(fileName, vThumbName);

                    if (!File.Exists(vThumbName))
                        vThumbName = "no_file";
                    
                    vThumbs.Add(new ThumbIndex() { ComicPath = fileName, ThumbPath = vThumbName });
                }
                //
                vCnt++;
                bwRefreshBiblioteca.ReportProgress(vCnt * 100 / fileEntries.Count());
            }

            var vJsonThumbIndex = JsonSerializer.Serialize(vThumbs);
            File.WriteAllText(thumbsIndexPath, vJsonThumbIndex);
        }

        private void validateFiles()
        {
            try
            {
                string[] fileEntries = Directory.GetFiles(txtUbicacionBib.Text, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".cbr") || s.EndsWith(".cbz")).ToArray();

                List<ThumbIndex> vThumbsFile;

                if (File.Exists(thumbsIndexPath))
                {
                    vThumbsFile = JsonSerializer.Deserialize<List<ThumbIndex>>(File.ReadAllText(thumbsIndexPath));

                    List<ThumbIndex> vCheckThumbs = new List<ThumbIndex>(vThumbsFile);

                    foreach (var vThumb in vCheckThumbs)
                    {
                        if (!File.Exists(vThumb.ComicPath))
                        {
                            try
                            {
                                File.Delete(vThumb.ThumbPath);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }

                            vThumbsFile.Remove(vThumb);
                        }
                    }

                    var vJsonThumbIndex = JsonSerializer.Serialize(vThumbsFile);
                    File.WriteAllText(thumbsIndexPath, vJsonThumbIndex);
                }               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void btnRefreshBibl_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Desea Agregar los Comics Nuevos de la biblioteca", "Cargar Biblioteca", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

            if (dialogResult.Equals(DialogResult.OK))
            {
                bwRefreshBiblioteca.RunWorkerAsync();
                btnRefreshBibl.Enabled = false;
            }
        }

        private void bwRefreshBiblioteca_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            pbarBiblioteca.Value = e.ProgressPercentage;
            btnRefreshBibl.Text = String.Format("{0} %", e.ProgressPercentage);
        }

        private void bwRefreshBiblioteca_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            btnRefreshBibl.Enabled = true;
            btnRefreshBibl.Text = "Cargar Comics";

            flpPreview.Controls.Clear();
            bwLoadBiblioteca.RunWorkerAsync();
        }

        private void bwLoadBiblioteca_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            // 
            if (!Directory.Exists(thumbsDir))
                return;
            //
            string[] fileEntries = Directory.EnumerateFiles(txtUbicacionBib.Text, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".cbr") || s.EndsWith(".cbz")).ToArray();

            List<ThumbIndex> vThumbs;
            if (File.Exists(thumbsIndexPath))
            {
                vThumbs = JsonSerializer.Deserialize<List<ThumbIndex>>(File.ReadAllText(thumbsIndexPath));
            }
            else
            {
                vThumbs = new List<ThumbIndex>();
            }

            double vTotalItems = vThumbs.Where(d => d.ComicPath.ToUpper().Contains(txtBuscarComic.Text.ToUpper())).Count();
            //
            foreach (ThumbIndex vIndex in vThumbs.Where(d => d.ComicPath.ToUpper().Contains(txtBuscarComic.Text.ToUpper())).OrderBy(a => a.ComicPath).Skip(vPageActual * vPageSize).Take(vPageSize))
            {
                try
                {
                    flpPreview_loadPicture(
                        string.Concat(vIndex.ThumbPath,
                        "?",
                        vIndex.ComicPath));
                }
                catch (Exception) { }
            }
            //
            double vTotalPagsD = vTotalItems / vPageSize;
            vTotalPages = (int)Math.Ceiling(vTotalPagsD);
            //
            lblPages_desc(String.Format("{0} / {1}", vPageActual + 1, vTotalPages));
        }

        private void flpPreview_loadPicture(string pPaths)
        {
            try
            {
                if (flpPreview.InvokeRequired)
                {
                    var d = new SafeCallDelegateString(flpPreview_loadPicture);
                    flpPreview.Invoke(d, new object[] { pPaths });
                }
                else
                {
                    string[] vPaths = pPaths.Split(new char[] { '?' });
                    var vPicture = File.Exists(vPaths[0]) ? new Bitmap(vPaths[0]) : null;
                    ComicPreview vCPreview = new ComicPreview() { Tag = vPaths[1] };
                    vCPreview.lblNombre.Text = Path.GetFileNameWithoutExtension(vPaths[1]);
                    vCPreview.pboxMain.Image = vPicture;
                    flpPreview.Controls.Add(vCPreview);
                }
            }
            catch (Exception ex)
            {
                showEx(ex);
            }
        }

        private void MainTabControl_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (bwLoadBiblioteca.IsBusy && e.TabPageIndex.Equals(2))
            {
                e.Cancel = true;
            }
        }

        private void bwLoadBiblioteca_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            MainTabControl.TabPages[2].Text = "Biblioteca";
        }

        private void lblPages_desc(string pPageDesc)
        {
            if (lblPages.InvokeRequired)
            {
                var d = new SafeCallDelegateString(lblPages_desc);
                lblPages.Invoke(d, new object[] { pPageDesc });
            }
            else
            {
                lblPages.Text = pPageDesc;
            }
        }

        private void txtBuscarComic_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                vPageActual = 0;
                //
                flpPreview.Controls.Clear();
                bwLoadBiblioteca.RunWorkerAsync();
                //
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void btnPrevPage_Click(object sender, EventArgs e)
        {
            if (!bwLoadBiblioteca.IsBusy)
            {
                if (vPageActual > 0)
                {
                    vPageActual--;
                    //
                    flpPreview.Controls.Clear();
                    bwLoadBiblioteca.RunWorkerAsync();
                }
            }
        }

        private void btnNextPage_Click(object sender, EventArgs e)
        {
            if (!bwLoadBiblioteca.IsBusy)
            {
                if (vPageActual + 1 < vTotalPages)
                {
                    vPageActual++;
                    //
                    flpPreview.Controls.Clear();
                    bwLoadBiblioteca.RunWorkerAsync();
                }
            }
        }
    }
}