using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BatteryLifeViewer
{
    public partial class Form1 : Form
    {
        private readonly Battery battery;

        public Form1()
        {
            InitializeComponent();
            battery = new Battery();
        }

        private void UpdateBattery()
        {
            //battery.Update();
            var batteryInfo = battery.batteryInfoZ[0];//!
            label9.Text = batteryInfo.CurrentCapacity.ToString();
            label10.Text = batteryInfo.FullCapacity.ToString();
            label11.Text = batteryInfo.DesignMaxCapacity.ToString();
            label7.Text = batteryInfo.Remaining.ToString();
            label8.Text = batteryInfo.RemainingDesign.ToString();
            label5.Text = batteryInfo.BatteryLife.ToString();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Text = battery.BasicInfo();
            textBox1.Text += battery.AdvancedInfo();
            battery.Update();
            var info = battery.batteryInfoZ[0];
            label9.Text = info.CurrentCapacity.ToString();
            label10.Text = info.FullCapacity.ToString();
            label11.Text = info.DesignMaxCapacity.ToString();
            label7.Text = info.Remaining.ToString();
            label8.Text = info.RemainingDesign.ToString();
            label5.Text = info.BatteryLife.ToString();
            progressBar1.Value = (int)info.Remaining;
            progressBar2.Value = (int)info.RemainingDesign > progressBar2.Maximum ? progressBar2.Maximum : (int)info.RemainingDesign;
            for (int i = 0; i < battery.batteryInfoZ.Length; i++)
            {
                listBox1.Items.Add("battery" + (i + 1));
            }
            listBox1.SelectedIndex = 0;//!
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(battery.batteryInfoZ?[0].CurrentCapacity.ToString());
        }
    }
}
