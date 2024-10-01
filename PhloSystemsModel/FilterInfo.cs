using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhloSystemsModel
{
    public class FilterInfo
    {
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public List<string> Sizes { get; set; }
        public string[] MostCommonWords { get; set; }
    }
}
