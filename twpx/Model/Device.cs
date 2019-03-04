using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace twpx.Model
{
    class Device
    {
        public string id
        {
            get;
            set;
        }
        public string ip
        {
            get;
            set;
        }
        public string port
        {
            get;
            set;
        }
        public string user
        {
            get;
            set;
        }
        public string pwd
        {
            get;
            set;
        }
        public string status = "离线";
        
        public void setStatus()
        {
            if (status.Equals("离线"))
            {
                this.status = "在线";
            }
            else
            {
                this.status = "离线";
            }
        }

        public string getStatus()
        {
            return status;
        }

    }
}
