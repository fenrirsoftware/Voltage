using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class DetayForm : Form
    {
        public DetayForm()
        {
            InitializeComponent();
            timer1.Start();
            this.Height = 182;
        }


    

        private async void DetayForm_Load(object sender, EventArgs e)
        {
            string dosya_dizin = AppDomain.CurrentDomain.BaseDirectory.ToString() + "energy-report.html";


        

           

            string outputFilePath = @"energy-report.html"; // kaydedilecek dosya adı
            string command = "powercfg /energy"; // cmd komutu
            ProcessStartInfo psi = new ProcessStartInfo("cmd.exe", "/c " + command);
            psi.RedirectStandardOutput = true;
            psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            psi.Verb = "runas";
            // Process nesnesi oluşturma ve çalıştırma
            Process process = Process.Start(psi);


            await InitializeAsync();
         
        }

        private async Task InitializeAsync()
        {
            await webView21.EnsureCoreWebView2Async(null);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            progressBar1.Value++;

            if (progressBar1.Value==65)
            {
                timer1.Stop();
                progressBar1.Visible = false;
                label1.Visible = false;
                webView21.Height=430;
                this.Height = 460;
                webView21.Visible = true;


                string htmlPath = Path.Combine(Application.StartupPath, "energy-report.html");
                string html = File.ReadAllText(htmlPath);
                webView21.NavigateToString(html);

            }
        }
    }
}
