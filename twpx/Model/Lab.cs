using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace twpx.Model
{
    class Lab
    {

        private int id;
        private int bid;
        private String bname;
        private String name;
        private String address;

        public int Id { get => id; set => id = value; }
        public int Bid { get => bid; set => bid = value; }
        public string Bname { get => bname; set => bname = value; }
        public string Name { get => name; set => name = value; }
        public string Address { get => address; set => address = value; }
    }
}
