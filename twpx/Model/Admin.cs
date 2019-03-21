using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace twpx.Model
{
    class Admin
    {
        private int id;
        private String name;
        private String phone;
        private int bid;
        private String bname;
        private String username;
        private String password;

        public int Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public string Phone { get => phone; set => phone = value; }
        public int Bid { get => bid; set => bid = value; }
        public string Bname { get => bname; set => bname = value; }
        public string Username { get => username; set => username = value; }
        public string Password { get => password; set => password = value; }
    }
}
