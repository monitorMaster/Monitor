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
using System.Runtime.InteropServices;

namespace twpx
{
    
    public partial class Devices : Form
    {
        SqlSugarClient db = SugarDao.GetInstance();
        Common Dcommon = new Common();
        //监听布防需要的变量
        private Int32 m_lUserID = -1;
        //private
        private Int32 m_lAlarmHandle = -1;//布防句柄
        private Int32 iListenHandle = -1;
        private int iDeviceNumber = 0; //添加设备个数
        private int iPicNumber = 0; //图片序号
        private uint iLastErr = 0;
        private string strErr;
        //private CHCNetSDK.MSGCallBack_V31 m_falarmData_V31 = null;
        private CHCNetSDK.MSGCallBack m_falarmData = null;
        public delegate void UpdateListBoxCallback(string strAlarmTime, string strDevIP, string strAlarmMsg);
        CHCNetSDK.NET_VCA_TRAVERSE_PLANE m_struTraversePlane = new CHCNetSDK.NET_VCA_TRAVERSE_PLANE();
        CHCNetSDK.NET_VCA_AREA m_struVcaArea = new CHCNetSDK.NET_VCA_AREA();
        CHCNetSDK.NET_VCA_INTRUSION m_struIntrusion = new CHCNetSDK.NET_VCA_INTRUSION();
        CHCNetSDK.UNION_STATFRAME m_struStatFrame = new CHCNetSDK.UNION_STATFRAME();
        CHCNetSDK.UNION_STATTIME m_struStatTime = new CHCNetSDK.UNION_STATTIME();
        //private CHCNetSDK.MSGCallBack_V31 m_falarmData_V31 = null;

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
                            Dcommon.AddCL(camera);
                            this.listView1.SelectedItems[i].SubItems[5].Text = "在线";
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                    else//设备在线就注销
                    {
                        count = Dcommon.GetCamera(this.listView1.SelectedItems[i].SubItems[1].Text);
                        try
                        {
                            Dcommon.RemoveCLByI(count - 1);
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

        //设备布防/撤防
        private void button5_Click(object sender, EventArgs e)
        {
            for(int i=0; i<Dcommon.GetCount(); i++)
            {
                if(Dcommon.GetAlarmHandle(i) < 0)
                {
                    setAlarm();
                }
                else
                {
                    closeAlarm();
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

        //读取设备信息
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
                //自动登录
                //Console.WriteLine("ip = " + i.ip + "\nport = " + i.port + "\nusername = " + i.user + "\npwd = " + i.pwd);
                if(Dcommon.AddCL(i.ip, Convert.ToInt16(i.port), i.user, i.pwd))
                {
                    i.setStatus();
                }
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

        //布防
        private void setAlarm()
        {
            CHCNetSDK.NET_DVR_SETUPALARM_PARAM struAlarmParam = new CHCNetSDK.NET_DVR_SETUPALARM_PARAM();
            struAlarmParam.dwSize = (uint)Marshal.SizeOf(struAlarmParam);
            struAlarmParam.byLevel = 1; //0- 一级布防,1- 二级布防
            struAlarmParam.byAlarmInfoType = 1;//智能交通设备有效，新报警信息类型
            struAlarmParam.byFaceAlarmDetection = 1;//1-人脸侦测

            for (int i = 0; i < Dcommon.GetCount(); i++)
            {
                Dcommon.SetAlarm(i, struAlarmParam);
                m_lAlarmHandle = Dcommon.GetAlarmHandle(i);
                if (m_lAlarmHandle < 0)
                {
                    iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    strErr = "布防失败，错误号：" + iLastErr; //布防失败，输出错误号
                    Dcommon.AddLog(strErr);
                }
                else
                {
                    strErr = "布防成功";
                    Dcommon.AddLog(strErr);
                    MessageBox.Show(strErr);
                }
                //btn_SetAlarm.Enabled = false;
            }
        }

        //撤防
        private void closeAlarm()
        {
            for (int i = 0; i < iDeviceNumber; i++)
            {
                if (m_lAlarmHandle >= 0)
                {
                    if (!Dcommon.CloseAlarm(i))
                    {
                        iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                        strErr = "撤防失败，错误号：" + iLastErr; //撤防失败，输出错误号
                        Dcommon.AddLog(strErr);
                    }
                    else
                    {
                        strErr = "撤防成功";
                        MessageBox.Show(strErr);
                        Dcommon.AddLog(strErr);
                        Dcommon.SetAlarmHandle(i, -1);
                    }
                }
                else
                {
                    strErr = "未布防";
                    MessageBox.Show(strErr);
                    Dcommon.AddLog(strErr);
                }
            }
            //btn_SetAlarm.Enabled = true;
        }

        //退出
        private void exitAlam()
        {
            //撤防
            //btnCloseAlarm_Click(sender, e);
            closeAlarm();

        }
    }
}
