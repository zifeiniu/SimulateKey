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

        public double TotalMilliseconds;

        /// <summary>
        /// 指南针修复角度
        /// </summary>
        public int FixAnger=0;



        public DateTime lastUpdate;

        public string GetTime() 
        {
            return $"{lastUpdate.ToShortTimeString()} - ({TotalMilliseconds})";
        }

        public double frequency;
        

        public void SetValue(double v)
        {
            if (FixValue!=0)
            {
                double cha = Math.Abs(lastValue - v);
                if (cha > FixValue && cha < 365 - FixValue)
                {
                    return;
                }
            }
            
            this.lastValue = this.Value;

            this.Value = v;

            if (v > Max)
            {
                Max = v;
            }

            if (v < Min)
            {
                Min = v;
            }
            TotalMilliseconds = (DateTime.Now - this.lastUpdate).TotalMilliseconds;
            frequency = Math.Round(1000 / TotalMilliseconds, 2);
            this.lastUpdate = DateTime.Now; 
            UpdateAction?.Invoke(v); 
        }

        public double lastValue;

        /// <summary>
        /// 指南针反转
        /// </summary>
        public bool FanZhuan;

        /// <summary>
        /// 修复指南针乱闪，角度突然超过50，忽略
        /// </summary>
        public double FixValue=0;

        public int CompassVlaue() 
        {
            
            if (FanZhuan)
            {
                Value = Math.Abs(Value - 365);
            }
            
            return ((int)Value + FixAnger) % 365;
             
        }




        public override string ToString()
        {
            return String.Join("-", this.Key, this.Fname, this.Value.ToString());
        }

    }
}
