using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hrmis.Models.Common
{
    public class AdhocAd
    {
        public String PostName  { get; set; }
        public int NoOfPosts  { get; set; }
        public String Qualification  { get; set; }
        public String ExpSkills  { get; set; }
        public double Salary  { get; set; }
        public int AgeLimit { get; set; }
        public DateTime LastDateToApply { get; set; }
    }
}