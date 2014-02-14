using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CR2ToJPG
{
    public partial class frmMain : Form
    {
        private bool _isWorking;

        public frmMain()
        {
            InitializeComponent();
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtInputDirectory.Text) || string.IsNullOrEmpty(txtOutputDirectory.Text))
            {
                MessageBox.Show("Input and Output Directories Are Required.", "CR2 To JPG", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            SetFormWorking(true);

            pbProgress.Value = 0;

            var converterOptions = new ConverterOptions
            {
                Files = Directory.GetFiles(txtInputDirectory.Text, "*.CR2"),
                OutputDirectory = txtOutputDirectory.Text
            };

            pbProgress.Maximum = converterOptions.Files.Length;

            bwConverter.RunWorkerAsync(converterOptions);
        }

        private void btnSelectInputDirectory_Click(object sender, EventArgs e)
        {
            var fbd = new FolderBrowserDialog();

            if (fbd.ShowDialog() == DialogResult.OK)
                txtInputDirectory.Text = fbd.SelectedPath;
        }

        private void btnSelectOutputDirectory_Click(object sender, EventArgs e)
        {
            var fbd = new FolderBrowserDialog();

            if (fbd.ShowDialog() == DialogResult.OK)
                txtOutputDirectory.Text = fbd.SelectedPath;
        }

        private void bwConverter_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            var converterOptions = e.Argument as ConverterOptions;
            var parallelOptions = GetParallelOptions();

            Parallel.ForEach(converterOptions.Files, parallelOptions, currentFile =>
            {
                Converter.ConvertImage(currentFile, converterOptions.OutputDirectory);
                bwConverter.ReportProgress(0);
            });
        }

        private void bwConverter_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            pbProgress.PerformStep();
        }

        private void bwConverter_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            SetFormWorking(false);

            MessageBox.Show("Conversion complete!", "CR2 To JPG", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = _isWorking;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            txtInputDirectory.TabStop =
            txtOutputDirectory.TabStop = false;

            lblVersion.Text = "v" + Application.ProductVersion;
        }

        private ParallelOptions GetParallelOptions()
        {
            return new ParallelOptions() { MaxDegreeOfParallelism = 2 };
        }

        private void lnkWebsite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://wmwood.net");
        }

        private void SetFormWorking(bool isWorking)
        {
            btnProcess.Enabled =
            btnSelectInputDirectory.Enabled =
            btnSelectOutputDirectory.Enabled = !isWorking;

            _isWorking =
            pbProgress.Visible =
            this.UseWaitCursor = isWorking;
        }
    }
}