using System;
using System.Data;
using System.Windows.Forms;

namespace CRUDMahasiswaADO
{
    public partial class Form2 : Form
    {
        DAL dbLogic = new DAL();

        private DataTable dtMahasiswa;
        private DataTable dtProdi;

        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            dtpTanggalMasuk.Format = DateTimePickerFormat.Custom;
            dtpTanggalMasuk.CustomFormat = "yyyy";
            dtpTanggalMasuk.ShowUpDown = true;
            cmbProdi.DropDownStyle = ComboBoxStyle.DropDownList;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            btnCetak.Enabled = false;
            btnRekapData.Enabled = false;

            LoadProdi();
        }

        private void LoadProdi()
        {
            try
            {
                dtProdi = dbLogic.LoadProdi();
                cmbProdi.DataSource = dtProdi;
                cmbProdi.DisplayMember = "KodeProdi";
                cmbProdi.ValueMember = "KodeProdi";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal Load Prodi : " + ex.Message);
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                dtMahasiswa = dbLogic.ReportMahasiswa(cmbProdi.Text, dtpTanggalMasuk.Value.Year);
                dataGridView1.DataSource = dtMahasiswa;

                if (dtMahasiswa.Rows.Count > 0)
                {
                    btnCetak.Enabled = true;
                    btnRekapData.Enabled = true;
                    MessageBox.Show("Data ditemukan : " + dtMahasiswa.Rows.Count + " mahasiswa");
                }
                else
                {
                    btnCetak.Enabled = false;
                    btnRekapData.Enabled = false;
                    MessageBox.Show("Data tidak ditemukan");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : " + ex.Message);
            }
        }

        private void btnRekapData_Click_1(object sender, EventArgs e)
        {
            if (dtMahasiswa == null || dtMahasiswa.Rows.Count == 0)
            {
                MessageBox.Show("Tidak ada data! Silakan klik LOAD terlebih dahulu.");
                return;
            }

            // PANGGIL FormReport (bukan Form3)
            FormReport frm = new FormReport(dtMahasiswa);
            frm.ShowDialog();
        }

        private void btnCetak_Click(object sender, EventArgs e)
        {
            if (dtMahasiswa == null || dtMahasiswa.Rows.Count == 0)
            {
                MessageBox.Show("Tidak ada data untuk dicetak!");
                return;
            }

            // PANGGIL FormReport (bukan Form3)
            FormReport frm = new FormReport(dtMahasiswa);
            frm.ShowDialog();
        }
    }
}