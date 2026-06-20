using System;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Sockets;

namespace CRUDMahasiswaADO
{
    public class DAL
    {
        // ============================================================
        // Langkah 19a: ambil IP lokal secara dinamis
        // ============================================================
        public static string GetLoacalIPAddress()
        {
            string localIP = string.Empty;
            try
            {
                var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        localIP = ip.ToString();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(
                    "Error getting local IP address: " + ex.Message);
            }
            return localIP;
        }

        // ============================================================
        // Langkah 19b: connection string dinamis pakai IP lokal
        // ============================================================
        public static string GetConnectionString()
        {
            // Ganti Data Source dengan nama server yang terlihat di SSMS Anda
            string connectionString =
                @"Data Source=LAPTOP-15DMI21A\AKMALPRSANJAY;Initial Catalog=DBAkademikADO;Integrated Security=True;TrustServerCertificate=True;";

            return connectionString;
        }

        // Koneksi reusable
        private SqlConnection conn =
            new SqlConnection(GetConnectionString());

        // DataAdapter & DataTable reusable
        SqlDataAdapter da;
        DataTable dtMahasiswa;
        DataTable dtProdi;

        // ============================================================
        // COUNT MAHASISWA (sp_CountMahasiswa dengan OUTPUT param)
        // ============================================================
        public int CountMhs()
        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            SqlCommand cmd =
                new SqlCommand("sp_CountMahasiswa", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            SqlParameter outputParam =
                new SqlParameter("@pCount", SqlDbType.Int);
            outputParam.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(outputParam);

            cmd.ExecuteNonQuery();

            return Convert.ToInt32(outputParam.Value);
        }

        // ============================================================
        // GET ALL MAHASISWA
        // ============================================================
        public DataTable GetMhs()
        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            SqlCommand cmd =
                new SqlCommand("sp_GetMahasiswa", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            da = new SqlDataAdapter(cmd);
            dtMahasiswa = new DataTable();
            da.Fill(dtMahasiswa);

            return dtMahasiswa;
        }

        // ============================================================
        // GET MAHASISWA BY NIM
        // ============================================================
        public DataTable GetMhsByNIM(string nim)
        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            SqlCommand cmd =
                new SqlCommand("sp_GetMahasiswaByNIM", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("pNIM", nim);

            da = new SqlDataAdapter(cmd);
            dtMahasiswa = new DataTable();
            da.Fill(dtMahasiswa);

            return dtMahasiswa;
        }

        // ============================================================
        // INSERT MAHASISWA (dengan foto BLOB)
        // ============================================================
        public void InsertMhs(
            string nim,
            string nama,
            string alamat,
            string jenisKelamin,
            DateTime tanggalLahir,
            string kodeProdi,
            byte[] foto)
        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            SqlTransaction trans = conn.BeginTransaction();
            try
            {
                SqlCommand command =
                    new SqlCommand("sp_InsertMahasiswa", conn, trans);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("pNIM", nim);
                command.Parameters.AddWithValue("pNama", nama);
                command.Parameters.AddWithValue("pAlamat", alamat);
                command.Parameters.AddWithValue("pTanggalLahir", tanggalLahir);
                command.Parameters.AddWithValue("pJenisKelamin", jenisKelamin);
                command.Parameters.AddWithValue("pNmProdi", kodeProdi);

                if (foto != null)
                    command.Parameters.AddWithValue("pFoto", foto);
                else
                    command.Parameters.AddWithValue("pFoto", DBNull.Value);

                command.ExecuteNonQuery();
                trans.Commit();
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw ex;
            }
            finally
            {
                conn.Close();
            }
        }

        // ============================================================
        // UPDATE MAHASISWA (dengan foto BLOB)
        // ============================================================
        public void UpdateMhs(
            string nim,
            string nama,
            string alamat,
            string jenisKelamin,
            DateTime tanggalLahir,
            string kodeProdi,
            byte[] foto)
        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            SqlCommand command =
                new SqlCommand("sp_UpdateMahasiswa", conn);
            command.Parameters.AddWithValue("pNIM", nim);
            command.Parameters.AddWithValue("pNama", nama);
            command.Parameters.AddWithValue("pAlamat", alamat);
            command.Parameters.AddWithValue("pJenisKelamin", jenisKelamin);
            command.Parameters.AddWithValue("pTanggalLahir", tanggalLahir);
            command.Parameters.AddWithValue("pNmProdi", kodeProdi);

            if (foto != null)
                command.Parameters.AddWithValue("pFoto", foto);
            else
                command.Parameters.AddWithValue("pFoto", DBNull.Value);

            command.CommandType = CommandType.StoredProcedure;
            command.ExecuteNonQuery();
        }

        // ============================================================
        // DELETE MAHASISWA
        // ============================================================
        public void DeleteMhs(string nim)
        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            SqlCommand cmd =
                new SqlCommand("sp_DeleteMahasiswa", conn);
            cmd.Parameters.AddWithValue("pNIM", nim);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.ExecuteNonQuery();
        }

        // ============================================================
        // RESET DATA (hapus semua & restore dari backup)
        // ============================================================
        public void ResetData()
        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            string deleteQuery = "DELETE FROM mahasiswa;";
            SqlCommand cmdDelete = new SqlCommand(deleteQuery, conn);
            cmdDelete.ExecuteNonQuery();

            string insertQuery = @"
                INSERT INTO mahasiswa
                SELECT * FROM mahasiswa_backup;";
            SqlCommand cmdInsert = new SqlCommand(insertQuery, conn);
            cmdInsert.ExecuteNonQuery();
        }

        // ============================================================
        // TEST INJECT (SQL Injection demo)
        // ============================================================
        public void testInject(string nim)
        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            string query =
                "Update mahasiswa set nama = 'HACKED' where NIM = " + nim;
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.ExecuteNonQuery();
        }

        // ============================================================
        // INSERT LOG
        // ============================================================
        public void InsertLog(string message)
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                SqlCommand cmd =
                    new SqlCommand("sp_LogMessage", conn);
                cmd.Parameters.AddWithValue("psn", message);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
            }
            catch
            {
                // Jangan throw agar tidak infinite loop
            }
        }

        // ============================================================
        // LOAD PRODI
        // ============================================================
        public DataTable LoadProdi()
        {
            DataTable dt = new DataTable();
            using (SqlConnection c =
                new SqlConnection(GetConnectionString()))
            {
                string query =
                    "SELECT KodeProdi FROM ProgramStudi";
                SqlDataAdapter adp =
                    new SqlDataAdapter(query, c);
                adp.Fill(dt);
            }
            return dt;
        }

        // ============================================================
        // GET PRODI (untuk Form Rekap)
        // ============================================================
        public DataTable getProdi()
        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            SqlCommand cmd =
                new SqlCommand(
                    "select namaprodi from prodi", conn);
            cmd.CommandType = CommandType.Text;
            dtProdi = new DataTable();
            da = new SqlDataAdapter(cmd);
            da.Fill(dtProdi);

            return dtProdi;
        }

        // ============================================================
        // GET DATA REKAP (untuk Form Report)
        // ============================================================
        public DataTable getDataRekap(string prodi, DateTime tanggalMasuk)
        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            SqlCommand cmd =
                new SqlCommand("sp_Report", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@inProdi", prodi);
            cmd.Parameters.AddWithValue(
                "@inTglMsuk",
                tanggalMasuk.Year.ToString());

            da = new SqlDataAdapter(cmd);
            dtMahasiswa = new DataTable();
            da.Fill(dtMahasiswa);
            return dtMahasiswa;
        }

        // ============================================================
        // REPORT MAHASISWA (untuk Form2)
        // ============================================================
        public DataTable ReportMahasiswa(string kodeProdi, int tahunMasuk)
        {
            DataTable dt = new DataTable();
            using (SqlConnection c =
                new SqlConnection(GetConnectionString()))
            {
                SqlCommand cmd =
                    new SqlCommand("sp_Report", c);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inProdi", kodeProdi);
                cmd.Parameters.AddWithValue("@inTglMsuk", tahunMasuk);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(dt);
            }
            return dt;
        }

        // ============================================================
        // GET ALL DATA CHART (sp_DashBoard)
        // ============================================================
        public DataTable getAllDataChart()
        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            SqlCommand cmd =
                new SqlCommand("sp_DashBoard", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            da = new SqlDataAdapter(cmd);
            dtMahasiswa = new DataTable();
            da.Fill(dtMahasiswa);
            return dtMahasiswa;
        }

        // ============================================================
        // GET DATA CHART BY TAHUN (sp_DashBoardByTahun)
        // ============================================================
        public DataTable getDataChartByTahun(DateTime thMasuk)
        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            SqlCommand cmd =
                new SqlCommand("sp_DashBoardByTahun", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@inTglMsuk", thMasuk.Year);
            da = new SqlDataAdapter(cmd);
            dtMahasiswa = new DataTable();
            da.Fill(dtMahasiswa);
            return dtMahasiswa;
        }
    }
}