using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AERC.Models.Service
{
    public class DBCommon
    {
        public class DbEvent
        {
            /// <summary>get and set DbMessage</summary>
            public string DbMessage { get; set; }
            /// <summary>get and set KeyValue</summary>
            public object KeyValue { get; set; }
        }
    }
}
