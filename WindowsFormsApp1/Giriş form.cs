using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Giriş_form : Form
    {
        public Giriş_form()
        {
            InitializeComponent();
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            progressBar1.Value++;

            if (progressBar1.Value == 65)
            {
                timer1.Stop();
                Form1 fr = new Form1();
                fr.Show();

                this.Hide();
              


            }
        }

        private async void Giriş_form_Load(object sender, EventArgs e)
        {

            string dosya_dizin = AppDomain.CurrentDomain.BaseDirectory.ToString() + "energy-report.html";


                



            string outputFilePath = @"energy-report.html"; // kaydedilecek dosya adı
            string command = "powercfg /energy"; //win10 cmd komutu
            string command11 = "powercfg /energy /output energyReport.html /duration 60"; //win11 cmd komutu
            ProcessStartInfo psi = new ProcessStartInfo("cmd.exe", "/c " + command);
            psi.RedirectStandardOutput = true;
            psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            psi.Verb = "runas";
            // Process nesnesi oluşturma ve çalıştırma
            Process process = Process.Start(psi);


            

        }
    }
}
