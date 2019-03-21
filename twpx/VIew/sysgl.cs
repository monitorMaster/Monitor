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
    public partial class Sysgl : Form
    {
        SqlSugarClient db = SugarDao.GetInstance();
        List<Building> buildings = new List<Building>();
        public Sysgl()
        {
            InitializeComponent();
            LoadData();
            List<String> blist = new List<String>();
            foreach (Building building in buildings)
            {
                blist.Add(building.Name);
            }
            comboBox1.DataSource = blist;
        }

        private void sysgl_Load(object sender, EventArgs e)
        {
            
        }

        public void LoadData()
        {
            buildings = db.Queryable<Building>().ToList();
            listView1.Items.Clear();
            var list = db.Queryable<Lab>().ToList();
            foreach (var i in list)
            {
                ListViewItem item = new ListViewItem();
                item.Text = i.Id.ToString();
                item.SubItems.Add(i.Name);
                item.SubItems.Add(i.Address);
                item.SubItems.Add(i.Bid.ToString());
                item.SubItems.Add(i.Bname);
                listView1.Items.Add(item);
            }
        }

        // 添加/更新信息
        public void saveData()
        {
            var lab = new Lab();
            var result = 0;
            lab.Name = textBox1.Text.Trim();
            lab.Address = textBox3.Text.Trim();
            lab.Bid = buildings.ElementAt(comboBox1.SelectedIndex).Id;
            lab.Bname = buildings.ElementAt(comboBox1.SelectedIndex).Name;
            if ("".Equals(textBox2.Text) || textBox2.Text == null)
            {
                result = db.Insertable(lab).ExecuteCommand();
            }
            else
            {
                lab.Id = int.Parse(textBox2.Text);
                result = db.Updateable(lab).ExecuteCommand();
            }
            
            LoadData();
            Console.WriteLine(result);
        }
        // 批量删除
        private void button3_Click(object sender, EventArgs e)
        {
            if (listView1.CheckedItems.Count == 0)
            {
                MessageBox.Show("未选中项！");
                return;
            }
            int[] ids = new int[listView1.CheckedItems.Count];
            for(int i = 0; i < listView1.CheckedItems.Count; i++)
            {
                ids[i] = int.Parse(listView1.CheckedItems[i].Text);
            }
            var result = db.Deleteable<Lab>().In(ids).ExecuteCommand();
            if (result == listView1.CheckedItems.Count) LoadData();

        }
        // 按钮添加或者更新
        private void button1_Click(object sender, EventArgs e)
        {
            saveData();
            
        }

        // 获取选中项
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0) {
                clearBlank();
                return;
            }
            textBox2.Text = listView1.SelectedItems[0].Text;
            textBox1.Text = listView1.SelectedItems[0].SubItems[1].Text;
            textBox3.Text = listView1.SelectedItems[0].SubItems[2].Text;
            comboBox1.Text = listView1.SelectedItems[0].SubItems[4].Text;
        }

        // 删除单项按钮
        private void button2_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("未选中项！");
                return;
            }
            int id = int.Parse(listView1.SelectedItems[0].Text);
            var result = db.Deleteable<Lab>().In(id).ExecuteCommand();
            if (result == 1) LoadData();

        }

        // 清除信息按钮
        private void button4_Click(object sender, EventArgs e)
        {
            clearBlank();
        }

        private void clearBlank()
        {
            textBox2.Text = "";
            textBox1.Text = "";
            textBox3.Text = "";
            comboBox1.Text = "";
        }
    }
}
