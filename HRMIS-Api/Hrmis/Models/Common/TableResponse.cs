using Hrmis.Models.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hrmis.Models.Common
{
    public class TableResponse<T> where T : class
    {
        public List<T> List { get; set; }
        public int Count { get; set; }
    }
}