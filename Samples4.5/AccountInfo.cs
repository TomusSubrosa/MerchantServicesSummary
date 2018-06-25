using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingSamples.Content 
{
    public class AccountInfo
    {
        public ulong AccountID;
        public string AccountName = "";

        public AccountInfo(ulong id, string name)
        {
            this.AccountID = id;
            this.AccountName = name;
        }
    }

}
