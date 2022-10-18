using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hrmis.Models.CustomModels
{
    public class HFQuery
    {
        public int Id { get; set; }
        public string HFMISCode { get; set; }
        public string FullName { get; set; }
    }
    public class ProfileQuery
    {
        public string Query { get; set; }
    }
}