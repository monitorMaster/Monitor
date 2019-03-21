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

namespace twpx.VIew
{
    public partial class Ptglygl : Form
    {
        SqlSugarClient db = SugarDao.GetInstance();
        List<Building> buildings = new List<Building>();
        public Ptglygl()
        {
            InitializeComponent();
            LoadData();
        }


        // 加载数据
        public void LoadData()
        {
            buildings = db.Queryable<Building>().ToList();
            List<String> blist = new List<String>();
            foreach (Building building in buildings)
            {
                blist.Add(building.Name);
            }
            comboBox1.DataSource = blist;
            listView1.Items.Clear();
            var list = db.Queryable<Admin>().ToList();
            foreach (var i in list)
            {
                ListViewItem item = new ListViewItem();
                item.Text = i.Id.ToString();
                item.SubItems.Add(i.Name);
                item.SubItems.Add(i.Phone);
                item.SubItems.Add(i.Bid.ToString());
                item.SubItems.Add(i.Bname);
                item.SubItems.Add(i.Username);
                item.SubItems.Add(i.Password);
                listView1.Items.Add(item);
            }
        }

        // 重置信息
        private void clearBlank()
        {
            textBox2.Text = "";
            textBox1.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            comboBox1.Text = "";
        }

        //重置按钮
        private void button1_Click(object sender, EventArgs e)
        {
            clearBlank();
        }
        //添加按钮
        private void button2_Click(object sender, EventArgs e)
        {
            var data = new Admin();
            var result = 0;
            data.Name = textBox1.Text.Trim();
            data.Phone = textBox3.Text.Trim();
            data.Username = textBox5.Text.Trim();
            data.Password = textBox4.Text.Trim();
            data.Bid = buildings.ElementAt(comboBox1.SelectedIndex).Id;
            data.Bname = buildings.ElementAt(comboBox1.SelectedIndex).Name;
            if ("".Equals(textBox2.Text) || textBox2.Text == null)
            {
                result = db.Insertable(data).ExecuteCommand();
            }
            else
            {
                data.Id = int.Parse(textBox2.Text);
                result = db.Updateable(data).ExecuteCommand();
            }
            LoadData();
            clearBlank();
        }
        // 刷新按钮
        private void button5_Click(object sender, EventArgs e)
        {
            LoadData();
        }
        // 批量删除按钮
        private void button4_Click(object sender, EventArgs e)
        {
            if (listView1.CheckedItems.Count == 0)
            {
                MessageBox.Show("未选中项！");
                return;
            }
            int[] ids = new int[listView1.CheckedItems.Count];
            for (int i = 0; i < listView1.CheckedItems.Count; i++)
            {
                ids[i] = int.Parse(listView1.CheckedItems[i].Text);
            }
            var result = db.Deleteable<Admin>().In(ids).ExecuteCommand();
            if (result == listView1.CheckedItems.Count) LoadData();
        }
        // 删除按钮
        private void button3_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("未选中项！");
                return;
            }
            int id = int.Parse(listView1.SelectedItems[0].Text);
            var result = db.Deleteable<Admin>().In(id).ExecuteCommand();
            if (result == 1) LoadData();
        }
        // 选中项
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                clearBlank();
                return;
            }
            textBox2.Text = listView1.SelectedItems[0].Text;
            textBox1.Text = listView1.SelectedItems[0].SubItems[1].Text;
            textBox3.Text = listView1.SelectedItems[0].SubItems[2].Text;
            comboBox1.Text = listView1.SelectedItems[0].SubItems[4].Text;
            textBox5.Text = listView1.SelectedItems[0].SubItems[5].Text;
            textBox4.Text = listView1.SelectedItems[0].SubItems[6].Text;
        }



        // END
    }
}
