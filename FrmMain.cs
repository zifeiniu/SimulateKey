using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using WindowsInput;
using WindowsInput.Native;

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
        DataModel dm = new DataModel();
        private void Sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string orgLine = sp.ReadLine();

            //if (orgLine.Contains("znz"))
            //{
            //    dm.SetValue(1);
            //    this.Text = dm.frequency.ToString();
            //}
            //return;

            //AddLog(orgLine);
            string Rline = orgLine.Replace('\r', ' ').Replace('?', ' ').Trim();

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
                Console.WriteLine(orgLine + ex.Message);

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

        Series Series1;
        DataTable table;
        private void Form1_Load(object sender, EventArgs e)
        {
            table = GetTable();
            dataGridView1.DataSource = table;
            LoadJson();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (button1.Text == "关闭")
                {
                    sp.Close();
                    button1.Text = "打开";
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
                AddLog(ex.Message);

            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                string Key = table.Rows[e.RowIndex]["Key"].ToString();
                if (DicValue.TryGetValue(Key, out DataModel value))
                {
                    AddSeries(value);
                    tabControl1.SelectedTab = tabPage2;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine();
            }
        }

        DataModel CompassModel;
        private void button4_Click(object sender, EventArgs e)
        {
            InitCompass();
            if (CompassModel == null)
            {
                if (DicValue.TryGetValue("znz", out DataModel value))
                {
                    CompassModel = value;
                    CompassModel.UpdateAction = this.SetAngle;

                    checkBox1.Checked = CompassModel.FanZhuan;
                    txtxiuzheng.Text = CompassModel.FixAnger.ToString();

                }
                else
                {
                    MessageBox.Show("没有指南针数据");
                }
            } 
        }
         
        private void button5_Click(object sender, EventArgs e)
        {
            if (CompassModel != null)
            {
                CompassModel.FixAnger = int.Parse(txtxiuzheng.Text);
            }
        } 

        #region 指南针
        private const int cnt = 365;//数量
        private float angle = (float)(2 * Math.PI / cnt);
        private const int R = 500;//长度
        Bitmap bmp ;
        Graphics g ;

        public void InitCompass() {
            bmp = new Bitmap(1000, 1000);
            g = Graphics.FromImage(bmp); 
        }
        Point LastPoint = new Point(0, 0);

        

        public void SetAngle(double value)
        {
            int jiaodu = CompassModel.CompassVlaue();

            UseComPassKeyBoard(jiaodu);
            double a = jiaodu * angle;
            labjiaodu.Text = jiaodu.ToString();
            Point ps = new Point(0, 0);
            Point pe = new Point((int)(R * Math.Cos(a)), (int)(R * Math.Sin(a)));
            Change(ref ps);
            Change(ref pe);
            g.DrawLine(new Pen(Color.White, 5), ps, LastPoint);
            LastPoint = pe;
            g.DrawLine(new Pen(Color.Red, 5), ps, pe);
            this.pictureBox1.Image = bmp; 
        }
        private void Change(ref Point p)
        {
            Size sz = this.ClientSize;
            p.X += bmp.Width / 2;
            p.Y = bmp.Height / 2 - p.Y;
        }

        #endregion

     

        private void button7_Click(object sender, EventArgs e)
        {
            SaveConfig();
        }
        public void LoadJson() 
        {
            if (!File.Exists(configFile))
            {
                return;
            }
            string json = File.ReadAllText(configFile);

            DataModel[] list = Newtonsoft.Json.JsonConvert.DeserializeObject<DataModel[]>(json);
            for (int i = 0; i < list.Length; i++)
            {
                if (!DicValue.ContainsKey(list[i].Key))
                {
                    DicValue.Add(list[i].Key, list[i]);
                }
            }
            
        }

        string configFile = "data.json";
        public void SaveConfig() 
        {
           DicValue.Values.ToList().ForEach(K => K.UpdateAction = null);
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(DicValue.Values.ToArray());
            File.WriteAllText(configFile, json);
        }

        Dictionary<string, Series> SeriesDic = new Dictionary<string, Series>();

        int PointPos = 0;

        public void AddSeries(DataModel dm) 
        {
            try
            {
                Series1 = new Series();
                Series1.ChartType = SeriesChartType.Spline;
                Series1.Name = dm.Fname;
                this.chart1.Series.Add(Series1);
                

                dm.UpdateAction += delegate (double x)
                {
                    try
                    {
                        PointPos++;
                        DataPoint dataPoint1 = new DataPoint(PointPos, x);
                        Series1.Points.Add(dataPoint1);
                    }
                    catch (Exception)
                    {

                    }
                };
            }
            catch (Exception)
            {

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                PointPos = 0;
                foreach (var key in SeriesDic.Keys)
                {
                    SeriesDic[key].Points.Clear();
                }
            }
            catch (Exception)
            {

            }
        }
         
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (CompassModel != null)
            { 
                CompassModel.FanZhuan  = checkBox1.Checked;   
            }
        }
        
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            
        }
        InputSimulator imn = new InputSimulator();

        public void UseComPassKeyBoard(int jiaodu)
        {
          
            try
            {
                if (!checkBox2.Checked)
                {
                    return;
                }
                if (jiaodu > 130 && jiaodu < 230)
                {
                    imn.Keyboard.KeyPress(VirtualKeyCode.VK_A);
                    return;
                }

                if (jiaodu > 300 || jiaodu < 50)
                {

                    imn.Keyboard.KeyPress(VirtualKeyCode.VK_D);
                 

                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

          



        }
    }


}
