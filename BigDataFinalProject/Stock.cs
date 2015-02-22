using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigDataFinalProject
{
    public class Stock
    {
        public int Cluster { get; set; }
        public string StockName { get; set; }
        public int DateNumber { get; set; }
        public double OpenValue { get; set; }
        public double CloseValue { get; set; }
        public double HighValue { get; set; }
        public double LowValue { get; set; }
    }
}
