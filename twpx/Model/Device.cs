namespace twpx.Model
{
    class Device
    {
        public string id
        {
            get;
            set;
        }
        public string lid
        {
            get;
            set;
        }
        public string lname
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
        public string username
        {
            get;
            set;
        }
        public string password
        {
            get;
            set;
        }
        public string status = "离线";
        
        public void setStatus()
        {
            if (status.Equals("离线"))
            {
                status = "在线";
            }
            else
            {
                status = "离线";
            }
        }

        public string getStatus()
        {
            return status;
        }

    }
}
