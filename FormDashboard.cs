using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace CRUDMahasiswaADO
{
    public partial class FormDashboard : Form
    {
        DAL dbLogic = new DAL();
        bool isInitializing = true;
        DataTable dt;
        int button = 0;

        public FormDashboard()
        {
            InitializeComponent();

            // --- Pengaturan DateTimePicker (Tahun) ---
            dtpTanggalMasuk.MinDate = new DateTime(2000, 1, 1);
            dtpTanggalMasuk.Format = DateTimePickerFormat.Custom;
            dtpTanggalMasuk.CustomFormat = "yyyy";
            dtpTanggalMasuk.ShowUpDown = true;
            dtpTanggalMasuk.MaxDate = DateTime.Now;

            // --- Pengaturan ComboBox Tipe Grafik ---
            cmbTipe.DropDownStyle = ComboBoxStyle.DropDownList;

            var items = new List<KeyValuePair<string, SeriesChartType>>
            {
                new KeyValuePair<string, SeriesChartType>("Kolom", SeriesChartType.Column),
                new KeyValuePair<string, SeriesChartType>("Pie",   SeriesChartType.Pie)
            };

            isInitializing = true;
            cmbTipe.DataSource = items;
            cmbTipe.DisplayMember = "Key";
            cmbTipe.ValueMember = "Value";
            cmbTipe.SelectedIndex = 0; // Set default ke Kolom
            isInitializing = false;

            // Load chart pertama kali saat aplikasi dibuka
            loadDataChart();
        }

        // ==========================================================
        // METHOD UTAMA UNTUK LOAD GRAFIK
        // ==========================================================
        public void loadDataChart()
        {
            // 1. Reset semua elemen chart
            chartProdi.Series.Clear();
            chartProdi.Titles.Clear();
            chartProdi.Legends.Clear();
            chartProdi.ChartAreas.Clear();

            // 2. Setup Area Chart & Sumbu
            ChartArea ca = new ChartArea("MainArea");
            ca.AxisX.Title = "Program Studi";
            ca.AxisY.Title = "Jumlah Mahasiswa";
            ca.AxisX.LabelStyle.Angle = -45; // Memiringkan tulisan prodi
            ca.BackColor = Color.Transparent;

            // Tambahan: Agar angka di sumbu Y menjadi bilangan bulat (tanpa koma)
            ca.AxisY.LabelStyle.Format = "0";

            chartProdi.ChartAreas.Add(ca);

            try
            {
                // 3. Ambil data dari database
                if (button == 1)
                    dt = dbLogic.getDataChartByTahun(dtpTanggalMasuk.Value);
                else
                    dt = dbLogic.getAllDataChart();

                // 4. Tentukan tipe chart berdasarkan pilihan ComboBox
                SeriesChartType tipe = (SeriesChartType)cmbTipe.SelectedValue;

                // 5. Buat series baru (penampung data)
                Series s = new Series("Mahasiswa");
                s.ChartType = tipe;

                // Jika chart pie, atur labelnya agar muncul angkanya
                if (tipe == SeriesChartType.Pie)
                {
                    s.IsValueShownAsLabel = true;
                    s.Label = "#VAL";       // Menampilkan nilai di potongan pie
                    s.LegendText = "#VALX"; // Menampilkan nama prodi di legend
                }

                // 6. Masukkan data ke dalam series
                foreach (DataRow row in dt.Rows)
                {
                    string prodi = row["NamaProdi"].ToString();
                    // Pastikan nama kolom di sini sama persis dengan yang ada di DAL.cs (JmlMhs)
                    int jumlah = Convert.ToInt32(row["JmlMhs"]);
                    s.Points.AddXY(prodi, jumlah);
                }

                // 7. Tambahkan series ke dalam chart
                chartProdi.Series.Add(s);

                // 8. Tambahkan Judul Utama Grafik (Biru tebal di atas)
                Title title = new Title(
                    "Jumlah Mahasiswa per Program Studi",
                    Docking.Top,
                    new Font("Arial", 14, FontStyle.Bold),
                    Color.DarkBlue);
                chartProdi.Titles.Add(title);

                // 9. Tambahkan Legenda (Keterangan di sebelah kanan)
                Legend legend = new Legend("MainLegend");
                legend.Docking = Docking.Right;
                chartProdi.Legends.Add(legend);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load data: " + ex.Message);
            }
        }

        // ==========================================================
        // EVENT HANDLER
        // ==========================================================

        private void cmbTipe_SelectedValueChanged(object sender, EventArgs e)
        {
            if (isInitializing) return; // Mencegah error saat form baru lahir
            loadDataChart();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            button = 1; // Indikator untuk filter tahun
            loadDataChart();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            button = 0; // Indikator untuk load semua data
            loadDataChart();
        }

        private void btnDataMahasiswa_Click(object sender, EventArgs e)
        {
            // Ganti 'FormMahasiswa' dengan nama form data mahasiswa Anda yang sebenarnya
            FormMahasiswa frm = new FormMahasiswa();
            frm.Show();
            this.Hide();
        }
    }
}