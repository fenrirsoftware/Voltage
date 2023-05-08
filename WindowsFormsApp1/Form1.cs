using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection.Emit;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using HtmlAgilityPack;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {

        string dosya_dizini = AppDomain.CurrentDomain.BaseDirectory.ToString() + "battery-report.html";
        public Form1()
        {
            InitializeComponent();
            timer1.Start();
           


        }
        private Image[] framesfull;
     
        private int currentFrameIndex;

        public double fcc1;
        public double dc1;
        private async void Form1_Load(object sender, EventArgs e)
        {
            await InitializeAsync();


         






            textBox9.Text = textBox6.Text;
            //renk kodları aracılığıyla load sonrası renk değişimi  
            this.BackColor = Color.FromArgb(34, 40, 49);
            panel1.BackColor = Color.FromArgb(57, 62, 70);
            //8
          

           
            bool isRunningWithAdministrator = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
            if (!isRunningWithAdministrator)
            {
                MessageBox.Show("Your System is Windows 7/8/10. \nPlease Right Click on this tool. \nThen choose Run as administrator.", "Voltage 1.0", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

            try
            {
                this.runWithCmd("powercfg /batteryreport && exit");
                Thread.Sleep(2000);



            }
            catch
            {
                MessageBox.Show("Cannot get battery report, are you using a laptop with battery?");
            }


            if (File.Exists(dosya_dizini) == true)
            {

                HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                // There are various options, set as needed
                htmlDoc.OptionFixNestedTags = true;
                // filePath is a path to a file containing the html
                htmlDoc.Load(@"battery-report.html");
                foreach (HtmlNode table in htmlDoc.DocumentNode.SelectNodes("//table"))
                {
                    foreach (HtmlNode row in table.SelectNodes("tr"))
                    {
                        if (row.InnerText.Contains("DESIGN CAPACITY"))
                        {
                            string dc = row.InnerText.Replace("DESIGN CAPACITY", "").Replace("mWh", "").Trim();
                            dc1 = Convert.ToDouble(dc);


                            textBox3.Text = row.InnerText.Replace("DESIGN CAPACITY", "").Trim();
                            //string text = row.InnerText.Replace("DESIGN CAPACITY", "").Trim(); // Ayarlanacak metin
                            //SetTextBoxSizeByText(textBox3, text);


                        }



                        if (row.InnerText.Contains("FULL CHARGE CAPACITY"))
                        {
                            string fcc = row.InnerText.Replace("FULL CHARGE CAPACITY", "").Replace("mWh", "").Trim();
                            fcc1 = Convert.ToDouble(fcc);


                            textBox4.Text = row.InnerText.Replace("FULL CHARGE CAPACITY", "").Trim();
                            textBox7.Text = row.InnerText.Replace("FULL CHARGE CAPACITY", "").Trim();
                            //string text = row.InnerText.Replace("FULL CHARGE CAPACITY", "").Trim(); // Ayarlanacak metin
                            //SetTextBoxSizeByText(textBox4, text);

                        }

                        if (row.InnerText.Contains("REPORT TIME"))
                        {
                            textBox2.Text = row.InnerText.Replace("REPORT TIME", "").Trim();
                            //  string text = row.InnerText.Replace("REPORT TIME", "").Trim(); // Ayarlanacak metin
                            //SetTextBoxSizeByText(textBox2, text);
                        }

                        if (row.InnerText.Contains("CHEMISTRY"))
                        {
                            textBox5.Text = row.InnerText.Replace("CHEMISTRY", "").Trim();
                            // string text = row.InnerText.Replace("CHEMISTRY", "").Trim(); // Ayarlanacak metin
                            //SetTextBoxSizeByText(textBox5, text);
                        }

                        if (row.InnerText.Contains("SYSTEM PRODUCT NAME"))
                        {
                            textBox1.Text = row.InnerText.Replace("SYSTEM PRODUCT NAME", "").Trim();
                            // string text = row.InnerText.Replace("SYSTEM PRODUCT NAME", "").Trim(); // Ayarlanacak metin
                            //SetTextBoxSizeByText(textBox1, text);
                        }

                    }

                    foreach (HtmlNode row in table.SelectNodes("tr"))
                    {
                        if (row.InnerText.Contains("Since OS install"))
                        {
                            string rowText = row.InnerText.Replace("Since OS install", "").Trim();
                            string[] values = rowText.Split(new string[] { "-", "\t" }, StringSplitOptions.RemoveEmptyEntries);

                            if (values.Length >= 2)
                            {
                                textBox11.Text=(values[0].Trim());
                            }
                        }
                    }


                }


                // Timer'ı başlat
                timer2.Start();

                int healt = Convert.ToUInt16(fcc1 / dc1 * 100);
                textBox8.Text = "%" + healt.ToString();
           
                if (healt >= 0 && healt <= 10)
                {
                    textBox10.Text = "Risk";
                    framesfull = new Image[]
                    {
                        Image.FromFile("DeathV0.png"),
                        Image.FromFile("vbos.png"),
                   };

                }

                else if (healt > 10 && healt <= 20)
                {
                    textBox10.Text = "Çok Kötü";
                    framesfull = new Image[]
                    {
                        Image.FromFile("zeroV.png"),
                        Image.FromFile("vbos.png"),
                   };

                }
                else if (healt > 20 && healt <= 40)
                {
                    textBox10.Text = "Kötü";

                    framesfull = new Image[]
                     {
                        Image.FromFile("QuarterV.png"),
                        Image.FromFile("vbos.png"),
                    };
                }

                else if (healt > 40 && healt <= 60)
                {
                    textBox10.Text = "Orta";
                    framesfull = new Image[]
                    {
                        Image.FromFile("HalfV.png"),
                        Image.FromFile("vbos.png"),
                   };

                }
                else if (healt > 60 && healt <= 100)
                {
                    textBox10.Text = "İyi";
                    framesfull = new Image[]
                    {
                        Image.FromFile("FULLv.png"),
                        Image.FromFile("vbos.png"),
                   };


                }


                currentFrameIndex = 0;
                pictureBox1.Image = framesfull[currentFrameIndex];

            }

        }

        /* private void SetTextBoxSizeByText(TextBox textBox, string text)
         {
             // TextBox kontrolünün mevcut yazı tipini alır
             Font font = textBox.Font;

             // Geçici bir grafik nesnesi oluşturulur
             using (Graphics graphics = textBox.CreateGraphics())
             {
                 // Metnin boyutunu ölçer
                 SizeF textSize = graphics.MeasureString(text, font);

                 // TextBox kontrolünün yeni genişlik ve yüksekliği ayarlanır
                 int newWidth = (int)textSize.Width + SystemInformation.VerticalScrollBarWidth + 2; // Scrollbar'ı da hesaplamak için genişliğe eklendi
                 int newHeight = (int)textSize.Height + 2; // Bazı ek boşluklar için yüksekliğe eklendi

                 textBox.Width = newWidth;
                 textBox.Height = newHeight;
             }
         }
        */
      
        private void timer1_Tick(object sender, EventArgs e)
        {
            // Batarya bilgilerini almak için bir ManagementObjectSearcher nesnesi oluştur
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Battery");

            // Bataryanın voltajını TextBox'a yazdır
            foreach (ManagementObject battery in searcher.Get())
            {
                object designVoltageObj = battery["DesignVoltage"];
                if (designVoltageObj != null)
                {

                    //mV değerini Vye çevirmek için  mV/1000 yapıyorum 
                    UInt64 voltageInMillivolts = Convert.ToUInt64(designVoltageObj);
                    double voltageInVolts = voltageInMillivolts / 1000.0;
                    textBox6.Text = voltageInVolts.ToString("F2") + " V";
                }
            }

        }


        private void runWithCmd(string command)
        {
            try
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process();  //process başlatıyorum
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;


                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = "/c" + command;
                startInfo.Verb = "runas";  //admin olarak çalıştırma
                process.StartInfo = startInfo;
                process.Start();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());  //hata yakalama
            }
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            // Batarya bilgilerini almak için bir ManagementObjectSearcher nesnesi oluştur
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Battery");

            // Bataryanın voltajını TextBox'a yazdır
            foreach (ManagementObject battery in searcher.Get())
            {
                object designVoltageObj = battery["DesignVoltage"];
                if (designVoltageObj != null)
                {

                    //mV değerini Vye çevirmek için  mV/1000 yapıyorum 
                    UInt64 voltageInMillivolts = Convert.ToUInt64(designVoltageObj);
                    double voltageInVolts = voltageInMillivolts / 1000.0;
                    textBox6.Text = voltageInVolts.ToString("F2") + " V";
                }
            }
        }

     
       
      
        private void sonuçToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void detaylarıGösterToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void detayToolStripMenuItem_Click(object sender, EventArgs e)
        {
         

            try
            {
                string htmlPath = Path.Combine(Application.StartupPath, "energy-report.html");
                string html = File.ReadAllText(htmlPath);
                webView21.Visible = true;
                label1.Visible = false; label2.Visible = false; label3.Visible = false; label4.Visible = false; label5.Visible = false; label6.Visible = false;
                textBox1.Visible = false; textBox2.Visible = false; textBox3.Visible = false; textBox4.Visible = false; textBox5.Visible = false; textBox6.Visible = false;
                webView21.NavigateToString(html);
            }
            catch (Exception)
            {
                MessageBox.Show("Cihazınız Detay görüntülenmesine izin vermemektedir. Program sahibi ile iletişime geçiniz.");
            }

        }
        private async Task InitializeAsync()
        {
            await webView21.EnsureCoreWebView2Async(null);
        }
        private void detay1_Load(object sender, EventArgs e)
        {

        }

        private void sonuçToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            webView21.Visible = false;
            label1.Visible = false; label2.Visible = false; label3.Visible = false; label4.Visible = false; label5.Visible = false; label6.Visible = false;
            textBox1.Visible = false; textBox2.Visible = false; textBox3.Visible = false; textBox4.Visible = false; textBox5.Visible = false; textBox6.Visible = false;
            panel2.Visible = true;
        }

        private void genelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panel2.Visible = false;
            webView21.Visible = false;
            label1.Visible = true; label2.Visible = true; label3.Visible = true; label4.Visible = true; label5.Visible = true; label6.Visible = true;
            textBox1.Visible = true; textBox2.Visible = true; textBox3.Visible = true; textBox4.Visible = true; textBox5.Visible = true; textBox6.Visible = true;
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            
        }

       
        

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            // Sonraki resmi göster
            currentFrameIndex++;
            if (currentFrameIndex >= framesfull.Length)
            {
                currentFrameIndex = 0;
            }
            pictureBox1.Image = framesfull[currentFrameIndex];
        }
    }
}

