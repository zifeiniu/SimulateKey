using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulateKey
{
    public class DataModel
    {
        public Action<double> UpdateAction;

        public string Key;

        public string Fname;

        public double Value;

        public double Min;

        public double Max;

        public DateTime lastUpdate;

        public string GetTime() 
        {
            return lastUpdate.ToShortTimeString() + ":" + lastUpdate.Second + ":"+ lastUpdate.Millisecond;
        }

        public double frequency;
        

        public void SetValue(double v)
        {
            this.Value = v;
            if (v > Max)
            {
                Max = v;
            }

            if (v < Min)
            {
                Min = v;
            }
            frequency = Math.Round(1000 / (DateTime.Now - this.lastUpdate).TotalMilliseconds, 2);
            this.lastUpdate = DateTime.Now;
            UpdateAction?.Invoke(v); 
        }

        public override string ToString()
        {
            return String.Join("-", this.Key, this.Fname, this.Value.ToString());
        }

    }
}
