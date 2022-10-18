using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hrmis.Models.Dto
{
    public class AppResponse<T> where T : class
    {
        public bool IsException { get; set; }
        public string Messages { get; set; }
        public T Data { get; set; }
    }
}