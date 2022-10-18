using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hrmis.Models.ViewModels.Dashboard
{
    public class DashboardTotalsViewModel
    {
        public int Applications { get; set; }
        public int Orders { get; set; }
        public int Files { get; set; }
        public int Profiles { get; set; }
        public int HealthFacilities { get; set; }
    }
}