using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReqBot
{
    public class User
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string NotificationOption { get; set; }

        public string Anonymity { get; set; }

        public bool CorrectEmailFormat { get; set; }
    }
}
