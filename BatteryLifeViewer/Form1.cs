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
            int index = listBox1.SelectedIndex;
            if (index < 0) return;
            var batteryInfo = battery.batteryInfoZ[index];
            label9.Text = batteryInfo.CurrentCapacity.ToString() + "Wh";
            label10.Text = batteryInfo.FullCapacity.ToString() + "Wh";
            label11.Text = batteryInfo.DesignMaxCapacity.ToString() + "Wh";
            label7.Text = batteryInfo.Remaining.ToString() + "%";
            label8.Text = batteryInfo.RemainingDesign.ToString() + "%";
            label5.Text = batteryInfo.BatteryLife.ToString() + "%";
            progressBar1.Value = (int)batteryInfo.Remaining;
            progressBar2.Value = (int)batteryInfo.RemainingDesign > progressBar2.Maximum ? progressBar2.Maximum : (int)batteryInfo.RemainingDesign;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Text = battery.BasicInfo();
            textBox1.Text += battery.AdvancedInfo();
            battery.Update();
            for (int i = 0; i < battery.batteryInfoZ.Length; i++)
            {
                listBox1.Items.Add("battery" + (i + 1));
            }
            listBox1.SelectedIndex = (listBox1.Items.Count > 0) ? 0 : -1;
            UpdateBattery();
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
