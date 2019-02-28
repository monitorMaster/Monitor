using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using twpx.Dao;
using twpx.Model;

namespace twpx
{
    public partial class Devices : Form
    {
        SqlSugarClient db = SugarDao.GetInstance();
        public Devices()
        {
            InitializeComponent();
            LoadData();
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string ip = textBox1.Text.Trim();
            string port = textBox2.Text.Trim();
            string user = textBox3.Text.Trim();
            string pwd = textBox4.Text.Trim();
            if(TextIsNull(ip)|| TextIsNull(port) || TextIsNull(user) || TextIsNull(pwd))
            {
                MessageBox.Show("选项不能为空！");
                return;
            }
            Device device = new Device();
            device.ip = ip;
            device.port = port;
            device.user = user;
            device.pwd = pwd;
            try
            {
                var t2 = db.Insertable(device).ExecuteCommand();
                Console.WriteLine("添加{0}个设备。", t2);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            LoadData();
        }

        public bool TextIsNull(string s)
        {
            if (s.Equals("") || s == "") return true;
            else return false;
        }

        
        public void LoadData()
        {
            listView1.Items.Clear();
            var list = db.Queryable<Device>().ToList();
            foreach (var i in list)
            {
                ListViewItem item = new ListViewItem();
                item.Text = i.id;
                item.SubItems.Add(i.ip);
                item.SubItems.Add(i.port);
                item.SubItems.Add(i.user);
                item.SubItems.Add(i.pwd);
                listView1.Items.Add(item);
            }
        }

        //移除设备
        private void button3_Click(object sender, EventArgs e)
        {
            if (listView1.CheckedItems.Count == 0)
            {
                MessageBox.Show("没有勾选相关设备");
                return;
            }
            else
            {
                int[] ids = new int[listView1.CheckedItems.Count];
                for(int i = 0; i < listView1.CheckedItems.Count; i++)
                {
                    ids[i] = Convert.ToInt32(listView1.CheckedItems[i].Text);
                }
                foreach(ListViewItem i in listView1.CheckedItems)
                {
                    i.Remove();
                }
                var t4 = db.Deleteable<Device>().In(ids).ExecuteCommand();
                Console.WriteLine("移除{0}个设备。",t4);
            }
        }
    }
}
