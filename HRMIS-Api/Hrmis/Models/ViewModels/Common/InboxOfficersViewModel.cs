using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hrmis.Models.ViewModels.Common
{
    public class InboxOfficersViewModel
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public DateTime DateTime { get; set; }
    }
}