using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallCenterClassLibrary
{
    public class Call
    {
        public string Id { get; private set; }
        public int DurationInSec { get; set; }
        public Call(string id)
        {
            Id = id;
        }
    }
}
