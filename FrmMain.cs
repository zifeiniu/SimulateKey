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

namespace SimulateKey
{
    public partial class FrmMain : Form
    {
        SerialPort sp = new System.IO.Ports.SerialPort();
          
        public FrmMain()
        {
            InitializeComponent();
            System.Windows.Forms.Form.CheckForIllegalCrossThreadCalls = false;
            sp.DataReceived += Sp_DataReceived;
            sp.ErrorReceived += Sp_ErrorReceived;
        }

        private void Sp_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            
        }

        public DataTable GetTable() 
        {
            DataTable table = new DataTable();
            table.Columns.Add("Key");
            table.Columns.Add("名称");
            table.Columns.Add("值");
            table.Columns.Add("更新时间");
            table.Columns.Add("频率");
            table.Columns.Add("Max");
            table.Columns.Add("Min");
            return table;

        }

        private void Sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string orgLine = sp.ReadLine();
            //AddLog(orgLine);
            string Rline = orgLine.Replace('\r',' ').Replace('?', ' ').Trim();
            
            try
            {
                string[] items = Rline.Split(':');

                if (Double.TryParse(items[1], out Double res))
                {
                    if (Process(items[0], res))
                    {
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(orgLine  + ex.Message);
                
            }

            try
            {    
                string[] lines = Rline.Trim().Split(' ');
                for (int i = 0; i < lines.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(lines[i]))
                    {
                        continue;
                    }
                    string[] items = lines[i].Split(':');
                    if (items.Length == 2)
                    {
                        if (Double.TryParse(items[1], out Double res))
                        {
                            if (!Process(items[0], res))
                            {
                                Console.WriteLine(orgLine);
                            }
                        }
                    } 
                } 
            }
            catch (Exception ex)
            {
                Console.WriteLine(orgLine + ex.Message);
            } 
        }

        public void AddLog(string msg)
        {
            txtLog.AppendText(msg + "\r\n"); 
        }

        public bool Process(string key, double value) 
        {
            if (DicValue.TryGetValue(key, out DataModel data))
            {
                data.SetValue(value);
                return true;
            }
            else
            {
                string Fname = KeyDefind.GetName(key);
                if (Fname != null) 
                {
                    DicValue.Add(key, new DataModel() { Key = key, Fname = Fname, Value = value, lastUpdate = DateTime.Now });
                    return true;
                }
            }
            return false;

        }

        DataTable table;
        private void Form1_Load(object sender, EventArgs e)
        {
            table = GetTable();
            dataGridView1.DataSource = table;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (button1.Text=="关闭")
                {
                    sp.Close();
                    button1.Text ="打开";
                }
                
                sp.PortName = txtCom.Text;
                sp.BaudRate = int.Parse(txtbote.Text);
                sp.Open();
                button1.Text = "关闭";

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        public Dictionary<string, DataModel> DicValue = new Dictionary<string, DataModel>();

 
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                foreach (var key in DicValue.Keys)
                {
                    DataRow dr = table.AsEnumerable().Where(K => K["Key"].ToString() == key).FirstOrDefault();
                    if (dr != null)
                    {
                        dr["更新时间"] = DicValue[key].GetTime();
                        dr["频率"] = DicValue[key].frequency;
                        dr["值"] = DicValue[key].Value;
                        dr["Max"] = DicValue[key].Max;
                        dr["Min"] = DicValue[key].Min;
                    }
                    else
                    {
                        DataRow dr1 = table.NewRow();
                        dr1["Key"] = DicValue[key].Key;
                        dr1["名称"] = DicValue[key].Fname;
                        dr1["值"] = DicValue[key].Value;
                        dr1["更新时间"] = DicValue[key].GetTime();
                        dr1["频率"] = DicValue[key].frequency;
                        dr1["Max"] = DicValue[key].Max;
                        dr1["Min"] = DicValue[key].Min;
                        table.Rows.Add(dr1);
                    }
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine();
             
            }
            
            

        }
    }

   
}
