using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace twpx.Model
{
    class Video
    {
        int id;
        int did;
        string name;
        DateTime time;
        int length;
        string path;
        string remark;

        
        public string Name { get => name; set => name = value; }
        public DateTime Time { get => time; set => time = value; }
        public int Length { get => length; set => length = value; }
        public string Path { get => path; set => path = value; }
        public string Remark { get => remark; set => remark = value; }
        public int Id { get => id; set => id = value; }
        public int Did { get => did; set => did = value; }
    }
}
