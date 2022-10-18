using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hrmis.Models.ViewModels.Application
{
    public class MobileTracking
    {
        public int Id { get; set; }
        public DateTime? DateTime { get; set; }
        public string Information { get; set; }
        public string Status { get; set; }
    }
}