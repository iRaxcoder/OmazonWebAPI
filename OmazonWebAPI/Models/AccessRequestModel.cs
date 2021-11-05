using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OmazonWebAPI.Models
{
    public class AccessRequestModel
    {
        public int RequestId { get; set; }
        public string Key { get; set; }
        public string Details{ get; set; }
        public int State { get; set; }
    }
}
