using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BatteryLifeViewer
{
    internal class Battery
    {
        public BatteryInformationZ[] batteryInfoZ;

        public string BasicInfo()
        {
            string battery = "";
            PowerStatus powerStatus = SystemInformation.PowerStatus;
            foreach (ManagementBaseObject managementBaseObject in new ManagementObjectSearcher(new ObjectQuery("Select * FROM Win32_Battery")).Get())
            {
                foreach (PropertyData property in managementBaseObject.Properties)
                    battery += $"[{property.Name}] : [{property.Value}]\r\n";
            }
            return battery;
        }

        public void Update()
        {
            var batteryList2 = new List<BatteryInformationZ>();
            try
            {
                for (int i = 0; ; i++)
                {
                    BatteryInformation info = BatteryInfo.GetBatteryInformation2(i);
                    batteryList2.Add(new BatteryInformationZ
                    {
                        CurrentCapacity = info.CurrentCapacity / 1000f,
                        DesignMaxCapacity = info.DesignedMaxCapacity / 1000f,
                        FullCapacity = info.FullChargeCapacity / 1000f,
                        Remaining = (float)info.CurrentCapacity / (float)info.FullChargeCapacity * 100.0f,
                        RemainingDesign = (float)info.CurrentCapacity / (float)info.DesignedMaxCapacity * 100.0f,
                        BatteryLife = (float)info.FullChargeCapacity / (float)info.DesignedMaxCapacity * 100.0f
                    });
                }
            }
            catch (Exception)
            {
                // ぬるぽ
            }
            batteryInfoZ = batteryList2.ToArray();
        }

        public string AdvancedInfo()
        {
            string batt = "";
            int num1 = 0;
            int num2 = 0;
            int num3 = 0;
            try
            {
                BatteryInformation batteryInformation = BatteryInfo.GetBatteryInformation();
                num1 = (int)batteryInformation.CurrentCapacity;
                num2 = batteryInformation.DesignedMaxCapacity;
                num3 = batteryInformation.FullChargeCapacity;
            }
            catch (Exception ex)
            {
                batt += "(　´∀｀)＜ぬるぽ！！！！！\r\n";
                batt += "↑C#だからNullReferenceExceptionなんだよ\r\n";
                batt += "バッテリが装着されてないかもねｗ\r\n";
            }
            float num4 = (float)num1 / 1000f;
            float num5 = (float)num2 / 1000f;
            float num6 = (float)num3 / 1000f;
            string str = $"現在量:{num4}  設計容量:{num5}  満充電容量:{num6}";
            float num7 = (float)((double)num1 / (double)num3 * 100.0);
            float num8 = (float)((double)num1 / (double)num2 * 100.0);
            float num9 = (float)((double)num3 / (double)num2 * 100.0);
            batt += str + "\r\n";
            batt += $"満充電容量に対する残量: {num7}%\r\n";
            batt += $"設計容量に対する残量  : {num8}%\r\n";
            batt += $"現在の充電能力        : {num9}%\r\n";
            batt += $"\n";
            batt += $"表示上の電池残量: {num7} % / {100f} %\r\n";
            batt += $"実際の電池残量  : {num8} % / {num9} %\r\n";
            return batt;
        }
    }
}
