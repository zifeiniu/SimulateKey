using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulateKey
{
    public static class KeyDefind
    {
        public static Dictionary<string, string> DicDefinde = new Dictionary<string, string>();
        static KeyDefind() 
        {
            DicDefinde.Add("znz", "指南针");
            DicDefinde.Add("cx", "磁力X");
            DicDefinde.Add("cy", "磁力Y");
            DicDefinde.Add("cz", "磁力Z");

            DicDefinde.Add("jx", "加速度X");
            DicDefinde.Add("jy", "加速度Y");
            DicDefinde.Add("jz", "加速度Z");
            DicDefinde.Add("jqd", "加速度强度");

            DicDefinde.Add("zylt", "自由落体");
            DicDefinde.Add("zq", "向左倾斜");
            DicDefinde.Add("yq", "向右倾斜");

            DicDefinde.Add("zd", "震动");

            DicDefinde.Add("a", "按下A");
            DicDefinde.Add("b", "按下B");

            DicDefinde.Add("3g", "3G");

            DicDefinde.Add("ld", "亮度");


        }

        public static string GetName(string key) 
        {
            if (DicDefinde.TryGetValue(key, out string Fname))
            {
                return Fname;
            } 
            return null;
        }
    }
}
