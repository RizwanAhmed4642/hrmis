using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Hrmis.Models.Common.Utility;

namespace Hrmis.Models.Common
{
    public class JsonResponseBody
    {
        public StatusEnum  Status { get; set; }
        public object Body { get; set; }
        public Exception Exception { get; set; }
        public bool HasException { get; set; }
    }
}