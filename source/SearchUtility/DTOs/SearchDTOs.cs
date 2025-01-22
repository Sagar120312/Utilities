using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchUtility.DTOs
{
    public class FilterCondition
    {
        public string Field { get; set; }
        public string Operator { get; set; }  // eq, gt, lt, contains, etc.
        public object Value { get; set; }
    }

    public class SortCondition
    {
        public string Field { get; set; }
        public string Order { get; set; }  // asc or desc
    }
}
