using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace twpx.Model
{
    class SadminModel
    {
        private int id;
        private String username;
        private String password;
        private String name;
        private String phone;

        public int Id { get => id; set => id = value; }
        public string Username { get => username; set => username = value; }
        public string Password { get => password; set => password = value; }
        public string Name { get => name; set => name = value; }
        public string Phone { get => phone; set => phone = value; }

        public SadminModel()
        {
        }
    }
}
