using System;
using System.Data;
using System.Windows.Forms;

namespace CRUDMahasiswaADO
{
    public partial class FormReport : Form
    {
        private DataTable data;

        public FormReport()
        {
            InitializeComponent();
        }

        public FormReport(DataTable dt)
        {
            InitializeComponent();
            data = dt;
        }

        private void FormReport_Load(object sender, EventArgs e)
        {
            try
            {
                // Ganti 'LaporanMahasiswa' jika nama file .rpt Anda berbeda
                LaporanMahasiswa rpt = new LaporanMahasiswa();

                if (data != null && data.Rows.Count > 0)
                {
                    rpt.SetDataSource(data);
                    crystalReportViewer1.ReportSource = rpt;
                    crystalReportViewer1.Refresh();
                }
                else
                {
                    MessageBox.Show("Data laporan kosong! Pastikan Anda memilih data terlebih dahulu.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal menampilkan laporan : " + ex.Message);
            }
        }
    }
}