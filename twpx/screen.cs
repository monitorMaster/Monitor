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
    public partial class Screen : Form
    {
        SqlSugarClient db = SugarDao.GetInstance();
        public Screen()
        {
            InitializeComponent();
            LoadData();
        }

        public void LoadData()
        {
            listView1.Items.Clear();
            var list = db.Queryable<Device>().ToList();
            foreach(var i in list)
            {
                Console.WriteLine(i.ip);
                ListViewItem item = new ListViewItem();
                item.Text = i.id;
                item.SubItems.Add(i.ip);
                listView1.Items.Add(item);
            }
        }

        private void Screen_Load(object sender, EventArgs e)
        {

        }
    }
}
