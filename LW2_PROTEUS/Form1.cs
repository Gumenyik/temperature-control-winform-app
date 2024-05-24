using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace LW2_PROTEUS
{
    public partial class Form1 : Form
    {
        SerialPort port;
        string data = "";
        string[] M;
        public Form1()
        {
            InitializeComponent();
            M = new string[] { "3F", "06", "5B", "4F", "66", "6D", "7D", "07", "7F", "6F", "40" };
            try
            {
                port = new SerialPort("COM1", 9600, Parity.None, 8, StopBits.One);//ініціалізуємо послідовний порт
                port.Open();//відкриваємо послідовний порт
                port.NewLine = "\r";//встановлюємо символ перенесення строки
            }
            catch (Exception)
            {
                button1.Enabled = false;
                MessageBox.Show("Порт не підключений");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string temperature = "";
            port.RtsEnable = true;//інформуємо мікроконтролер про наявність даних для передачі
            while (!port.CtsHolding) System.Threading.Thread.Sleep(1);//очікуємо готовність мікроконтролера прийняти дані
            port.WriteLine("0");//передаємо команду для зчитування температури
            port.RtsEnable = false;//більше немає, що передавати
            temperature = port.ReadLine();//зчитуємо температуру
            temperature = temperature.Substring(temperature.Length - 4);
            Int16 c = Convert.ToInt16(temperature, 16);
            label1.Text = ((double)c / 16).ToString() + "°C";
            data = ((double)c / 16).ToString();
            button2.Enabled = true;
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            port.Close();//закриваємо послідовний порт
        }

        private void button2_Click(object sender, EventArgs e)
        {
            port.RtsEnable = true;//інформуємо мікроконтролер про наявність даних для передачі
            while (!port.CtsHolding) System.Threading.Thread.Sleep(1);//очікуємо готовність мікроконтролера прийняти дані
            port.WriteLine("1");//передаємо команду 
            port.RtsEnable = false;//більше немає, що передавати

            string s1, s2, s3, s4;
            int a1, a2;
            int a = 0;
            if (data.Length > 3) MessageBox.Show("Невірний формат");
            else
            {
                if (int.TryParse(data, out a))
                {
                    if(a<0)
                    {
                        a *= (-1);
                        a1 = a / 10;
                        a2 = a % 10;
                        s1 = M[10];
                        s2 = M[a1];
                        s3 = M[a2];
                        s4 = M[0];
                    }
                    else
                    {
                        a1 = a / 10;
                        a2 = a % 10;
                        s1 = M[0];
                        s2 = M[a1];
                        s3 = M[a2];
                        s4 = M[0];
                    }
                    port.WriteLine(s1 + s2 + s3 + s4);//надсилаємо на послідовний порт значення, які слід відобразити на семисегментних
                }
                else MessageBox.Show("Невірний формат");
            }


        }
    }
}
