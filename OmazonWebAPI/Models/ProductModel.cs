using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OmazonWebAPI.Models
{
    public class ProductModel
    {
        public string Category { get; set; }
        public string Name { get; set; }
        public string Price { get; set; }
        public string ÍmagePath { get; set; }
        public int Stock { get; set; }
    }
}
