using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hrmis.Models.Dto
{
    public class HFTypeCounts
    {
        public HFTypeCounts(string typeName, int count)
        {
            TypeName = typeName;
            Count = count;
        }
        public string TypeName { get; set; }
        public int Count { get; set; }
    }
}