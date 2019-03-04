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
using NVRCsharpDemo;

namespace twpx
{
    
    public partial class Devices : Form
    {
        SqlSugarClient db = SugarDao.GetInstance();
        Common Dcommon = new Common();
        public Devices()
        {
            InitializeComponent();
            LoadData();
        }

        //登录、注销设备
        private void button4_Click(object sender, EventArgs e)
        {
            
            string ip;//存储ip
            int count = 0;//存储得到链表中确定设备的位置
            if (listView1.SelectedItems.Count == 0) return;
            else
            {
                int selectCount = this.listView1.SelectedItems.Count;
                Camera camera = null;
                for(int i=0; i<selectCount; i++)
                {
                    ip = this.listView1.SelectedItems[i].SubItems[5].Text;
                    if (ip.Equals("离线"))//如果设备离线就登录
                    {
                        camera = new Camera(this.listView1.SelectedItems[i].SubItems[1].Text,
                        Convert.ToInt16(this.listView1.SelectedItems[i].SubItems[2].Text),
                        this.listView1.SelectedItems[i].SubItems[3].Text,
                        this.listView1.SelectedItems[i].SubItems[4].Text);
                        try
                        {
                            Dcommon.addCL(camera);
                            this.listView1.SelectedItems[i].SubItems[5].Text = "在线";
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                    else//设备在线就注销
                    {
                        count = Dcommon.getCamera(this.listView1.SelectedItems[i].SubItems[1].Text);
                        try
                        {
                            Dcommon.removeCLByI(count - 1);
                            this.listView1.SelectedItems[i].SubItems[5].Text = "离线";
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        
                    }

                    camera = null;
                    //把新建的对象登录，修改状态
                }
                
            }
            
        }


        //添加设备
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
                item.SubItems.Add(i.getStatus());
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

        //更新设备
        private void button2_Click(object sender, EventArgs e)
        {

        }

        //批量导入设备
        private void button6_Click(object sender, EventArgs e)
        {

        }
    }
}
