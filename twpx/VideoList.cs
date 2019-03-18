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
    public partial class VideoList : Form
    {
        SqlSugarClient db = SugarDao.GetInstance();
        public VideoList()
        {
            InitializeComponent();
            LoadData();
        }


        public void LoadData()
        {
            listView1.Items.Clear();
            var list = db.Queryable<Record>().ToList();
            foreach (var i in list)
            {
                ListViewItem item = new ListViewItem();
                item.Text = i.id.ToString();
                item.SubItems.Add(i.cid.ToString());
                item.SubItems.Add(i.cname);
                item.SubItems.Add(i.date.ToString());
                item.SubItems.Add(i.path.ToString());
                item.SubItems.Add(i.filename);
                listView1.Items.Add(item);
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count <= 0)
            {
                return;
            }
            var si = listView1.SelectedItems[0];
            label2.Text = si.SubItems[0].Text;
            label9.Text = si.SubItems[1].Text;
            label10.Text = si.SubItems[2].Text;
            label11.Text = si.SubItems[3].Text;
            label12.Text = si.SubItems[4].Text;
            textBox1.Text = si.SubItems[5].Text;
        }

       

        private void VideoList_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(listView1.SelectedItems.Count <= 0)
            {
                MessageBox.Show("未选中录像文件");
                return;
            }
            var si = listView1.SelectedItems[0];
            var frm = new replay(si.SubItems[5].Text);
            frm.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            var si = listView1.SelectedItems[0];
            var frm = new replay(si.SubItems[5].Text);
            frm.Show();
        }
    }
}
