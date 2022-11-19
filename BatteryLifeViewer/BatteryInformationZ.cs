using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatteryLifeViewer
{
    internal class BatteryInformationZ
    {
        public float CurrentCapacity { get; set; }
        public float DesignMaxCapacity { get; set; }
        public float FullCapacity { get; set; }
        public float Remaining { get; set; }
        public float RemainingDesign { get; set; }
        public float BatteryLife { get; set; }
    }
}
