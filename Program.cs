using System;
using System.Windows.Forms;

namespace CRUDMahasiswaADO
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Ganti 'Form1' atau 'FormMahasiswa' menjadi 'FormDashboard'
            Application.Run(new FormDashboard());
        }
    }
}