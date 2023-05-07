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
using System.Windows.Forms;
using HtmlAgilityPack;
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

        private void Form1_Load(object sender, EventArgs e)
        {
            //renk kodları aracılığıyla load sonrası renk değişimi  
            this.BackColor = Color.FromArgb(34, 40, 49);
            panel1.BackColor = Color.FromArgb(57, 62, 70);


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
                            textBox3.Text = row.InnerText.Replace("DESIGN CAPACITY", "").Trim();
                        }
                        if (row.InnerText.Contains("FULL CHARGE CAPACITY"))
                        {
                            textBox4.Text = row.InnerText.Replace("FULL CHARGE CAPACITY", "").Trim();
                        }

                        if (row.InnerText.Contains("REPORT TIME"))
                        {
                            textBox2.Text = row.InnerText.Replace("REPORT TIME", "").Trim();
                        }

                        if (row.InnerText.Contains("CHEMISTRY"))
                        {
                            textBox5.Text = row.InnerText.Replace("CHEMISTRY", "").Trim();
                        }

                        if (row.InnerText.Contains("SYSTEM PRODUCT NAME"))
                        {
                            textBox1.Text = row.InnerText.Replace("SYSTEM PRODUCT NAME", "").Trim();
                        }
                    }

                }





            }

        }

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

        private void button1_Click(object sender, EventArgs e)
        {
            DetayForm fr = new DetayForm();
            fr.Show();

        }






               
    }
}

